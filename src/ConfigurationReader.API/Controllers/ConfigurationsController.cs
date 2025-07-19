using ConfigurationReader.Application.Models;
using ConfigurationReader.Application.Services;
using ConfigurationReader.Common;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationReader.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationsController : ControllerBase
    {
        private readonly IConfigurationService configurationService;

        public ConfigurationsController(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }

        [HttpPost]
        public async Task<ServiceResponse> CreateAsync([FromBody] CreateConfigurationModel request)
        {
            return await this.configurationService.CreateAsync(request);
        }

        [HttpGet]
        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync([FromQuery] string? name)
        {
            return await this.configurationService.GetAllAsync(name);
        }

        [HttpGet("/apps/{applicationName}/configurations")]
        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllByApplicationNameAsync([FromRoute] string applicationName)
        {
            return await this.configurationService.GetAllByApplicationNameAsync(applicationName);
        }

        [HttpDelete("{id}")]
        public async Task<ServiceResponse> DeleteAsync([FromRoute]int id)
        {
            return await this.configurationService.DeleteAsync(id);
        }
    }
}
