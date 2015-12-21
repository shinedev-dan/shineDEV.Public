using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountPlugin.Models.Owin
{
    public class SiteUserEmail
    {
        public SiteUserEmail(string email, string userId)
        {
            if (email == null) throw new ArgumentNullException("email");
            if (userId == null) throw new ArgumentNullException("userId");

            Id = GenerateKey(email);
            UserId = userId;
            Email = email;
        }

        public string Id { get; private set; }
        public string UserId { get; private set; }
        public string Email { get; private set; }

        public ConfirmationRecord ConfirmationRecord { get; private set; }

        internal void SetConfirmed()
        {
            if (ConfirmationRecord == null)
            {
                ConfirmationRecord = new ConfirmationRecord();
            }
        }

        internal void SetUnconfirmed()
        {
            ConfirmationRecord = null;
        }

        internal static string GenerateKey(string email)
        {
            return string.Format(Constants.SiteUserEmailKeyTemplate, email);
        }
    }
}