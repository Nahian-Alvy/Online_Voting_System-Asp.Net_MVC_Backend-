using System.Web.Http;

namespace VotingSystem.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Remove XML formatter if you only want JSON
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}