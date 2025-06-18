using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.Diagnostics;

#nullable disable

namespace SimplifiedDnd.DataBase.Migrations;

/// <inheritdoc />
internal partial class Initial : Migration {
  /// <summary>
  /// Applies the initial database schema for the application, creating tables for classes, species, characters, and their relationships, along with relevant indexes and constraints.
  /// </summary>
  protected override void Up(MigrationBuilder migrationBuilder) {
    Debug.Assert(migrationBuilder is not null);

    migrationBuilder.CreateTable(
      name: "classes",
      columns: table => new {
        id = table.Column<int>(type: "integer", nullable: false)
          .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
        name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
      },
      constraints: table => table.PrimaryKey("PK_classes", x => x.id));

    migrationBuilder.CreateTable(
      name: "species",
      columns: table => new {
        id = table.Column<int>(type: "integer", nullable: false)
          .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
        name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
        speed = table.Column<int>(type: "integer", nullable: false)
      },
      constraints: table => table.PrimaryKey("PK_species", x => x.id));

    migrationBuilder.CreateTable(
      name: "characters",
      columns: table => new {
        id = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 36, nullable: false),
        name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
        player_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
        specie_id = table.Column<int>(type: "integer", nullable: false)
      },
      constraints: table => {
        table.PrimaryKey("PK_characters", x => x.id);
        table.ForeignKey(
          name: "FK_characters_species_specie_id",
          column: x => x.specie_id,
          principalTable: "species",
          principalColumn: "id",
          onDelete: ReferentialAction.Restrict);
      });

    migrationBuilder.CreateTable(
      name: "character_classes",
      columns: table => new {
        character_id = table.Column<Guid>(type: "uuid", nullable: false),
        class_id = table.Column<int>(type: "integer", nullable: false),
        is_main_class = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
      },
      constraints: table => {
        table.PrimaryKey("PK_character_classes", x => new { x.character_id, x.class_id });
        table.ForeignKey(
          name: "FK_character_classes_characters_character_id",
          column: x => x.character_id,
          principalTable: "characters",
          principalColumn: "id",
          onDelete: ReferentialAction.Cascade);
        table.ForeignKey(
          name: "FK_character_classes_classes_class_id",
          column: x => x.class_id,
          principalTable: "classes",
          principalColumn: "id",
          onDelete: ReferentialAction.Cascade);
      });

    migrationBuilder.CreateIndex(
      name: "IX_character_classes_class_id",
      table: "character_classes",
      column: "class_id");

    migrationBuilder.CreateIndex(
      name: "IX_characters_name_player_name",
      table: "characters",
      columns: ["name", "player_name"],
      unique: true);

    migrationBuilder.CreateIndex(
      name: "IX_characters_specie_id",
      table: "characters",
      column: "specie_id");

    migrationBuilder.CreateIndex(
      name: "IX_classes_name",
      table: "classes",
      column: "name",
      unique: true);

    migrationBuilder.CreateIndex(
      name: "IX_species_name",
      table: "species",
      column: "name",
      unique: true);
  }

  /// <summary>
  /// Reverts the initial database schema by dropping the character_classes, characters, classes, and species tables.
  /// </summary>
  protected override void Down(MigrationBuilder migrationBuilder) {
    Debug.Assert(migrationBuilder is not null);

    migrationBuilder.DropTable(name: "character_classes");
    migrationBuilder.DropTable(name: "characters");
    migrationBuilder.DropTable(name: "classes");
    migrationBuilder.DropTable(name: "species");
  }
}