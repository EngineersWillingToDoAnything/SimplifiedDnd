using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.DataBase.Configurations;

internal class CharacterDbEntityConfiguration : IEntityTypeConfiguration<CharacterDbEntity> {
  public void Configure(EntityTypeBuilder<CharacterDbEntity> builder) {
    builder.ToTable("characters");
    
    builder.HasKey(c => c.Id);
    
    builder.HasIndex(c => new {c.Name, c.PlayerName})
      .IsUnique();
    
    builder.Property(c => c.Id)
      .IsFixedLength()
      .HasMaxLength(36)
      .HasColumnName("id");
    
    builder.Property(c => c.Name)
      .HasMaxLength(256)
      .IsRequired()
      .HasColumnName("name");

    builder.Property(c => c.PlayerName)
      .HasMaxLength(256)
      .IsRequired()
      .HasColumnName("player_name");

    builder.Property(c => c.SpecieId)
      .HasColumnName("specie_id");
    
    builder.HasOne(c => c.Specie)
      .WithMany(s => s.Characters)
      .HasForeignKey(c => c.SpecieId)
      .OnDelete(DeleteBehavior.Restrict)
      .IsRequired();
  }
}