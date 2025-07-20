using ConfigurationReader.Application.Models;
using ConfigurationReader.Application.Services;
using ConfigurationReader.Application.Services.Interfaces;
using ConfigurationReader.Application.Strategies;
using ConfigurationReader.Application.Strategies.Interfaces;
using ConfigurationReader.Common;
using ConfigurationReader.Common.Enums;
using ConfigurationReader.Data.Entities;
using ConfigurationReader.Data.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ConfigurationReader.Tests
{
    [TestFixture]
    public class ConfigurationServiceTests
    {
        private IConfigurationStrategy nonFilteredConfigurationStrategy;
        private IConfigurationStrategy filteredConfigurationStrategy;
        private IConfigurationService configurationService;

        private ConfigurationStrategyFactory configurationStrategyFactory;
        private Mock<IRedisCacheService> mockRedisCacheService;
        private Mock<ILogger<NonFilteredConfigurationStrategy>> mockNonFilteredLogger;
        private Mock<ILogger<FilteredConfigurationStrategy>> mockFilteredLogger;
        private Mock<ILogger<ConfigurationService>> mockConfigurationServiceLogger;
        private Mock<IConfigurationRepository> mockConfigurationRepository;

        [SetUp]
        public void Init()
        {
            this.mockRedisCacheService = new Mock<IRedisCacheService>();
            this.mockNonFilteredLogger = new Mock<ILogger<NonFilteredConfigurationStrategy>>();
            this.mockFilteredLogger = new Mock<ILogger<FilteredConfigurationStrategy>>();
            this.mockConfigurationServiceLogger = new Mock<ILogger<ConfigurationService>>();
            this.mockConfigurationRepository = new Mock<IConfigurationRepository>();

            this.nonFilteredConfigurationStrategy = new NonFilteredConfigurationStrategy(
                this.mockRedisCacheService.Object,
                this.mockNonFilteredLogger.Object,
                this.mockConfigurationRepository.Object);

            this.filteredConfigurationStrategy = new FilteredConfigurationStrategy(
                this.mockRedisCacheService.Object,
                this.mockFilteredLogger.Object,
                this.mockConfigurationRepository.Object);

            this.configurationStrategyFactory = new ConfigurationStrategyFactory((NonFilteredConfigurationStrategy)this.nonFilteredConfigurationStrategy, (FilteredConfigurationStrategy)this.filteredConfigurationStrategy);

            this.configurationService = new ConfigurationService(
                this.mockRedisCacheService.Object,
                this.mockConfigurationServiceLogger.Object,
                configurationStrategyFactory,
                this.mockConfigurationRepository.Object);
        }


        [Test]
        public async Task GetAllByApplicationNameAsync_RedisCache_Should_Return_Config()
        {
            //Arrange
            this.mockRedisCacheService
                .Setup(x => x.GetAsync<List<ConfigurationDto>>(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<ConfigurationDto>()
                    {
                        new ConfigurationDto
                        {
                            Id = 1,
                            Name = "IsBasketEnabled",
                            Type = "Boolean",
                            Value = "0",
                            ApplicationName = "SERVICE-A"
                        },
                        new ConfigurationDto
                        {
                            Id = 2,
                            Name = "MaxItemCount",
                            Type = "Int",
                            Value = "50",
                            ApplicationName = "SERVICE-B"
                        }
                    }
                );

            //Act
            var result = await this.configurationService.GetAllByApplicationNameAsync("SERVICE-A");

            //Assert
            this.mockConfigurationRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Never);

            this.mockRedisCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<ConfigurationDto>>(), It.IsAny<TimeSpan>()), Times.Never);

            result.Result.Should().BeEquivalentTo(
                new List<ConfigurationDto>()
                    {
                        new ConfigurationDto
                        {
                            Id = 1,
                            Name = "IsBasketEnabled",
                            Type = "Boolean",
                            Value = "0",
                            ApplicationName = "SERVICE-A"
                        },
                    }
                );
        }

        [Test]
        public async Task GetAllByApplicationNameAsync_RedisCache_Should_Return_Null()
        {
            //Arrange
            this.mockRedisCacheService
                .Setup(x => x.GetAsync<List<ConfigurationDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<ConfigurationDto>?)null);

            this.mockConfigurationRepository
                .Setup(x => x.GetAllByApplicationNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<List<ConfigurationEntity>>(
                    new List<ConfigurationEntity>()
                    {
                        new ConfigurationEntity
                        {
                            Id = 1,
                            Name = "IsBasketEnabled",
                            Type = FeatureType.Boolean,
                            Value = "0",
                            ApplicationName = "SERVICE-A"
                        }
                    }
                ));

            //Act
            await this.configurationService.GetAllByApplicationNameAsync("SERVICE-A");

            //Assert
            this.mockConfigurationRepository.Verify(x => x.GetAllByApplicationNameAsync(It.IsAny<string>()), Times.Once);

            this.mockRedisCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<ConfigurationDto>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        #region Strategy Tests

        [Test]
        public async Task NonFilteredConfigurationStrategy_RedisCache_Should_Return_Config()
        {
            //Arrange
            this.mockRedisCacheService
                .Setup(x => x.GetAsync<List<ConfigurationDto>>(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<ConfigurationDto>()
                    {
                        new ConfigurationDto
                        {
                            Id = 1,
                            Name = "IsBasketEnabled",
                            Type = "Boolean",
                            Value = "0",
                            ApplicationName = "SERVICE-A"
                        },
                        new ConfigurationDto
                        {
                            Id = 2,
                            Name = "MaxItemCount",
                            Type = "Int",
                            Value = "50",
                            ApplicationName = "SERVICE-B"
                        }
                    }
                );

            //Act
            await this.nonFilteredConfigurationStrategy.GetAllAsync(null);

            //Assert
            this.mockConfigurationRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Never);

            this.mockRedisCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<ConfigurationDto>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Test]
        public async Task NonFilteredConfigurationStrategy_RedisCache_Should_Return_Null()
        {
            //Arrange
            this.mockRedisCacheService
                .Setup(x => x.GetAsync<List<ConfigurationDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<ConfigurationDto>?)null);

            this.mockConfigurationRepository
                .Setup(x => x.GetAllAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<List<ConfigurationEntity>>(
                    new List<ConfigurationEntity>()
                    {
                        new ConfigurationEntity
                        {
                            Id = 1,
                            Name = "IsBasketEnabled",
                            Type = FeatureType.Boolean,
                            Value = "0",
                            ApplicationName = "SERVICE-A"
                        },
                        new ConfigurationEntity
                        {
                            Id = 2,
                            Name = "MaxItemCount",
                            Type = FeatureType.Int,
                            Value = "50",
                            ApplicationName = "SERVICE-B"
                        }
                    }
                ));

            //Act
            await this.nonFilteredConfigurationStrategy.GetAllAsync(null);

            //Assert
            this.mockConfigurationRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Once);

            this.mockRedisCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<ConfigurationDto>>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Test]
        public async Task FilteredConfigurationStrategy_RedisCache_Should_Return_Config()
        {
            //Arrange
            this.mockRedisCacheService
                .Setup(x => x.GetAsync<List<ConfigurationDto>>(It.IsAny<string>()))
                .ReturnsAsync(
                    new List<ConfigurationDto>()
                    {
                        new ConfigurationDto
                        {
                            Id = 1,
                            Name = "IsBasketEnabled",
                            Type = "Boolean",
                            Value = "0",
                            ApplicationName = "SERVICE-A"
                        },
                        new ConfigurationDto
                        {
                            Id = 2,
                            Name = "MaxItemCount",
                            Type = "Int",
                            Value = "50",
                            ApplicationName = "SERVICE-B"
                        }
                    }
                );

            //Act
            var result = await this.filteredConfigurationStrategy.GetAllAsync("IsBasketEnabled");

            //Assert
            this.mockConfigurationRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Never);

            this.mockRedisCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<ConfigurationDto>>(), It.IsAny<TimeSpan>()), Times.Never);

            result.Result.Should().BeEquivalentTo(
                new List<ConfigurationDto>()
                    {
                        new ConfigurationDto
                        {
                            Id = 1,
                            Name = "IsBasketEnabled",
                            Type = "Boolean",
                            Value = "0",
                            ApplicationName = "SERVICE-A"
                        },
                    }
                );
        }

        [Test]
        public async Task FilteredConfigurationStrategy_RedisCache_Should_Return_Null()
        {
            //Arrange
            this.mockRedisCacheService
                .Setup(x => x.GetAsync<List<ConfigurationDto>>(It.IsAny<string>()))
                .ReturnsAsync((List<ConfigurationDto>?)null);

            this.mockConfigurationRepository
                .Setup(x => x.GetAllAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<List<ConfigurationEntity>>(
                    new List<ConfigurationEntity>()
                    {
                        new ConfigurationEntity
                        {
                            Id = 1,
                            Name = "IsBasketEnabled",
                            Type = FeatureType.Boolean,
                            Value = "0",
                            ApplicationName = "SERVICE-A"
                        },
                        new ConfigurationEntity
                        {
                            Id = 2,
                            Name = "MaxItemCount",
                            Type = FeatureType.Int,
                            Value = "50",
                            ApplicationName = "SERVICE-B"
                        }
                    }
                ));

            //Act
            await this.filteredConfigurationStrategy.GetAllAsync("IsBasketEnabled");

            //Assert
            this.mockConfigurationRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Once);

            this.mockRedisCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<ConfigurationDto>>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        #endregion
    }
}
