using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WildlifeConservation.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SerialNumber = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Model = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subspecies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subspecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subspecies_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SubspeciesId = table.Column<int>(type: "integer", nullable: false),
                    Sex = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Subspecies_SubspeciesId",
                        column: x => x.SubspeciesId,
                        principalTable: "Subspecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    CollarId = table.Column<int>(type: "integer", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: true),
                    AlertType = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_Collars_CollarId",
                        column: x => x.CollarId,
                        principalTable: "Collars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CollarAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    CollarId = table.Column<int>(type: "integer", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnassignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollarAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollarAssignments_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollarAssignments_Collars_CollarId",
                        column: x => x.CollarId,
                        principalTable: "Collars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocationPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    CollarId = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    Altitude = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SignalType = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationPoints_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationPoints_Collars_CollarId",
                        column: x => x.CollarId,
                        principalTable: "Collars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RangerReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnimalId = table.Column<int>(type: "integer", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ReportType = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangerReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RangerReports_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RangerReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Collars",
                columns: new[] { "Id", "Manufacturer", "Model", "Notes", "SerialNumber", "Status" },
                values: new object[,]
                {
                    { 1, "EcoTelemetry", "SavannaTrack X1", "Solar-assisted battery pack.", "WC-GPS-1001", 1 },
                    { 2, "EcoTelemetry", "SavannaTrack X1", "Configured for elephant migration interval.", "WC-GPS-1002", 1 },
                    { 3, "RangeSense", "PredatorLink P4", "High-frequency predator tracking.", "WC-GPS-2001", 1 },
                    { 4, "RangeSense", "CanidTrail C2", "Cold-weather battery profile.", "WC-GPS-3001", 1 },
                    { 5, "EcoTelemetry", "ForestTrack F2", "Dense canopy satellite profile.", "WC-GPS-1003", 1 }
                });

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Large herbivorous mammals monitored for migration and habitat pressure.", "Elephant" },
                    { 2, "Large carnivores monitored for pride movement and human-wildlife conflict.", "Lion" },
                    { 3, "Social carnivores monitored for range behavior and conservation planning.", "Wolf" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "IsActive", "Role" },
                values: new object[,]
                {
                    { 1, "maya.ranger@example.org", "Maya Nkosi", true, 0 },
                    { 2, "luka.research@example.org", "Luka Petrovic", true, 1 },
                    { 3, "anika.admin@example.org", "Anika Rao", true, 2 }
                });

            migrationBuilder.InsertData(
                table: "RangerReports",
                columns: new[] { "Id", "AnimalId", "CreatedAt", "Description", "Latitude", "Longitude", "ReportType", "Severity", "UserId" },
                values: new object[] { 3, null, new DateTime(2026, 1, 11, 8, 0, 0, 0, DateTimeKind.Utc), "Fresh snares found along dry stream bed.", -1.774000m, 35.097000m, 3, 2, 2 });

            migrationBuilder.InsertData(
                table: "Subspecies",
                columns: new[] { "Id", "Description", "Name", "SpeciesId" },
                values: new object[,]
                {
                    { 1, "Open savanna elephant population.", "African Savanna Elephant", 1 },
                    { 2, "Forest and grassland elephant population.", "Asian Elephant", 1 },
                    { 3, "Lion population tracked across protected ranges.", "East African Lion", 2 },
                    { 4, "Wolf population monitored near reserve corridors.", "Eurasian Wolf", 3 }
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "DateOfBirth", "IsActive", "Name", "Notes", "Sex", "SubspeciesId" },
                values: new object[,]
                {
                    { 1, new DateTime(2014, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, "Amara", "Matriarch of a small herd.", 2, 1 },
                    { 2, new DateTime(2017, 9, 3, 0, 0, 0, 0, DateTimeKind.Utc), true, "Kito", "Often seen near the western watering route.", 1, 1 },
                    { 3, new DateTime(2019, 2, 18, 0, 0, 0, 0, DateTimeKind.Utc), true, "Sera", "Adult lioness in northern pride.", 2, 3 },
                    { 4, null, true, "Batu", "Dispersing male observed near reserve edge.", 1, 4 },
                    { 5, new DateTime(2016, 7, 22, 0, 0, 0, 0, DateTimeKind.Utc), true, "Nila", "Relocated after crop-raiding incident.", 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Alerts",
                columns: new[] { "Id", "AlertType", "AnimalId", "CollarId", "CreatedAt", "CreatedByUserId", "Description", "IsResolved", "ResolvedAt", "Severity" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, new DateTime(2026, 1, 11, 8, 0, 0, 0, DateTimeKind.Utc), null, "Animal moved close to the southern reserve boundary.", false, null, 2 },
                    { 2, 2, 3, 3, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), null, "Collar battery is below threshold.", false, null, 1 },
                    { 3, 4, 4, 4, new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), 1, "Manual watch requested after boundary movement.", true, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 4, 5, 5, null, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), 2, "Review relocation zone condition after ranger report.", false, null, 0 }
                });

            migrationBuilder.InsertData(
                table: "CollarAssignments",
                columns: new[] { "Id", "AnimalId", "AssignedAt", "CollarId", "Notes", "Reason", "UnassignedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), 1, "Initial collaring during health check.", null, null },
                    { 2, 2, new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), 2, "Assigned after herd survey.", null, null },
                    { 3, 3, new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), 3, "Assigned near northern pride range.", null, null },
                    { 4, 4, new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), 4, "Assigned near reserve boundary.", null, null },
                    { 5, 5, new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), 5, "Assigned after relocation release.", null, null }
                });

            migrationBuilder.InsertData(
                table: "LocationPoints",
                columns: new[] { "Id", "Altitude", "AnimalId", "CollarId", "Latitude", "Longitude", "Notes", "RecordedAt", "SignalType" },
                values: new object[,]
                {
                    { 1, 1524.50m, 1, 1, -1.942345m, 35.148900m, "Near southern water point.", new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, 1527.10m, 1, 1, -1.935120m, 35.153420m, null, new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 3, 1495.00m, 2, 2, -1.851100m, 35.221200m, null, new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 4, 1492.20m, 2, 2, -1.844980m, 35.229310m, "Moving east.", new DateTime(2026, 1, 11, 8, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 5, 1411.00m, 3, 3, -1.712420m, 35.012780m, "Northern pride range.", new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 6, 1409.30m, 3, 3, -1.708700m, 35.018050m, null, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 7, 864.00m, 4, 4, 44.138700m, 20.458100m, "Boundary corridor.", new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 8, 872.30m, 4, 4, 44.142900m, 20.462450m, null, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 9, 912.00m, 5, 5, 11.551200m, 76.231500m, "Post-release forest range.", new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 10, 918.80m, 5, 5, 11.557400m, 76.236100m, null, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                table: "RangerReports",
                columns: new[] { "Id", "AnimalId", "CreatedAt", "Description", "Latitude", "Longitude", "ReportType", "Severity", "UserId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), "Herd sighted moving calmly toward grazing area.", -1.935000m, 35.153000m, 0, 0, 1 },
                    { 2, 3, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), "Collar antenna appears tilted but still reporting.", -1.708500m, 35.018000m, 2, 1, 1 },
                    { 4, 4, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), "Tracks and visual confirmation near corridor camera.", 44.142800m, 20.462300m, 0, 0, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_AnimalId",
                table: "Alerts",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CollarId",
                table: "Alerts",
                column: "CollarId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CreatedByUserId",
                table: "Alerts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_SubspeciesId",
                table: "Animals",
                column: "SubspeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_CollarAssignments_AnimalId",
                table: "CollarAssignments",
                column: "AnimalId",
                unique: true,
                filter: "\"UnassignedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CollarAssignments_CollarId",
                table: "CollarAssignments",
                column: "CollarId",
                unique: true,
                filter: "\"UnassignedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Collars_SerialNumber",
                table: "Collars",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationPoints_AnimalId_RecordedAt",
                table: "LocationPoints",
                columns: new[] { "AnimalId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationPoints_CollarId_RecordedAt",
                table: "LocationPoints",
                columns: new[] { "CollarId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RangerReports_AnimalId",
                table: "RangerReports",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_RangerReports_UserId",
                table: "RangerReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Name",
                table: "Species",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subspecies_SpeciesId_Name",
                table: "Subspecies",
                columns: new[] { "SpeciesId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "CollarAssignments");

            migrationBuilder.DropTable(
                name: "LocationPoints");

            migrationBuilder.DropTable(
                name: "RangerReports");

            migrationBuilder.DropTable(
                name: "Collars");

            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Subspecies");

            migrationBuilder.DropTable(
                name: "Species");
        }
    }
}
