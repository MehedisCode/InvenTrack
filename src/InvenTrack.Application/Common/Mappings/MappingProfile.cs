namespace InvenTrack.Application.Common.Mappings;

using AutoMapper;
using InvenTrack.Domain.Entities;
using InvenTrack.Application.Features.Products.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));
    }
}
