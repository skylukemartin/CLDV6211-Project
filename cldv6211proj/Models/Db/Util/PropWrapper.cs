using System.Reflection;

namespace cldv6211proj.Models.Db.Util
{
    // ref: https://stackoverflow.com/a/12149346
    // .....^ Praying this holds references to variables rather than specific values, and modifying them modifies them everywhere..
    // Update: it didn't work .. time to hack around with delegates
    // ref: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions
    // ref: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/get
    // ref: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/set
    public class PropWrapper
    {
        public Type ValueType { get; }
        public Func<object?> GetValue { get; }
        public Action<object?> SetValue { get; }
        public object? Value
        {
            get => GetValue;
            set => SetValue(value);
        }

        public PropWrapper(Type valueType, Func<object?> getter, Action<object?> setter)
        {
            ValueType = valueType;
            GetValue = getter;
            SetValue = setter;
        }

        public PropWrapper(object value, Func<object?> getter, Action<object?> setter)
        {
            ValueType = value.GetType();
            GetValue = getter;
            SetValue = setter;
        }

        public static readonly Dictionary<Type, Func<string, object?>> ParseStringTypeMap =
            new()
            {
                { typeof(string), s => s },
                { typeof(int), s => int.Parse(s) },
                { typeof(float), s => float.Parse(s) },
                { typeof(double), s => double.Parse(s) },
                { typeof(decimal), s => decimal.Parse(s) },
            };

        public void SetByString(string value)
        {
            if (!ParseStringTypeMap.Keys.Contains(ValueType))
                throw new NotImplementedException($"Type {ValueType} has not been implemented.");
            Value = ParseStringTypeMap[ValueType](value);
        }

        public static readonly Dictionary<Type, Func<object, object?>> ConvertTypeMap =
            new()
            {
                { typeof(string), Convert.ToString },
                { typeof(int), o => Convert.ToInt32(o) },
                { typeof(int?), o => Convert.ToInt32(o) },
                { typeof(bool), o => Convert.ToBoolean(o) },
                { typeof(bool?), o => Convert.ToBoolean(o) },
                { typeof(float), o => Convert.ToSingle(o) },
                { typeof(float?), o => Convert.ToSingle(o) },
                { typeof(double), o => Convert.ToDouble(o) },
                { typeof(double?), o => Convert.ToDouble(o) },
                { typeof(decimal), o => Convert.ToDecimal(o) },
                { typeof(decimal?), o => Convert.ToDecimal(o) },
            };

        public void SetByObject(object value)
        {
            if (!ConvertTypeMap.Keys.Contains(ValueType))
                throw new NotImplementedException($"Type {ValueType} has not been implemented.");
            Value = ConvertTypeMap[ValueType](value);
        }

        public static Dictionary<string, PropWrapper> WrapToDic(
            object instance,
            string[]? skipPropNames = null,
            string[]? firstPropNames = null
        )
        {
            // ref: https://stackoverflow.com/a/9210493
            // .. let's take some time and reflect ... was it all worth it in the end?
            // ....(get it? because reflection is apparently slow and takes time ahaahh)

            var props = instance
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            HashSet<string>? firstHash =
                (firstPropNames == null || firstPropNames.Length == 0)
                    ? null // Skip creating HashSet if firstNames is null or empty
                    : firstPropNames?.ToHashSet<string>();
            HashSet<string>? skipHash =
                (skipPropNames == null || skipPropNames.Length == 0)
                    ? null // Skip creating HashSet if skipNames is null or empty
                    : skipPropNames?.ToHashSet<string>();

            if (firstHash != null)
                if (skipHash != null)
                    skipHash.UnionWith(firstHash);
                else
                    skipHash = firstHash;

            var unsorted = props.ToDictionary(
                prop => prop.Name,
                prop => new PropWrapper(
                    prop.PropertyType,
                    () => prop.GetValue(instance, null)!,
                    (value) => prop.SetValue(instance, value)
                )
            );

            Dictionary<string, PropWrapper>? propMap =
                firstHash == null
                    ? unsorted
                    : unsorted
                        .Where(p => firstHash.Contains(p.Key))
                        .Concat(unsorted.Where(p => skipHash == null || !skipHash.Contains(p.Key)))
                        .ToDictionary();

            return propMap;
        }
    }
}
