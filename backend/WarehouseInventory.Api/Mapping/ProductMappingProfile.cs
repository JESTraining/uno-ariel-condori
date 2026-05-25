using AutoMapper;
using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Domain.Entities;

namespace WarehouseInventory.Api.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductListItemDto>()
            .ForCtorParam(nameof(ProductListItemDto.Category), opt => opt.MapFrom(src => src.Category!.Id))
            .ForCtorParam(nameof(ProductListItemDto.Location), opt => opt.MapFrom(src => src.Location!.Id))
            .ForCtorParam(nameof(ProductListItemDto.IsLowStock), opt => opt.MapFrom(src => src.CurrentStock < src.ReorderThreshold));

        CreateMap<Product, ProductDetailsDto>()
            .ForCtorParam(nameof(ProductDetailsDto.Category), opt => opt.MapFrom(src => src.Category!.Id))
            .ForCtorParam(nameof(ProductDetailsDto.Location), opt => opt.MapFrom(src => src.Location!.Id));
    }
}
