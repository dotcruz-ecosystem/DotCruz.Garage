using DotCruz.Garage.Application.Abstractions.Data;
using Moq;

namespace CommonTestUtilities.Data;

public class UnitOfWorkBuilder
{
    public static IUnitOfWork Build() => new Mock<IUnitOfWork>().Object;
}
