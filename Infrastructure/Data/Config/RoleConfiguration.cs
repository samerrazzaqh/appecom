using Core.Entities.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.Config;

public class RoleConfiguration: IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole 
            { 
                Id = "8f14e45f-ceea-4b19-a2b7-08c8e4a9d0a1",
                Name = "Admin", 
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000001"
            },
            new IdentityRole 
            { 
                Id = "3c9e6d1a-45f3-4b2e-9a7c-1d2e3f4a5b6c",
                Name = "User", 
                NormalizedName = "USER",
                ConcurrencyStamp = "00000000-0000-0000-0000-000000000002"
            }
        );
    }
}