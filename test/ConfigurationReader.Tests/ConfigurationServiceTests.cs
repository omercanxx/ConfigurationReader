using ConfigurationReader.Application.Services;
using ConfigurationReader.Application.Strategies;
using ConfigurationReader.Data.Repository;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ConfigurationReader.Tests
{
    [TestFixture]
    public class ConfigurationServiceTests
    {
        private IConfigurationService configurationService;

        private Mock<ConfigurationStrategyFactory> configurationFetchStrategyFactory;
        private Mock<ILogger<ConfigurationService>> logger;
        private Mock<IDistributedCache> distributedCache;
        private Mock<IConfigurationRepository> configurationRepository;

        [SetUp]
        public void Init()
        {
            this.configurationFetchStrategyFactory = new Mock<ConfigurationStrategyFactory>();
            this.logger = new Mock<ILogger<ConfigurationService>>();
            this.distributedCache = new Mock<IDistributedCache>();
            this.configurationRepository = new Mock<IConfigurationRepository>();

            this.configurationService = new ConfigurationService(
                this.configurationFetchStrategyFactory.Object,
                this.logger.Object,
                this.distributedCache.Object,
                this.configurationRepository.Object);
        }

        [Test]
        public async Task CacheShouldBeReturnsConfigurations()
        {
            //Arrange
            this.distributedCache
                .Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("mocked-value");

            //_mockBankingAppSettings.Setup(x => x.CurrentValue).Returns(new BankingAppSettings
            //{
            //    PendingDepositTransactionsCountThreshold = 20
            //});

            ////Act
            //await _bankTransactionService.CheckPendingDepositTransactionsCount();

            ////Assert
            //_mockBankTransactionRepository.Verify(x => x.GetPendingDepositTransactionsCount(), Times.Once);
            //_mockBankingAppSettings.Verify(x => x.CurrentValue, Times.Once);
            //_mockLogService.Verify(x => x.Error(It.IsAny<string>(), DomainType.Banking), Times.Once);
        }
    }
}
