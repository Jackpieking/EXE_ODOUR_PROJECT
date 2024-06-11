using System.Collections.Concurrent;

namespace ODour.Domain.Share.Filter;

public static class SortingFilterTypes
{
    public const string DEFAULT = "none";

    public static class Product
    {
        public const string PROD_NAME_DESC = "prod:name:desc";

        public const string PROD_NAME_ASC = "prod:name:asc";

        public const string PROD_PRICE_ASC = "prod:price:asc";

        public const string PROD_PRICE_DESC = "prod:price:desc";

        public static readonly ConcurrentBag<string> AppFilterList =
            new() { PROD_NAME_ASC, PROD_NAME_DESC, PROD_PRICE_ASC, PROD_PRICE_DESC, DEFAULT };
    }
}
