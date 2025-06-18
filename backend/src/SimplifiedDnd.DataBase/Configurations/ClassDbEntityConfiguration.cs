using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplifiedDnd.DataBase.Entities;

namespace SimplifiedDnd.DataBase.Configurations;

internal class ClassDbEntityConfiguration : IEntityTypeConfiguration<ClassDbEntity> {
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