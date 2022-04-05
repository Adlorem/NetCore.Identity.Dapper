using System.Dynamic;
using System.Text;

namespace NetCore.Identity.Dapper.Extensions
{
    /// <summary>
    /// Extensions for dapper. 
    /// </summary>
    internal static class DapperExtensions
    {
        /// <summary>
        /// Creates Insert string from model. Used when dynamic query generation is needed.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        /// <param name="dbSchema"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static string DapperCreateInsert<TModel>(this TModel model, string tableName, string dbSchema = "dbo") where TModel : class
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var sb = new StringBuilder();
            sb.Append($"INSERT INTO [{dbSchema}].[{tableName}] (");
            foreach (var property in model.GetType().GetProperties())
            {
                sb.Append($"[{property.Name}], ");
            }
            sb.Length -= 2;
            sb.Append(") VALUES (");
            foreach (var property in model.GetType().GetProperties())
            {
                sb.Append($"@{property.Name}, ");
            }
            sb.Length -= 2;
            sb.Append(");");
            return sb.ToString();
        }
        /// <summary>
        /// Creates Update string from model. Used when dynamic query generation is needed.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        /// <param name="whereParameter"></param>
        /// <param name="dbSchema"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static string DapperCreateUpdate<TModel>(this TModel model, string tableName, string whereParameter, string dbSchema = "dbo") where TModel : class
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var sb = new StringBuilder();
            sb.Append($"UPDATE [{dbSchema}].[{tableName}] SET ");
            foreach (var property in model.GetType().GetProperties())
            {
                sb.Append($"{property.Name} = @{property.Name}, ");
            }
            sb.Length -= 2;
            if (!string.IsNullOrEmpty(whereParameter))
            {
                sb.Append($" WHERE {whereParameter}=@{whereParameter}");
            }
            sb.Append(";");
            return sb.ToString();
        }
        /// <summary>
        /// Creates dynamic properties object from model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static object DapperCreateProperties<TModel>(this TModel model) where TModel : class
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            dynamic data = new ExpandoObject();

            IDictionary<string, object> dictionary = (IDictionary<string, object>)data;

            foreach (var property in model.GetType().GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(model, null));
            }

            return data;
        }

    }
}
