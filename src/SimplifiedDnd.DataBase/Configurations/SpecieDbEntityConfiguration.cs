using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.DataBase.Configurations;

internal class SpecieDbEntityConfiguration : IEntityTypeConfiguration<SpecieDbEntity> {
  public void Configure(EntityTypeBuilder<SpecieDbEntity> builder) {
    builder.ToTable("species");

    builder.HasKey(x => x.Id);

    builder.HasIndex(x => x.Name)
      .IsUnique();

    builder.Property(x => x.Id)
      .ValueGeneratedOnAdd()
      .HasColumnName("id");

    builder.Property(x => x.Name)
      .HasMaxLength(256)
      .IsRequired()
      .HasColumnName("name");

    builder.Property(x => x.Speed)
      .IsRequired()
      .HasColumnName("speed");
  }
}