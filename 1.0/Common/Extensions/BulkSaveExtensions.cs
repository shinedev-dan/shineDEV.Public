using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Dapper;

namespace Common.Extensions
{
    public static class BulkSaveExtensions
    {
        public static readonly Dictionary<Type, SqlDbType> SQLTYPEMAP = new Dictionary<Type, SqlDbType>()
        {
            { typeof(Boolean), SqlDbType.Bit }
            ,{ typeof(Byte), SqlDbType.TinyInt }
            ,{ typeof(Byte[]), SqlDbType.VarBinary }
            ,{ typeof(DateTime), SqlDbType.DateTime }
            ,{ typeof(Decimal), SqlDbType.Decimal }
            ,{ typeof(Double), SqlDbType.Float }
            ,{ typeof(Guid), SqlDbType.UniqueIdentifier }
            ,{ typeof(Int16), SqlDbType.SmallInt }
            ,{ typeof(Int32), SqlDbType.Int }
            ,{ typeof(Int64), SqlDbType.BigInt }
            ,{ typeof(Single), SqlDbType.Real }
            ,{ typeof(String), SqlDbType.VarChar }
            ,{ typeof(TimeSpan), SqlDbType.Time }
        };

        private static Func<T, object> CreateGetFuncFor<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "Type");
            var property = Expression.Property(parameter, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda(typeof(Func<T, object>), convert, parameter);
            return (Func<T, object>)lambda.Compile();
        }

        private static string GetCreateStatement(IDbConnection openConn, Type type, string targetTableName)
        {
            // Get source table definition
            var targetTableDef = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(targetTableName))
                targetTableDef = openConn.Query<TableDefinition>(SCHEMAQUERY, new { TableName = targetTableName }
                      , null, true, null, CommandType.Text).ToDictionary(f => f.ColumnName.ToUpper(), f => f.DataType);

            var fields = type.GetProperties()
                .Where(p => SQLTYPEMAP.ContainsKey((Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType)))
                .Select(p => new
                {
                    p.Name,
                    FieldType = (Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType)
                }).ToList();

            // Define the temp table
            string createTable = string.Format("create table #BulkSave{0} (", type.Name);
            createTable += "\r\nBulkSaveID INT NOT NULL,";
            fields.ForEach(f =>
            {
                if (targetTableDef.ContainsKey(f.Name.ToUpper()))
                {
                    createTable += string.Format("\r\n{0} {1}", f.Name, targetTableDef[f.Name.ToUpper()]);
                    createTable += " NULL,";
                }
                else
                {
                    createTable += string.Format("\r\n{0} {1}", f.Name, SQLTYPEMAP[f.FieldType]);
                    if (f.FieldType == typeof(string) || f.FieldType == typeof(byte[]))
                        createTable += "(MAX)";
                    createTable += " NULL,";
                }
            });
            createTable.TrimEnd(',');
            createTable += "\r\n);";

            return createTable;
        }

        public static void BulkSave<T>(this IEnumerable<T> list, IDbConnection openConn, string procedureName
            , int batchSize, int timeOut, string targetTableName)
        {
            // Get the Getter methods via reflection
            var properties = typeof(T).GetProperties()
                .Where(p => SQLTYPEMAP.ContainsKey((Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType))
                && !p.GetCustomAttributes(typeof(BulkSaveModify), true).Any())
                .ToList();
            var fields = properties.Select(p => new
            {
                p.Name,
                p.PropertyType,
                FieldType = (Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType),
                GetValue = CreateGetFuncFor<T>(p.Name)
            }).ToList();

            Dictionary<int, T> idMapping = new Dictionary<int, T>();
            int tempID = 0;

            // Create a data table and populate it with the object data
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("BulkSaveId", typeof(Int64)));
            fields.ForEach(f => table.Columns.Add(new DataColumn(f.Name, f.FieldType)));
            foreach (T obj in list)
            {
                idMapping.Add(++tempID, obj);
                var row = table.NewRow();
                row["BulkSaveId"] = tempID;
                fields.ForEach(f => row[f.Name] = f.GetValue(obj) ?? DBNull.Value);
                table.Rows.Add(row);
            }

            // Define and create the temp table
            string createTable = GetCreateStatement(openConn, typeof(T), targetTableName);
            openConn.Execute(createTable, null, null, null, CommandType.Text);

            // Insert all records into the temp table
            SqlConnection cnn = null;
            if (!(openConn is SqlConnection))
            {
                var property = openConn.GetType().GetProperty("InnerConnection");//maybe this is an object that wraps a SqlConnection
                if (property != null)
                    cnn = property.GetValue(openConn) as SqlConnection;
            }
            else
            {
                cnn = (SqlConnection)openConn;
            }
            if (cnn == null)
                throw new Exception("Bulk Saving requires an instance of SqlConnection.");

            SqlBulkCopy copy = new SqlBulkCopy(cnn);
            copy.BatchSize = batchSize;
            copy.BulkCopyTimeout = timeOut;
            copy.DestinationTableName = string.Format("#BulkSave{0}", typeof(T).Name);
            copy.WriteToServer(table);

            // Exexcute bulk upsert and load results/additional data into datatable
            DataTable resulttable = new DataTable();
            using (var cmd = new SqlCommand(procedureName, cnn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(resulttable);
            }

            // Update the Key property value, and any properties marked with [BulkSaveKey] or [BulkSaveModify]
            var modifyProps = properties.Where(p => p.GetCustomAttributes(typeof(BulkSaveKey), true).Any() || p.GetCustomAttributes(typeof(BulkSaveModify), true).Any()).ToList();
            foreach (DataRow row in resulttable.Rows)
            {
                var tempid = Convert.ToInt32(row["BulkSaveId"].ToString());
                var mapped = idMapping[tempid];
                modifyProps.ForEach(mp => typeof(T).GetProperty(mp.Name).SetValue(mapped, row[mp.Name]));
            }

            // Drop the temp table
            string dropTable = string.Format("drop table #BulkSave{0};", typeof(T).Name);
            openConn.Execute(dropTable, null, null, null, CommandType.Text);
        }

        [AttributeUsage(AttributeTargets.Property, Inherited = true)]
        public class BulkSaveKey : System.Attribute { }
        [AttributeUsage(AttributeTargets.Property, Inherited = true)]
        public class BulkSaveExclude : System.Attribute { }
        [AttributeUsage(AttributeTargets.Property, Inherited = true)]
        public class BulkSaveModify : System.Attribute { }

        private class BulkSaveResult
        {
            public int TempID { get; set; }
            public int ID { get; set; }
        }

        private class TableDefinition
        {
            public string ColumnName { get; set; }
            public string DataType { get; set; }
        }

        static readonly string SCHEMAQUERY = @"select column_name as ColumnName, data_type + 
    case
        when data_type like '%text' or data_type like 'image' or data_type like 'sql_variant' or data_type like 'xml'
            then ''
        when data_type = 'float'
            then '(' + convert(varchar(10), isnull(numeric_precision, 18)) + ')'
        when data_type = 'numeric' or data_type = 'decimal'
            then '(' + convert(varchar(10), isnull(numeric_precision, 18)) + ',' + convert(varchar(10), isnull(numeric_scale, 0)) + ')'
        when (data_type like '%char' or data_type like '%binary') and character_maximum_length = -1
            then '(max)'
        when character_maximum_length is not null
            then '(' + convert(varchar(10), character_maximum_length) + ')'
        else ''
    end as DataType
from information_schema.columns
where table_name = @TableName";
    }
}