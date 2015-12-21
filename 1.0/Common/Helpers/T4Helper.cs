using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Extensions;

namespace Common.Helpers
{
    public class T4Helper
    {
        public class Sql
        {
            public static List<Column> GetColumns(Table t, Boolean incPK, Boolean incNonPK)
            {
                var pks = new List<Column>();
                foreach (Column c in t.Columns)
                    if (c.InPrimaryKey && incPK)
                        pks.Add(c);
                    else if (!c.InPrimaryKey && incNonPK)
                        pks.Add(c);

                if (pks.IsNullOrEmpty())
                    throw new Exception("No PK Found");

                return pks;
            }

            public static String GetColumnDeclaration(Column c)
            {
                int maxLen = c.DataType.MaximumLength;
                int maxPrecision = c.DataType.NumericPrecision;
                int numericScale = c.DataType.NumericScale;

                var s = String.Format("@{0} {1}{2}",
                    c.Name
                    , GetDataTypeDeclaration(c.DataType)
                    , c.Nullable ? " = NULL" : String.Empty);

                return s;
            }

            public static string GetDataTypeDeclaration(DataType dataType)
            {
                string name = dataType.Name.ToUpper();
                switch (dataType.SqlDataType)
                {
                    case SqlDataType.Binary:
                    case SqlDataType.Char:
                    case SqlDataType.NChar:
                    case SqlDataType.NVarChar:
                    case SqlDataType.VarBinary:
                    case SqlDataType.VarChar:
                        return string.Format("{0}({1})", name, dataType.MaximumLength);

                    case SqlDataType.NVarCharMax:
                    case SqlDataType.VarBinaryMax:
                    case SqlDataType.VarCharMax:
                        return string.Format("{0}(MAX)", name);

                    case SqlDataType.Decimal:
                    case SqlDataType.Numeric:
                        return string.Format("{0}({1}, {2})", name, dataType.NumericPrecision, dataType.NumericScale);
                }
                return name;
            }
        }

        public class Poco
        {
            public static String MapFromSqlType(String sqlType)
            {
                switch (sqlType.ToLower())
                {
                    case "bit":
                        return "Boolean";
                    case "uniqueidentifier":
                        return "Guid";
                    case "datetime":
                    case "datetime2":
                        return "DateTime";
                    case "int":
                        return "Int32";
                    case "smallint":
                        return "Int16";
                    case "bigint":
                        return "Int64";
                    case "varchar":
                    case "nvarchar":
                    case "text":
                    case "ntext":
                        return "String";
                }
                return "String";
            }

            public static String OutputProperty(Column column)
            {
                var name = column.Name;
                var sqlDataType = column.DataType.Name;
                var isNullable = column.Nullable;
                var isPrimaryKey = column.InPrimaryKey;
                var type = MapFromSqlType(sqlDataType);
                var typeFormat = !type.Matches("String") && isNullable ? "{0}?" : "{0}";

                return String.Format(
                    "public {0} {1} {{ get; set; }}",
                    String.Format(typeFormat, type),
                    name);
            }

            public static IEnumerable<string> GatherProperties(ColumnCollection columns)
            {
                List<String> list = new List<String>();
                foreach (Column col in columns)
                {
                    list.Add(OutputProperty(col));
                }
                return list;
            }

            public static String GetFunctionParams(IEnumerable<Column> columns)
            {
                Boolean first = true;
                String val = String.Empty;
                foreach (Column column in columns)
                {
                    var name = column.Name;
                    var sqlDataType = column.DataType.Name;
                    var isNullable = column.Nullable;
                    var isPrimaryKey = column.InPrimaryKey;
                    var type = MapFromSqlType(sqlDataType);
                    var typeFormat = !type.Matches("String") && isNullable ? "{0}?" : "{0}";

                    val += String.Format("{0}{1} {2}", first ? String.Empty : ", ", String.Format(typeFormat, type), name.ToLower());
                }
                return val;
            }

            public static String GetDapperParams(IEnumerable<Column> columns)
            {
                Boolean first = true;
                String val = String.Empty;
                foreach (Column column in columns)
                {
                    var name = column.Name;
                    var sqlDataType = column.DataType.Name;
                    var isNullable = column.Nullable;
                    var isPrimaryKey = column.InPrimaryKey;
                    var type = MapFromSqlType(sqlDataType);
                    var typeFormat = !type.Matches("String") && isNullable ? "{0}?" : "{0}";

                    val += String.Format("{0}{1} = {2}", first ? String.Empty : ", ", name, name.ToLower());
                }
                return val;
            }
        }
    }
}