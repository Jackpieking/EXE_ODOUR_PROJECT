namespace ODour.FastEndpointApi.Feature.Auth.RegisterAsUser.HttpResponse;

/// <summary>
///     register as user extension methods.
/// </summary>
internal static class LazyRegisterAsUserHttResponseMapper
{
    private static RegisterAsUserHttpResponseManager _registerAsUserHttpResponseManager;

    internal static RegisterAsUserHttpResponseManager Get()
    {
        return _registerAsUserHttpResponseManager ??= new();
    }
}
