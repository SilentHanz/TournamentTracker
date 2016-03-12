using Ninject;
using TournamentReport.Services;

namespace TournamentReport.App_Start
{
    public static class Bindings
    {
        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        internal static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IWebSecurityService>().To<WebSecurityService>();
            kernel.Bind<IMessengerService>().To<MessengerService>();
            kernel.Bind<ITournamentContext>().To<TournamentContext>();
        }
    }
}