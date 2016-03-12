using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using NSubstitute;
using TournamentReport;
using TournamentReport.Controllers;
using TournamentReport.Models;
using TournamentReport.Services;
using Xunit;

public class AccountControllerTests
{
    public class TheForgotPasswordMethod
    {
        public TheForgotPasswordMethod()
        {
            // Fake out env for VirtualPathUtility.ToAbsolute(..)
            string path = AppDomain.CurrentDomain.BaseDirectory;
            const string virtualDir = "/";
            AppDomain.CurrentDomain.SetData(".appDomain", "*");
            AppDomain.CurrentDomain.SetData(".appPath", path);
            AppDomain.CurrentDomain.SetData(".appVPath", virtualDir);
            AppDomain.CurrentDomain.SetData(".hostingVirtualPath", virtualDir);
            AppDomain.CurrentDomain.SetData(".hostingInstallDir", HttpRuntime.AspInstallDirectory);
            var tw = new StringWriter();
            HttpWorkerRequest wr = new SimpleWorkerRequest("default.aspx", "", tw);
            HttpContext.Current = new HttpContext(wr);
        }

        [Theory]
        [InlineData(42, "attacker@example.com", true)]
        [InlineData(42, "haacked@example.com", false)]
        [InlineData(43, "haacked@example.com", true)]
        public void DoesNotSendMessageIfUserIsNotConfirmedOrIfEmailDoesNotMatchUser(int userId, string submittedEmail, bool isConfirmed)
        {
            var webSecurity = Substitute.For<IWebSecurityService>();
            webSecurity.GetUserId("haacked").Returns(userId);
            webSecurity.IsConfirmed("haacked").Returns(isConfirmed);
            var messengerService = Substitute.For<IMessengerService>();
            var users = new TestDbSet<User> { new User { Id = 42, Name = "haacked", Email = "haacked@example.com" }, new User() };
            var tournamentContext = Substitute.For<ITournamentContext>();
            tournamentContext.Users.Returns(users);
            var accountController = new AccountController(webSecurity, messengerService, tournamentContext);
            var request = Substitute.For<HttpRequestBase>();
            request.Url.Returns(new Uri("http://localhost/"));
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Returns(request);
            accountController.ControllerContext = new ControllerContext(httpContext, new RouteData(), accountController);
            var forgotPasswordModel = new ForgotPasswordModel
            {
                UserName = "haacked",
                Email = submittedEmail
            };

            accountController.ForgotPassword(forgotPasswordModel);

            messengerService.DidNotReceive().Send(Args.String, Args.String, Args.String, Args.String, Args.Boolean);
        }

        [Fact]
        public void SendsResetMessageIfUserIsConfirmedAndEmailMatchesUserEmail()
        {
            var webSecurity = Substitute.For<IWebSecurityService>();
            webSecurity.GetUserId("haacked").Returns(42);
            webSecurity.IsConfirmed("haacked").Returns(true);
            var messengerService = Substitute.For<IMessengerService>();
            var users = new TestDbSet<User> { new User { Id = 42, Name = "haacked", Email = "haacked@example.com" }, new User() };
            var tournamentContext = Substitute.For<ITournamentContext>();
            tournamentContext.Users.Returns(users);
            var accountController = new AccountController(webSecurity, messengerService, tournamentContext);
            var request = Substitute.For<HttpRequestBase>();
            request.Url.Returns(new Uri("http://localhost/"));
            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Returns(request);
            accountController.ControllerContext = new ControllerContext(httpContext, new RouteData(), accountController);
            var forgotPasswordModel = new ForgotPasswordModel
            {
                UserName = "haacked",
                Email = "haacked@example.com"
            };

            accountController.ForgotPassword(forgotPasswordModel);

            messengerService.Received().Send(Args.String, Args.String, Args.String, Args.String, Args.Boolean);
        }
    }
}
