using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasData(
            new AppUser 
            { 
                Id = "4c293c9f-57f9-4033-b255-39982f020db2", 
                DispalyName = "Samer Razzaq",
                UserName = "samerrazzaqh",
                NormalizedUserName = "SAMERRAZZAQH",
                Email = "samerrazzaqh@gmail.com",
                NormalizedEmail = "SAMERRAZZAQH@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAECWDEXPMmv4ywmxN/KBnL7FxpDa/CLJG2xsTMo8PSjfn/WQ5vUxwf/wHXuDYkyyTmA==",
                SecurityStamp = "6E4TGE4CG7IJCZG7ZAIKHCAG5EDDZPYI",
                ConcurrencyStamp = "b45109f7-5f98-4fc2-ad62-7763826c245e",
                PhoneNumber =  null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,
            }
        );
    }
}