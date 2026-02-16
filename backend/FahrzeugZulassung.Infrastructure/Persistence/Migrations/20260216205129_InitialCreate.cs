using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FahrzeugZulassung.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fahrzeuge",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FIN = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    Kennzeichen = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Marke = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Modell = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Erstzulassung = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Farbe = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Hubraum = table.Column<int>(type: "integer", nullable: true),
                    Leistung = table.Column<int>(type: "integer", nullable: true),
                    Kraftstoffart = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ErstelltAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeaendertAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fahrzeuge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Standorte",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Strasse = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Hausnummer = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PLZ = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Ort = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IstAktiv = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ErstelltAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeaendertAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Standorte", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Vorname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nachname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rolle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StandortId = table.Column<Guid>(type: "uuid", nullable: true),
                    IstAktiv = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FehlgeschlageneLoginVersuche = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    GesperrtBis = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErstelltAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeaendertAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Standorte_StandortId",
                        column: x => x.StandortId,
                        principalTable: "Standorte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
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
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BenutzerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Aktion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Entitaet = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntitaetId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AlteWerte = table.Column<string>(type: "text", nullable: true),
                    NeueWerte = table.Column<string>(type: "text", nullable: true),
                    IPAdresse = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Zeitstempel = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_BenutzerId",
                        column: x => x.BenutzerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Kunden",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BenutzerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Vorname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nachname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Strasse = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Hausnummer = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PLZ = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Ort = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Geburtsdatum = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Telefon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DatenschutzAkzeptiert = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DatenschutzAkzeptiertAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErstelltAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeaendertAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kunden", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kunden_AspNetUsers_BenutzerId",
                        column: x => x.BenutzerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auftraege",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuftragNummer = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Typ = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Entwurf"),
                    KundeId = table.Column<Guid>(type: "uuid", nullable: false),
                    FahrzeugId = table.Column<Guid>(type: "uuid", nullable: false),
                    StandortId = table.Column<Guid>(type: "uuid", nullable: false),
                    ErstelltVonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Step1Abgeschlossen = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Step2Abgeschlossen = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Step3Abgeschlossen = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AGBAkzeptiert = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AGBAkzeptiertAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnterschriftVorhanden = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UnterschriftAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IKfzReferenz = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AnIKfzGesendetAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IKfzAntwort = table.Column<string>(type: "text", nullable: true),
                    Bemerkungen = table.Column<string>(type: "text", nullable: true),
                    ErstelltAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeaendertAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AbgeschlossenAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auftraege", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auftraege_AspNetUsers_ErstelltVonId",
                        column: x => x.ErstelltVonId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auftraege_Fahrzeuge_FahrzeugId",
                        column: x => x.FahrzeugId,
                        principalTable: "Fahrzeuge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auftraege_Kunden_KundeId",
                        column: x => x.KundeId,
                        principalTable: "Kunden",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Auftraege_Standorte_StandortId",
                        column: x => x.StandortId,
                        principalTable: "Standorte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dokumente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuftragId = table.Column<Guid>(type: "uuid", nullable: false),
                    Typ = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DateiName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DateiPfad = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateiGroesse = table.Column<long>(type: "bigint", nullable: false),
                    HochgeladenVonId = table.Column<Guid>(type: "uuid", nullable: false),
                    HochgeladenAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dokumente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dokumente_AspNetUsers_HochgeladenVonId",
                        column: x => x.HochgeladenVonId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dokumente_Auftraege_AuftragId",
                        column: x => x.AuftragId,
                        principalTable: "Auftraege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rechnungen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RechnungNummer = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AuftragId = table.Column<Guid>(type: "uuid", nullable: false),
                    Betrag = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Offen"),
                    Faellig = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BezahltAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Zahlungsmethode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ZahlungsReferenz = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ErstelltAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeaendertAm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rechnungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rechnungen_Auftraege_AuftragId",
                        column: x => x.AuftragId,
                        principalTable: "Auftraege",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StandortId",
                table: "AspNetUsers",
                column: "StandortId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Aktion",
                table: "AuditLogs",
                column: "Aktion");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_BenutzerId",
                table: "AuditLogs",
                column: "BenutzerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Entitaet",
                table: "AuditLogs",
                column: "Entitaet");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Entitaet_EntitaetId",
                table: "AuditLogs",
                columns: new[] { "Entitaet", "EntitaetId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntitaetId",
                table: "AuditLogs",
                column: "EntitaetId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Zeitstempel",
                table: "AuditLogs",
                column: "Zeitstempel");

            migrationBuilder.CreateIndex(
                name: "IX_Auftraege_AuftragNummer",
                table: "Auftraege",
                column: "AuftragNummer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auftraege_ErstelltAm",
                table: "Auftraege",
                column: "ErstelltAm");

            migrationBuilder.CreateIndex(
                name: "IX_Auftraege_ErstelltVonId",
                table: "Auftraege",
                column: "ErstelltVonId");

            migrationBuilder.CreateIndex(
                name: "IX_Auftraege_FahrzeugId",
                table: "Auftraege",
                column: "FahrzeugId");

            migrationBuilder.CreateIndex(
                name: "IX_Auftraege_IKfzReferenz",
                table: "Auftraege",
                column: "IKfzReferenz");

            migrationBuilder.CreateIndex(
                name: "IX_Auftraege_KundeId",
                table: "Auftraege",
                column: "KundeId");

            migrationBuilder.CreateIndex(
                name: "IX_Auftraege_StandortId",
                table: "Auftraege",
                column: "StandortId");

            migrationBuilder.CreateIndex(
                name: "IX_Auftraege_Status",
                table: "Auftraege",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Dokumente_AuftragId",
                table: "Dokumente",
                column: "AuftragId");

            migrationBuilder.CreateIndex(
                name: "IX_Dokumente_HochgeladenAm",
                table: "Dokumente",
                column: "HochgeladenAm");

            migrationBuilder.CreateIndex(
                name: "IX_Dokumente_HochgeladenVonId",
                table: "Dokumente",
                column: "HochgeladenVonId");

            migrationBuilder.CreateIndex(
                name: "IX_Dokumente_Typ",
                table: "Dokumente",
                column: "Typ");

            migrationBuilder.CreateIndex(
                name: "IX_Fahrzeuge_FIN",
                table: "Fahrzeuge",
                column: "FIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fahrzeuge_Kennzeichen",
                table: "Fahrzeuge",
                column: "Kennzeichen");

            migrationBuilder.CreateIndex(
                name: "IX_Fahrzeuge_Marke_Modell",
                table: "Fahrzeuge",
                columns: new[] { "Marke", "Modell" });

            migrationBuilder.CreateIndex(
                name: "IX_Kunden_BenutzerId",
                table: "Kunden",
                column: "BenutzerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kunden_Email",
                table: "Kunden",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Kunden_Nachname_Vorname",
                table: "Kunden",
                columns: new[] { "Nachname", "Vorname" });

            migrationBuilder.CreateIndex(
                name: "IX_Rechnungen_AuftragId",
                table: "Rechnungen",
                column: "AuftragId");

            migrationBuilder.CreateIndex(
                name: "IX_Rechnungen_BezahltAm",
                table: "Rechnungen",
                column: "BezahltAm");

            migrationBuilder.CreateIndex(
                name: "IX_Rechnungen_Faellig",
                table: "Rechnungen",
                column: "Faellig");

            migrationBuilder.CreateIndex(
                name: "IX_Rechnungen_RechnungNummer",
                table: "Rechnungen",
                column: "RechnungNummer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rechnungen_Status",
                table: "Rechnungen",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Standorte_Email",
                table: "Standorte",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Standorte_IstAktiv",
                table: "Standorte",
                column: "IstAktiv");

            migrationBuilder.CreateIndex(
                name: "IX_Standorte_Name",
                table: "Standorte",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Standorte_PLZ",
                table: "Standorte",
                column: "PLZ");
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
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Dokumente");

            migrationBuilder.DropTable(
                name: "Rechnungen");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Auftraege");

            migrationBuilder.DropTable(
                name: "Fahrzeuge");

            migrationBuilder.DropTable(
                name: "Kunden");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Standorte");
        }
    }
}
