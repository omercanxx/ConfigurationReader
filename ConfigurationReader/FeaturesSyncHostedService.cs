using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;

namespace ConfigurationReader.FeatureProvider
{
    public class FeaturesSyncHostedService : IHostedService, IDisposable
    {
        private readonly FeaturesConfigurationSource featuresConfigurationSource;
        private readonly FeaturesProviderOptions options;
        private readonly ILogger<FeaturesSyncHostedService> logger;
        private readonly SemaphoreSlim signal = new SemaphoreSlim(0);
        private readonly CancellationTokenSource stoppingCts = new CancellationTokenSource();
        private readonly FeaturesSyncServiceReporter featuresSyncServiceReporter;
        private readonly HttpClient? httpClient;
        private readonly string requestUri;
        private Timer? timer;
        private Task? executingTask;
        private bool isDisposed;

        public FeaturesSyncHostedService(
            IHttpClientFactory httpClientFactory,
            IOptions<FeaturesProviderOptions> options,
            IFeaturesSyncServiceReporter featuresSyncServiceReporter,
            ILogger<FeaturesSyncHostedService> logger)
        {
            this.featuresConfigurationSource = ConfigurationBuilderExtensions.GetFeaturesConfigurationSource();
            this.options = options.Value;
            this.logger = logger;

            string applicationName = this.options.ApplicationName ?? this.featuresConfigurationSource.ApplicationName;
            this.requestUri = $"/apps/{applicationName}/features";

            this.featuresSyncServiceReporter = (FeaturesSyncServiceReporter)featuresSyncServiceReporter;

            if (!string.IsNullOrEmpty(this.options.FeaturesApiUrl))
            {
                this.httpClient = httpClientFactory.CreateClient("FeaturesApi");
                this.featuresSyncServiceReporter.FeaturesApiUrl = this.httpClient.BaseAddress?.ToString().TrimEnd('/') + this.requestUri;
            }

            this.featuresSyncServiceReporter.ApplicationName = applicationName;
            this.featuresSyncServiceReporter.RefreshIntervalInSeconds = this.options.RefreshIntervalInSeconds;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(this.options.FeaturesApiUrl))
            {
                this.logger.LogWarning($"{nameof(FeaturesProviderOptions)}.{nameof(FeaturesProviderOptions.FeaturesApiUrl)} is empty, Features will not be synced!");
            }
            else
            {
                this.logger.LogInformation("Starting");

                if (this.options.RefreshIntervalInSeconds <= 0)
                {
                    this.logger.LogWarning($"{nameof(FeaturesProviderOptions)}.{nameof(FeaturesProviderOptions.RefreshIntervalInSeconds)} is {this.options.RefreshIntervalInSeconds}, Features will be get only once!");

                    this.executingTask = Task.Run(this.GetFeaturesAsync, cancellationToken);
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
                await this.GetFeaturesAsync();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Hosted service")]
        private async Task GetFeaturesAsync()
        {
            if (this.httpClient == null)
            {
                throw new InvalidOperationException("httpClient is null!");
            }

            DateTime? callTime = null;
            string? result = null;

            FeatureDto[]? features = null;

            try
            {
                HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync(this.requestUri);

                callTime = DateTime.Now;

                httpResponseMessage.EnsureSuccessStatusCode();

                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                features = JsonConvert.DeserializeObject<FeatureDto[]>(response);

                if (features == null)
                {
                    throw new JsonException("Could not convert response to FeatureDto[] !");
                }

                this.featuresConfigurationSource.Refresh(features);

                result = "Success";
            }
            catch (Exception ex)
            {
                callTime ??= DateTime.Now;
                result = ex.Message;

                this.logger.LogError(ex, "FeaturesSyncHostedService problem");
            }
            finally
            {
                this.featuresSyncServiceReporter.LastApiCallTime = callTime;
                this.featuresSyncServiceReporter.LastApiCallResult = result;
                this.featuresSyncServiceReporter.Features = features?.ToDictionary(f => f.Name, f => f.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}