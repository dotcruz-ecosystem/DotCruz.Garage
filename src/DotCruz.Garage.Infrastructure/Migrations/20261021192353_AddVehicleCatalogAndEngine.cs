using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotCruz.Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleCatalogAndEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "engine",
                table: "vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "vehicle_brands",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_engines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_engines", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_models",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_models", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicle_models_vehicle_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "vehicle_brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_brands_name",
                table: "vehicle_brands",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_engines_name_type",
                table: "vehicle_engines",
                columns: new[] { "name", "type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_models_brand_id_type_name",
                table: "vehicle_models",
                columns: new[] { "brand_id", "type", "name" },
                unique: true);

            SeedInitialCatalog(migrationBuilder);
        }

        private static void SeedInitialCatalog(MigrationBuilder migrationBuilder)
        {
            var now = DateTimeOffset.UtcNow;

            void InsertBrand(Guid id, string name)
            {
                migrationBuilder.InsertData(
                    table: "vehicle_brands",
                    columns: new[] { "id", "name", "created_at", "updated_at", "deleted_at" },
                    values: new object[] { id, name, now, now, null! });
            }

            void InsertModel(Guid brandId, string name, int type)
            {
                migrationBuilder.InsertData(
                    table: "vehicle_models",
                    columns: new[] { "id", "brand_id", "name", "type", "created_at", "updated_at", "deleted_at" },
                    values: new object[] { Guid.NewGuid(), brandId, name, type, now, now, null! });
            }

            void InsertEngine(string name, int? type)
            {
                migrationBuilder.InsertData(
                    table: "vehicle_engines",
                    columns: new[] { "id", "name", "type", "created_at", "updated_at", "deleted_at" },
                    values: new object[] { Guid.NewGuid(), name, type.HasValue ? type.Value : null!, now, now, null! });
            }

            // --- CARS (Type = 1) ---
            var fiatId = Guid.NewGuid();
            InsertBrand(fiatId, "Fiat");
            foreach (var m in new[] { "Strada", "Toro", "Argo", "Mobi", "Pulse", "Fastback", "Fiorino", "Cronos", "Titano", "Uno", "Palio", "Siena" })
                InsertModel(fiatId, m, 1);

            var vwId = Guid.NewGuid();
            InsertBrand(vwId, "Volkswagen");
            foreach (var m in new[] { "Polo", "T-Cross", "Nivus", "Saveiro", "Virtus", "Taos", "Amarok", "Gol", "Voyage", "Fox", "Jetta", "Golf" })
                InsertModel(vwId, m, 1);

            var gmId = Guid.NewGuid();
            InsertBrand(gmId, "Chevrolet");
            foreach (var m in new[] { "Onix", "Onix Plus", "Tracker", "S10", "Montana", "Spin", "Cruze" })
                InsertModel(gmId, m, 1);

            var toyotaId = Guid.NewGuid();
            InsertBrand(toyotaId, "Toyota");
            foreach (var m in new[] { "Hilux", "Corolla", "Corolla Cross", "Yaris Hatch", "Yaris Sedan", "SW4", "RAV4" })
                InsertModel(toyotaId, m, 1);

            var hyundaiId = Guid.NewGuid();
            InsertBrand(hyundaiId, "Hyundai");
            foreach (var m in new[] { "HB20", "HB20S", "Creta", "Tucson" })
                InsertModel(hyundaiId, m, 1);

            var renaultId = Guid.NewGuid();
            InsertBrand(renaultId, "Renault");
            foreach (var m in new[] { "Kwid", "Duster", "Kardian", "Oroch", "Sandero", "Logan", "Master" })
                InsertModel(renaultId, m, 1);

            var jeepId = Guid.NewGuid();
            InsertBrand(jeepId, "Jeep");
            foreach (var m in new[] { "Renegade", "Compass", "Commander", "Wrangler" })
                InsertModel(jeepId, m, 1);

            var nissanId = Guid.NewGuid();
            InsertBrand(nissanId, "Nissan");
            foreach (var m in new[] { "Kicks", "Frontier", "Versa", "Sentra" })
                InsertModel(nissanId, m, 1);

            var fordId = Guid.NewGuid();
            InsertBrand(fordId, "Ford");
            foreach (var m in new[] { "Ranger", "Maverick", "Territory", "Bronco Sport", "Ka", "EcoSport" })
                InsertModel(fordId, m, 1);

            var bydId = Guid.NewGuid();
            InsertBrand(bydId, "BYD");
            foreach (var m in new[] { "Dolphin", "Dolphin Mini", "Song Plus", "Yuan Plus", "Seal" })
                InsertModel(bydId, m, 1);

            var gwmId = Guid.NewGuid();
            InsertBrand(gwmId, "GWM");
            foreach (var m in new[] { "Haval H6", "Ora 03" })
                InsertModel(gwmId, m, 1);

            var audiId = Guid.NewGuid();
            InsertBrand(audiId, "Audi");
            foreach (var m in new[] { "A3", "A4", "Q3", "Q5" })
                InsertModel(audiId, m, 1);

            var mercedesId = Guid.NewGuid();
            InsertBrand(mercedesId, "Mercedes-Benz");
            foreach (var m in new[] { "Classe C", "Classe A", "GLA", "GLB", "Sprinter" })
                InsertModel(mercedesId, m, 1);

            // --- CARS & MOTOS (Hybrid Brands) ---
            var hondaId = Guid.NewGuid();
            InsertBrand(hondaId, "Honda");
            foreach (var m in new[] { "HR-V", "Civic", "City Sedan", "City Hatchback", "CR-V", "Fit", "WR-V" })
                InsertModel(hondaId, m, 1);
            foreach (var m in new[] { "CG 160 Titan", "CG 160 Fan", "CG 160 Start", "Biz 125", "Biz 110i", "NXR 160 Bros", "Pop 110i", "CB 300F Twister", "CB 500F", "CB 500X", "XRE 300", "XRE 190", "PCX 160", "ADV 150", "Hornet 600", "CBR 650R", "NC 750X", "Sahara 300" })
                InsertModel(hondaId, m, 2);

            var bmwId = Guid.NewGuid();
            InsertBrand(bmwId, "BMW");
            foreach (var m in new[] { "Série 3", "X1", "X3", "X5" })
                InsertModel(bmwId, m, 1);
            foreach (var m in new[] { "R 1250 GS", "G 310 R", "G 310 GS", "F 850 GS", "F 900 R", "S 1000 RR" })
                InsertModel(bmwId, m, 2);

            var suzukiId = Guid.NewGuid();
            InsertBrand(suzukiId, "Suzuki");
            foreach (var m in new[] { "Jimny", "Vitara" })
                InsertModel(suzukiId, m, 1);
            foreach (var m in new[] { "GSX-S750", "V-Strom 650", "Hayabusa", "Burgman 125", "Yes 125" })
                InsertModel(suzukiId, m, 2);

            // --- MOTOS (Type = 2) ---
            var yamahaId = Guid.NewGuid();
            InsertBrand(yamahaId, "Yamaha");
            foreach (var m in new[] { "Factor 150", "Factor 125", "Fazer 150", "Fazer FZ25", "Crosser 150", "Lander 250", "MT-03", "MT-07", "MT-09", "NMAX 160", "Fluo 125", "R3", "XMAX 250" })
                InsertModel(yamahaId, m, 2);

            var kawasakiId = Guid.NewGuid();
            InsertBrand(kawasakiId, "Kawasaki");
            foreach (var m in new[] { "Ninja 300", "Ninja 400", "Z400", "Z900", "Versys 650", "Vulcan S" })
                InsertModel(kawasakiId, m, 2);

            var royalId = Guid.NewGuid();
            InsertBrand(royalId, "Royal Enfield");
            foreach (var m in new[] { "Hunter 350", "Classic 350", "Meteor 350", "Himalayan 411", "Himalayan 450", "Interceptor 650", "Continental GT 650" })
                InsertModel(royalId, m, 2);

            var triumphId = Guid.NewGuid();
            InsertBrand(triumphId, "Triumph");
            foreach (var m in new[] { "Tiger 900", "Tiger 1200", "Street Triple", "Bonneville T100" })
                InsertModel(triumphId, m, 2);

            var shinerayId = Guid.NewGuid();
            InsertBrand(shinerayId, "Shineray");
            foreach (var m in new[] { "Worker 125", "Jet 125", "Phoenix 50", "SH 175" })
                InsertModel(shinerayId, m, 2);

            var haojueId = Guid.NewGuid();
            InsertBrand(haojueId, "Haojue");
            foreach (var m in new[] { "DR 160", "Chopper Road 150", "Lindy 125", "NK 150" })
                InsertModel(haojueId, m, 2);

            // --- ENGINES FOR CARS (Type = 1) ---
            foreach (var eng in new[] { "1.0", "1.0 Turbo", "1.2 Turbo", "1.3", "1.3 Turbo", "1.4", "1.4 Turbo", "1.5", "1.5 Turbo", "1.6", "1.6 16V", "1.8", "2.0", "2.0 Turbo", "2.5", "2.8 Turbo Diesel", "3.0", "3.0 V6", "V6", "V8", "Híbrido", "100% Elétrico" })
                InsertEngine(eng, 1);

            // --- ENGINES FOR MOTOS (Type = 2) ---
            foreach (var eng in new[] { "50cc", "100cc", "110cc", "125cc", "150cc", "160cc", "190cc", "200cc", "250cc", "300cc", "350cc", "400cc", "450cc", "500cc", "600cc", "650cc", "700cc", "750cc", "800cc", "900cc", "1000cc", "1100cc", "1200cc", "1250cc", "1300cc", "Elétrica" })
                InsertEngine(eng, 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicle_engines");

            migrationBuilder.DropTable(
                name: "vehicle_models");

            migrationBuilder.DropTable(
                name: "vehicle_brands");

            migrationBuilder.DropColumn(
                name: "engine",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "type",
                table: "vehicles");
        }
    }
}
