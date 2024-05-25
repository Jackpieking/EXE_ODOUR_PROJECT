using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ODour.Domain.Share.AccountStatus.Entities;
using ODour.Domain.Share.Category.Entities;
using ODour.Domain.Share.Order.Entities;
using ODour.Domain.Share.Payment.Entities;
using ODour.Domain.Share.Product.Entities;
using ODour.Domain.Share.Role.Entities;
using ODour.Domain.Share.SeedFlag.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.PostgresRelationalDb.Data;

public static class EntityDataSeeding
{
    private static readonly Guid AdminId = Guid.Parse(
        input: "ffccce5f-fbb1-4c1e-be6b-b22028134056"
    );

    public static async Task<bool> SeedAsync(
        ODourContext context,
        UserManager<UserEntity> userManager,
        RoleManager<RoleEntity> roleManager,
        CancellationToken cancellationToken
    )
    {
        // Check if all tables of database are empty.
        var areTablesEmtpy = await AreAllTablesEmptyAsync(context, cancellationToken);

        if (!areTablesEmtpy)
        {
            return false;
        }

        // Start seeding.
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

        var seedRoles = await GetSeedRoleEntitiesAsync(
            context: context,
            cancellationToken: cancellationToken
        );

        var seedUsers = GetSeedUserEntities();

        return true;
    }

    private static async Task<bool> AreAllTablesEmptyAsync(
        ODourContext context,
        CancellationToken cancellationToken
    )
    {
        var isTableEmtpy = await context
            .Set<SeedFlagEntity>()
            .AnyAsync(cancellationToken: cancellationToken);

        if (!isTableEmtpy)
        {
            return false;
        }

        return true;
    }

