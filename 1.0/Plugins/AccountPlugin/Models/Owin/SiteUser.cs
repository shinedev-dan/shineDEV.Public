using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AccountPlugin.Models.Owin
{
    public class SiteUser : IdentityUser
    {
        private List<SiteUserClaim> _claims;

        [JsonConstructor]
        public SiteUser(string userName)
        {
            if (userName == null) throw new ArgumentNullException("userName");

            Id = userName;// GenerateKey(userName);
            UserName = userName;
            _claims = new List<SiteUserClaim>();
        }

        public SiteUser(string userName, string email) : this(userName)
        {
            Email = email;
        }

        //public string Id { get; private set; }
        //public string UserName { get; set; }
        //public string Email { get; private set; }
        //public string PhoneNumber { get; private set; }
        //public string PasswordHash { get; private set; }
        //public string SecurityStamp { get; private set; }
        public bool IsLockoutEnabled { get; private set; }
        public bool IsTwoFactorEnabled { get; private set; }

        //public int AccessFailedCount { get; private set; }
        public DateTimeOffset? LockoutEndDate { get; private set; }

        public new IEnumerable<SiteUserClaim> Claims
        {
            get
            {
                return _claims;
            }

            private set
            {
                if (_claims == null)
                {
                    _claims = new List<SiteUserClaim>();
                }

                _claims.AddRange(value);
            }
        }

        public virtual void EnableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = true;
        }

        public virtual void DisableTwoFactorAuthentication()
        {
            IsTwoFactorEnabled = false;
        }

        public virtual void EnableLockout()
        {
            IsLockoutEnabled = true;
        }

        public virtual void DisableLockout()
        {
            IsLockoutEnabled = false;
        }

        public virtual void SetEmail(string email)
        {
            Email = email;
        }

        public virtual void SetPhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public virtual void SetPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public virtual void SetSecurityStamp(string securityStamp)
        {
            SecurityStamp = securityStamp;
        }

        public virtual void IncrementAccessFailedCount()
        {
            AccessFailedCount++;
        }

        public virtual void ResetAccessFailedCount()
        {
            AccessFailedCount = 0;
        }

        public virtual void LockUntil(DateTimeOffset lockoutEndDate)
        {
            LockoutEndDate = lockoutEndDate;
        }

        public virtual void AddClaim(Claim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            AddClaim(new SiteUserClaim(claim));
        }

        public virtual void AddClaim(SiteUserClaim siteUserClaim)
        {
            if (siteUserClaim == null)
            {
                throw new ArgumentNullException("siteUserClaim");
            }

            _claims.Add(siteUserClaim);
        }

        public virtual void RemoveClaim(SiteUserClaim siteUserClaim)
        {
            if (siteUserClaim == null)
            {
                throw new ArgumentNullException("siteUserClaim");
            }

            _claims.Remove(siteUserClaim);
        }

        // statics
        internal static string GenerateKey(string userName)
        {
            return string.Format(Constants.SiteUserKeyTemplate, userName);
        }

        public System.Threading.Tasks.Task<ClaimsIdentity> GenerateUserIdentityAsync(SiteUserManager manager)
        {
            var userIdentity = manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}