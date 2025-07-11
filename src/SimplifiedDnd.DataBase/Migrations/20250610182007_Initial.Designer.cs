﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SimplifiedDnd.DataBase.Contexts;
using System;

#nullable disable

namespace SimplifiedDnd.DataBase.Migrations;

[DbContext(typeof(MainDbContext))]
[Migration("20250610182007_Initial")]
internal partial class Initial {
  /// <inheritdoc />
  protected override void BuildTargetModel(ModelBuilder modelBuilder) {
#pragma warning disable 612, 618
    modelBuilder
      .HasAnnotation("ProductVersion", "9.0.5")
      .HasAnnotation("Relational:MaxIdentifierLength", 63);

    NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.CharacterClassDbEntity", b => {
      b.Property<Guid>("CharacterId")
        .HasColumnType("uuid")
        .HasColumnName("character_id");

      b.Property<int>("ClassId")
        .HasColumnType("integer")
        .HasColumnName("class_id");

      b.Property<bool>("IsMainClass")
        .ValueGeneratedOnAdd()
        .HasColumnType("boolean")
        .HasDefaultValue(false)
        .HasColumnName("is_main_class");

      b.HasKey("CharacterId", "ClassId");

      b.HasIndex("ClassId");

      b.ToTable("character_classes", (string)null);
    });

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.CharacterDbEntity", b => {
      b.Property<Guid>("Id")
        .HasMaxLength(36)
        .HasColumnType("uuid")
        .HasColumnName("id")
        .IsFixedLength();

      b.Property<string>("Name")
        .IsRequired()
        .HasMaxLength(256)
        .HasColumnType("character varying(256)")
        .HasColumnName("name");

      b.Property<string>("PlayerName")
        .IsRequired()
        .HasMaxLength(256)
        .HasColumnType("character varying(256)")
        .HasColumnName("player_name");

      b.Property<int>("SpecieId")
        .HasColumnType("integer")
        .HasColumnName("specie_id");

      b.HasKey("Id");

      b.HasIndex("SpecieId");

      b.HasIndex("Name", "PlayerName")
        .IsUnique();

      b.ToTable("characters", (string)null);
    });

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.ClassDbEntity", b => {
      b.Property<int>("Id")
        .ValueGeneratedOnAdd()
        .HasColumnType("integer")
        .HasColumnName("id");

      NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

      b.Property<string>("Name")
        .IsRequired()
        .HasMaxLength(64)
        .HasColumnType("character varying(64)")
        .HasColumnName("name");

      b.HasKey("Id");

      b.HasIndex("Name")
        .IsUnique();

      b.ToTable("classes", (string)null);
    });

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.SpecieDbEntity", b => {
      b.Property<int>("Id")
        .ValueGeneratedOnAdd()
        .HasColumnType("integer")
        .HasColumnName("id");

      NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

      b.Property<string>("Name")
        .IsRequired()
        .HasMaxLength(256)
        .HasColumnType("character varying(256)")
        .HasColumnName("name");

      b.Property<int>("Speed")
        .HasColumnType("integer")
        .HasColumnName("speed");

      b.HasKey("Id");

      b.HasIndex("Name")
        .IsUnique();

      b.ToTable("species", (string)null);
    });

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.CharacterClassDbEntity", b => {
      b.HasOne("SimplifiedDnd.DataBase.Entities.CharacterDbEntity", "Character")
        .WithMany("Classes")
        .HasForeignKey("CharacterId")
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();

      b.HasOne("SimplifiedDnd.DataBase.Entities.ClassDbEntity", "Class")
        .WithMany("Characters")
        .HasForeignKey("ClassId")
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();

      b.Navigation("Character");

      b.Navigation("Class");
    });

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.CharacterDbEntity", b => {
      b.HasOne("SimplifiedDnd.DataBase.Entities.SpecieDbEntity", "Specie")
        .WithMany("Characters")
        .HasForeignKey("SpecieId")
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired();

      b.Navigation("Specie");
    });

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.CharacterDbEntity", b => {
      b.Navigation("Classes");
    });

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.ClassDbEntity", b => {
      b.Navigation("Characters");
    });

    modelBuilder.Entity("SimplifiedDnd.DataBase.Entities.SpecieDbEntity", b => {
      b.Navigation("Characters");
    });
#pragma warning restore 612, 618
  }
}