using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PollBuilder.Domain.Entities;

namespace PollBuilder.Infrastructure.Configurations;

public class PollOptionConfiguration : IEntityTypeConfiguration<PollOption>
{
    public void Configure(EntityTypeBuilder<PollOption> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
               .HasMaxLength(300)
               .IsRequired();
    }
}