using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.DataBase.Configurations;

internal class ClassDbEntityConfiguration : IEntityTypeConfiguration<ClassDbEntity> {
  /// <summary>
  /// Configures the entity mapping for <see cref="ClassDbEntity"/> in Entity Framework Core.
  /// </summary>
  /// <param name="builder">The builder used to configure the <see cref="ClassDbEntity"/> entity.</param>
  public void Configure(EntityTypeBuilder<ClassDbEntity> builder) {
    builder.ToTable("classes");
    
    builder.HasKey(c => c.Id);
    
    builder.HasIndex(c => c.Name)
      .IsUnique();
    
    builder.Property(c => c.Id)
      .ValueGeneratedOnAdd()
      .IsRequired()
      .HasColumnName("id");
    
    builder.Property(c => c.Name)
      .HasMaxLength(64)
      .IsRequired()
      .HasColumnName("name");
    

    builder.HasMany(c => c.Characters)
      .WithOne(cc => cc.Class)
      .HasForeignKey(cc => cc.ClassId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}