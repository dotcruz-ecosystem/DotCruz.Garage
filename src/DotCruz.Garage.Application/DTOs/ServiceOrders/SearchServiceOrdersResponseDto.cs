namespace DotCruz.Garage.Application.DTOs.ServiceOrders;

public record SearchServiceOrdersResponseDto(
    IReadOnlyCollection<ServiceOrderDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);
