namespace ODour.PostgresRelationalDb.Common
{
    internal static class CommonConstant
    {
        internal static class DatabaseNativeType
        {
            internal const string TEXT = "TEXT";

            internal const string TIMESTAMPTZ = "TIMESTAMPTZ";
        }

        internal static class DatabaseSchemaName
        {
            internal const string MAIN = "main";

            internal const string SLAVE = "slave";

            internal const string USER = "user";

            internal const string SYSTEM = "system";

            internal const string ROLE = "role";

            internal const string PRODUCT = "product";

            internal const string PAYMENT = "payment";

            internal const string ORDER = "order";

            internal const string EVENT = "event";

            internal const string CATEGORY = "category";

            internal const string ACCOUNT_STATUS = "account_status";

            internal const string VOUCHER = "voucher";
        }
    }
}
