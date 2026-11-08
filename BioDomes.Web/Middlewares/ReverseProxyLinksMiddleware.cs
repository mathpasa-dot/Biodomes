/**
 * q : Quel fichier crée le pipeline de middleware ?
 * a : Le pipeline de middleware est créé dans le fichier Program.cs, où les middlewares sont ajoutés à l'application à l'aide de méthodes d'extension telles que UseLogOrigin() pour le LogOriginMiddleware.
 *
 * q : Quelle méthode app.UseXxx(...) appeler ?
 * a : Pour utiliser le LogOriginMiddleware, vous devez appeler
 * la méthode d'extension app.UseLogOrigin() dans le fichier Program.cs, après avoir enregistré le middleware dans les services.
 *
 * q :Quelle particularité présente le contenu du dossier wwwroot ?
 * a : Le dossier wwwroot est un répertoire spécial dans les applications ASP.NET
 * Core qui est utilisé pour stocker les fichiers statiques tels que les images,
 * les fichiers CSS et JavaScript. Les fichiers placés dans ce dossier sont accessibles
 * directement via des URL, ce qui permet de servir facilement des ressources statiques aux clients.
 *
 *
 */

namespace BioDomes.Web.Middlewares;

public class ReverseProxyLinksMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var pathBase))
        {
            context.Request.PathBase = pathBase[^1];
            if (context.Request.Path.StartsWithSegments(context.Request.PathBase, out var path))
                context.Request.Path = path;
        }

        return Task.Run(() => next(context));
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder ReverseProxyMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ReverseProxyLinksMiddleware>();
    }
}