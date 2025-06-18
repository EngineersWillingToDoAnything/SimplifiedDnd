using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.DataBase.Configurations;

internal class CharacterClassDbEntityConfiguration : IEntityTypeConfiguration<CharacterClassDbEntity> {
  /// <summary>
  /// Configures the database schema for the CharacterClassDbEntity, including table mapping, composite primary key, required properties, column names, and default values.
  /// </summary>
  /// <param name="builder">The builder used to configure the CharacterClassDbEntity entity type.</param>
  public void Configure(EntityTypeBuilder<CharacterClassDbEntity> builder) {
    builder.ToTable("character_classes");
    
    builder.HasKey(cc => new { cc.CharacterId, cc.ClassId });
    
    builder.Property(cc => cc.CharacterId)
      .IsRequired()
      .HasColumnName("character_id");
    
    builder.Property(cc => cc.ClassId)
      .IsRequired()
      .HasColumnName("class_id");
    
    builder.Property(cc => cc.IsMainClass)
      .HasDefaultValue(false)
      .IsRequired()
      .HasColumnName("is_main_class");
  }
}