namespace ODour.Configuration.Infrastructure.Persistence.SessionStorage;

public sealed class RedisSessionStorageOption
{
    public int IdleTimeoutInSecond { get; set; }

    public CookieOption Cookie { get; set; } = new();

    public sealed class CookieOption
    {
        public string Name { get; set; }

        public bool HttpOnly { get; set; }

        public bool IsEssential { get; set; }

        public int SecurePolicy { get; set; }

        public int SameSite { get; set; }
    }
}
