using Core.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            builder.OwnsOne(x => x.shippingAddress,
                n => { n.WithOwner(); });

            builder.HasMany(x => x.orderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.status).HasConversion(o => o.ToString(),
                o => (Status)Enum.Parse(typeof(Status), o));

            builder.Property(m => m.SubTotal).HasColumnType("decimal(18,2)");
        }
    }
}
