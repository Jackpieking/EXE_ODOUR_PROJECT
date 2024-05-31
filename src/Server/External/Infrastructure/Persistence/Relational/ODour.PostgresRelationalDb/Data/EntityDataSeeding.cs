using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ODour.Configuration.Presentation.WebApi.SecurityKey;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.Category.Entities;
using ODour.Domain.Share.Events;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Payment.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.System.Entities;
using ODour.Domain.Share.User.Entities;
using ODour.Domain.Share.Voucher.Entities;
using static ODour.Application.Share.Common.CommonConstant;

namespace ODour.PostgresRelationalDb.Data;

public static class EntityDataSeeding
{
    private static readonly Guid AdminId = Guid.Parse(
        input: "ffccce5f-fbb1-4c1e-be6b-b22028134056"
    );

    public static async Task<bool> SeedAsync(
        DbContext context,
        Lazy<UserManager<UserEntity>> userManager,
        Lazy<RoleManager<RoleEntity>> roleManager,
        Lazy<IDataProtectionProvider> dataProtectionProvider,
        Lazy<AppBaseProtectionSecurityKeyOption> protectionSecurityKeyOption,
        CancellationToken cancellationToken
    )
    {
        // Check if all tables of database are empty.
        var areTablesEmtpy = await AreAllTablesEmptyAsync(context, cancellationToken);

        if (!areTablesEmtpy)
        {
            return true;
        }

        // Start seeding.
        await MarkAsAlreadySeedAsync(context: context, cancellationToken: cancellationToken);

        await SeedAccountStatusEntitiesAsync(
            context: context,
            cancellationToken: cancellationToken
        );

        await SeedCategoryEntitiesAsync(context: context, cancellationToken: cancellationToken);

        await SeedOrderStatusEntitiesAsync(context: context, cancellationToken: cancellationToken);

        await SeedPaymentMethodEntitiesAsync(
            context: context,
            cancellationToken: cancellationToken
        );

        await SeedProductStatusEntitiesAsync(
            context: context,
            cancellationToken: cancellationToken
        );

        await SeedProductEntitiesAsync(context: context, cancellationToken: cancellationToken);

        await SeedSystemAccountEntitiesAsync(
            context: context,
            protectionProvider: dataProtectionProvider.Value,
            protectionSecurityKeyOption: protectionSecurityKeyOption.Value,
            cancellationToken: cancellationToken
        );

        await SeedVoucherTypeEntitiesAsync(context: context, cancellationToken: cancellationToken);

        await SeedVoucherEntitiesAsync(context: context, cancellationToken: cancellationToken);

        var seedRoles = await GetSeedRoleEntitiesAsync(
            context: context,
            cancellationToken: cancellationToken
        );

        var seedUsers = await GetSeedUserEntitiesAsync(
            context: context,
            protectionProvider: dataProtectionProvider.Value,
            protectionSecurityKeyOption: protectionSecurityKeyOption.Value,
            cancellationToken: cancellationToken
        );

        #region Transaction
        var executedTransactionResult = false;

        await context
            .Database.CreateExecutionStrategy()
            .ExecuteAsync(operation: async () =>
            {
                await using var dbTransaction = await context.Database.BeginTransactionAsync(
                    cancellationToken: cancellationToken
                );

                try
                {
                    foreach (var newRole in seedRoles)
                    {
                        await roleManager.Value.CreateAsync(role: newRole);
                    }

                    // test user.
                    var user = seedUsers.Find(match: user =>
                        user.UserName.Equals(value: "testUser", StringComparison.OrdinalIgnoreCase)
                    );

                    await userManager.Value.CreateAsync(user: user);

                    await userManager.Value.AddToRoleAsync(user: user, role: "user");

                    var emailConfirmationToken =
                        await userManager.Value.GenerateEmailConfirmationTokenAsync(user: user);

                    await userManager.Value.ConfirmEmailAsync(
                        user: user,
                        token: emailConfirmationToken
                    );

                    // banned user.
                    user = seedUsers.Find(match: user =>
                        user.UserName.Equals(
                            value: "bannedUser",
                            StringComparison.OrdinalIgnoreCase
                        )
                    );

                    await userManager.Value.CreateAsync(user: user);

                    await userManager.Value.AddToRoleAsync(user: user, role: "user");

                    emailConfirmationToken =
                        await userManager.Value.GenerateEmailConfirmationTokenAsync(user: user);

                    await userManager.Value.ConfirmEmailAsync(
                        user: user,
                        token: emailConfirmationToken
                    );

                    // Save all changings.
                    await context.SaveChangesAsync(cancellationToken: cancellationToken);

                    await dbTransaction.CommitAsync(cancellationToken: cancellationToken);

                    executedTransactionResult = true;
                }
                catch
                {
                    await dbTransaction.RollbackAsync(cancellationToken: cancellationToken);
                }
            });
        #endregion

        return executedTransactionResult;
    }

