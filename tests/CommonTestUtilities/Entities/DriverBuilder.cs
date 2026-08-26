using Bogus;
using DotCruz.Garage.Domain.Entities.Drivers;

namespace CommonTestUtilities.Entities;

public class DriverBuilder
{
    public static Driver Build(
        Guid? tenantId = null,
        Guid? userId = null,
        string? name = null,
        string? cpf = null,
        string? cnh = null,
        string cnhCategory = "B",
        DateTimeOffset? cnhExpirationDate = null,
        string? email = null,
        string? phone = null)
    {
        var faker = new Faker("pt_BR");

        return Driver.Create(
            tenantId: tenantId ?? Guid.NewGuid(),
            userId: userId ?? Guid.NewGuid(),
            name: name ?? faker.Name.FullName(),
            cpf: cpf ?? faker.Random.Replace("###.###.###-##"),
            cnh: cnh ?? faker.Random.Replace("###########"),
            cnhCategory: cnhCategory,
            cnhExpirationDate: cnhExpirationDate ?? DateTimeOffset.UtcNow.AddYears(2),
            email: email ?? faker.Internet.Email(),
            phone: phone ?? faker.Phone.PhoneNumber("(##) 9####-####")
        );
    }
}
