using System;
using System.Data;
using System.Data.Common;
using CavemanTools.Model;
using SqlFu.Builders;

namespace SqlFu.Providers
{
    using System.Data.SqlClient;

    public abstract class DbProvider : IDbProvider
    {
        private DbProviderFactory _factory;

        protected DbProvider(string dialect,string providerName)
        {
            SqlClientFactory.Instance.
            _factory = DbProviderFactory.GetFactory(providerName);
            ProviderId = dialect;
            _func=new Lazy<DbFunctions>(GetFunctions);
        }

        private Lazy<DbFunctions> _func;

        public DbFunctions Functions => _func.Value;        
        

        protected abstract DbFunctions GetFunctions();

        public virtual void SetupParameter(DbParameter param, string name, object value)
        {
            name = name ?? "";
            param.ParameterName = string.Concat(ParamPrefix, name);
            value = SqlFuManager.Config.Converters.ConvertValueObject(value);
            param.Value = value ?? DBNull.Value;
        }

        public string EscapeTableName(string name, string schema = "")
        {
            if (!schema.IsNullOrEmpty())
            {
                schema = EscapeIdentifier(schema)+".";
            }

            return schema + EscapeIdentifier(name);
        }

        public abstract string ParamPrefix { get; }

        public string FormatSql(string sql, string[] paramNames)
        {
            Array.ForEach(paramNames, p => { sql = sql.Replace("@" + p, ParamPrefix + p); });
            return sql;
        }

        public virtual void OnCommandExecuting(DbCommand cmd)
        {
           
        }

        public string ProviderId { get; }

        public DbConnection CreateConnection()
        {
            return _factory.CreateConnection();
        }

        public abstract string EscapeIdentifier(string name);
        public abstract string GetColumnType(Type type);

      
        //public virtual bool DbTypeHasPrecision(Type type)
        //{
        //    return type == typeof (Single) || type == typeof (Double);
        //}

        //public virtual bool DbTypeHasSize(Type type)
        //{
        //    return type == typeof (string) || type == typeof (byte[]);
        //}

        //public abstract string GetTypeMaxSize(Type type);

        private IDatabaseTools _tools;
        public IDatabaseTools DatabaseTools => _tools ?? (_tools = GetTools());

        public virtual string FormatIndexOptions(string idxDef, string options = "")
        {
            return idxDef;
        }

        public abstract string GetIdentityKeyword();
        public abstract bool IsDbBusy(DbException ex);
        public abstract bool IsUniqueViolation(DbException ex, string keyName = "");

        public abstract string GetSqlForDropTableIfExists(string name, string schema = null);

        public abstract string AddReturnInsertValue(string values, string identityColumn);


        public abstract IDbProviderExpressions GetExpressionsHelper();
        public abstract string FormatQueryPagination(string sql, Pagination page, ParametersManager pm);

        protected abstract IDatabaseTools GetTools();
        

        internal static string Escape(string s,string startId,string endId)
        {
            s.MustNotBeEmpty();

            //If the identifier already includes the escape characters, we return
            //the identifier as is.
            if (s[0]==startId[0] && s[s.Length-1]==endId[0])
                return s;
            if (s.Contains("\""))
                return s;

            //Single part identifier can be returned as is.
            return startId + s + endId;           
        }
    }
}