    private static async Task<bool> AreAllTablesEmptyAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        var areTablesNotEmtpy = await context
            .Set<SeedFlagEntity>()
            .AnyAsync(cancellationToken: cancellationToken);

        if (areTablesNotEmtpy)
        {
            return false;
        }

        return true;
    }

    private static async Task MarkAsAlreadySeedAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        await context.AddAsync(
            entity: new SeedFlagEntity { Id = Guid.NewGuid() },
            cancellationToken: cancellationToken
        );
    }

    private static async Task SeedAccountStatusEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string AccountStatusAddedEventType = "AccountStatusAdded";

        var newAccountStatuses = new List<AccountStatusEntity>
        {
            new()
            {
                Id = Guid.Parse(input: "88b4c299-13af-4c75-bb8c-a961ee149997"),
                Name = "Chờ xác nhận đăng ký",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "4ac49b1d-eb67-4530-a919-4d9de312ee1f"),
                Name = "Đã xác nhận đăng ký",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "0c7bed0c-2478-43fd-9a6d-cd084980f749"),
                Name = "Bị cấm trong hệ thống",
                IsTemporarilyRemoved = false
            }
        };

        foreach (var newAccountStatus in newAccountStatuses)
        {
            var newAccountStatusEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = AccountStatusAddedEventType,
                StreamId = newAccountStatus.Id.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newAccountStatus,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(
                entity: newAccountStatusEvent,
                cancellationToken: cancellationToken
            );
        }

        await context.AddRangeAsync(
            entities: newAccountStatuses,
            cancellationToken: cancellationToken
        );
    }

    private static async Task SeedCategoryEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string CategoryAddedEventType = "CategoryAdded";

        var newCategories = new List<CategoryEntity>
        {
            new()
            {
                Id = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "Nam",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "Nữ",
                IsTemporarilyRemoved = false
            }
        };

        foreach (var newCategory in newCategories)
        {
            var newCategoryEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = CategoryAddedEventType,
                StreamId = newCategory.Id.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newCategory,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(entity: newCategoryEvent, cancellationToken: cancellationToken);
        }

        await context.AddRangeAsync(entities: newCategories, cancellationToken: cancellationToken);
    }

    private static async Task SeedOrderStatusEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string OrderStatusAddedEventType = "OrderStatusAdded";

        var newOrderStatuses = new List<OrderStatusEntity>
        {
            new()
            {
                Id = Guid.Parse(input: "0e696741-97f6-444a-b265-025c8c394fc9"),
                Name = "Chờ thanh toán",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "f86e2f2a-5dcc-4546-a4de-ea297ee22dc5"),
                Name = "Chờ xử lý",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "9b1f0fcc-bdd4-4e57-9444-b19c3b2f0547"),
                Name = "Đang xử lý",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "3a398200-af38-4832-82d9-ed65c248041b"),
                Name = "Đang giao hàng",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "37eb8293-c17a-45a6-89ab-ba640d4001ff"),
                Name = "Giao hàng thành công",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "2f3eae5b-d4ed-4fff-ab1c-6a538721ffca"),
                Name = "Đã hủy",
                IsTemporarilyRemoved = false
            }
        };

        foreach (var newOrderStatus in newOrderStatuses)
        {
            var newOrderStatusEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = OrderStatusAddedEventType,
                StreamId = newOrderStatus.Id.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newOrderStatus,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(
                entity: newOrderStatusEvent,
                cancellationToken: cancellationToken
            );
        }

        await context.AddRangeAsync(
            entities: newOrderStatuses,
            cancellationToken: cancellationToken
        );
    }

    private static async Task SeedPaymentMethodEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string PayemntMethodAddedEventType = "PayemntMethodAdded";

        var newPaymentMethods = new List<PaymentMethodEntity>
        {
            new()
            {
                Id = Guid.Parse(input: "845e7be4-b3e3-4483-9fde-65694ee2d9b9"),
                Name = "Thanh toán khi nhận hàng",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "885e24a5-2e53-4c91-8a44-281be8fe4a17"),
                Name = "Chuyển khoản",
                IsTemporarilyRemoved = false
            }
        };

        foreach (var newPaymentMethod in newPaymentMethods)
        {
            var newPaymentMethodEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = PayemntMethodAddedEventType,
                StreamId = newPaymentMethod.Id.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newPaymentMethod,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(
                entity: newPaymentMethodEvent,
                cancellationToken: cancellationToken
            );
        }

        await context.AddRangeAsync(
            entities: newPaymentMethods,
            cancellationToken: cancellationToken
        );
    }

    private static async Task SeedProductStatusEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string ProductStatusAddedEventType = "ProductStatusAdded";

        var newProductStatuses = new List<ProductStatusEntity>
        {
            new()
            {
                Id = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                Name = "Còn hàng",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "8c3dc836-9266-4de3-a776-d2c0fca887ca"),
                Name = "Tạm hết hàng",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "0bded5d1-cca6-4955-928b-5f1b3afc9fe8"),
                Name = "Ngưng nhập hàng",
                IsTemporarilyRemoved = false
            }
        };

        foreach (var newProductStatus in newProductStatuses)
        {
            var newProductStatusEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = ProductStatusAddedEventType,
                StreamId = newProductStatus.Id.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newProductStatus,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(
                entity: newProductStatusEvent,
                cancellationToken: cancellationToken
            );
        }

        await context.AddRangeAsync(
            entities: newProductStatuses,
            cancellationToken: cancellationToken
        );
    }

    private static async Task SeedProductEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string ProductAddedEventType = "ProductAdded";

        var newProducts = new List<ProductEntity>
        {
            new()
            {
                Id = "LD01",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "CREED AVENTUS",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Chypre Fruity - hương trái cây Chypre.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Cam Bergamot, Quả lý chua đen, Quả dứa (quả thơm).\",\"HUONG_GIUA\":\"Gỗ Bu-lô, Cây hoắc hương, Hoa hồng.\",\"HUONG_CUOI\":\"Xạ hương, Rêu cây sồi, Hương vani, Long diên hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 18 giờ.\",\"TREN_VAI\":\"Trên 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD02",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "CK ONE GOLD",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Oriental - tông mùi phương đông.\",\"Floral - tông hoa.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Quả sung, Cam Bergamot, Lá xô thơm.\",\"HUONG_GIUA\":\"Hoa trắng, Hoa nhài, Hoa violet.\",\"HUONG_CUOI\":\"Gỗ guaiac, Cỏ hương bài, Hoắc hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD03",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "212 VIP MEN",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Leather - tông da thuộc.\",\"Fruity - hương trái cây.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Chanh, tiêu, Gừng, Chanh dây.\",\"HUONG_GIUA\":\"Rượu Vodka, Rượu Gin, Bạc hà.\",\"HUONG_CUOI\":\"Hổ phách, Da thuộc.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD04",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "BLEU DE CHANEL",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Oriental — tông mùi Phương Đông.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Chanh vàng, Ớt hồng, Bạc hà.\",\"HUONG_GIUA\":\"Dưa vàng, Hoa nhài, Gừng.\",\"HUONG_CUOI\":\"Gỗ tuyết tùng, Hổ phách, Gỗ đàn hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 18 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD05",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "SAUVAGE DIOR",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Oriental — tông mùi Phương Đông.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Cam Bergamot, Tiêu.\",\"HUONG_GIUA\":\"Tiêu Sichuan, Hoa oải Hương, Tiêu hồng, Cỏ hương bài, Hoắc hương, Phong lữ, Nhựa Elemi.\",\"HUONG_CUOI\":\"Ambroxan, Tuyết tùng, Labannum.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 18 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD06",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "BVL AQVA",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Aquatic — hương biển.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Hoa cam Neroli, Quả bưởi, Quả quýt hồng.\",\"HUONG_GIUA\":\"Rong biển, Cây hương thảo, Hương nước.\",\"HUONG_CUOI\":\"Gỗ tuyết tùng Virginia, Hổ phách.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 18 giờ.\",\"TREN_VAI\":\"Trên 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD07",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "POLO BLUE",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Aquatic — hương biển.\",\"Leather - tông da thuộc.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Cam Bergamot, Bạch đậu khấu, Hương nước biển.\",\"HUONG_GIUA\":\"Hoa diên vĩ (Orris), Cây húng quế, Cỏ đuôi ngựa, Cây đơn sâm.\",\"HUONG_CUOI\":\"Cỏ hương bài, Da lộn, Cây hoắc hương, Hương của gỗ.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"16 - 20 giờ.\",\"TREN_VAI\":\"Trên 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD08",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "ACQUA DI GIÒ",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Aquatic — hương biển.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Quả cam, Quả chanh xanh, Cam Bergamot, Quả chanh vàng.\",\"HUONG_GIUA\":\"Hoa nước biển, Quả đào, Hoa nhài, Hương Calone.\",\"HUONG_CUOI\":\"Rêu sồi, Gỗ tuyết tùng, Xạ hương trắng.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"16 - 20 giờ.\",\"TREN_VAI\":\"Trên 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD09",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "ALLURE HOMME SPORT",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Woody - tông mùi gỗ.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Chanh vàng, Cam Bergamot.\",\"HUONG_GIUA\":\"Gỗ đàn hương, Hương gỗ.\",\"HUONG_CUOI\":\"Vani Madagascar, Cỏ Vetiver.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"16 - 22 giờ.\",\"TREN_VAI\":\"Trên 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD10",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "REPLICA JAZZ CLUB",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Leather - tông da thuộc.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Cây thuốc lá, Da thuộc, Chanh vàng, Hạt tiêu hồng, Hoa cam Neroli.\",\"HUONG_GIUA\":\"Rượu Rum, Cây đơn sâm, Dầu cỏ hương bài.\",\"HUONG_CUOI\":\"Cây thuốc lá, Bồ đề, Hương vani.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"16 - 22 giờ.\",\"TREN_VAI\":\"Trên 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD11",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "VERSACE EROS",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Fresh - tông mùi tươi mát.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Táo xanh, Bạc hà.\",\"HUONG_GIUA\":\"Đậu Tonka, Hoa phong lữ.\",\"HUONG_CUOI\":\"Cỏ hương bài, Vani.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"16 - 22 giờ.\",\"TREN_VAI\":\"3 - 5 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD12",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "GUCCI GUILTY INTENSE",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"White Floral - hương hoa trắng.\",\"Powdery - hương phấn.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Hoa Lavender, Quả chanh vàng.\",\"HUONG_GIUA\":\"Hoa cam châu Phi, Hoa cam Neroli.\",\"HUONG_CUOI\":\"Cây hoắc hương, Gỗ tuyết tùng, Hổ phách.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"18 - 24 giờ.\",\"TREN_VAI\":\"3 - 5 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD13",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                Name = "D&G KING",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Gourmand - món ăn.\",\"Oriental — tông mùi Phương Đông.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Cam máu, Ớt Pimento, Quả bách xù, Quả chanh vàng, Bạch đậu khấu.\",\"HUONG_GIUA\":\"Rượu sung, Hoa phong lữ, Hoa oải hương, Xô thơm.\",\"HUONG_CUOI\":\"Gỗ tuyết tùng, Hoắc hương, Cỏ gấu, Cỏ hương bài.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"16 - 20 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD14",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "FANCY LOVE",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Oriental — tông mùi Phương Đông.\",\"Floral - tông hoa.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Cam Bergamot, Hoa đào, Lá Goji, Rượu sâm panh hồng.\",\"HUONG_GIUA\":\"Hoa sen, Hoa mẫu đơn, Hoa plumeria, Hoa nhài, Hoa hồng Thổ Nhĩ Kỳ.\",\"HUONG_CUOI\":\"Hoa phách kem, Xạ hương gỗ vàng, Hoắc hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD15",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "CHLOE'",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Green, Herb — tông mùi thảo dược, cây xanh.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Hoa mẫu đơn, Hoa lan Nam Phi, Quả vải.\",\"HUONG_GIUA\":\"Hoa hồng, Hoa linh lan thung lũng, Hoa mộc lan.\",\"HUONG_CUOI\":\"Hổ phách, Gỗ tuyết tùng Virginia.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD16",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "VERSACE YELLOW",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"White Floral - hương hoa trắng.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Lê, Citron, Hoa cam.\",\"HUONG_GIUA\":\"Cam Bergamot, Neroli.\",\"HUONG_CUOI\":\"Gỗ đàn hương, Hoa huệ.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD17",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "GUCCI GUILTY",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Floral Green - Hương hoa cỏ xanh tự nhiên.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Vải thiều, Ghi chú nước.\",\"HUONG_GIUA\":\"Dâu dại, Hoa huệ.\",\"HUONG_CUOI\":\"Xạ hương, Gỗ đàn hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD18",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "PARIS HILTON",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"White Floral - hương hoa trắng.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Quả mâm xôi, Hoa huệ của thung lũng.\",\"HUONG_GIUA\":\"Hoa diên vĩ, Hoa hồng trắng.\",\"HUONG_CUOI\":\"Vani, Gỗ đàn hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD19",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "LV CONTRE MOI",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Oriental — tông mùi Phương Đông.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Cam Bergamot, Quả chanh vàng, Hương thảo mộc.\",\"HUONG_GIUA\":\"Hoa mộc lan, Hoa cam, Quả lê, Hoa hồng.\",\"HUONG_CUOI\":\"Cây vông vang, Ca cao, Hương Vani Tahita, Hương Vani Madagascar.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD20",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "VERY SEXY",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Floral Fruity — hương hoa cỏ trái cây.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Trái quýt, Cà phê Cappuccino, Cây xương rồng, Tiêu đen.\",\"HUONG_GIUA\":\"Hoa tú cầu, Hoa trà (Camelia), Hoa phong lan.\",\"HUONG_CUOI\":\"Hương gỗ, Quả mâm xôi đen, Xạ hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD21",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "FOR HER",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Woody — tông mùi gỗ.\", \"Floral — tông hoa.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Hoa hồng, Quả đào.\",\"HUONG_GIUA\":\"Xạ hương, Hổ phách.\",\"HUONG_CUOI\":\"Gỗ đàn hương, Cây hoắc hương, Gỗ trầm hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = "LD22",
                ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                CategoryId = Guid.Parse(input: "f081ab44-7eb4-4ae9-9a35-5dfaf6e82c1c"),
                Name = "FOR HER",
                UnitPrice = 199_000,
                Description =
                    "{\"NHOM_HUONG\":[\"Woody — tông mùi gỗ.\", \"Floral — tông hoa.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Hoa hồng, Quả đào.\",\"HUONG_GIUA\":\"Xạ hương, Hổ phách.\",\"HUONG_CUOI\":\"Gỗ đàn hương, Cây hoắc hương, Gỗ trầm hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"12 - 16 giờ.\",\"TREN_VAI\":\"2 - 3 ngày.\"}}",
                QuantityInStock = 10,
                IsTemporarilyRemoved = false
            }
        };

        foreach (var newProduct in newProducts)
        {
            var newProductEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = ProductAddedEventType,
                StreamId = newProduct.Id,
                Data = JsonSerializer.Serialize(
                    value: newProduct,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(entity: newProductEvent, cancellationToken: cancellationToken);
        }

        await context.AddRangeAsync(entities: newProducts, cancellationToken: cancellationToken);
    }

    private static async Task<List<RoleEntity>> GetSeedRoleEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string RoleAddedEventType = "RoleAdded";

        var adminRoleId = Guid.Parse(input: "c95f4aae-2a41-4c76-9cc4-f1d632409525");
        var userRoleId = Guid.Parse(input: "8ebe29bc-4706-4fda-bb28-ed127d150c05");

        var newRoles = new List<RoleEntity>
        {
            new()
            {
                Id = adminRoleId,
                Name = "admin",
                RoleDetail = new() { RoleId = adminRoleId, IsTemporarilyRemoved = false }
            },
            new()
            {
                Id = userRoleId,
                Name = "user",
                RoleDetail = new() { RoleId = userRoleId, IsTemporarilyRemoved = false }
            }
        };

        foreach (var newRole in newRoles)
        {
            var newRoleEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = RoleAddedEventType,
                StreamId = newRole.Id.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newRole,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(entity: newRoleEvent, cancellationToken: cancellationToken);
        }

        return newRoles;
    }

    private static async Task<List<UserEntity>> GetSeedUserEntitiesAsync(
        DbContext context,
        IDataProtectionProvider protectionProvider,
        AppBaseProtectionSecurityKeyOption protectionSecurityKeyOption,
        CancellationToken cancellationToken
    )
    {
        const string UserAddedEventType = "UserAdded";
        const string UserBannedEventType = "UserBanned";

        var testUserId = Guid.Parse(input: "8ebe29bc-4706-4fda-bb28-ed127d150c05");
        var bannedUserId = Guid.Parse(input: "9a2b6683-a16e-4270-a23c-3e09a6e27345");

        var testUserProtector = protectionProvider.CreateProtector(
            purpose: $"{protectionSecurityKeyOption.Value}<{testUserId}>"
        );

        var bannedUserProtector = protectionProvider.CreateProtector(
            purpose: $"{protectionSecurityKeyOption.Value}<{bannedUserId}>"
        );

        var newUsers = new List<UserEntity>
        {
            new()
            {
                Id = testUserId,
                UserName = "testUser",
                Email = "khoaprovn041@gmail.com",
                PhoneNumber = "0706042250",
                PasswordHash = testUserProtector.Protect(plaintext: "Khoa1234"),
                UserDetail = new()
                {
                    UserId = testUserId,
                    FirstName = "Thắng",
                    LastName = "Trần Minh",
                    Gender = true,
                    AvatarUrl =
                        "https://www.shutterstock.com/image-vector/young-smiling-man-avatar-brown-600nw-2261401207.jpg",
                    AccountStatusId = Guid.Parse(input: "4ac49b1d-eb67-4530-a919-4d9de312ee1f"),
                    IsTemporarilyRemoved = false
                }
            },
            new()
            {
                Id = bannedUserId,
                UserName = "bannedUser",
                Email = "khoamapdit03@gmail.com",
                PhoneNumber = "0706042250",
                PasswordHash = bannedUserProtector.Protect(plaintext: "Khoa1234"),
                UserDetail = new()
                {
                    UserId = bannedUserId,
                    FirstName = "Khánh",
                    LastName = "Nguyễn Quốc",
                    Gender = true,
                    AvatarUrl =
                        "https://st3.depositphotos.com/3431221/13621/v/450/depositphotos_136216036-stock-illustration-man-avatar-icon-hipster-character.jpg",
                    AccountStatusId = Guid.Parse(input: "0c7bed0c-2478-43fd-9a6d-cd084980f749"),
                    IsTemporarilyRemoved = false
                }
            }
        };

        await context.AddAsync(
            new EventEntity()
            {
                Id = Guid.NewGuid(),
                Type = UserBannedEventType,
                StreamId = bannedUserId.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newUsers.Find(match: user => user.Id == bannedUserId),
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            },
            cancellationToken: cancellationToken
        );

        await context.AddAsync(
            new EventEntity()
            {
                Id = Guid.NewGuid(),
                Type = UserAddedEventType,
                StreamId = testUserId.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newUsers.Find(match: user => user.Id == testUserId),
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = App.DefaultGuidValue
            },
            cancellationToken: cancellationToken
        );

        return newUsers;
    }

    private static async Task SeedSystemAccountEntitiesAsync(
        DbContext context,
        IDataProtectionProvider protectionProvider,
        AppBaseProtectionSecurityKeyOption protectionSecurityKeyOption,
        CancellationToken cancellationToken
    )
    {
        const string SystemAccountAddedEventType = "SystemAccountAdded";

        var protector = protectionProvider.CreateProtector(
            purpose: $"{protectionSecurityKeyOption.Value}<{AdminId}>"
        );

        var admin = new SystemAccountEntity
        {
            Id = AdminId,
            UserName = "khoaAdmin",
            NormalizedUserName = "KHOAADMIN",
            Email = "jaykhoale@gmail.com",
            NormalizedEmail = "JAYKHOALE@GMAIL.COM",
            PasswordHash = protector.Protect(plaintext: "Admin123@"),
            AccessFailedCount = default,
            LockoutEnd = App.MinTimeInUTC,
            AccountStatusId = Guid.Parse(input: "0c7bed0c-2478-43fd-9a6d-cd084980f749"),
            SystemAccountRoles = new List<SystemAccountRoleEntity>
            {
                new() { RoleId = Guid.Parse(input: "c95f4aae-2a41-4c76-9cc4-f1d632409525"), }
            }
        };

        await context.AddAsync(
            entity: new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = SystemAccountAddedEventType,
                StreamId = admin.Id.ToString(),
                Data = JsonSerializer.Serialize(
                    value: admin,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = App.DefaultGuidValue
            },
            cancellationToken: cancellationToken
        );

        await context.AddAsync(entity: admin, cancellationToken: cancellationToken);
    }

    private static async Task SeedVoucherTypeEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string VoucherTypeAddedEventType = "VoucherTypeAdded";

        var newVoucherTypes = new List<VoucherTypeEntity>
        {
            new()
            {
                Id = Guid.Parse(input: "cd67f5e8-aa8b-4285-b58c-303ec26bf947"),
                Name = "Sự kiện",
                IsTemporarilyRemoved = false
            },
            new()
            {
                Id = Guid.Parse(input: "d0b05d89-51c8-4d01-b6f8-4dbe62db58dc"),
                Name = "Tích lũy",
                IsTemporarilyRemoved = false
            }
        };

        foreach (var newVoucherType in newVoucherTypes)
        {
            var newVoucherTypeEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = VoucherTypeAddedEventType,
                StreamId = newVoucherType.Id.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newVoucherType,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(
                entity: newVoucherTypeEvent,
                cancellationToken: cancellationToken
            );
        }

        await context.AddRangeAsync(
            entities: newVoucherTypes,
            cancellationToken: cancellationToken
        );
    }

    private static async Task SeedVoucherEntitiesAsync(
        DbContext context,
        CancellationToken cancellationToken
    )
    {
        const string VoucherAddedEventType = "VoucherAdded";

        var newVouchers = new List<VoucherEntity>
        {
            new()
            {
                Code = "chQNlXetJYmssKxjoHks",
                Name = "Giảm 10% cho tất cả sản phẩm",
                QuantityInStock = 10_000,
                DiscountPercentage = 0.1M,
                Description =
                    "Tất cả sản phẩm trong shop đều sẽ được giảm 10%, mỗi sản phẩm chỉ được áp dụng 1 lần",
                DueDate = DateTime.MaxValue.ToUniversalTime(),
                StartDate = DateTime.UtcNow,
                IsTemporarilyRemoved = false,
                VoucherTypeId = Guid.Parse(input: "cd67f5e8-aa8b-4285-b58c-303ec26bf947")
            }
        };

        foreach (var newVoucher in newVouchers)
        {
            var newVoucherEvent = new EventEntity
            {
                Id = Guid.NewGuid(),
                Type = VoucherAddedEventType,
                StreamId = newVoucher.Code.ToString(),
                Data = JsonSerializer.Serialize(
                    value: newVoucher,
                    options: App.DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(entity: newVoucherEvent, cancellationToken: cancellationToken);
        }

        await context.AddRangeAsync(entities: newVouchers, cancellationToken: cancellationToken);
    }
}