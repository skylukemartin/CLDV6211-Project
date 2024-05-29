using System.Data.SqlClient;

namespace cldv6211proj.Models.Db.Base
{
    using System;
    using System.Text;
    using Util;

    public interface ITable
    {
        public string Prefix { get; }
        public string GenerateSeedCmds(bool nuke = false);
    }

    public interface ITable<M> : ITable
        where M : RecordModel, new()
    {
        public List<Record<M>> Records { get; set; }
        public int AddRecord(Record<M> record);
        public bool UpdateRecord(Record<M> record);
        public bool DeleteRecord(Record<M> record);
        public Record<M> ModelToRecord(M model);
    }

    public class Table<M> : ITable<M>
        where M : RecordModel, new()
    {
        public string Prefix { get; }
        public readonly SqlTable<Table<M>, M> Db;

        public List<Record<M>> Records { get; set; }

        public Table(string? prefix = null, List<Record<M>>? records = null, bool live = true)
        {
            Prefix = prefix ?? typeof(M).Name;
            Db = new(this, live);
            Records = records ?? [];
        }

        public List<Record<M>> FetchAllRecords()
        {
            var dbRecords = Db.SelectRecords();
            Records = dbRecords;
            return Records;
        }

        public string GenerateSeedCmds(bool nuke = false)
        {
            var records = Db.SelectRecords();
            SqlTable<Table<M>, M> dryDb = new(this, live: false);
            StringBuilder bob = new();
            var saveCmds = () => bob.Append(dryDb.LastCmdText).Append("\n\n");
            if (nuke)
            {
                dryDb.DropTable();
                saveCmds();
            }
            dryDb.CreateTable();
            saveCmds();
            dryDb.InsertRecords(records);
            saveCmds();
            return bob.ToString();
        }

        public List<Record<M>> ConvertModelsToRecords(M[] models) =>
            models.Select(m => new Record<M>(this, m)).ToList();

        public void Nuke(
            bool drill = true,
            Action<string>? newsReporter = null,
            SqlTable<Table<M>, M>? safeDb = null
        )
        {
            safeDb ??= drill ? new(this, live: false) : Db;
            if (newsReporter == null)
                newsReporter = Console.WriteLine;
            var newsReport = () => newsReporter(safeDb.LastCmdText);
            safeDb.DropTable();
            newsReport();
        }

        public void InitDb(
            M[] models,
            bool nuke = false,
            bool drill = true,
            Action<string>? newsReporter = null
        )
        {
            var records = ConvertModelsToRecords(models);
            var safeDb = drill ? new(this, live: false) : Db;
            if (newsReporter == null)
                newsReporter = Console.WriteLine;
            var newsReport = () => newsReporter(safeDb.LastCmdText);
            if (nuke)
                Nuke(drill, newsReporter, safeDb);
            safeDb.CreateTable();
            newsReport();
            safeDb.InsertRecords(records);
            newsReport();
        }

        public int AddRecord(Record<M> record)
        {
            Db.InsertRecord(record, record.Model.ID > 0);
            var dbRec = Db.SelectRecords([.. record.Columns.Values]).LastOrDefault(record);
            if (dbRec.Model.ID == 0)
            {
                Console.WriteLine("Failed to add record to database.");
                return -1;
            }
            Records.Add(dbRec);
            return dbRec.Model.ID;
        }

        public int AddRecord(M model) => AddRecord(new Record<M>(this, model));

        public int AddRecords(List<Record<M>> records)
        {
            var ret = Db.InsertRecords(records);
            Console.WriteLine($"Db.InsertRecords(records) ret={ret}");
            FetchAllRecords();
            return ret;
        }

        public bool UpdateRecord(Record<M> record)
        {
            if (record.Model.ID <= 0)
            {
                Console.WriteLine("Cannot update record without a valid ID.");
                return false;
            }
            var found = Records.FindIndex(r => r.Model.ID == record.Model.ID);
            if (found != -1)
                Records[found] = record;
            else
                Console.WriteLine("Couldn't find record by ID.");
            return Db.UpdateRecord(record) != 0;
        }

        public bool UpdateRecord(M model) => UpdateRecord(ModelToRecord(model));

        public bool DeleteRecord(Record<M> record)
        {
            Records.Remove(record);
            if (record.Model.ID < 1)
                Console.WriteLine("Cannot delete record with undefined ID..");
            return Db.DeleteRecord(record) > 0;
        }

        public bool DeleteRecord(M model) => DeleteRecord(ModelToRecord(model));

        public Record<M>? SelectRecord(M model, HashSet<string> matchPropNames)
        {
            var foundRecords = Db.SelectRecords(new Record<M>(this, model), matchPropNames);
            if (foundRecords.Count == 0)
                return null;
            return foundRecords.First();
        }

        public Record<M> ModelToRecord(M model)
        {
            var foundIndex = Records.FindIndex(rec => rec.Model == model);
            if (foundIndex != -1)
                return Records[foundIndex];
            return new Record<M>(this, model);
        }

        public Record<M>? LazyFindRecord(Func<Record<M>, bool> where) =>
            // Records.FirstOrDefault(where, null)??
            // Argument of type 'Func<Record<M>, bool>' cannot be used for parameter 'predicate'
            // of type 'Func<Record<M>?, bool>' in 'Record<M>? Enumerable.FirstOrDefault<Record<M>?>
            // (IEnumerable<Record<M>?> source, Func<Record<M>?, bool> predicate, Record<M>? defaultValue)'
            // due to differences in the nullability of reference types.CS8620
            Records.FirstOrDefault(ffs => null == ffs ? false : where(ffs), null)
            ?? FetchAllRecords().FirstOrDefault(ffs => null != ffs ? where(ffs) : false, null);

        public Record<M>? LazyFindRecord(int recordID)
        {
            if (recordID < 1)
                return null;
            return LazyFindRecord(rec => rec.Model.ID == recordID);
        }
    }

