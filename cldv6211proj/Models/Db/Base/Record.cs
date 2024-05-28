namespace cldv6211proj.Models.Db.Base
{
    using Util;

    public interface IRecordModel
    {
        public int ID { get; set; }
    }

    public class RecordModel : IRecordModel
    {
        public int ID { get; set; }

        public RecordModel() { }
    }

    public class Record<M>
        where M : RecordModel, new()
    { // ??= https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator
        private readonly ITable<M> table;

        public M Model { get; }
        public Dictionary<string, PropWrapper> Props { get; }
        public Dictionary<string, RecordColumn> Columns { get; }

        public Record(ITable<M> table, M model)
        {
            this.table = table;
            Model = model;
            Props = PropWrapper.WrapToDic(Model, firstPropNames: ["ID"]);
            Columns = Props
                .Select(prop => new RecordColumn(table.Prefix, prop.Key, prop.Value))
                .ToDictionary(col => col.ColumnName, col => col);
        }

        public Record(ITable<M> table)
            : this(table, new M()) { }
    }

    public class RecordColumn
    {
        public string TablePrefix { get; }
        public string ColumnName { get; }
        public string PropName { get; }
        public PropWrapper Prop { get; }

        public RecordColumn(
            string tablePrefix,
            string propName,
            PropWrapper prop,
            string columnName = ""
        )
        {
            TablePrefix = tablePrefix;
            PropName = propName;
            Prop = prop;
            if (columnName != "")
                ColumnName = columnName;
            else if (!propName.Contains("ID") || propName == "ID")
                ColumnName = tablePrefix + propName;
            else
                ColumnName = propName;
        }

        public RecordColumn(
            string tablePrefix,
            KeyValuePair<string, PropWrapper> propKvp,
            string columnName = ""
        )
            : this(tablePrefix, propKvp.Key, propKvp.Value, columnName) { }

        public static string SqlEscapeValue(object? value = null) =>
            $"'{value?.ToString()?.Replace("'", "''")}'";

        public string SqlValueEscaped() => SqlEscapeValue(Prop.GetValue());

        public string SqlEqualValue(object? value = null) =>
            $"{ColumnName} = {SqlEscapeValue(value ?? Prop.GetValue())}";

        public string SqlDefinition()
        {
            if (!ColumnName.Contains("ID"))
                return $"{ColumnName} {SqlTypeMap[Prop.ValueType]}"; // Non key attribute
            if (ColumnName.Contains(TablePrefix))
                return $"{ColumnName} INT IDENTITY(1,1) PRIMARY KEY"; // PK attribute
            // If we got this far, it must be an FK attribute
            return $"{ColumnName} INT NOT NULL FOREIGN KEY REFERENCES {ColumnName.Replace("ID", "")}Table({ColumnName})";
        }

        private static readonly Dictionary<Type, string> SqlTypeMap =
            new()
            {
                { typeof(string), "VARCHAR(255)" },
                { typeof(int), "INT NOT NULL" },
                { typeof(bool), "BIT NOT NULL" },
                { typeof(float), "REAL NOT NULL" },
                { typeof(double), "FLOAT NOT NULL" },
                { typeof(decimal), "DECIMAL NOT NULL" },
                { typeof(int?), "INT" },
                { typeof(bool?), "BIT" },
                { typeof(float?), "REAL" },
                { typeof(double?), "FLOAT" },
                { typeof(decimal?), "DECIMAL" },
            };
    }
}
