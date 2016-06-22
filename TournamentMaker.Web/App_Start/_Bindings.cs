using SimpleInjector;
using TournamentReport.Services;

namespace TournamentReport.App_Start
{
    public static class Bindings
    {
        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="container">The container.</param>
        internal static void RegisterServices(Container container)
        {
            container.Register<IWebSecurityService, WebSecurityService>();
            container.Register<IMessengerService, MessengerService>();
            container.Register<ITournamentContext, TournamentContext>(Lifestyle.Scoped);
        }
    }
}