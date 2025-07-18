using ConfigurationReader.Application.Models;
using ConfigurationReader.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationReader.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeatureService featureService;

        public FeaturesController(IFeatureService featureService)
        {
            this.featureService = featureService;
        }

        [HttpPost]
        public async Task CreateAsync([FromBody] CreateFeatureModel request)
        {
            await this.featureService.CreateAsync(request);
        }

        [HttpGet]
        public async Task<List<FeatureDto>> GetAllAsync()
        {
            return await this.featureService.GetAllAsync();
        }
    }
}
