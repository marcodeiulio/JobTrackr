namespace JobTrackr.Application.Companies.Commands.Shared;

public static class CompanyCommandValidatorHelpers
{
    public static bool BeValidUrlIfProvided(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            return uriResult.Host.Contains('.') && !uriResult.Host.EndsWith('.');

        if (!url.Contains("://") && Uri.TryCreate($"http://{url}", UriKind.Absolute, out var looseUriResult))
        {
            var host = looseUriResult.Host;
            return host.Contains('.') && !host.EndsWith('.');
        }

        return false;
    }
}