    private static async Task SeedAccountStatusEntitiesAsync(
        ODourContext context,
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
            var newAccountStatusEvent = new AccountStatusEventEntity
            {
                Id = Guid.NewGuid(),
                Type = AccountStatusAddedEventType,
                StreamId = newAccountStatus.Id,
                OldData = JsonSerializer.Serialize(value: string.Empty),
                NewData = JsonSerializer.Serialize(
                    value: newAccountStatus,
                    options: Application
                        .Share
                        .Common
                        .CommonConstant
                        .App
                        .DefaultJsonSerializerOptions
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
        ODourContext context,
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
            var newCategoryEvent = new CategoryEventEntity
            {
                Id = Guid.NewGuid(),
                Type = CategoryAddedEventType,
                StreamId = newCategory.Id,
                OldData = JsonSerializer.Serialize(value: string.Empty),
                NewData = JsonSerializer.Serialize(
                    value: newCategory,
                    options: Application
                        .Share
                        .Common
                        .CommonConstant
                        .App
                        .DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(entity: newCategoryEvent, cancellationToken: cancellationToken);
        }

        await context.AddRangeAsync(entities: newCategories, cancellationToken: cancellationToken);
    }

    private static async Task SeedOrderStatusEntitiesAsync(
        ODourContext context,
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
                Id = Guid.Parse(input: "37eb8293-c17a-45a6-89ab-ba640d4001ff"),
                Name = "Đã hủy",
                IsTemporarilyRemoved = false
            }
        };

        foreach (var newOrderStatus in newOrderStatuses)
        {
            var newOrderStatusEvent = new OrderStatusEventEntity
            {
                Id = Guid.NewGuid(),
                Type = OrderStatusAddedEventType,
                StreamId = newOrderStatus.Id,
                OldData = JsonSerializer.Serialize(value: string.Empty),
                NewData = JsonSerializer.Serialize(
                    value: newOrderStatus,
                    options: Application
                        .Share
                        .Common
                        .CommonConstant
                        .App
                        .DefaultJsonSerializerOptions
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
        ODourContext context,
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
            var newPaymentMethodEvent = new PaymentMethodEventEntity
            {
                Id = Guid.NewGuid(),
                Type = PayemntMethodAddedEventType,
                StreamId = newPaymentMethod.Id,
                OldData = JsonSerializer.Serialize(value: string.Empty),
                NewData = JsonSerializer.Serialize(
                    value: newPaymentMethod,
                    options: Application
                        .Share
                        .Common
                        .CommonConstant
                        .App
                        .DefaultJsonSerializerOptions
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
        ODourContext context,
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
            var newProductStatusEvent = new ProductStatusEventEntity
            {
                Id = Guid.NewGuid(),
                Type = ProductStatusAddedEventType,
                StreamId = newProductStatus.Id,
                OldData = JsonSerializer.Serialize(value: string.Empty),
                NewData = JsonSerializer.Serialize(
                    value: newProductStatus,
                    options: Application
                        .Share
                        .Common
                        .CommonConstant
                        .App
                        .DefaultJsonSerializerOptions
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
        ODourContext context,
        CancellationToken cancellationToken
    )
    {
        List<ProductEntity> newProducts =
            new()
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
                },
                new()
                {
                    Id = "LD02",
                    ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                    CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                    Name = "CK ONE GOLD",
                    UnitPrice = 199_000,
                    Description =
                        "{\"NHOM_HUONG\":[\"Oriental - tông mùi phương đông.\",\"Floral - tông hoa.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Quả sung, Cam Bergamot, Lá xô thơm.\",\"HUONG_GIUA\":\"Hoa trắng, Hoa nhài, Hoa violet.\",\"HUONG_CUOI\":\"Gỗ guaiac, Cỏ hương bài, Hoắc hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 16 giờ.\",\"TREN_VAI\":\"2 -3 ngày.\"}}",
                    QuantityInStock = 10,
                },
                new()
                {
                    Id = "LD03",
                    ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                    CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                    Name = "212 VIP MEN",
                    UnitPrice = 199_000,
                    Description =
                        "{\"NHOM_HUONG\":[\"Leather - tông da thuộc.\",\"Fruity - hương trái cây.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Chanh, Tiêu, Gừng, Chanh dây.\",\"HUONG_GIUA\":\"Rượu Vodka, Rượu Gin, Bạc hà.\",\"HUONG_CUOI\":\"Hổ phách, Da thuộc.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 16 giờ.\",\"TREN_VAI\":\"2 -3 ngày.\"}}",
                    QuantityInStock = 10,
                },
                new()
                {
                    Id = "LD04",
                    ProductStatusId = Guid.Parse(input: "a185f047-b809-46be-b40a-d6c9c185def9"),
                    CategoryId = Guid.Parse(input: "42e48d19-b2f6-4112-b14f-0b0131b339d5"),
                    Name = "BLEU DE CHANEL",
                    UnitPrice = 199_000,
                    Description =
                        "{\"NHOM_HUONG\":[\"Oriental — tông mùi Phương Đông.\"],\"TANG_HUONG\":{\"HUONG_DAU\":\"Chanh vàng, Ớt hồng, Bạc hà.\",\"HUONG_GIUA\":\"Dưa vàng, Hoa nhài, Gừng.\",\"HUONG_CUOI\":\"Gỗ tuyết tùng, Hổ phách, Gỗ đàn hương.\"},\"DO_LUU_HUONG\":{\"TREN_DA\":\"14 - 18 giờ.\",\"TREN_VAI\":\"2 -3 ngày.\"}}",
                    QuantityInStock = 10,
                }
            };

        await context.AddRangeAsync(entities: newProducts, cancellationToken: cancellationToken);
    }

    private static async Task<List<RoleEntity>> GetSeedRoleEntitiesAsync(
        ODourContext context,
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
                Name = "Admin",
                RoleDetail = new() { RoleId = adminRoleId, IsTemporarilyRemoved = false }
            },
            new()
            {
                Id = userRoleId,
                Name = "User",
                RoleDetail = new() { RoleId = userRoleId, IsTemporarilyRemoved = false }
            }
        };

        foreach (var newRole in newRoles)
        {
            var newRoleEvent = new RoleEventEntity
            {
                Id = Guid.NewGuid(),
                Type = RoleAddedEventType,
                StreamId = newRole.Id,
                OldData = JsonSerializer.Serialize(value: string.Empty),
                NewData = JsonSerializer.Serialize(
                    value: newRole,
                    options: Application
                        .Share
                        .Common
                        .CommonConstant
                        .App
                        .DefaultJsonSerializerOptions
                ),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = AdminId
            };

            await context.AddAsync(entity: newRoleEvent, cancellationToken: cancellationToken);
        }

        return newRoles;
    }

    private static List<UserEntity> GetSeedUserEntities()
    {
        var testUserId = Guid.Parse(input: "8ebe29bc-4706-4fda-bb28-ed127d150c05");
        var bannedUserId = Guid.Parse(input: "9a2b6683-a16e-4270-a23c-3e09a6e27345");

        return new()
        {
            new()
            {
                Id = AdminId,
                UserName = "khoaAdmin",
                Email = "jaykhoale@gmail.com",
                PhoneNumber = "0706042250",
                UserDetail = new()
                {
                    UserId = AdminId,
                    FirstName = "Khoa",
                    LastName = "Lê Đình Đăng",
                    Gender = true,
                    AvatarUrl =
                        "https://w7.pngwing.com/pngs/340/946/png-transparent-avatar-user-computer-icons-software-developer-avatar-child-face-heroes-thumbnail.png",
                    AccountStatusId = Guid.Parse(input: "4ac49b1d-eb67-4530-a919-4d9de312ee1f")
                }
            },
            new()
            {
                Id = testUserId,
                UserName = "testUser",
                Email = "khoaprovn041@gmail.com",
                PhoneNumber = "0706042250",
                UserDetail = new()
                {
                    UserId = testUserId,
                    FirstName = "Thắng",
                    LastName = "Trần Minh",
                    Gender = true,
                    AvatarUrl =
                        "https://www.shutterstock.com/image-vector/young-smiling-man-avatar-brown-600nw-2261401207.jpg",
                    AccountStatusId = Guid.Parse(input: "4ac49b1d-eb67-4530-a919-4d9de312ee1f")
                }
            },
            new()
            {
                Id = bannedUserId,
                UserName = "bannedUser",
                Email = "khoamapdit03@gmail.com",
                PhoneNumber = "0706042250",
                UserDetail = new()
                {
                    UserId = bannedUserId,
                    FirstName = "Khánh",
                    LastName = "Nguyễn Quốc",
                    Gender = true,
                    AvatarUrl =
                        "https://st3.depositphotos.com/3431221/13621/v/450/depositphotos_136216036-stock-illustration-man-avatar-icon-hipster-character.jpg",
                    AccountStatusId = Guid.Parse(input: "0c7bed0c-2478-43fd-9a6d-cd084980f749")
                }
            }
        };
    }
}
