﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ODour.Domain.Share.Base.Entities;
using ODour.Domain.Share.Payment.Entities;
using ODour.Domain.Share.User.Entities;

namespace ODour.Domain.Share.Order.Entities;

public sealed class OrderEntity : IEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    #region ForeignKeys
    public Guid OrderStatusId { get; set; }

    public Guid UserId { get; set; }

    public Guid PaymentMethodId { get; set; }
    #endregion

    public string FullName { get; set; }

    public string PhoneNumber { get; set; }

    public long OrderCode { get; set; }

    public string OrderNote { get; set; }

    public decimal TotalPrice { get; set; }

    public string DeliveredAddress { get; set; }

    public DateTime DeliveredAt { get; set; }

    #region NavigationProperties
    public PaymentMethodEntity PaymentMethod { get; set; }

    public UserDetailEntity User { get; set; }

    public OrderStatusEntity OrderStatus { get; set; }
    #endregion

    #region NavigationCollections
    public IEnumerable<OrderItemEntity> OrderItems { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "Orders";

        public static class OrderCode
        {
            public const long MinValue = default;

            public const long MaxValue = long.MaxValue;
        }

        public static class OrderNote
        {
            public const int MinLength = 2;

            public const int MaxLength = 500;
        }

        public static class FullName
        {
            public const int MinLength = 2;

            public const int MaxLength = 100;
        }

        public static class PhoneNumber
        {
            public const int MinLength = 2;

            public const int MaxLength = 20;
        }

        public static class TotalPrice
        {
            public const int Precision = 12;

            public const int Scale = 2;
        }

        public static class DeliveredAddress
        {
            public const int MinLength = 2;

            public const int MaxLength = 500;
        }
    }
    #endregion

    public static long GenerateOrderCode(DateTime dateTime)
    {
        const int length = 16;

        // Format date and time for extraction (modify if needed)
        var formattedDateTime = dateTime.ToString(format: "yyyyMMddHHmmss");

        var codeBuilder = new StringBuilder(value: formattedDateTime);

        var codeLength = length - formattedDateTime.Length;

        codeBuilder.Append(value: GenerateRandomNumberCode(length: codeLength));

        return long.Parse(s: codeBuilder.ToString());

        static string GenerateRandomNumberCode(int length)
        {
            const int FromInclusive = default;
            const int ToExclusive = 9;

            // Build the code string with random digits
            var code = new StringBuilder(capacity: length);

            for (int idx = 0; idx < length; idx++)
            {
                // Generate digit between 0 and 9 (inclusive)
                code.Append(
                    value: RandomNumberGenerator.GetInt32(
                        fromInclusive: FromInclusive,
                        toExclusive: ToExclusive
                    )
                );
            }

            return code.ToString();
        }
    }
}
