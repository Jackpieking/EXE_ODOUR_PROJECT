namespace ODour.Application.Share.Session;

public sealed class AppSessionModel<TSource>
{
    public TSource Value { get; set; }

    public static readonly AppSessionModel<TSource> NotFound;
}
