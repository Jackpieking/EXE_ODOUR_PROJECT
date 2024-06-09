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

            migrationBuilder.EnsureSchema(name: "main.system");

            migrationBuilder.EnsureSchema(name: "main.order");

            migrationBuilder.EnsureSchema(name: "main.category");

            migrationBuilder.EnsureSchema(name: "main.event");

            migrationBuilder.EnsureSchema(name: "main.payment");

            migrationBuilder.EnsureSchema(name: "main.product");

            migrationBuilder.EnsureSchema(name: "main.voucher");

            migrationBuilder.EnsureSchema(name: "main.role");

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
                schema: "main.system",
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
                name: "Events",
                schema: "main.event",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(
                        type: "character varying(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    StreamId = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                },
                comment: "Contain events."
            );

            migrationBuilder.CreateTable(
                name: "JobRecords",
                schema: "main.system",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QueueID = table.Column<string>(type: "TEXT", nullable: false),
                    ExecuteAfter = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    ExpireOn = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    IsComplete = table.Column<bool>(type: "boolean", nullable: false),
                    CommandJson = table.Column<string>(type: "JSONB", nullable: false),
                    FailureCount = table.Column<int>(type: "integer", nullable: false),
                    CancelledOn = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    FailureReason = table.Column<string>(type: "JSONB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRecords", x => x.Id);
                },
                comment: "Contain job records."
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
                schema: "main.system",
                columns: table => new { Id = table.Column<Guid>(type: "uuid", nullable: false) },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeedFlags", x => x.Id);
                },
                comment: "Contain seed flags."
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
                name: "VoucherTypes",
                schema: "main.voucher",
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
                    table.PrimaryKey("PK_VoucherTypes", x => x.Id);
                },
                comment: "Contain voucher types."
            );

            migrationBuilder.CreateTable(
                name: "SystemAccounts",
                schema: "main.system",
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
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "EventSnapshots",
                schema: "main.event",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSnapshots", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_EventSnapshots_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "main.event",
                        principalTable: "Events",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain event snapshots."
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
                    Description = table.Column<string>(type: "JSONB", nullable: false),
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
                        principalColumn: "Id"
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
                    AppPasswordHash = table.Column<string>(type: "TEXT", nullable: false),
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
                        principalColumn: "Id"
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
                name: "Vouchers",
                schema: "main.voucher",
                columns: table => new
                {
                    Code = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false),
                    DiscountPercentage = table.Column<decimal>(
                        type: "numeric(7,2)",
                        precision: 7,
                        scale: 2,
                        nullable: false
                    ),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    IsTemporarilyRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    VoucherTypeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Vouchers_VoucherTypes_VoucherTypeId",
                        column: x => x.VoucherTypeId,
                        principalSchema: "main.voucher",
                        principalTable: "VoucherTypes",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain vouchers."
            );

            migrationBuilder.CreateTable(
                name: "SystemAccountRoles",
                schema: "main.system",
                columns: table => new
                {
                    SystemAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_SystemAccountRoles",
                        x => new { x.SystemAccountId, x.RoleId }
                    );
                    table.ForeignKey(
                        name: "FK_SystemAccountRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id"
                    );
                    table.ForeignKey(
                        name: "FK_SystemAccountRoles_SystemAccounts_SystemAccountId",
                        column: x => x.SystemAccountId,
                        principalSchema: "main.system",
                        principalTable: "SystemAccounts",
                        principalColumn: "Id"
                    );
                },
                comment: "Contain system account roles."
            );

            migrationBuilder.CreateTable(
                name: "SystemAccountTokens",
                schema: "main.system",
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
                    table.PrimaryKey(
                        "PK_SystemAccountTokens",
                        x => new
                        {
                            x.SystemAccountId,
                            x.LoginProvider,
                            x.Name
                        }
                    );
                    table.ForeignKey(
                        name: "FK_SystemAccountTokens_SystemAccounts_SystemAccountId",
                        column: x => x.SystemAccountId,
                        principalSchema: "main.system",
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "main.product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
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
                name: "CartItems",
                schema: "main.order",
                columns: table => new
                {
                    ProductId = table.Column<string>(
                        type: "character varying(10)",
                        nullable: false
                    ),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => new { x.ProductId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "main.product",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_CartItems_UserDetails_UserId",
                        column: x => x.UserId,
                        principalSchema: "main.user",
                        principalTable: "UserDetails",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade
                    );
                },
                comment: "Contain cart items."
            );

            migrationBuilder.CreateTable(
                name: "ProductVouchers",
                schema: "main.voucher",
                columns: table => new
                {
                    ProductId = table.Column<string>(
                        type: "character varying(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    VoucherCode = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVouchers", x => new { x.ProductId, x.VoucherCode });
                    table.ForeignKey(
                        name: "FK_ProductVouchers_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "main.product",
                        principalTable: "Products",
                        principalColumn: "Id"
                    );
                    table.ForeignKey(
                        name: "FK_ProductVouchers_Vouchers_VoucherCode",
                        column: x => x.VoucherCode,
                        principalSchema: "main.voucher",
                        principalTable: "Vouchers",
                        principalColumn: "Code"
                    );
                },
                comment: "Contain product vouchers."
            );

            migrationBuilder.CreateTable(
                name: "UserVouchers",
                schema: "main.voucher",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VoucherCode = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVouchers", x => new { x.UserId, x.VoucherCode });
                    table.ForeignKey(
                        name: "FK_UserVouchers_UserDetails_UserId",
                        column: x => x.UserId,
                        principalSchema: "main.user",
                        principalTable: "UserDetails",
                        principalColumn: "UserId"
                    );
                    table.ForeignKey(
                        name: "FK_UserVouchers_Vouchers_VoucherCode",
                        column: x => x.VoucherCode,
                        principalSchema: "main.voucher",
                        principalTable: "Vouchers",
                        principalColumn: "Code"
                    );
                },
                comment: "Contain user vouchers."
            );

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                schema: "main.order",
                table: "CartItems",
                column: "UserId"
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
                name: "IX_ProductVouchers_VoucherCode",
                schema: "main.voucher",
                table: "ProductVouchers",
                column: "VoucherCode"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId"
            );

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccountRoles_RoleId",
                schema: "main.system",
                table: "SystemAccountRoles",
                column: "RoleId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccount_NormalizedEmail",
                schema: "main.system",
                table: "SystemAccounts",
                column: "NormalizedEmail",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccount_NormalizedUserName",
                schema: "main.system",
                table: "SystemAccounts",
                column: "NormalizedUserName",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_SystemAccounts_AccountStatusId",
                schema: "main.system",
                table: "SystemAccounts",
                column: "AccountStatusId"
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
                name: "IX_UserVouchers_VoucherCode",
                schema: "main.voucher",
                table: "UserVouchers",
                column: "VoucherCode"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VoucherTypeId",
                schema: "main.voucher",
                table: "Vouchers",
                column: "VoucherTypeId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AppExceptionLoggingEntities", schema: "main.system");

            migrationBuilder.DropTable(name: "CartItems", schema: "main.order");

            migrationBuilder.DropTable(name: "EventSnapshots", schema: "main.event");

            migrationBuilder.DropTable(name: "JobRecords", schema: "main.system");

            migrationBuilder.DropTable(name: "OrderItems", schema: "main.order");

            migrationBuilder.DropTable(name: "ProductMedias", schema: "main.product");

            migrationBuilder.DropTable(name: "ProductVouchers", schema: "main.voucher");

            migrationBuilder.DropTable(name: "RoleClaims");

            migrationBuilder.DropTable(name: "RoleDetails", schema: "main.role");

            migrationBuilder.DropTable(name: "SeedFlags", schema: "main.system");

            migrationBuilder.DropTable(name: "SystemAccountRoles", schema: "main.system");

            migrationBuilder.DropTable(name: "SystemAccountTokens", schema: "main.system");

            migrationBuilder.DropTable(name: "UserClaims");

            migrationBuilder.DropTable(name: "UserLogins");

            migrationBuilder.DropTable(name: "UserRoles");

            migrationBuilder.DropTable(name: "UserTokens");

            migrationBuilder.DropTable(name: "UserVouchers", schema: "main.voucher");

            migrationBuilder.DropTable(name: "Events", schema: "main.event");

            migrationBuilder.DropTable(name: "Orders", schema: "main.order");

            migrationBuilder.DropTable(name: "Products", schema: "main.product");

            migrationBuilder.DropTable(name: "SystemAccounts", schema: "main.system");

            migrationBuilder.DropTable(name: "Roles");

            migrationBuilder.DropTable(name: "UserDetails", schema: "main.user");

            migrationBuilder.DropTable(name: "Vouchers", schema: "main.voucher");

            migrationBuilder.DropTable(name: "OrderStatuses", schema: "main.order");

            migrationBuilder.DropTable(name: "PaymentMethods", schema: "main.payment");

            migrationBuilder.DropTable(name: "Categories", schema: "main.category");

            migrationBuilder.DropTable(name: "ProductStatuses", schema: "main.product");

            migrationBuilder.DropTable(name: "AccountStatuses", schema: "main.account_status");

            migrationBuilder.DropTable(name: "Users");

            migrationBuilder.DropTable(name: "VoucherTypes", schema: "main.voucher");
        }
    }
}
