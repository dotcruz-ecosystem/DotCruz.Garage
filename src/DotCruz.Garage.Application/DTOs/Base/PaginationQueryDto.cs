namespace DotCruz.Garage.Application.DTOs.Base;

public sealed record PaginationQueryDto(int PageNumber = 1, int PageSize = 10);
