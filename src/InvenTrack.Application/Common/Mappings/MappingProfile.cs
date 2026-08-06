namespace InvenTrack.Application.Common.Mappings;

using AutoMapper;
using InvenTrack.Domain.Entities;
using InvenTrack.Application.Features.Products.DTOs;
using InvenTrack.Application.Features.Inventory.DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

        CreateMap<StockTransaction, StockTransactionDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product.Name))
            .ForMember(d => d.SKU, opt => opt.MapFrom(s => s.Product.SKU))
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.FullName))
            .ForMember(d => d.TransactionType, opt => opt.MapFrom(s => s.TransactionType.ToString()));
    }
}
