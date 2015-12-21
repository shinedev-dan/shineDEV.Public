using AccountPlugin.Core;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace AccountPlugin.Models.Owin
{
    public class SiteUserManager : UserManager<SiteUser>
    {
        public SiteUserManager(SiteUserStore<SiteUser> store)
            : base(store)
        {
        }

        public static SiteUserManager Create(IdentityFactoryOptions<SiteUserManager> options, IOwinContext context)
        {
            var manager = new SiteUserManager(new SiteUserStore<SiteUser>("DefaultConnection", context));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<SiteUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            manager.EmailService = new EmailService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<SiteUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                    {
                        // how long reset password and confirmation email codes are valid for
                        TokenLifespan = TimeSpan.FromHours(12)
                    };
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class SiteSignInManager : SignInManager<SiteUser, string>
    {
        public SiteSignInManager(SiteUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(SiteUser user)
        {
            return user.GenerateUserIdentityAsync((SiteUserManager)UserManager);
        }

        public static SiteSignInManager Create(IdentityFactoryOptions<SiteSignInManager> options, IOwinContext context)
        {
            return new SiteSignInManager(context.GetUserManager<SiteUserManager>(), context.Authentication);
        }
    }
}