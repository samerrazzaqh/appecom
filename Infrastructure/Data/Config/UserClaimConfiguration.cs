using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config;



public class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
    {
        var userId = "4c293c9f-57f9-4033-b255-39982f020db2";
        var userClaims = new List<IdentityUserClaim<string>>();

        int idCounter = 1;
        foreach (var claim in ClaimsStore.AllClaims)
        {
            userClaims.Add(new IdentityUserClaim<string>
            {
                Id = idCounter++,
                UserId = userId,
                ClaimType = claim.Type,
                ClaimValue = "true",
            });
        }

        builder.HasData(userClaims);
    }
}







// public class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
// {
//     public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
//     {
//         builder.HasData(
//             new IdentityUserClaim<string>
//             {
//                 Id = 1, // إجباري تحديده في الـ Seeding
//                 UserId = "4c293c9f-57f9-4033-b255-39982f020db2", // نفس Id المستخدم الخاص بك
//                 ClaimType = "Edit Role",
//                 ClaimValue = "true"
//             },
//             new IdentityUserClaim<string>
//             {
//                 Id = 2,
//                 UserId = "4c293c9f-57f9-4033-b255-39982f020db2",
//                 ClaimType = "Delete Role",
//                 ClaimValue = "true"
//             },
//             new IdentityUserClaim<string>
//             {
//                 Id = 3,
//                 UserId = "4c293c9f-57f9-4033-b255-39982f020db2",
//                 ClaimType = "Create Role",
//                 ClaimValue = "true"
//             }
//         );
//     }
// }