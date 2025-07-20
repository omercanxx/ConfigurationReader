using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace ConfigurationReader
{
    public class ConfigurationSyncHostedService : IHostedService, IDisposable
    {
        private readonly ConfigurationSource configurationSource;
        private readonly ConfigurationProviderOptions options;
        private readonly ILogger<ConfigurationSyncHostedService> logger;
        private readonly SemaphoreSlim signal = new SemaphoreSlim(0);
        private readonly CancellationTokenSource stoppingCts = new CancellationTokenSource();
        private readonly ConfigurationReader configurationReader;
        private Timer? timer;
        private Task? executingTask;
        private bool isDisposed;

        public ConfigurationSyncHostedService(
            IOptions<ConfigurationProviderOptions> options,
            ILogger<ConfigurationSyncHostedService> logger)
        {
            this.configurationSource = ConfigurationBuilderExtensions.GetConfigurationSource();
            this.options = options.Value;
            this.logger = logger;
            string applicationName = this.options.ApplicationName ?? this.configurationSource.ApplicationName;

            this.configurationReader = new ConfigurationReader(applicationName, this.options.ConnectionString, this.options.RefreshIntervalInSeconds);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(this.configurationReader.ConnectionString))
            {
                this.logger.LogWarning($"{nameof(ConfigurationReader)}.{nameof(ConfigurationReader.ConnectionString)} is empty, Configurations will not be synced!");
            }
            else
            {
                this.logger.LogInformation("Starting");

                if (this.configurationReader.RefreshIntervalInSeconds <= 0)
                {
                    this.logger.LogWarning($"{nameof(ConfigurationReader)}.{nameof(ConfigurationReader.RefreshIntervalInSeconds)} is {this.configurationReader.RefreshIntervalInSeconds}, Configurations will be get only once!");

                    this.executingTask = Task.Run(this.GetConfigurationsAsync, cancellationToken);
                }
                else
                {
                    this.timer = new Timer((_) => this.signal.Release(), null, TimeSpan.Zero, TimeSpan.FromSeconds(this.configurationReader.RefreshIntervalInSeconds));

                    this.executingTask = Task.Run(this.ExecuteAsync, cancellationToken);
                }

                this.logger.LogInformation("Started");
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Stopping");

            try
            {
                // Signal cancellation to the executing method
                this.timer?.Change(Timeout.Infinite, 0);
                this.stoppingCts.Cancel();
            }
            finally
            {
                if (this.executingTask != null)
                {
                    // Wait until the task completes or the stop token triggers
                    await Task.WhenAny(this.executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
                }
            }

            this.logger.LogInformation("Stopped");
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.stoppingCts.Dispose();
                this.timer?.Dispose();
                this.signal.Dispose();
            }

            this.isDisposed = true;
        }

        private async Task ExecuteAsync()
        {
            while (!this.stoppingCts.IsCancellationRequested)
            {
                await this.signal.WaitAsync(this.stoppingCts.Token);
                await this.GetConfigurationsAsync();
            }
        }

        private async Task GetConfigurationsAsync()
        {
            DateTime? callTime = null;
            string? result = null;

            List<ConfigurationDto>? configurations = null;

            try
            {
                configurations = await this.configurationReader.GetValuesAsync();

                callTime = DateTime.Now;

                this.configurationSource.Refresh(configurations);

                result = "Success";
            }
            catch (Exception ex)
            {
                callTime ??= DateTime.Now;
                result = ex.Message;

                this.logger.LogError(ex, "ConfigurationSyncHostedService problem");
            }
            finally
            {
                this.configurationReader.LastApiCallTime = callTime;
                this.configurationReader.LastApiCallResult = result;
                this.configurationReader.Configurations = configurations?.ToDictionary(f => f.Name, f => f.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}