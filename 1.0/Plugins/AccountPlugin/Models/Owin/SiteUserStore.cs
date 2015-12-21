using DAL.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Common.Extensions;
using Common.Helpers;
using System.Transactions;

namespace AccountPlugin.Models.Owin
{
    public class SiteUserStore<TUser> : IUserStore<TUser>,
    IUserClaimStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserEmailStore<TUser>,
    IUserLockoutStore<TUser, string>,
    IUserSecurityStampStore<TUser>,
    IUserTwoFactorStore<TUser, string>,
    IDisposable where TUser : SiteUser
    {
        private readonly IOwinContext ctx;
        private readonly Func<IDbConnection> cnn;

        public SiteUserStore(String connectionStringName, IOwinContext context)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            var factory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            this.cnn = () =>
                {
                    var x = factory.CreateConnection();
                    x.ConnectionString = connectionString.ConnectionString;
                    x.Open();
                    return x;
                };
            this.ctx = context;
        }

        #region IUserStore
        public async Task CreateAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            var dbuser = TransformUserToDb(owinuser);
            dbuser.DateFormat = ListHelper.GetEnumDescription(ListHelper.DateFormats.yyyyMMdd);
            dbuser.TimeFormat = ListHelper.GetEnumDescription(ListHelper.TimeFormats.hhmm);

            await Task.Run(() => dbuser.Upsert());
            await Task.Run(() => UserClaim.DeleteForUser(dbuser.Id));
            await Task.Run(() => UserClaim.InsertBulk(dbuser.Claims)); 
            
            ctx.Set<Cached>("SiteUserStore-" + owinuser.UserName, new Cached { Value = owinuser });
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            return FindByNameAsync(userId);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (userName.IsNullOrWhiteSpace()) throw new ArgumentNullException("userName");

