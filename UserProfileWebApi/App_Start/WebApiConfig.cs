using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using UserProfileWebApi;
using System.Web.Http;

namespace UserProfileWebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
            );

            
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Contact>("Contacts");
            builder.EntitySet<Profile>("Profiles");
            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
