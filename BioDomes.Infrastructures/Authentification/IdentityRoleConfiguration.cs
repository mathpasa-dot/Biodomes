using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Authentification;

public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    // Identifiants fixes : ils ne doivent pas dépendre de la machine (nom d'hôte d'un conteneur, culture...).
    public const string AdminId = "a1b2c3d4-0000-4000-8000-000000000001";

    public const string EmployeeId = "a1b2c3d4-0000-4000-8000-000000000002";

    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        var adminRole = new IdentityRole
        {
            Id = AdminId, ConcurrencyStamp = AdminId, Name = "Admin", NormalizedName = "ADMIN"
        };

        var employeeRole = new IdentityRole
        {
            Id = EmployeeId, ConcurrencyStamp = EmployeeId, Name = "Employee", NormalizedName = "EMPLOYEE"
        };

        builder.HasData(adminRole, employeeRole);
    }
}