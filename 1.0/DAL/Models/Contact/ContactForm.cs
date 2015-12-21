using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;
using Common.Helpers;
using System.Data;
using Common.Extensions;
using Dapper;

namespace DAL.Models.Contact
{
    public class ContactForm
    {
        public Int64 Id { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String Message { get; set; }
        public DateTime? ReadDateUtc { get; set; }
        public DateTime? ResponsedDateUtc { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        [BulkSaveExtensions.BulkSaveModify]
        public Int64? BulkSaveId { get; set; }
        [BulkSaveExtensions.BulkSaveModify]
        public String UpsertAction { get; set; }

        public static ContactForm Get(Int64 id)
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                return cnn.Query<ContactForm>("usp_ContactForm_Get", new { Id = id }, null, false, null, CommandType.StoredProcedure).FirstOrDefault();
        }

        public void Insert()
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                this.Id = cnn.Query<Int64>("usp_ContactForm_Insert", generateParams(this, false, true), null, true, null, CommandType.StoredProcedure).FirstOrDefault();
        }

        public void Update()
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                cnn.Query("usp_ContactForm_Update", generateParams(this, false, true), null, true, null, CommandType.StoredProcedure);
        }

        public void Upsert()
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
            {
                var reader = cnn.ExecuteReader("usp_ContactForm_Upsert", generateParams(this, true, true), null, null, CommandType.StoredProcedure);
                while (reader.Read())
                {
                    this.Id = Convert.ToInt64(reader["Id"].ToString());
                    this.UpsertAction = reader["UpsertAction"].ToString();
                }
            }
        }

        public static void UpsertBulk(List<ContactForm> list)
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                list.BulkSave(null, "usp_ContactForm_Upsert_Bulk", 250, 30, "ContactForm");
            // var insertedCount = list.Count(o => o.UpsertAction.Matches("INSERT"));
        }

        public static void DeleteFlag(Int64 id, Boolean delete)
        {
            using (var cnn = DbHelper.CreateOpenConnection(DbHelper.Database.Default))
                cnn.Query("usp_ContactForm_DeleteFlag", new { Id = id, Delete = delete }, null, true, null, CommandType.StoredProcedure);
        }

        private static object generateParams(ContactForm obj, Boolean incPK, Boolean incNonPK)
        {
            var dbArgs = new DynamicParameters();
            if (incPK) dbArgs.Add("Id", obj.Id);
            if (incNonPK) dbArgs.Add("Name", obj.Name);
            if (incNonPK) dbArgs.Add("Email", obj.Email);
            if (incNonPK) dbArgs.Add("PhoneNumber", obj.PhoneNumber);
            if (incNonPK) dbArgs.Add("Message", obj.Message);
            if (incNonPK) dbArgs.Add("ReadDateUtc", obj.ReadDateUtc);
            if (incNonPK) dbArgs.Add("ResponsedDateUtc", obj.ResponsedDateUtc);
            if (incNonPK) dbArgs.Add("CreatedDateUtc", obj.CreatedDateUtc);
            return dbArgs;
        }
    }
}