    public class SqlTable<T, M>
        where T : ITable<M>
        where M : RecordModel, new()
    {
        private readonly T table;
        private readonly bool live;
        private string _lastCmdText = "";
        public string LastCmdText
        {
            get => _lastCmdText;
        }

        public SqlTable(T table, bool live = true)
        {
            this.table = table;
            this.live = live;
        }

        private string CacheCmd(string cmd)
        {
            _lastCmdText = cmd;
            return cmd;
        }

        private readonly Func<string, bool> dbgTrue = (msg) =>
        {
            Console.WriteLine(msg);
            return true;
        };

        private int ExecNonQuery(string cmdText, bool exec = true) =>
            SqlExec.NonQuery(CacheCmd(cmdText), exec);

        private int ExecReader(
            string cmdText,
            Action<SqlDataReader> action,
            int maxReads = int.MaxValue,
            bool exec = true
        ) => SqlExec.Reader(CacheCmd(cmdText), action, maxReads, exec);

        public void CreateTable()
        {
            StringBuilder bob = new StringBuilder();
            bob.Append($"CREATE TABLE {table.Prefix}Table (\n\t");
            bob.Append(
                string.Join(
                    ",\n\t",
                    table
                        .Records.FirstOrDefault(new Record<M>(table))
                        .Columns.Select(col => col.Value.SqlDefinition())
                )
            );
            bob.Append("\n);");
            ExecNonQuery(bob.ToString(), exec: live);
        }

        public void DropTable() => ExecNonQuery($"DROP TABLE {table.Prefix}Table;", live);

        public void InsertRecord(Record<M> record, bool withID = false)
        {
            var columns = withID
                ? record.Columns
                : record.Columns.Where(colKv => colKv.Value.PropName != "ID");
            var align = "\n            ";
            var cmdText =
                $"INSERT INTO {table.Prefix}Table ({align}"
                + string.Join($",{align}", columns.Select(colKv => colKv.Value.ColumnName))
                + "\n)\nVALUES ("
                + string.Join(", ", columns.Select(colKv => colKv.Value.SqlValueEscaped()))
                + ");";
            ExecNonQuery(cmdText, exec: live);
        }

        public int InsertRecords(List<Record<M>> records, bool withID = false)
        {
            if (records.Count == 0 && dbgTrue("No records given to insert."))
                return -1; //throw new Exception("No records given to insert.");

            var filtered = (Dictionary<string, RecordColumn> record) =>
                withID ? record : record.Where(colKv => colKv.Value.PropName != "ID");
            var align = "\n            ";
            var cmdText =
                $"INSERT INTO {table.Prefix}Table ({align}"
                + string.Join(
                    $",{align}",
                    filtered(records.First().Columns).Select(colKv => colKv.Value.ColumnName)
                )
                + "\n)\nVALUES ("
                + string.Join(
                    "),\n       (",
                    records.Select(record =>
                        string.Join(
                            ", ",
                            filtered(record.Columns).Select(colKv => colKv.Value.SqlValueEscaped())
                        )
                    )
                )
                + ");";
            return ExecNonQuery(cmdText, exec: live);
        }

        private List<Record<M>> SelectRecords(string? cmdText = null)
        {
            if (cmdText == null || cmdText == "")
                cmdText = $"SELECT * FROM {table.Prefix}Table;";
            List<Record<M>> matches = [];
            ExecReader(
                cmdText,
                reader =>
                {
                    var record = new Record<M>(table);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string key = reader.GetName(i);
                        if (record.Columns.ContainsKey(key))
                            record.Columns[key].Prop.SetByObject(reader[i]);
                    }
                    matches.Add(record);
                },
                exec: live
            );
            return matches;
        }

