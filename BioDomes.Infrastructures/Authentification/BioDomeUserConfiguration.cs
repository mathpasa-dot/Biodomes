using BioDomes.Infrastructures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioDomes.Infrastructures.Authentification;

public class BioDomeUserConfiguration : IEntityTypeConfiguration<User>
{
    public static readonly string SystemId = "SYSTEM";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.DateOfBirth)
            .IsRequired();
        builder.Property<string?>(user => user.ResearchTeam);
        SeedUsers(builder);
    }

    private void SeedUsers(EntityTypeBuilder<User> builder)
    {
        builder.HasData(new User
        {
            Id = SystemId,
            UserName = "system@local",
            NormalizedUserName = "SYSTEM@LOCAL",
            Email = "system@local",
            NormalizedEmail = "SYSTEM@LOCAL",
            EmailConfirmed = true,

            // 👇 volontairement vide
            PasswordHash = null,
            SecurityStamp = "SYSTEM_STAMP",
            
            LockoutEnabled = true,
            LockoutEnd = DateTimeOffset.MaxValue
        });
    }
}