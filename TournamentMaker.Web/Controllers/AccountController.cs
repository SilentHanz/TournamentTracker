using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TournamentReport.Models;
using TournamentReport.Services;

namespace TournamentReport.Controllers
{
    public class AccountController : Controller
    {
        IWebSecurityService webSecurity;
        IMessengerService messengerService;
        ITournamentContext tournamentContext;

        public AccountController(IWebSecurityService webSecurity, IMessengerService messengerService, ITournamentContext tournamentContext)
        {
            this.webSecurity = webSecurity;
            this.messengerService = messengerService;
            this.tournamentContext = tournamentContext;
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************
        public ActionResult Login()
        {
            return RedirectToAction("LogOn");
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (webSecurity.Login(model.UserName, model.Password, model.RememberMe))
                {
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            webSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewBag.PasswordLength = webSecurity.MinPasswordLength;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var requireEmailConfirmation =
                    Convert.ToBoolean(ConfigurationManager.AppSettings["requireEmailConfirmation"] ?? "false");
                var token = webSecurity.CreateUserAndAccount(model.UserName, model.Password,
                                                                    requireConfirmationToken: requireEmailConfirmation);

                if (requireEmailConfirmation)
                {
                    // Send email to user with confirmation token
                    if (Request.Url != null)
                    {
                        string hostUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                        string confirmationUrl = hostUrl +
                                                 VirtualPathUtility.ToAbsolute("~/Account/Confirm?confirmationCode=" +
                                                                               HttpUtility.UrlEncode(token));

                        const string fromAddress = "Your Email Address";
                        var toAddress = model.Email;
                        const string subject =
                            "Thanks for registering but first you need to confirm your registration...";
                        var body =
                            string.Format(
                                "Your confirmation code is: {0}. Visit <a href=\"{1}\">{1}</a> to activate your account.",
                                token, confirmationUrl);

                        // NOTE: This is just for sample purposes
                        // It's generally a best practice to not send emails (or do anything on that could take a long time and potentially fail)
                        // on the same thread as the main site
                        // You should probably hand this off to a background MessageSender service by queueing the email, etc.
                        messengerService.Send(fromAddress, toAddress, subject, body, true);
                    }

                    // Thank the user for registering and let them know an email is on its way
                    return RedirectToAction("Thanks", "Account");
                }
                // Navigate back to the homepage and exit
                webSecurity.Login(model.UserName, model.Password);
                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = webSecurity.MinPasswordLength;
            return View(model);
        }

        public ActionResult Confirm()
        {
            string confirmationToken = Request.QueryString["confirmationCode"];
            webSecurity.Logout();

            if (!string.IsNullOrEmpty(confirmationToken))
            {
                if (webSecurity.ConfirmAccount(confirmationToken))
                {
                    ViewBag.Message =
                        "Registration Confirmed! Click on the login link at the top right of the page to continue.";
                }
                else
                {
                    ViewBag.Message = "Could not confirm your registration info";
                }
            }

            return View();
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = webSecurity.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (webSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = webSecurity.MinPasswordLength;
            return View(model);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            var isValid = false;
            var resetToken = string.Empty;

            if (ModelState.IsValid)
            {
                var userId = webSecurity.GetUserId(model.UserName);
                var user = tournamentContext.Users.Find(userId);
                if (user != null && webSecurity.IsConfirmed(model.UserName) && user.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    resetToken = webSecurity.GeneratePasswordResetToken(model.UserName);
                    isValid = true;
                }

                if (isValid)
                {
                    if (Request.Url != null)
                    {
                        string hostUrl = Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
                        string resetUrl = hostUrl +
                                          VirtualPathUtility.ToAbsolute("~/Account/PasswordReset?resetToken=" +
                                                                        HttpUtility.UrlEncode(resetToken));

                        var fromAddress = "Your Email Address";
                        var toAddress = model.Email;
                        var subject = "Password reset request";
                        var body =
                            string.Format(
                                "Use this password reset token to reset your password. <br/>The token is: {0}<br/>Visit <a href='{1}'>{1}</a> to reset your password.<br/>",
                                resetToken, resetUrl);

                        messengerService.Send(fromAddress, toAddress, subject, body, true);
                    }
                }
                return RedirectToAction("ForgotPasswordMessage");
            }
            return View(model);
        }

        public ActionResult ForgotPasswordMessage()
        {
            return View();
        }

        public ActionResult PasswordReset()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordReset(PasswordResetModel model)
        {
            if (ModelState.IsValid)
            {
                if (webSecurity.ResetPassword(model.ResetToken, model.NewPassword))
                {
                    return RedirectToAction("PasswordResetSuccess");
                }
                ModelState.AddModelError("", "The password reset token is invalid.");
            }

            return View(model);
        }

        public ActionResult PasswordResetSuccess()
        {
            return View();
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult Thanks()
        {
            return View();
        }
    }
}