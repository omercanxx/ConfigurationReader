using ConfigurationReader.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly ConfigurationSyncServiceReporter configurationSyncReporter;
        private readonly HttpClient? httpClient;
        private readonly string requestUri;
        private Timer? timer;
        private Task? executingTask;
        private bool isDisposed;

        public ConfigurationSyncHostedService(
            IHttpClientFactory httpClientFactory,
            IOptions<ConfigurationProviderOptions> options,
            IConfigurationSyncServiceReporter configurationSyncServiceReporter,
            ILogger<ConfigurationSyncHostedService> logger)
        {
            this.configurationSource = ConfigurationBuilderExtensions.GetConfigurationSource();
            this.options = options.Value;
            this.logger = logger;
            string applicationName = this.options.ApplicationName ?? this.configurationSource.ApplicationName;
            this.requestUri = $"/apps/{applicationName}/configurations";
            this.configurationSyncReporter = (ConfigurationSyncServiceReporter)configurationSyncServiceReporter;

            if (!string.IsNullOrEmpty(this.options.ConfigurationApiUrl))
            {
                this.httpClient = httpClientFactory.CreateClient("ConfigurationApi");
                this.configurationSyncReporter.ConfigurationApiUrl = this.httpClient.BaseAddress?.ToString().TrimEnd('/') + this.requestUri;
            }

            this.configurationSyncReporter.ApplicationName = applicationName;
            this.configurationSyncReporter.RefreshIntervalInSeconds = this.options.RefreshIntervalInSeconds;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(this.options.ConfigurationApiUrl))
            {
                this.logger.LogWarning($"{nameof(ConfigurationProviderOptions)}.{nameof(ConfigurationProviderOptions.ConfigurationApiUrl)} is empty, Configurations will not be synced!");
            }
            else
            {
                this.logger.LogInformation("Starting");

                if (this.options.RefreshIntervalInSeconds <= 0)
                {
                    this.logger.LogWarning($"{nameof(ConfigurationProviderOptions)}.{nameof(ConfigurationProviderOptions.RefreshIntervalInSeconds)} is {this.options.RefreshIntervalInSeconds}, Configurations will be get only once!");

                    this.executingTask = Task.Run(this.GetConfigurationsAsync, cancellationToken);
                }
                else
                {
                    this.timer = new Timer((_) => this.signal.Release(), null, TimeSpan.Zero, TimeSpan.FromSeconds(this.options.RefreshIntervalInSeconds));

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
                this.httpClient?.Dispose();
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
            if (this.httpClient == null)
            {
                throw new InvalidOperationException("httpClient is null!");
            }

            DateTime? callTime = null;
            string? result = null;

            ServiceResponse<List<ConfigurationDto>>? configurations = null;

            try
            {
                //var reader = new ConfigurationReader(this.configurationSyncReporter.ApplicationName, "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Config;Integrated Security=True;Connect Timeout=30", this.configurationSyncReporter.RefreshIntervalInSeconds);
                //var test = await reader.GetValueAsync<string>("IsBasketEnabled");

                HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync(this.requestUri);

                callTime = DateTime.Now;

                httpResponseMessage.EnsureSuccessStatusCode();

                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                configurations = JsonConvert.DeserializeObject<ServiceResponse<List<ConfigurationDto>>>(response);

                if (configurations == null)
                {
                    throw new JsonException("Could not convert response to List<ConfigurationDto>!");
                }

                this.configurationSource.Refresh(configurations.Result);

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
                this.configurationSyncReporter.LastApiCallTime = callTime;
                this.configurationSyncReporter.LastApiCallResult = result;
                this.configurationSyncReporter.Configurations = configurations?.Result?.ToDictionary(f => f.Name, f => f.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}