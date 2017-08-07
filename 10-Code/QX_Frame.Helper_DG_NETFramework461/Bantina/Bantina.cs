using QX_Frame.Helper_DG.Extends;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/**
 * author:qixiao
 * create:2017-8-7 10:46:07
 * */
namespace QX_Frame.Helper_DG.Bantina
{
    /// <summary>
    /// Bantina Interface
    /// </summary>
    public interface IBantina
    {
        Task<bool> Add<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> Update<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> Delete<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        Task<bool> Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> where) where TEntity : class;
        bool Exist<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        int QueryCount<TEntity>() where TEntity : class;
        int QueryCount<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        TEntity QueryEntity<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        List<TEntity> QueryEntities<TEntity>() where TEntity : class;
        List<TEntity> QueryEntities<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        List<TEntity> QueryEntitiesPaging<TEntity, TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> orderBy, bool isDESC = false) where TEntity : class;
        List<TEntity> QueryEntitiesPaging<TEntity, TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> orderBy, Expression<Func<TEntity, bool>> where, bool isDESC = false) where TEntity : class;
        List<TEntity> QueryEntitiesPaging<TEntity, TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> orderBy, out int count, bool isDESC = false) where TEntity : class;
        List<TEntity> QueryEntitiesPaging<TEntity, TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> orderBy, Expression<Func<TEntity, bool>> where, out int count, bool isDESC = false) where TEntity : class;
    }

    public abstract class Bantina : Sql_Helper_DG, IDisposable, IBantina
    {
        #region Constructor

        public Bantina()
        {
            if (string.IsNullOrEmpty(ConnString_Default))
            {
                throw new Exception_DG("ConnString_Default Must Be Declared When Initiation ! -- QX_Frame.Helper_DG.Bantina");
            }

        }

        public Bantina(string connString) => ConnString_Default = connString;

        public Bantina(string connString_RW, string connString_R)
        {
            ConnString_RW = connString_RW;
            ConnString_R = connString_R;
        }

        #endregion

        #region Add Method

        public async Task<bool> Add<TEntity>(TEntity entity) where TEntity : class
        {
            Type entityType = typeof(TEntity);

            string tableName = GetTablaName(entityType);

            StringBuilder builder_front = new StringBuilder();
            StringBuilder builder_behind = new StringBuilder();

            List<SqlParameter> sqlParameterList = new List<SqlParameter>();

            builder_front.Append("INSERT INTO ");
            builder_front.Append(tableName);
            builder_front.Append(" (");
            builder_behind.Append(" VALUES (");

            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                    continue;
                }

                //key :
                //KeyAttribute keyAttr=propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) as KeyAttribute;

                //Column :
                if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    if (!string.IsNullOrEmpty(columnAttr.ColumnName))
                    {
                        builder_front.Append(columnAttr.ColumnName);
                        builder_front.Append(",");

                        builder_behind.Append("@");
                        builder_behind.Append(columnAttr.ColumnName);
                        builder_behind.Append(",");

                        sqlParameterList.Add(new SqlParameter("@" + columnAttr.ColumnName, propertyInfo.GetValue(entity)));
                    }
                    else
                    {
                        builder_front.Append(propertyInfo.Name);
                        builder_front.Append(",");

                        builder_behind.Append("@");
                        builder_behind.Append(propertyInfo.Name);
                        builder_behind.Append(",");

                        sqlParameterList.Add(new SqlParameter("@" + propertyInfo.Name, propertyInfo.GetValue(entity)));
                    }
                }

                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                    builder_front.Append(")");

                    builder_behind.Remove(builder_behind.Length - 1, 1);
                    builder_behind.Append(")");
                }
            }

            //Generate SqlStatement
            string sql = builder_front.Append(builder_behind.ToString()).ToString();

            bool result = false;

            //Execute Task That Execute SqlStatement
            await Task.Run(() =>
             {
                 result = ExecuteNonQuery(sql, System.Data.CommandType.Text, sqlParameterList.ToArray()) > 0;
             });

            // Cache Deal:if Table change,we should renew the cache value
            HttpRuntimeCache_Helper_DG.Cache_Add(tableName, result);

            return result;
        }

        #endregion

        #region Update Method

        /// <summary>
        /// Update Method : the [Key] Attribute must be provide!
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Update<TEntity>(TEntity entity) where TEntity : class
        {
            int keyCount = 0;

            Type entityType = typeof(TEntity);

            string tableName = GetTablaName(entityType);

            StringBuilder builder_front = new StringBuilder();
            StringBuilder builder_where = new StringBuilder();

            List<SqlParameter> sqlParameterList = new List<SqlParameter>();

            builder_front.Append("UPDATE ");
            builder_front.Append(tableName);
            builder_front.Append(" SET ");
            builder_where.Append(" WHERE ");

            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //key :
                if (propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute keyAttr)
                {
                    if (!string.IsNullOrEmpty(keyAttr.ColumnName))
                    {
                        builder_where.Append(keyAttr.ColumnName);
                        builder_where.Append("=");
                        builder_where.Append("@");
                        builder_where.Append(keyAttr.ColumnName);

                        sqlParameterList.Add(new SqlParameter("@" + keyAttr.ColumnName, propertyInfo.GetValue(entity)));
                    }
                    else
                    {
                        builder_where.Append(propertyInfo.Name);
                        builder_where.Append("=");
                        builder_where.Append("@");
                        builder_where.Append(propertyInfo.Name);

                        sqlParameterList.Add(new SqlParameter("@" + propertyInfo.Name, propertyInfo.GetValue(entity)));
                    }
                    keyCount++;
                }

                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                    continue;
                }

                //Column :
                if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    if (!string.IsNullOrEmpty(columnAttr.ColumnName))
                    {
                        builder_front.Append(columnAttr.ColumnName);
                        builder_front.Append("=");
                        builder_front.Append("@");
                        builder_front.Append(columnAttr.ColumnName);
                        builder_front.Append(",");

                        sqlParameterList.Add(new SqlParameter("@" + columnAttr.ColumnName, propertyInfo.GetValue(entity)));
                    }
                    else
                    {
                        builder_front.Append(propertyInfo.Name);
                        builder_front.Append("=");
                        builder_front.Append("@");
                        builder_front.Append(propertyInfo.Name);
                        builder_front.Append(",");

                        sqlParameterList.Add(new SqlParameter("@" + propertyInfo.Name, propertyInfo.GetValue(entity)));
                    }
                }

                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                }
            }

            if (keyCount == 0)
            {
                throw new Exception_DG("[KeyAttribute] must be provide.");
            }
            else if (keyCount > 1)
            {
                throw new Exception_DG("[KeyAttribute] only one suppport.");
            }

            //Generate SqlStatement
            string sql = builder_front.Append(builder_where.ToString()).ToString();

            bool result = false;

            //Execute Task That Execute SqlStatement
            await Task.Run(() =>
            {
                result = ExecuteNonQuery(sql, System.Data.CommandType.Text, sqlParameterList.ToArray()) > 0;
            });

            // Cache Deal:if Table change,we should renew the cache value
            HttpRuntimeCache_Helper_DG.Cache_Add(tableName, result);

            return result;
        }

        /// <summary>
        /// Update Method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<bool> Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            string lambdaString = where.ToString();

            Type entityType = typeof(TEntity);

            string tableName = GetTablaName(entityType);

            StringBuilder builder_front = new StringBuilder();

            List<SqlParameter> sqlParameterList = new List<SqlParameter>();


            builder_front.Append("UPDATE ");
            builder_front.Append(lambdaString.Substring(0, lambdaString.IndexOf("=>")));
            builder_front.Append(" SET ");

            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                    continue;
                }

                //key :
                if (propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute keyAttr)
                {
                    if (!string.IsNullOrEmpty(keyAttr.ColumnName))
                    {
                        builder_front.Append(keyAttr.ColumnName);
                        builder_front.Append("=");
                        builder_front.Append("@");
                        builder_front.Append(keyAttr.ColumnName);
                        builder_front.Append(",");

                        sqlParameterList.Add(new SqlParameter("@" + keyAttr.ColumnName, propertyInfo.GetValue(entity)));
                    }
                    else
                    {
                        builder_front.Append(propertyInfo.Name);
                        builder_front.Append("=");
                        builder_front.Append("@");
                        builder_front.Append(propertyInfo.Name);
                        builder_front.Append(",");

                        sqlParameterList.Add(new SqlParameter("@" + propertyInfo.Name, propertyInfo.GetValue(entity)));
                    }
                }

                //Column :
                if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    if (!string.IsNullOrEmpty(columnAttr.ColumnName))
                    {
                        builder_front.Append(columnAttr.ColumnName);
                        builder_front.Append("=");
                        builder_front.Append("@");
                        builder_front.Append(columnAttr.ColumnName);
                        builder_front.Append(",");

                        sqlParameterList.Add(new SqlParameter("@" + columnAttr.ColumnName, propertyInfo.GetValue(entity)));
                    }
                    else
                    {
                        builder_front.Append(propertyInfo.Name);
                        builder_front.Append("=");
                        builder_front.Append("@");
                        builder_front.Append(propertyInfo.Name);
                        builder_front.Append(",");

                        sqlParameterList.Add(new SqlParameter("@" + propertyInfo.Name, propertyInfo.GetValue(entity)));
                    }
                }

                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                    builder_front.Append(" From ");
                    builder_front.Append(tableName);
                    builder_front.Append(" ");
                }
            }

            //Generate SqlStatement

            string sql = builder_front.Append(lambdaString.LambdaToSqlStatement()).ToString();

            bool result = false;

            //Execute Task That Execute SqlStatement
            await Task.Run(() =>
            {
                result = ExecuteNonQuery(sql, System.Data.CommandType.Text, sqlParameterList.ToArray()) > 0;
            });

            // Cache Deal:if Table change,we should renew the cache value
            HttpRuntimeCache_Helper_DG.Cache_Add(tableName, result);

            return result;
        }

        #endregion

        #region Delete Method

        /// <summary>
        /// Delete Mothod : the [Key] Attribute must be provide!
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> Delete<TEntity>(TEntity entity) where TEntity : class
        {
            int keyCount = 0;

            Type entityType = typeof(TEntity);

            string tableName = GetTablaName(entityType);

            StringBuilder builder_front = new StringBuilder();
            StringBuilder builder_where = new StringBuilder();

            List<SqlParameter> sqlParameterList = new List<SqlParameter>();

            builder_front.Append("DELETE ");
            builder_front.Append(tableName);
            builder_where.Append(" WHERE ");

            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //key :
                if (propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute keyAttr)
                {
                    if (!string.IsNullOrEmpty(keyAttr.ColumnName))
                    {
                        builder_where.Append(keyAttr.ColumnName);
                        builder_where.Append("=");
                        builder_where.Append("@");
                        builder_where.Append(keyAttr.ColumnName);

                        sqlParameterList.Add(new SqlParameter("@" + keyAttr.ColumnName, propertyInfo.GetValue(entity)));
                    }
                    else
                    {
                        builder_where.Append(propertyInfo.Name);
                        builder_where.Append("=");
                        builder_where.Append("@");
                        builder_where.Append(propertyInfo.Name);

                        sqlParameterList.Add(new SqlParameter("@" + propertyInfo.Name, propertyInfo.GetValue(entity)));
                    }
                }
            }

            if (keyCount == 0)
            {
                throw new Exception_DG("[KeyAttribute] must be provide.");
            }
            else if (keyCount > 1)
            {
                throw new Exception_DG("[KeyAttribute] only one suppport.");
            }

            //Generate SqlStatement
            string sql = builder_front.Append(builder_where.ToString()).ToString();

            bool result = false;

            //Execute Task That Execute SqlStatement
            await Task.Run(() =>
            {
                result = ExecuteNonQuery(sql, System.Data.CommandType.Text, sqlParameterList.ToArray()) > 0;
            });

            // Cache Deal:if Table change,we should renew the cache value
            HttpRuntimeCache_Helper_DG.Cache_Add(tableName, result);

            return result;
        }

        /// <summary>
        /// Delete Method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<bool> Delete<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            string lambdaString = where.ToString();

            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder_front = new StringBuilder();

            List<SqlParameter> sqlParameterList = new List<SqlParameter>();

            builder_front.Append("DELETE ");
            builder_front.Append(lambdaString.Substring(0, lambdaString.IndexOf("=>")));
            builder_front.Append(" From ");
            builder_front.Append(tableName);
            builder_front.Append(" ");
            builder_front.Append(lambdaString.LambdaToSqlStatement());

            //Generate SqlStatement
            string sql = builder_front.ToString();

            bool result = false;

            //Execute Task That Execute SqlStatement
            await Task.Run(() =>
            {
                result = ExecuteNonQuery(sql, System.Data.CommandType.Text, sqlParameterList.ToArray()) > 0;
            });

            // Cache Deal:if Table change,we should renew the cache value
            HttpRuntimeCache_Helper_DG.Cache_Add(tableName, result);

            return result;
        }

        #endregion

        #region Query Method -- Exist/Count/Entity/Entities/EntitiesPaging

        /// <summary>
        /// Exist
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Exist<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            string lambdaString = where.ToString();

            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder = new StringBuilder();

            builder.Append("SELECT COUNT(0) FROM ");
            builder.Append(tableName);
            builder.Append(" ");
            builder.Append(lambdaString.LambdaToSqlStatement());

            //Generate SqlStatement
            string sql = builder.ToString();

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
           {
               return ExecuteScalar(sql);
           });

            return result.ToInt() > 0;
        }

        /// <summary>
        /// QueryCount
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public int QueryCount<TEntity>() where TEntity : class
        {
            string tableName = GetTablaName(typeof(TEntity));

            //Generate SqlStatement
            string sql = "SELECT COUNT(0) FROM " + tableName;

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return ExecuteScalar(sql);
            });

            return result.ToInt();
        }

        /// <summary>
        /// QueryCount
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            string lambdaString = where.ToString();

            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder = new StringBuilder();


            builder.Append("SELECT COUNT(0) FROM ");
            builder.Append(tableName);
            builder.Append(" ");
            builder.Append(lambdaString.LambdaToSqlStatement());

            //Generate SqlStatement
            string sql = builder.ToString();

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return ExecuteScalar(sql);
            });

            return result.ToInt();
        }

        /// <summary>
        /// QueryEntity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public TEntity QueryEntity<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            string lambdaString = where.ToString();

            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder = new StringBuilder();

            builder.Append("SELECT TOP (1) * FROM ");
            builder.Append(tableName);
            builder.Append(" ");
            builder.Append(lambdaString.LambdaToSqlStatement());

            //Generate SqlStatement
            string sql = builder.ToString();

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return Return_T_ByDataSet<TEntity>(ExecuteDataSet(sql));
            });

            return result as TEntity;
        }

        /// <summary>
        /// QueryEntities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public List<TEntity> QueryEntities<TEntity>() where TEntity : class
        {
            string tableName = GetTablaName(typeof(TEntity));

            //Generate SqlStatement
            string sql = "SELECT * FROM " + tableName;

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return Return_List_T_ByDataSet<TEntity>(ExecuteDataSet(sql));
            });

            return result as List<TEntity>;
        }

        /// <summary>
        /// QueryEntities
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<TEntity> QueryEntities<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class
        {
            string lambdaString = where.ToString();

            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder = new StringBuilder();

            builder.Append("SELECT * FROM ");
            builder.Append(tableName);
            builder.Append(" ");
            builder.Append(lambdaString.LambdaToSqlStatement());

            //Generate SqlStatement
            string sql = builder.ToString();

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return Return_List_T_ByDataSet<TEntity>(ExecuteDataSet(sql));
            });

            return result as List<TEntity>;
        }

        /// <summary>
        /// QueryEntitiesPaging
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="isDESC"></param>
        /// <returns></returns>
        public List<TEntity> QueryEntitiesPaging<TEntity, TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> orderBy, bool isDESC = false) where TEntity : class
        {
            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder = new StringBuilder();

            builder.Append("SELECT TOP ");
            builder.Append(pageSize);
            builder.Append(" * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ");
            builder.Append(orderBy.ToString().LambdaToSqlStatementOrderBy());
            if (isDESC)
            {
                builder.Append(" DESC ");
            }
            else
            {
                builder.Append(" ASC ");
            }
            builder.Append(") AS RowNumber,* FROM ");
            builder.Append(tableName);
            builder.Append(" WHERE 1=1");
            builder.Append(") AS TTTAAABBBLLLEEE  WHERE RowNumber > (");
            builder.Append(pageSize);
            builder.Append(" * (");
            builder.Append(pageIndex);
            builder.Append(" - 1))");

            //Generate SqlStatement
            string sql = builder.ToString();

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return Return_List_T_ByDataSet<TEntity>(ExecuteDataSet(sql));
            });

            return result as List<TEntity>;
        }

        /// <summary>
        /// QueryEntitiesPaging
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="isDESC"></param>
        /// <returns></returns>
        public List<TEntity> QueryEntitiesPaging<TEntity, TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> orderBy, Expression<Func<TEntity, bool>> where, bool isDESC = false) where TEntity : class
        {
            string lambdaString = where.ToString();

            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder = new StringBuilder();


            builder.Append("SELECT TOP ");
            builder.Append(pageSize);
            builder.Append(" * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ");
            builder.Append(orderBy.ToString().LambdaToSqlStatementOrderBy());
            if (isDESC)
            {
                builder.Append(" DESC ");
            }
            else
            {
                builder.Append(" ASC ");
            }
            builder.Append(") AS RowNumber,* FROM ");
            builder.Append(tableName);
            builder.Append(" ");
            builder.Append(lambdaString.LambdaToSqlStatement());
            builder.Append(") AS TTTAAABBBLLLEEE  WHERE RowNumber > (");
            builder.Append(pageSize);
            builder.Append(" * (");
            builder.Append(pageIndex);
            builder.Append(" - 1))");

            //Generate SqlStatement
            string sql = builder.ToString();

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return Return_List_T_ByDataSet<TEntity>(ExecuteDataSet(sql));
            });

            return result as List<TEntity>;
        }

        /// <summary>
        /// QueryEntitiesPaging
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="count"></param>
        /// <param name="isDESC"></param>
        /// <returns></returns>
        public List<TEntity> QueryEntitiesPaging<TEntity, TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> orderBy, out int count, bool isDESC = false) where TEntity : class
        {
            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder = new StringBuilder();

            builder.Append("SELECT TOP ");
            builder.Append(pageSize);
            builder.Append(" * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ");
            builder.Append(orderBy.ToString().LambdaToSqlStatementOrderBy());
            if (isDESC)
            {
                builder.Append(" DESC ");
            }
            else
            {
                builder.Append(" ASC ");
            }
            builder.Append(") AS RowNumber,* FROM ");
            builder.Append(tableName);
            builder.Append(" WHERE 1=1");
            builder.Append(") AS TTTAAABBBLLLEEE  WHERE RowNumber > (");
            builder.Append(pageSize);
            builder.Append(" * (");
            builder.Append(pageIndex);
            builder.Append(" - 1))");

            //Generate SqlStatement
            string sql = builder.ToString();

            count = QueryCount<TEntity>();

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return Return_List_T_ByDataSet<TEntity>(ExecuteDataSet(sql));
            });

            return result as List<TEntity>;
        }

        /// <summary>
        /// QueryEntitiesPaging
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="count"></param>
        /// <param name="isDESC"></param>
        /// <returns></returns>
        public List<TEntity> QueryEntitiesPaging<TEntity, TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> orderBy, Expression<Func<TEntity, bool>> where, out int count, bool isDESC = false) where TEntity : class
        {
            string lambdaString = where.ToString();

            string tableName = GetTablaName(typeof(TEntity));

            StringBuilder builder = new StringBuilder();


            builder.Append("SELECT TOP ");
            builder.Append(pageSize);
            builder.Append(" * FROM (SELECT ROW_NUMBER() OVER (ORDER BY ");
            builder.Append(orderBy.ToString().LambdaToSqlStatementOrderBy());
            if (isDESC)
            {
                builder.Append(" DESC ");
            }
            else
            {
                builder.Append(" ASC ");
            }
            builder.Append(") AS RowNumber,* FROM ");
            builder.Append(tableName);
            builder.Append(" ");
            builder.Append(lambdaString.LambdaToSqlStatement());
            builder.Append(") AS TTTAAABBBLLLEEE  WHERE RowNumber > (");
            builder.Append(pageSize);
            builder.Append(" * (");
            builder.Append(pageIndex);
            builder.Append(" - 1))");

            //Generate SqlStatement
            string sql = builder.ToString();

            //Get Data Count
            count = QueryCount(where);

            //Cache Support
            string cacheKey = string.Concat(tableName, sql).GetHashCode().ToString();

            object result = CacheChannel(tableName, cacheKey, () =>
            {
                return Return_List_T_ByDataSet<TEntity>(ExecuteDataSet(sql));
            });

            return result as List<TEntity>;
        }
        #endregion

        #region prepare

        /// <summary>
        /// Get Table Name by Attribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetTablaName(Type type)
        {
            object[] objs = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (objs.FirstOrDefault() is TableAttribute attr)
                return !string.IsNullOrEmpty(attr.TableName) == true ? attr.TableName : type.Name;
            return type.Name;
        }
        #endregion

        #region Cache Support

        /// <summary>
        /// Cache Channel
        /// </summary>
        /// <param name="tableName">tableName</param>
        /// <param name="cacheKey">cacheKey</param>
        /// <param name="func">Func<object></param>
        /// <returns></returns>
        private object CacheChannel(string tableName, string cacheKey, Func<object> func)
        {
            /**
             * author:qixiao
             * create:2017-8-7 22:47:13
             * Howw to judge a cache lose efficacy when we update the table data ？
             * we add another cache name changeCache， let key = tableName，expire time default ，like 1.
             * once we gain data from cache ， validate changeCache has any value not null.if it is ,regard value changed,we should gain table data renew not from cache.
             * if it is null , regard as unchanged, if cacheCache expire ,dataCache expired too,we can gain cache relieved
             * */
            if (HttpRuntimeCache_Helper_DG.Cache_Get(tableName) == null)
            {
                object cacheValue = HttpRuntimeCache_Helper_DG.Cache_Get(cacheKey);
                if (cacheValue != null)
                {
                    return cacheValue;
                }
            }

            //Execute Action
            object result = func();

            HttpRuntimeCache_Helper_DG.Cache_Add(cacheKey, result);
            HttpRuntimeCache_Helper_DG.Cache_Delete(tableName);

            return result;
        }

        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() => GC.Collect();
    }
}
