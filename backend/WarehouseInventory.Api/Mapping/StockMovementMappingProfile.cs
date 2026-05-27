using AutoMapper;
using WarehouseInventory.Api.Contracts.Stock;
using WarehouseInventory.Api.Domain.Entities;

namespace WarehouseInventory.Api.Mapping;

public class StockMovementMappingProfile : Profile
{
    public StockMovementMappingProfile()
    {
        CreateMap<StockMovement, StockMovementDto>();
    }
}
