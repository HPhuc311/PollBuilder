using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PollBuilder.Domain.Entities;

namespace PollBuilder.Infrastructure.Configurations;

public class PollConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasMaxLength(1000);

        builder.HasMany(x => x.Options)
               .WithOne(x => x.Poll)
               .HasForeignKey(x => x.PollId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Votes)
               .WithOne(x => x.Poll)
               .HasForeignKey(x => x.PollId)
               .OnDelete(DeleteBehavior.NoAction);
    }


}