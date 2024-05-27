using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ODour.PostgresRelationalDb.Migrations
{
    /// <inheritdoc />
    public partial class M1_Init_Db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: "main.account_status");

            migrationBuilder.EnsureSchema(name: "main.app_log");

            migrationBuilder.EnsureSchema(name: "main.category");

            migrationBuilder.EnsureSchema(name: "main.event_snapshot");

            migrationBuilder.EnsureSchema(name: "main.order");

            migrationBuilder.EnsureSchema(name: "main.payment");

            migrationBuilder.EnsureSchema(name: "main.product");

            migrationBuilder.EnsureSchema(name: "main.role");

            migrationBuilder.EnsureSchema(name: "main.seed_flag");

            migrationBuilder.EnsureSchema(name: "main.system_account");

            migrationBuilder.EnsureSchema(name: "main.user");

            migrationBuilder
                .AlterDatabase()
                .Annotation(
                    "Npgsql:CollationDefinition:case_insensitive",
                    "en-u-ks-primary,en-u-ks-primary,icu,False"
                );

            migrationBuilder.CreateTable(
                name: "AccountStatuses",
                schema: "main.account_status",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatuses", x => x.Id);
                },
                comment: "Contain account statuses."
            );

            migrationBuilder.CreateTable(
                name: "AppExceptionLoggingEntities",
                schema: "main.app_log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorStackTrace = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppExceptionLoggingEntities", x => x.Id);
                },
                comment: "Contain app exception loggings."
            );

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "main.category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                },
                comment: "Contain categories."
            );

            migrationBuilder.CreateTable(
                name: "EventSnapshots",
                schema: "main.event_snapshot",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamId = table.Column<string>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSnapshots", x => new { x.EventId, x.StreamId });
                },
                comment: "Contain event snapshots."
            );

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                schema: "main.order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.Id);
                },
                comment: "Contain order statuses."
            );

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "main.payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                },
                comment: "Contain payment methods."
            );

            migrationBuilder.CreateTable(
                name: "ProductStatuses",
                schema: "main.product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStatuses", x => x.Id);
                },
                comment: "Contain product statuses."
            );

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    NormalizedName = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                },
                comment: "Contain roles."
            );

            migrationBuilder.CreateTable(
                name: "SeedFlags",
                schema: "main.seed_flag",
                columns: table => new { Id = table.Column<Guid>(type: "uuid", nullable: false) },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeedFlags", x => x.Id);
                },
                comment: "Contain seed flags."
            );

            migrationBuilder.CreateTable(
                name: "SystemAccountEvents",
                schema: "main.system_account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAccountEvents", x => x.Id);
                },
                comment: "Contain system account events."
            );

            migrationBuilder.CreateTable(
                name: "UserEvents",
                schema: "main.user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvents", x => x.Id);
                },
                comment: "Contain user events."
            );

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    NormalizedUserName = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    Email = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    NormalizedEmail = table.Column<string>(
                        type: "character varying(256)",
                        maxLength: 256,
                        nullable: true
                    ),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                },
                comment: "Contain users."
            );

            migrationBuilder.CreateTable(
                name: "SystemAccounts",
                schema: "main.system_account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(
                        type: "character varying(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    NormalizedUserName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Email = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    NormalizedEmail = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    LockoutEnd = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    AccountStatusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemAccounts_AccountStatuses_AccountStatusId",
                        column: x => x.AccountStatusId,
                        principalSchema: "main.account_status",
                        principalTable: "AccountStatuses",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain system accounts."
            );

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "main.order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderCode = table.Column<long>(type: "bigint", nullable: false),
                    OrderNote = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    TotalPrice = table.Column<decimal>(
                        type: "numeric(12,2)",
                        precision: 12,
                        scale: 2,
                        nullable: false
                    ),
                    DeliveredAddress = table.Column<string>(
                        type: "character varying(500)",
                        maxLength: 500,
                        nullable: false
                    ),
                    DeliveredAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalSchema: "main.order",
                        principalTable: "OrderStatuses",
                        principalColumn: "Id"
                    );
                    table.ForeignKey(
                        name: "FK_Orders_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "main.payment",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain orders."
            );

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "main.product",
                columns: table => new
                {
                    Id = table.Column<string>(
                        type: "character varying(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    ProductStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    UnitPrice = table.Column<decimal>(
                        type: "numeric(12,2)",
                        precision: 12,
                        scale: 2,
                        nullable: false
                    ),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "main.category",
                        principalTable: "Categories",
                        principalColumn: "Id"
                    );
                    table.ForeignKey(
                        name: "FK_Products_ProductStatuses_ProductStatusId",
                        column: x => x.ProductStatusId,
                        principalSchema: "main.product",
                        principalTable: "ProductStatuses",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain products."
            );

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                },
                comment: "Contain role claims."
            );

            migrationBuilder.CreateTable(
                name: "RoleDetails",
                schema: "main.role",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleDetails", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_RoleDetails_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                },
                comment: "Contain role details."
            );

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                },
                comment: "Contain user claims."
            );

            migrationBuilder.CreateTable(
                name: "UserDetails",
                schema: "main.user",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                    LastName = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    AvatarUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<bool>(type: "boolean", nullable: false),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    AccountStatusId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserDetails_AccountStatuses_AccountStatusId",
                        column: x => x.AccountStatusId,
                        principalSchema: "main.account_status",
                        principalTable: "AccountStatuses",
                        principalColumn: "Id"
                    );
                    table.ForeignKey(
                        name: "FK_UserDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                },
                comment: "Contain user details."
            );

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                },
                comment: "Contain user logins."
            );

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                },
                comment: "Contain user roles."
            );

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_UserTokens",
                        x => new
                        {
                            x.UserId,
                            x.LoginProvider,
                            x.Name
                        }
                    );
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                },
                comment: "Contain user tokens."
            );

            migrationBuilder.CreateTable(
                name: "AccountStatusEvents",
                schema: "main.account_status",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatusEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountStatusEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain account status events."
            );

            migrationBuilder.CreateTable(
                name: "CategoryEvents",
                schema: "main.category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain category events."
            );

            migrationBuilder.CreateTable(
                name: "OrderStatusEvents",
                schema: "main.order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatusEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain order status events."
            );

            migrationBuilder.CreateTable(
                name: "PaymentMethodEvents",
                schema: "main.payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethodEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain payment method events."
            );

            migrationBuilder.CreateTable(
                name: "ProductEvents",
                schema: "main.product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<string>(
                        type: "character varying(16)",
                        maxLength: 16,
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain product events."
            );

            migrationBuilder.CreateTable(
                name: "ProductMediaEvents",
                schema: "main.product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMediaEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMediaEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain product media events."
            );

            migrationBuilder.CreateTable(
                name: "ProductStatusEvents",
                schema: "main.product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStatusEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductStatusEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain product status events."
            );

            migrationBuilder.CreateTable(
                name: "RoleEvents",
                schema: "main.role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain role events."
            );

            migrationBuilder.CreateTable(
                name: "SystemAccountTokenEvents",
                schema: "main.system_account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAccountTokenEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemAccountTokenEvents_SystemAccounts_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain system account token events."
            );

            migrationBuilder.CreateTable(
                name: "SystemAccountTokens",
                schema: "main.system_account",
                columns: table => new
                {
                    SystemAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(450)",
                        maxLength: 450,
                        nullable: false
                    ),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAccountTokens", x => x.SystemAccountId);
                    table.ForeignKey(
                        name: "FK_SystemAccountTokens_SystemAccounts_SystemAccountId",
                        column: x => x.SystemAccountId,
                        principalSchema: "main.system_account",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain system account tokens."
            );

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "main.order",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<string>(
                        type: "character varying(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    SellingPrice = table.Column<decimal>(
                        type: "numeric(12,2)",
                        precision: 12,
                        scale: 2,
                        nullable: false
                    ),
                    SellingQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "main.order",
                        principalTable: "Orders",
                        principalColumn: "Id"
                    );
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "main.product",
                        principalTable: "Products",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain order items."
            );

            migrationBuilder.CreateTable(
                name: "ProductMedias",
                schema: "main.product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<string>(
                        type: "character varying(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    UploadOrder = table.Column<int>(type: "integer", nullable: false),
                    StorageUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMedias_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "main.product",
                        principalTable: "Products",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain product medias."
            );

            migrationBuilder.CreateTable(
                name: "OrderEvents",
                schema: "main.order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderEvents_UserDetails_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.user",
                        principalTable: "UserDetails",
                        principalColumn: "UserId"
                    );
                },
                comment: "Contain order events."
            );

            migrationBuilder.CreateTable(
                name: "OrderItemEvents",
                schema: "main.order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemEvents_UserDetails_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.user",
                        principalTable: "UserDetails",
                        principalColumn: "UserId"
                    );
                },
                comment: "Contain order item events."
            );

            migrationBuilder.CreateTable(
                name: "UserTokenEvents",
                schema: "main.user",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OldData = table.Column<string>(type: "TEXT", nullable: false),
                    NewData = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokenEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTokenEvents_UserDetails_CreatedBy",
                        column: x => x.CreatedBy,
                        principalSchema: "main.user",
                        principalTable: "UserDetails",
                        principalColumn: "UserId"
                    );
                },
                comment: "Contain user token events."
            );

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatusEvents_CreatedBy",
                schema: "main.account_status",
                table: "AccountStatusEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_CategoryEvents_CreatedBy",
                schema: "main.category",
                table: "CategoryEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderEvents_CreatedBy",
                schema: "main.order",
                table: "OrderEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemEvents_CreatedBy",
                schema: "main.order",
                table: "OrderItemEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                schema: "main.order",
                table: "OrderItems",
                column: "ProductId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusId",
                schema: "main.order",
                table: "Orders",
                column: "OrderStatusId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId",
                schema: "main.order",
                table: "Orders",
                column: "PaymentMethodId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusEvents_CreatedBy",
                schema: "main.order",
                table: "OrderStatusEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodEvents_CreatedBy",
                schema: "main.payment",
                table: "PaymentMethodEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductEvents_CreatedBy",
                schema: "main.product",
                table: "ProductEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductMediaEvents_CreatedBy",
                schema: "main.product",
                table: "ProductMediaEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductMedias_ProductId",
                schema: "main.product",
                table: "ProductMedias",
                column: "ProductId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                schema: "main.product",
                table: "Products",
                column: "CategoryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductStatusId",
                schema: "main.product",
                table: "Products",
                column: "ProductStatusId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductStatusEvents_CreatedBy",
                schema: "main.product",
                table: "ProductStatusEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RoleEvents_CreatedBy",
                schema: "main.role",
                table: "RoleEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccount_NormalizedEmail",
                schema: "main.system_account",
                table: "SystemAccounts",
                column: "NormalizedEmail",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccount_NormalizedUserName",
                schema: "main.system_account",
                table: "SystemAccounts",
                column: "NormalizedUserName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccounts_AccountStatusId",
                schema: "main.system_account",
                table: "SystemAccounts",
                column: "AccountStatusId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccountTokenEvents_CreatedBy",
                schema: "main.system_account",
                table: "SystemAccountTokenEvents",
                column: "CreatedBy"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_AccountStatusId",
                schema: "main.user",
                table: "UserDetails",
                column: "AccountStatusId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId"
            );

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail"
            );

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserTokenEvents_CreatedBy",
                schema: "main.user",
                table: "UserTokenEvents",
                column: "CreatedBy"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AccountStatusEvents", schema: "main.account_status");

            migrationBuilder.DropTable(name: "AppExceptionLoggingEntities", schema: "main.app_log");

            migrationBuilder.DropTable(name: "CategoryEvents", schema: "main.category");

            migrationBuilder.DropTable(name: "EventSnapshots", schema: "main.event_snapshot");

            migrationBuilder.DropTable(name: "OrderEvents", schema: "main.order");

            migrationBuilder.DropTable(name: "OrderItemEvents", schema: "main.order");

            migrationBuilder.DropTable(name: "OrderItems", schema: "main.order");

            migrationBuilder.DropTable(name: "OrderStatusEvents", schema: "main.order");

            migrationBuilder.DropTable(name: "PaymentMethodEvents", schema: "main.payment");

            migrationBuilder.DropTable(name: "ProductEvents", schema: "main.product");

            migrationBuilder.DropTable(name: "ProductMediaEvents", schema: "main.product");

            migrationBuilder.DropTable(name: "ProductMedias", schema: "main.product");

            migrationBuilder.DropTable(name: "ProductStatusEvents", schema: "main.product");

            migrationBuilder.DropTable(name: "RoleClaims");

            migrationBuilder.DropTable(name: "RoleDetails", schema: "main.role");

            migrationBuilder.DropTable(name: "RoleEvents", schema: "main.role");

            migrationBuilder.DropTable(name: "SeedFlags", schema: "main.seed_flag");

            migrationBuilder.DropTable(name: "SystemAccountEvents", schema: "main.system_account");

            migrationBuilder.DropTable(
                name: "SystemAccountTokenEvents",
                schema: "main.system_account"
            );

            migrationBuilder.DropTable(name: "SystemAccountTokens", schema: "main.system_account");

            migrationBuilder.DropTable(name: "UserClaims");

            migrationBuilder.DropTable(name: "UserEvents", schema: "main.user");

            migrationBuilder.DropTable(name: "UserLogins");

            migrationBuilder.DropTable(name: "UserRoles");

            migrationBuilder.DropTable(name: "UserTokenEvents", schema: "main.user");

            migrationBuilder.DropTable(name: "UserTokens");

            migrationBuilder.DropTable(name: "Orders", schema: "main.order");

            migrationBuilder.DropTable(name: "Products", schema: "main.product");

            migrationBuilder.DropTable(name: "SystemAccounts", schema: "main.system_account");

            migrationBuilder.DropTable(name: "Roles");

            migrationBuilder.DropTable(name: "UserDetails", schema: "main.user");

            migrationBuilder.DropTable(name: "OrderStatuses", schema: "main.order");

            migrationBuilder.DropTable(name: "PaymentMethods", schema: "main.payment");

            migrationBuilder.DropTable(name: "Categories", schema: "main.category");

            migrationBuilder.DropTable(name: "ProductStatuses", schema: "main.product");

            migrationBuilder.DropTable(name: "AccountStatuses", schema: "main.account_status");

            migrationBuilder.DropTable(name: "Users");
        }
    }
}
