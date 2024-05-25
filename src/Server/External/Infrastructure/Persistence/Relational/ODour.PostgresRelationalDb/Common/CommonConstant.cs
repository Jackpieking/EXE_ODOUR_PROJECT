namespace ODour.PostgresRelationalDb.Common;

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

        internal const string REPLICA = "replica";

        internal const string USER = "user";

        internal const string SYSTEM_ACCOUNT = "system_account";

        internal const string SEED_FLAG = "seed_flag";

        internal const string ROLE = "role";

        internal const string PRODUCT = "product";

        internal const string PAYMENT = "payment";

        internal const string ORDER = "order";

        internal const string EVENT_SNAPSHOT = "event_snapshot";

        internal const string CATEGORY = "category";

        internal const string ACCOUNT_STATUS = "account_status";
    }
}
