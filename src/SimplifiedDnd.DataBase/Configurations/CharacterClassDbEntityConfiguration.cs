using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.DataBase.Configurations;

internal class CharacterClassDbEntityConfiguration : IEntityTypeConfiguration<CharacterClassDbEntity> {
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