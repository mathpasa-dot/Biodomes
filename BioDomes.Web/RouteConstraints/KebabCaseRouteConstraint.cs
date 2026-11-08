using BioDomes.Domains;

namespace BioDomes.Web.RouteConstraints;

public class KebabCaseRouteConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value) || value is null)
            return false;

        var slug = value.ToString();

        // Le slug doit être identique à sa version KebabCase
        return slug == slug!.ToKebabCase();
    }
}