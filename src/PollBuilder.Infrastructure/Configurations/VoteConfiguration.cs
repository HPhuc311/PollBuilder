using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PollBuilder.Domain.Entities;

namespace PollBuilder.Infrastructure.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IPAddress)
               .HasMaxLength(100);

        builder.Property(x => x.FingerPrint)
               .HasMaxLength(200);

        builder.HasOne(x => x.PollOption)
               .WithMany(x => x.Votes)
               .HasForeignKey(x => x.PollOptionId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}