        private string WhereColumns(List<RecordColumn>? matchValues = null)
        {
            if (matchValues == null || matchValues.Count == 0)
                return "";
            var invalid = matchValues.FindIndex(col =>
                col.PropName == "ID" && (col.Prop.GetValue() ?? 0).Equals(0)
            );
            if (invalid != -1)
                matchValues.RemoveAt(invalid);
            if (matchValues.Count != 0)
                return "\nWHERE "
                    + string.Join("\n  AND ", matchValues.Select(col => col.SqlEqualValue()));
            return "";
        }

        public List<Record<M>> SelectRecords(List<RecordColumn>? matchColumnValues = null) =>
            SelectRecords(
                $"SELECT * FROM {table.Prefix}Table {WhereColumns(matchColumnValues)}".Trim() + ";"
            );

        public List<Record<M>> SelectRecords(Record<M> record, HashSet<string> matchPropNames) =>
            SelectRecords(
                (
                    $"SELECT * FROM {table.Prefix}Table "
                    + WhereColumns(
                        record
                            .Columns.Values.Where(cv => matchPropNames.Contains(cv.PropName))
                            .ToList()
                    )
                ).Trim() + ";"
            );

        public int UpdateRecord(Record<M> record, int? ID = null)
        {
            var cmdText =
                $"UPDATE {table.Prefix}Table"
                + "\nSET "
                + string.Join(
                    ", ",
                    record
                        .Columns.Where(colKv => colKv.Value.PropName != "ID")
                        .Select((colKv) => colKv.Value.SqlEqualValue())
                )
                + "\nWHERE "
                + record
                    .Columns.Last(colKv => colKv.Value.PropName == "ID")
                    .Value.SqlEqualValue(ID);
            return ExecNonQuery(cmdText, live);
        }

        public int UpdateRecords(List<Record<M>> records)
        {
            int count = 0;
            foreach (var record in records)
                count += UpdateRecord(record);
            return count;
        }

        public int DeleteRecord(Record<M> record, int? ID = null)
        {
            var cmdText =
                $"DELETE FROM {table.Prefix}Table"
                + "\nWHERE "
                + record
                    .Columns.Last(colKv => colKv.Value.PropName == "ID")
                    .Value.SqlEqualValue(ID);
            return ExecNonQuery(cmdText, live);
        }

        public int DeleteRecords(List<Record<M>> records)
        {
            int count = 0;
            foreach (var record in records)
                count += DeleteRecord(record);
            return count;
        }
    }
}
