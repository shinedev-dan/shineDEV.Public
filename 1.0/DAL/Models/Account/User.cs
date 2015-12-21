using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;
using Common.Helpers;
using System.Data;
using Common.Extensions;

namespace DAL.Models.Account
{
    public class User
    {
        [BulkSaveExtensions.BulkSaveKey]
        public Int64 Id { get; set; }
        public String UserName { get; set; }
        public String PasswordHash { get; set; }
        public String SecurityStamp { get; set; }
        public String Email { get; set; }
        public Boolean EmailConfirmed { get; set; }
        public String PhoneNumber { get; set; }
        public Boolean PhoneNumberConfirmed { get; set; }
        public Boolean LockoutEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public Int32 AccessFailedCount { get; set; }
        public Int64 OrganizationId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String DateFormat { get; set; }
        public String TimeFormat { get; set; }
        public Boolean Deleted { get; set; }
        public DateTime CreatedDateUtc { get; set; }

        [BulkSaveExtensions.BulkSaveModify]
        public Int64? BulkSaveId { get; set; }
        [BulkSaveExtensions.BulkSaveModify]
        public String UpsertAction { get; set; }
        [BulkSaveExtensions.BulkSaveExclude]
        public List<UserClaim> Claims { get; set; }

        public User()
        {
            Claims = new List<UserClaim>();
        }

        public static User Get(Int64? id, string username, string email)
        {
            User user = null;
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
            {
                var multi = cnn.QueryMultiple("usp_User_Get", new { id, username, email }, null, null, CommandType.StoredProcedure);
                user = multi.Read<User>().FirstOrDefault();
                if (!multi.IsConsumed)
                {
                    var claims = multi.Read<UserClaim>().ToList();
                    if (!claims.IsNullOrEmpty())
                        user.Claims.AddRange(claims);
                }
            }
            return user;
        }

        public void Upsert() {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                this.Id = cnn.Query<Int64>("usp_User_Upsert", generateParams(), null, true, null, CommandType.StoredProcedure).FirstOrDefault();
            this.Claims.ForEach(c => c.UserId = this.Id);
        }

        public static void UpsertBulk(List<User> users)
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                users.BulkSave(null, "usp_User_Upsert_Bulk", 250, 30, "User");
            var insertedCount = users.Count(u => u.UpsertAction.Matches("INSERT"));
        }

        public static void DeleteFlag(Int64 id, Boolean delete)
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                cnn.Query("usp_User_DeleteFlag", new { id, delete }, null, true, null, CommandType.StoredProcedure);
        }

        private object generateParams()
        {
            return new 
            { 
                Id, UserName, PasswordHash, SecurityStamp, 
                Email, EmailConfirmed, PhoneNumber, PhoneNumberConfirmed, 
                LockoutEnabled, LockoutEndDateUtc, AccessFailedCount,
                OrganizationId, FirstName, LastName, DateFormat, TimeFormat, Deleted
            };
        }
    }

    public class UserClaim
    {
        [BulkSaveExtensions.BulkSaveKey]
        public Int64 Id { get; set; }
        public Int64 UserId { get; set; }
        public String Type { get; set; }
        public String Value { get; set; }

        public static List<UserClaim> ListForUser(Int64 userid)
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                return cnn.Query<UserClaim>("usp_UserClaim_ListForUser", new { userid }, null, true, null, CommandType.StoredProcedure).ToList();
        }

        public static void InsertBulk(List<UserClaim> claims)
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                claims.BulkSave(cnn, "usp_UserClaim_Insert_Bulk", 250, 30, "UserClaim");
        }

        public static void DeleteForUser(Int64 userid)
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                cnn.Query("usp_UserClaim_DeleteForUser", new { userid }, null, true, null, CommandType.StoredProcedure);
        }
    }

    //public class UserLogin
    //{
    //    public Int64 Id { get; set; }
    //    public Int64 UserId { get; set; }
    //    public String ProviderName { get; set; } //150
    //    public String ProviderKey { get; set; } //150

    //    public static UserLogin Get();
    //    public void Create();
    //    public void Update();
    //    public void Delete();
    //}

    //public class UserEmail
    //{
    //    public Int64 Id { get; set; }
    //    public Int64 UserId { get; set; }
    //    public String ProviderName { get; set; } //150
    //    public String ProviderKey { get; set; } //150

    //    public static UserEmail Get();
    //    public void Create();
    //    public void Update();
    //    public void Delete();
    //}

    //public class UserPhoneNumber
    //{
    //    public Int64 Id { get; set; }
    //    public Int64 UserId { get; set; }
    //    public String Value { get; set; } //150

    //    public static UserPhoneNumber Get();
    //    public void Create();
    //    public void Update();
    //    public void Delete();
    //}
}