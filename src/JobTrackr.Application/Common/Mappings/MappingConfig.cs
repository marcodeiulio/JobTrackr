using JobTrackr.Application.Companies.DTOs;
using JobTrackr.Domain.Entities;
using Mapster;

namespace JobTrackr.Application.Common.Mappings;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Company, CompanyDto>.NewConfig();
    }
}