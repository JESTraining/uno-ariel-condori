using AutoMapper;
using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Domain.Entities;

namespace WarehouseInventory.Api.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductListItemDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category!.Name))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location!.Name))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.CurrentStock < src.ReorderThreshold));

        CreateMap<Product, ProductDetailsDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category!.Name))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location!.Name));
    }
}
