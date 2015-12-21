using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AccountPlugin.Models.Owin
{
    public class SiteUserClaim
    {
        public SiteUserClaim(Claim claim)
        {
            if (claim == null) throw new ArgumentNullException("claim");

            ClaimType = claim.Type;
            ClaimValue = claim.Value;
        }

        [JsonConstructor]
        public SiteUserClaim(string claimType, string claimValue)
        {
            if (claimType == null) throw new ArgumentNullException("claimType");
            if (claimValue == null) throw new ArgumentNullException("claimValue");

            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        public string ClaimType { get; private set; }
        public string ClaimValue { get; private set; }
    }
}