            var cached = ctx.Get<Cached>("SiteUserStore-" + userName);
            var owinuser = cached != null ? cached.Value : null;
            if (owinuser == null)
            {
                var dbuser = User.Get(null, userName, null);
                owinuser = TransformUserFromDb(dbuser);
                ctx.Set<Cached>("SiteUserStore-" + userName, new Cached { Value = owinuser });
            }
            return Task.FromResult(owinuser);
        }

        public Task UpdateAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            var dbuser = TransformUserToDb(owinuser);
            ctx.Set<Cached>("SiteUserStore-" + owinuser.UserName, new Cached { Value = owinuser });
            return Task.Run(() => dbuser.Upsert());
        }

        public Task DeleteAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            var id = owinuser.Id.ParseInt64();
            ctx.Set<Cached>("SiteUserStore-" + owinuser.UserName, null);
            return Task.Run(() => User.DeleteFlag(id, true));
        }
        #endregion

        #region IUserClaimStore
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            if (user == null) throw new ArgumentNullException("user");

            return Task.FromResult<IList<Claim>>(user.Claims.Select(clm => new Claim(clm.ClaimType, clm.ClaimValue)).ToList());
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");

            user.AddClaim(claim);
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (claim == null) throw new ArgumentNullException("claim");

            SiteUserClaim userClaim = user.Claims
                .FirstOrDefault(clm => clm.ClaimType == claim.Type && clm.ClaimValue == claim.Value);

            if (userClaim != null)
            {
                user.RemoveClaim(userClaim);
            }

            return Task.FromResult(0);
        }
        #endregion

        #region IUserPasswordStore
        public Task<string> GetPasswordHashAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            return Task.FromResult<string>(owinuser.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            return Task.FromResult<bool>(owinuser.PasswordHash != null);
        }

        public Task SetPasswordHashAsync(TUser owinuser, string passwordHash)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            owinuser.SetPasswordHash(passwordHash);
            return Task.FromResult(0);
        }
        #endregion

        #region IUserEmailStore
        public async Task<TUser> FindByEmailAsync(string email)
        {
            var dbuser = User.Get(null, null, email);
            var owinuser = TransformUserFromDb(dbuser);
            return await Task.FromResult(owinuser);
        }

        public Task<string> GetEmailAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            return Task.FromResult(owinuser.Email);
        }

        public async Task<bool> GetEmailConfirmedAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            return await Task.FromResult(owinuser.EmailConfirmed);
        }

        public Task SetEmailAsync(TUser owinuser, string email)
        {
            if (owinuser == null) throw new ArgumentNullException("user");
            if (email.IsNullOrWhiteSpace()) throw new ArgumentNullException("email");

            owinuser.SetEmail(email);
            return Task.FromResult(0);
        }

        public async Task SetEmailConfirmedAsync(TUser owinuser, bool confirmed)
        {
            if (owinuser == null) throw new ArgumentNullException("user");
            if (confirmed == null) throw new ArgumentNullException("confirmed");

            var dbuser = User.Get(null, null, owinuser.Id);
            dbuser.EmailConfirmed = confirmed;
            dbuser.Upsert();
            owinuser.EmailConfirmed = confirmed;
            await Task.FromResult(0);
        }
        #endregion

        #region IUserLockoutStore
        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");
            if (owinuser.LockoutEndDate == null) throw new InvalidOperationException("LockoutEndDate has no value.");

            return Task.FromResult(owinuser.LockoutEndDate.Value);
        }

        public Task SetLockoutEndDateAsync(TUser owinuser, DateTimeOffset lockoutEnd)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            owinuser.LockUntil(lockoutEnd);
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            owinuser.IncrementAccessFailedCount();
            return Task.FromResult(owinuser.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            owinuser.ResetAccessFailedCount();
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            return Task.FromResult(owinuser.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            return Task.FromResult(owinuser.IsLockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser owniuser, bool enabled)
        {
            if (owniuser == null) throw new ArgumentNullException("user");

            if (enabled)
            {
                owniuser.EnableLockout();
            }
            else
            {
                owniuser.DisableLockout();
            }

            return Task.FromResult(0);
        }
        #endregion

        #region IUserSecurityStampStore
        public Task<string> GetSecurityStampAsync(TUser owinuser)
        {
            if (owinuser == null) throw new ArgumentNullException("user");

            return Task.FromResult<string>(owinuser.SecurityStamp);
        }

        public Task SetSecurityStampAsync(TUser owinuser, string stamp)
        {
            if (owinuser == null) throw new ArgumentNullException("user");
            if (stamp.IsNullOrWhiteSpace()) throw new ArgumentNullException("stamp");

            owinuser.SetSecurityStamp(stamp);
            return Task.FromResult(0);
        }
        #endregion

        #region IUserTwoFactorStore
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.IsTwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            if (user == null) throw new ArgumentNullException("user");

            if (enabled)
            {
                user.EnableTwoFactorAuthentication();
            }
            else
            {
                user.DisableTwoFactorAuthentication();
            }

            return Task.FromResult(0);
        }
        #endregion

        #region Transforms
        public TUser TransformUserFromDb(User dbuser)
        {
            if (dbuser == null)
                return null;

            var owinuser = new SiteUser(dbuser.UserName, dbuser.Email);
            owinuser.Id = dbuser.UserName;
            owinuser.PasswordHash = dbuser.PasswordHash;
            owinuser.SecurityStamp = dbuser.SecurityStamp;
            owinuser.Email = dbuser.Email;
            owinuser.EmailConfirmed = dbuser.EmailConfirmed;
            owinuser.PhoneNumber = dbuser.PhoneNumber;
            owinuser.PhoneNumberConfirmed = dbuser.PhoneNumberConfirmed;
            owinuser.AccessFailedCount = dbuser.AccessFailedCount;
            owinuser.LockoutEnabled = dbuser.LockoutEnabled;
            if (!dbuser.Claims.IsNullOrEmpty())
                dbuser.Claims.ForEach(c => owinuser.AddClaim(new Claim(c.Type, c.Value)));
            return owinuser as TUser;
        }

        public User TransformUserToDb(TUser owinuser)
        {
            if (owinuser == null)
                return null;

            var dbuser = User.Get(null, owinuser.UserName, null);
            if (dbuser == null)
                dbuser = new User { UserName = owinuser.UserName };
            dbuser.PasswordHash = owinuser.PasswordHash;
            dbuser.SecurityStamp = owinuser.SecurityStamp;
            dbuser.Email = owinuser.Email;
            dbuser.EmailConfirmed = owinuser.EmailConfirmed;
            dbuser.PhoneNumber = owinuser.PhoneNumber;
            dbuser.PhoneNumberConfirmed = owinuser.PhoneNumberConfirmed;
            dbuser.AccessFailedCount = owinuser.AccessFailedCount;
            dbuser.LockoutEnabled = owinuser.LockoutEnabled;
            if (!owinuser.Claims.IsNullOrEmpty())
            {
                foreach (var c in owinuser.Claims)
                    dbuser.Claims.Add(new UserClaim { UserId = dbuser.Id, Type = c.ClaimType, Value = c.ClaimValue });
            }
            return dbuser;
        }
        #endregion

        class Cached
        {
            public TUser Value { get; set; }
        }

        #region Dispose
        protected void Dispose(bool disposing)
        {
            //if (_disposeDocumentSession && disposing && _documentSession != null)
            //{
            //    _documentSession.Dispose();
            //}
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}