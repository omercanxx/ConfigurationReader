using ConfigurationReader.Application.Models;
using ConfigurationReader.Common;
using ConfigurationReader.Common.Extensions;
using ConfigurationReader.Data.Entities;

namespace ConfigurationReader.Application.Mappings
{
    public static class Mapping
    {
        public static ServiceResponse<List<ConfigurationDto>> ToConfigurationDtoList(this List<ConfigurationEntity> entities)
        {
            return new ServiceResponse<List<ConfigurationDto>>(entities.Select(f => new ConfigurationDto
            {
                Id = f.Id,
                Name = f.Name,
                Type = f.Type.ToString(),
                Value = f.Value,
                IsActive = f.IsActive,
                ApplicationName = f.ApplicationName,
                CreatedAt = f.CreatedAt.ToDateString(DateFormatExtensions.CustomDateTimeFormat),
                UpdatedAt = f.UpdatedAt.ToDateString(DateFormatExtensions.CustomDateTimeFormat)
            }).ToList());
        }
    }
}
