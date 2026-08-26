using DotCruz.Garage.Domain.Entities.Base;
using DotCruz.Garage.Domain.Entities.Fines;

namespace DotCruz.Garage.Domain.Entities.Drivers;

public class Driver : BaseEntity, ITenantEntity
{
    public const int CnhSuspensionPointThreshold = 15;

    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Cpf { get; private set; } = null!;
    public string Cnh { get; private set; } = null!;
    public string CnhCategory { get; private set; } = null!;
    public DateTimeOffset CnhExpirationDate { get; private set; }
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    public virtual ICollection<VehicleDriverAssignment> Assignments { get; private init; } = new List<VehicleDriverAssignment>();
    public virtual ICollection<TrafficFine> Fines { get; private init; } = new List<TrafficFine>();

    private Driver() { }

    public static Driver Create(
        Guid tenantId,
        Guid userId,
        string name,
        string cpf,
        string cnh,
        string cnhCategory,
        DateTimeOffset cnhExpirationDate,
        string email,
        string phone)
    {
        return new Driver
        {
            TenantId = tenantId,
            UserId = userId,
            Name = name,
            Cpf = cpf,
            Cnh = cnh,
            CnhCategory = cnhCategory.ToUpperInvariant(),
            CnhExpirationDate = cnhExpirationDate,
            Email = email,
            Phone = phone,
            IsActive = true
        };
    }

    public void Update(
        string name,
        string cpf,
        string cnh,
        string cnhCategory,
        DateTimeOffset cnhExpirationDate,
        string email,
        string phone)
    {
        Name = name;
        Cpf = cpf;
        Cnh = cnh;
        CnhCategory = cnhCategory.ToUpperInvariant();
        CnhExpirationDate = cnhExpirationDate;
        Email = email;
        Phone = phone;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public bool IsCnhExpired(DateTimeOffset? now = null) =>
        (now ?? DateTimeOffset.UtcNow) > CnhExpirationDate;

    public int CalculateAccumulatedPoints(DateTimeOffset? referenceDate = null)
    {
        var refDate = referenceDate ?? DateTimeOffset.UtcNow;
        var limitDate = refDate.AddMonths(-12);

        return Fines
            .Where(f => f.InfractionDate >= limitDate && f.DeletedAt == null)
            .Sum(f => f.Points);
    }

    public bool IsSuspensionRisk(DateTimeOffset? referenceDate = null)
    {
        if (IsCnhExpired(referenceDate))
            return true;

        return CalculateAccumulatedPoints(referenceDate) >= CnhSuspensionPointThreshold;
    }
}
