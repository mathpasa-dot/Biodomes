using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BioDomes.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResearchTeam = table.Column<string>(type: "TEXT", nullable: true),
                    UserAvatar = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.MaterialId);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesRegimes",
                columns: table => new
                {
                    RegimeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Label = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesRegimes", x => x.RegimeId);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesType",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Label = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesType", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Biomes",
                columns: table => new
                {
                    BiomeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Temperature = table.Column<string>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    Humidity = table.Column<string>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biomes", x => x.BiomeId);
                    table.ForeignKey(
                        name: "FK_Biomes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.EquipmentId);
                    table.ForeignKey(
                        name: "FK_Equipments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: true),
                    Height = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    RegimeId = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.SpeciesId);
                    table.ForeignKey(
                        name: "FK_Species_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Species_SpeciesRegimes_RegimeId",
                        column: x => x.RegimeId,
                        principalTable: "SpeciesRegimes",
                        principalColumn: "RegimeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Species_SpeciesType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "SpeciesType",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BiomeEquipments",
                columns: table => new
                {
                    BiomeId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiomeEquipments", x => new { x.BiomeId, x.EquipmentId });
                    table.ForeignKey(
                        name: "FK_BiomeEquipments_Biomes_BiomeId",
                        column: x => x.BiomeId,
                        principalTable: "Biomes",
                        principalColumn: "BiomeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BiomeEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentMaterials",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaterialId = table.Column<int>(type: "INTEGER", nullable: false),
                    FlowType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentMaterials", x => new { x.EquipmentId, x.MaterialId, x.FlowType });
                    table.ForeignKey(
                        name: "FK_EquipmentMaterials_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BiomeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BiomeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Delta = table.Column<int>(type: "INTEGER", nullable: false),
                    NewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiomeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BiomeHistory_Biomes_BiomeId",
                        column: x => x.BiomeId,
                        principalTable: "Biomes",
                        principalColumn: "BiomeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BiomeHistory_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BiomeSpecies",
                columns: table => new
                {
                    BiomeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiomeSpecies", x => new { x.BiomeId, x.SpeciesId });
                    table.ForeignKey(
                        name: "FK_BiomeSpecies_Biomes_BiomeId",
                        column: x => x.BiomeId,
                        principalTable: "Biomes",
                        principalColumn: "BiomeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BiomeSpecies_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredationHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BiomeId = table.Column<int>(type: "INTEGER", nullable: false),
                    PredatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredationHistory_Biomes_BiomeId",
                        column: x => x.BiomeId,
                        principalTable: "Biomes",
                        principalColumn: "BiomeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PredationHistory_Species_PredatorId",
                        column: x => x.PredatorId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PredationHistory_Species_PreyId",
                        column: x => x.PreyId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Predations",
                columns: table => new
                {
                    PredatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    PreyId = table.Column<int>(type: "INTEGER", nullable: false),
                    BiomeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predations", x => new { x.PredatorId, x.PreyId, x.BiomeId });
                    table.ForeignKey(
                        name: "FK_Predations_Biomes_BiomeId",
                        column: x => x.BiomeId,
                        principalTable: "Biomes",
                        principalColumn: "BiomeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Predations_Species_PredatorId",
                        column: x => x.PredatorId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Predations_Species_PreyId",
                        column: x => x.PreyId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a1b2c3d4-0000-4000-8000-000000000001", "a1b2c3d4-0000-4000-8000-000000000001", "Admin", "ADMIN" },
                    { "a1b2c3d4-0000-4000-8000-000000000002", "a1b2c3d4-0000-4000-8000-000000000002", "Employee", "EMPLOYEE" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DateOfBirth", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ResearchTeam", "SecurityStamp", "TwoFactorEnabled", "UserAvatar", "UserName" },
                values: new object[] { "SYSTEM", 0, "dc4913f9-337c-4359-a0e4-72ba47515195", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "system@local", true, true, new DateTimeOffset(new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new TimeSpan(0, 0, 0, 0, 0)), "SYSTEM@LOCAL", "SYSTEM@LOCAL", null, null, false, "", "SYSTEM_STAMP", false, "", "system@local" });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "MaterialId", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "Lumière", "lumiere" },
                    { 2, "Eau", "eau" },
                    { 3, "Azote", "azote" },
                    { 4, "Hydrogène", "hydrogene" },
                    { 5, "Électricité", "electricite" },
                    { 6, "Chaleur", "chaleur" }
                });

            migrationBuilder.InsertData(
                table: "SpeciesRegimes",
                columns: new[] { "RegimeId", "Label" },
                values: new object[,]
                {
                    { 1, "Photosynthèse" },
                    { 2, "Herbivore" },
                    { 3, "Carnivore" },
                    { 4, "Omnivore" },
                    { 5, "Coprophages" },
                    { 6, "Limivores" },
                    { 7, "Detritivores" },
                    { 8, "Heterotrophie" }
                });

            migrationBuilder.InsertData(
                table: "SpeciesType",
                columns: new[] { "TypeId", "Label" },
                values: new object[,]
                {
                    { 1, "Mammifères" },
                    { 2, "Oiseaux" },
                    { 3, "Poissons" },
                    { 4, "Reptiles" },
                    { 5, "Amphibiens" },
                    { 6, "Crustacés" },
                    { 7, "Arachnides" },
                    { 8, "Insectes" },
                    { 9, "Myriapodes" },
                    { 10, "Mollusques" },
                    { 11, "Echinodermes" },
                    { 12, "Anélides" },
                    { 13, "Cnidaires" },
                    { 14, "Plathelminthes" },
                    { 15, "Porifères" },
                    { 16, "Plantes" },
                    { 17, "Mousses" },
                    { 18, "Algues" },
                    { 19, "Lichen" },
                    { 20, "Champignons" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a1b2c3d4-0000-4000-8000-000000000002", "SYSTEM" });

            migrationBuilder.InsertData(
                table: "Biomes",
                columns: new[] { "BiomeId", "Humidity", "Name", "Slug", "Temperature", "UserId" },
                values: new object[,]
                {
                    { 1, "12.0", "Experimental Ecosystem", "experimental-ecosystem", "20.0", "SYSTEM" },
                    { 2, "5.0", "Désert", "desert", "38.0", "SYSTEM" },
                    { 3, "10.0", "Campagne", "campagne", "18.0", "SYSTEM" },
                    { 4, "9.0", "Forêt tempérée", "foret-temperee", "12.0", "SYSTEM" },
                    { 5, "22.0", "Forêt tropicale", "foret-tropicale", "27.5", "SYSTEM" },
                    { 6, "5.0", "Montagne", "montagne", "5.0", "SYSTEM" }
                });

            migrationBuilder.InsertData(
                table: "Equipments",
                columns: new[] { "EquipmentId", "ImagePath", "IsAvailable", "Name", "Slug", "UserId" },
                values: new object[,]
                {
                    { 1, "/images/equipments/lampe.jpg", true, "Lampe", "lampe", "SYSTEM" },
                    { 2, "/images/equipments/pompe.jpg", true, "Pompe", "pompe", "SYSTEM" },
                    { 3, "/images/equipments/generateur-azote.jpg", true, "Générateur azote", "generateur-azote", "SYSTEM" },
                    { 4, "/images/equipments/generateur-hydrogene.jpg", true, "Générateur hydrogène", "generateur-hydrogene", "SYSTEM" }
                });

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "SpeciesId", "Height", "ImagePath", "IsAvailable", "Name", "RegimeId", "Slug", "TypeId", "UserId", "Weight" },
                values: new object[,]
                {
                    { 1, 180, "/images/species/lion.jpg", true, "Lion", 3, "lion", 1, "SYSTEM", 150.0 },
                    { 2, 225, "/images/species/boa.png", true, "Boa", 3, "boa", 4, "SYSTEM", 13.0 },
                    { 3, 600, "/images/species/hetre.jpg", true, "Hetre", 1, "hetre", 16, "SYSTEM", 830.0 },
                    { 4, 2500, "/images/species/chene.jpg", true, "Chene", 1, "chene", 16, "SYSTEM", 2100.0 },
                    { 5, 1, "/images/species/mouche.jpg", true, "Mouche", 5, "mouche", 8, "SYSTEM", 0.001 },
                    { 6, 4, "/images/species/araignee.jpg", true, "Araignée", 3, "araignee", 7, "SYSTEM", 0.095000000000000001 },
                    { 7, 18, "/images/species/hirondelle.jpg", true, "Hirondelle", 3, "hirondelle", 2, "SYSTEM", 0.044999999999999998 },
                    { 8, 50, "/images/species/lievre.jpg", true, "Lièvre", 2, "lievre", 1, "SYSTEM", 4.0 },
                    { 9, 23, "/images/species/paturin-des-pres.png", true, "Pâturin des prés", 1, "paturin-des-pres", 16, "SYSTEM", 0.001 },
                    { 10, 9, "/images/species/souris.jpg", true, "Souris", 4, "souris", 1, "SYSTEM", 0.025000000000000001 },
                    { 11, 5, "/images/species/scorpion.jpg", true, "Scorpion", 3, "scorpion", 7, "SYSTEM", 0.02 },
                    { 12, 40, "/images/species/fennec.jpg", true, "Fennec", 4, "fennec", 1, "SYSTEM", 1.5 },
                    { 13, 10, "/images/species/gerboise.jpg", true, "Gerboise", 2, "gerboise", 1, "SYSTEM", 0.10000000000000001 },
                    { 14, 120, "/images/species/serpent-a-sonnette.jpg", true, "Serpent à sonnette", 3, "serpent-a-sonnette", 4, "SYSTEM", 2.0 },
                    { 15, 200, "/images/species/cactus.jpg", true, "Cactus", 1, "cactus", 16, "SYSTEM", 500.0 },
                    { 16, 40, "/images/species/renard.jpg", true, "Renard", 3, "renard", 1, "SYSTEM", 6.0 },
                    { 17, 10, "/images/species/verre-de-terre.jpg", true, "Ver de terre", 7, "ver-de-terre", 12, "SYSTEM", 0.002 },
                    { 18, 45, "/images/species/poule.jpg", true, "Poule", 4, "poule", 2, "SYSTEM", 2.0 },
                    { 19, 100, "/images/species/ble.jpg", true, "Blé", 1, "ble", 16, "SYSTEM", 0.002 },
                    { 20, 150, "/images/species/cerf.jpg", true, "Cerf", 2, "cerf", 1, "SYSTEM", 150.0 },
                    { 21, 80, "/images/species/loup.jpg", true, "Loup", 3, "loup", 1, "SYSTEM", 40.0 },
                    { 22, 10, "/images/species/champignon-forestier.jpg", true, "Champignon forestier", 7, "champignon-forestier", 20, "SYSTEM", 0.5 },
                    { 23, 70, "/images/species/jaguar.jpg", true, "Jaguar", 3, "jaguar", 1, "SYSTEM", 100.0 },
                    { 24, 60, "/images/species/singe.jpg", true, "Singe", 4, "singe", 1, "SYSTEM", 10.0 },
                    { 25, 10, "/images/species/grenouille.jpg", true, "Grenouille", 3, "grenouille", 5, "SYSTEM", 0.5 },
                    { 26, 20, "/images/species/banane.jpg", true, "Banane", 1, "banane", 16, "SYSTEM", 0.14999999999999999 },
                    { 27, 70, "/images/species/aigle.jpg", true, "Aigle", 3, "aigle", 2, "SYSTEM", 5.0 },
                    { 28, 90, "/images/species/chamois.jpg", true, "Chamois", 2, "chamois", 1, "SYSTEM", 50.0 },
                    { 29, 30, "/images/species/herbes-alpines.jpg", true, "Herbes alpines", 1, "herbes-alpines", 16, "SYSTEM", 0.5 }
                });

            migrationBuilder.InsertData(
                table: "BiomeEquipments",
                columns: new[] { "BiomeId", "EquipmentId", "Count" },
                values: new object[,]
                {
                    { 1, 1, 10 },
                    { 3, 1, 10 },
                    { 4, 1, 10 },
                    { 5, 1, 10 },
                    { 6, 1, 10 }
                });

            migrationBuilder.InsertData(
                table: "BiomeSpecies",
                columns: new[] { "BiomeId", "SpeciesId", "Count" },
                values: new object[,]
                {
                    { 1, 1, 2 },
                    { 1, 2, 23 },
                    { 1, 3, 120 },
                    { 1, 4, 100 },
                    { 1, 5, 90000 },
                    { 1, 6, 474 },
                    { 1, 7, 1000 },
                    { 1, 8, 150 },
                    { 1, 9, 899000 },
                    { 1, 10, 11960 },
                    { 2, 11, 80 },
                    { 2, 12, 10 },
                    { 2, 13, 120 },
                    { 2, 14, 25 },
                    { 2, 15, 300 },
                    { 3, 10, 2400 },
                    { 3, 16, 10 },
                    { 3, 17, 30000 },
                    { 3, 18, 30 },
                    { 3, 19, 30000 },
                    { 4, 4, 200 },
                    { 4, 20, 80 },
                    { 4, 21, 20 },
                    { 4, 22, 150 },
                    { 5, 5, 100000 },
                    { 5, 23, 1 },
                    { 5, 24, 10 },
                    { 5, 25, 200 },
                    { 5, 26, 1334 },
                    { 6, 27, 12 },
                    { 6, 28, 60 },
                    { 6, 29, 400 }
                });

            migrationBuilder.InsertData(
                table: "EquipmentMaterials",
                columns: new[] { "EquipmentId", "FlowType", "MaterialId" },
                values: new object[,]
                {
                    { 1, "Produced", 1 },
                    { 1, "Consumed", 5 },
                    { 1, "Produced", 6 },
                    { 2, "Consumed", 2 },
                    { 2, "Produced", 2 },
                    { 2, "Consumed", 5 },
                    { 2, "Produced", 6 },
                    { 3, "Produced", 3 },
                    { 3, "Consumed", 5 },
                    { 4, "Produced", 4 },
                    { 4, "Consumed", 5 }
                });

            migrationBuilder.InsertData(
                table: "Predations",
                columns: new[] { "BiomeId", "PredatorId", "PreyId" },
                values: new object[,]
                {
                    { 1, 1, 2 },
                    { 1, 1, 8 },
                    { 1, 2, 8 },
                    { 1, 2, 10 },
                    { 1, 6, 5 },
                    { 1, 7, 5 },
                    { 1, 7, 6 },
                    { 1, 8, 9 },
                    { 1, 10, 9 },
                    { 3, 10, 19 },
                    { 2, 12, 11 },
                    { 2, 12, 13 },
                    { 2, 14, 13 },
                    { 3, 16, 10 },
                    { 3, 16, 18 },
                    { 3, 17, 19 },
                    { 3, 18, 17 },
                    { 3, 18, 19 },
                    { 4, 21, 20 },
                    { 5, 23, 24 },
                    { 5, 24, 25 },
                    { 5, 24, 26 },
                    { 5, 25, 5 },
                    { 5, 25, 26 },
                    { 6, 27, 28 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiomeEquipments_EquipmentId",
                table: "BiomeEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BiomeHistory_BiomeId_SpeciesId_Timestamp",
                table: "BiomeHistory",
                columns: new[] { "BiomeId", "SpeciesId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_BiomeHistory_SpeciesId",
                table: "BiomeHistory",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Biomes_UserId",
                table: "Biomes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BiomeSpecies_SpeciesId",
                table: "BiomeSpecies",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentMaterials_MaterialId",
                table: "EquipmentMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_UserId",
                table: "Equipments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PredationHistory_BiomeId_PredatorId_PreyId",
                table: "PredationHistory",
                columns: new[] { "BiomeId", "PredatorId", "PreyId" });

            migrationBuilder.CreateIndex(
                name: "IX_PredationHistory_BiomeId_Timestamp",
                table: "PredationHistory",
                columns: new[] { "BiomeId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_PredationHistory_PredatorId",
                table: "PredationHistory",
                column: "PredatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PredationHistory_PreyId",
                table: "PredationHistory",
                column: "PreyId");

            migrationBuilder.CreateIndex(
                name: "IX_Predations_BiomeId",
                table: "Predations",
                column: "BiomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Predations_PreyId",
                table: "Predations",
                column: "PreyId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_RegimeId",
                table: "Species",
                column: "RegimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_TypeId",
                table: "Species",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_UserId",
                table: "Species",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BiomeEquipments");

            migrationBuilder.DropTable(
                name: "BiomeHistory");

            migrationBuilder.DropTable(
                name: "BiomeSpecies");

            migrationBuilder.DropTable(
                name: "EquipmentMaterials");

            migrationBuilder.DropTable(
                name: "PredationHistory");

            migrationBuilder.DropTable(
                name: "Predations");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Biomes");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SpeciesRegimes");

            migrationBuilder.DropTable(
                name: "SpeciesType");
        }
    }
}
