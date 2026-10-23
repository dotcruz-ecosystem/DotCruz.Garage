using DotCruz.Garage.Infrastructure.Data.Repositories.Vehicles;

namespace Infrastructure.Test.Data;

public class VehicleCatalogRepositoryTest
{
    [Fact]
    public void SortEnginesNaturally_Sorts_Motorcycles_Numerically()
    {
        var input = new[]
        {
            "1000cc", "100cc", "1100cc", "110cc", "1200cc", "1250cc", "125cc",
            "1300cc", "150cc", "160cc", "190cc", "200cc", "250cc", "300cc",
            "321cc", "350cc", "400cc", "450cc", "500cc", "50cc", "600cc",
            "650cc", "700cc", "750cc", "800cc", "900cc", "Elétrica"
        };

        var result = VehicleCatalogRepository.SortEnginesNaturally(input).ToList();

        var expected = new[]
        {
            "50cc", "100cc", "110cc", "125cc", "150cc", "160cc", "190cc", "200cc",
            "250cc", "300cc", "321cc", "350cc", "400cc", "450cc", "500cc", "600cc",
            "650cc", "700cc", "750cc", "800cc", "900cc", "1000cc", "1100cc",
            "1200cc", "1250cc", "1300cc", "Elétrica"
        };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SortEnginesNaturally_Sorts_Cars_Numerically()
    {
        var input = new[]
        {
            "2.0", "1.0", "1.4 Turbo", "3.0", "1.0 Turbo", "1.2 Turbo", "1.6",
            "V8", "V6", "Híbrido", "100% Elétrico", "2.0 Turbo"
        };

        var result = VehicleCatalogRepository.SortEnginesNaturally(input).ToList();

        var expected = new[]
        {
            "1.0", "1.0 Turbo", "1.2 Turbo", "1.4 Turbo", "1.6", "2.0", "2.0 Turbo", "3.0",
            "V6", "V8", "100% Elétrico", "Híbrido"
        };

        Assert.Equal(expected, result);
    }
}
