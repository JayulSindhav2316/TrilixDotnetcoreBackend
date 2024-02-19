using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;
using Telerik.Reporting.Cache.Interfaces;

namespace Max.Reporting.Infrastructure.MySqlStorage
{
    public class MySqlServerStorage : IStorage2,IStorage
    {

        interface IDeleteBehavior
        {
            void DeleteMasterKey(string key);

            void DeleteSet(string key);
        }

        class AutoDeleteBehavior : IDeleteBehavior
        {
            readonly MySqlServerStorage owner;

            public AutoDeleteBehavior(MySqlServerStorage owner)
            {
                this.owner = owner;
            }

            void IDeleteBehavior.DeleteMasterKey(string key)
            {
                try
                {
                    UseBehavior(new ProcedureDeleteBehavior(this.owner));
                }
                catch (System.Exception)
                {
                    UseBehavior(new QueryDeleteBehavior(this.owner));
                }

                void UseBehavior(IDeleteBehavior newBehavior)
                {
                    newBehavior.DeleteMasterKey(key);
                    this.owner.deleteBehavior = newBehavior;
                }
            }

            void IDeleteBehavior.DeleteSet(string key)
            {
                try
                {
                    UseBehavior(new ProcedureDeleteBehavior(this.owner));
                }
                catch (System.Exception)
                {
                    UseBehavior(new QueryDeleteBehavior(this.owner));
                }

                void UseBehavior(IDeleteBehavior newBehavior)
                {
                    newBehavior.DeleteSet(key);
                    this.owner.deleteBehavior = newBehavior;
                }
            }
        }

        class ProcedureDeleteBehavior : IDeleteBehavior
        {
            const string DeleteSetSP = "sp_tr_DeleteSet";
            const string DeleteLikeSP = "sp_tr_DeleteLike";

            readonly MySqlServerStorage owner;

            public ProcedureDeleteBehavior(MySqlServerStorage owner)
            {
                this.owner = owner;
            }

            /// <summary>
            /// Deletes a set of values denoted by the given key.
            /// </summary>
            /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.DeleteInSet"/>
            public void DeleteSet(string key)
            {
                this.owner.WithStoredProc(DeleteSetSP, key, cmd => cmd.ExecuteNonQuery());
            }

            public void DeleteMasterKey(string key)
            {
                this.owner.WithStoredProc(DeleteLikeSP, key, cmd => cmd.ExecuteNonQuery());
            }
        }

        class QueryDeleteBehavior : IDeleteBehavior
        {
            const string DeleteSetQuery =
                "DELETE FROM tr_Set WHERE Id = @Key";

            const string DeleteMasterKeyQuery =
                @"DELETE FROM tr_String WHERE Id LIKE @Key + '%' " +
                @"DELETE FROM tr_Object WHERE Id LIKE @Key + '%' " +
                @"DELETE FROM tr_Set WHERE Id LIKE @Key + '%' ";

            readonly MySqlServerStorage owner;

            public QueryDeleteBehavior(MySqlServerStorage owner)
            {
                this.owner = owner;
            }

            void IDeleteBehavior.DeleteMasterKey(string key)
            {
                this.owner.WithCommand(DeleteMasterKeyQuery, key, cmd => cmd.ExecuteNonQuery());
            }

            void IDeleteBehavior.DeleteSet(string key)
            {
                this.owner.WithCommand(DeleteSetQuery, key, cmd => cmd.ExecuteNonQuery());
            }
        }

        const string Schema = "";

        const string AcquireLockQuery = Schema + "sp_tr_AcquireLock";

        const string ExistsQuery = Schema + "sp_tr_Exists";
        const string ReportExistsQuery = Schema + "sp_tr_ExistsInReport";
        const string GetStringQuery = Schema + "sp_tr_GetString";
        const string SetStringQuery = Schema + "sp_tr_SetString";
        const string GetBytesQuery = Schema + "sp_tr_GetBytes";
        const string GetReportBytesQuery = Schema + "sp_tr_GetReportBytes";
        const string SetBytesQuery = Schema + "sp_tr_SetObject";
        const string SetReportDefinitionQuery = Schema + "sp_tr_SetReportDefinition";
        const string DeleteQuery = Schema + "sp_tr_Delete";

        const string ExistsInSetQuery = Schema + "sp_tr_ExistsInSet";
        const string AddInSetQuery = Schema + "sp_tr_AddInSet";
        const string GetCountInSetQuery = Schema + "sp_tr_GetCountInSet";
        const string GetAllMembersInSetQuery = Schema + "sp_tr_GetMembersInSet";
        const string DeleteInSetQuery = Schema + "sp_tr_DeleteInSet";

        const string ClearAllQuery = Schema + "sp_tr_ClearAll";

        const int MinPoolSize = 30;

        readonly int commandTimeout = 30; // seconds
        IDeleteBehavior deleteBehavior;

        public MySqlServerStorage(IConfiguration configuration)
        {
            this.deleteBehavior = new AutoDeleteBehavior(this);                        
            this.ConnectionString = AdjustConnectionString(configuration.GetConnectionString("Default"));
        }

        public MySqlServerStorage(IConfiguration configuration, int commandTimeout)
            : this(configuration, cmdTimeout: commandTimeout)
        {
        }

        internal MySqlServerStorage(IConfiguration configuration, int? cmdTimeout)
            : this(configuration)
        {
            this.commandTimeout = cmdTimeout ?? this.commandTimeout;
        }

        internal string ConnectionString { get; }

        // tests
        internal int CommandTimeout => this.commandTimeout;

        /// <summary>
        /// Acquires a lock on a named resource.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.AcquireLock"/>
        public IDisposable AcquireLock(string key)
        {
            return new TransactionLock(key, this);
        }

        /// <summary>
        /// Retrieves a value indicating if a single value (string or byte array)
        /// exists in the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.Exists"/>
        public bool Exists(string key)
        {
            return this.WithStoredProc<bool>(
                ExistsQuery,
                key,
                cmd =>
                {
                    var returnValue = CreateReturnValueParameter();
                    cmd.Parameters.Add(returnValue);

                    cmd.ExecuteNonQuery();

                    return ((Int16)returnValue.Value) > 0;
                });
        }

        /// <summary>
        /// Retrieves a value indicating if a single value (string or byte array)
        /// exists in the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.Exists"/>
        public bool ReportExists(string key)
        {
            return this.WithStoredProc<bool>(
                ReportExistsQuery,
                key,
                cmd =>
                {
                    var returnValue = CreateReturnValueParameter();
                    cmd.Parameters.Add(returnValue);

                    cmd.ExecuteNonQuery();

                    return ((Int16)returnValue.Value) > 0;
                });
        }

        /// <summary>
        /// Stores a string value under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.SetString"/>
        public void SetString(string key, string value)
        {
            this.WithStoredProc(
                SetStringQuery,
                key,
                cmd =>
                {
                    cmd.Parameters.Add(new MySqlParameter("p_Value", MySqlDbType.VarChar, 4000) { Value = value });
                    cmd.ExecuteNonQuery();
                });
        }

        /// <summary>
        /// Stores a byte array value under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.SetBytes"/>
        public void SetBytes(string key, byte[] value)
        {
            this.WithStoredProc(
                SetBytesQuery,
                key,
                cmd =>
                {
                    cmd.Parameters.Add(new MySqlParameter("p_Value", MySqlDbType.LongBlob) { Value = value });
                    cmd.ExecuteNonQuery();
                });
        }
        public void SetReportDefinition(string key, string value)
        {
            this.WithStoredProc(
                SetReportDefinitionQuery,
                key,
                cmd =>
                {
                    cmd.Parameters.Add(new MySqlParameter("p_Value", MySqlDbType.LongText) { Value = value });
                    cmd.ExecuteNonQuery();
                });
        }
        /// <summary>
        /// Retrieves a string value stored under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetString"/>
        public string GetString(string key)
        {
            return this.WithStoredProc<string>(
                GetStringQuery,
                key,
                cmd =>
                {
                    return (string)cmd.ExecuteScalar();
                });
        }

        /// <summary>
        /// Retrieves a byte array value stored under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetBytes"/>
        public byte[] GetBytes(string key)
        {
            return this.WithStoredProc<byte[]>(
                GetBytesQuery,
                key,
                cmd =>
                {
                    return (byte[])cmd.ExecuteScalar();
                });
        }

        /// <summary>
        /// Retrieves a byte array value stored under particular key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetBytes"/>
        public string GetReportBytes(string key)
        {
            return this.WithStoredProc<string>(
                GetReportBytesQuery,
                key,
                cmd =>
                {
                    return (string)cmd.ExecuteScalar();
                });
        }

        /// <summary>
        /// Deletes a key with its value (string or byte array) from the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.Delete"/>
        public void Delete(string key)
        {
            this.WithStoredProc(DeleteQuery, key, cmd => cmd.ExecuteNonQuery());
        }

        /// <summary>
        /// Retrieves a value indicating if a set of values
        /// exists in the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.ExistsInSet"/>
        public bool ExistsInSet(string key, string value)
        {
            return this.WithStoredProc<bool>(
                ExistsInSetQuery,
                key,
                cmd =>
                {
                    var pValue = new MySqlParameter("p_Value", MySqlDbType.VarChar, 255) { Value = value };
                    var returnValue = CreateReturnValueParameter();
                    cmd.Parameters.AddRange(new[] { pValue, returnValue });

                    cmd.ExecuteNonQuery();

                    return ((Int16)returnValue.Value) > 0;
                });
        }

        /// <summary>
        /// Retrieves the count of the values in a set value stored in the storage.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetCountInSet"/>
        public long GetCountInSet(string key)
        {
            return this.WithStoredProc<long>(GetCountInSetQuery, key, cmd => { return (int)cmd.ExecuteScalar(); });
        }

        /// <summary>
        /// Retrieves all members in a set of string values.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.GetAllMembersInSet"/>
        public IEnumerable<string> GetAllMembersInSet(string key)
        {
            return this.WithStoredProc<List<string>>(
                GetAllMembersInSetQuery,
                key,
                cmd =>
                {
                    var result = new List<string>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0));
                        }

                        reader.Close();
                    }

                    return result;
                });
        }

        /// <summary>
        /// Adds a single string value to a set of values denoted from the given key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.AddInSet"/>
        public void AddInSet(string key, string value)
        {
            this.WithStoredProc(
                AddInSetQuery,
                key,
                cmd =>
                {
                    var pValue = new MySqlParameter("p_Value", MySqlDbType.VarChar, 255) { Value = value };
                    cmd.Parameters.Add(pValue);

                    cmd.ExecuteNonQuery();
                });
        }

        /// <summary>
        /// Deletes a single string value from a set of values denoted from the given key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.DeleteInSet"/>
        public bool DeleteInSet(string key, string value)
        {
            return this.WithStoredProc<bool>(
                DeleteInSetQuery,
                key,
                cmd =>
                {
                    var pValue = new MySqlParameter("p_Value", MySqlDbType.VarChar, 255) { Value = value };
                    var returnValue = CreateReturnValueParameter();
                    cmd.Parameters.AddRange(new[] { pValue, returnValue });

                    cmd.ExecuteNonQuery();

                    return ((Int16)returnValue.Value) > 0;
                });
        }

        /// <summary>
        /// Deletes a set of values denoted by the given key.
        /// </summary>
        /// <seealso cref="Telerik.Reporting.Cache.Interfaces.IStorage.DeleteInSet"/>
        public void DeleteSet(string key)
        {
            this.deleteBehavior.DeleteSet(key);
        }

        public void DeleteMasterKey(string key)
        {
            this.deleteBehavior.DeleteMasterKey(key);
        }

       
        /// <summary>
        /// Utility method. Clears all data from the storage data tables.
        /// </summary>
        public void ClearAllData()
        {
            using (var conn = this.NewConnection())
            {
                var cmd = new MySqlCommand(ClearAllQuery, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = this.commandTimeout
                };

                cmd.ExecuteNonQuery();
            }
        }

        static string AdjustConnectionString(string connectionString)
        {
            var builder =
                new MySqlConnectionStringBuilder(connectionString);

            if (!builder.ShouldSerialize("Min Pool Size") &&
                builder.MaximumPoolSize == 0 &&
                builder.MaximumPoolSize >= MinPoolSize)
            {
                builder.MaximumPoolSize = MinPoolSize;
            }

            return builder.ConnectionString;
        }

        static MySqlParameter CreateKeyParameter(string key)
        {
            return new MySqlParameter("p_Key", MySqlDbType.VarChar, 255)
            {
                Value = key,
            };
        }

        static MySqlParameter CreateReturnValueParameter()
        {
            return new MySqlParameter("p_return",MySqlDbType.Int16)
            {
                Direction = ParameterDirection.Output,
            };
        }

        void WithStoredProc(string query, string key, Action<MySqlCommand> action)
        {
            using (var conn = this.NewConnection())
            {
                var cmd = new MySqlCommand(query, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = this.commandTimeout
                };

                cmd.Parameters.Add(CreateKeyParameter(key));

                action(cmd);
            }
        }

        T WithStoredProc<T>(string query, string key, Func<MySqlCommand, T> function)
        {
            using (var conn = this.NewConnection())
            {
                var cmd = new MySqlCommand(query, conn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = this.commandTimeout
                };

                cmd.Parameters.Add(CreateKeyParameter(key));

                return function(cmd);
            }
        }

        void WithCommand(string query, string key, Action<MySqlCommand> action)
        {
            using (var conn = this.NewConnection())
            {
                var cmd = new MySqlCommand(query, conn)
                {
                    CommandType = CommandType.Text,
                    CommandTimeout = this.commandTimeout
                };

                cmd.Parameters.Add(CreateKeyParameter(key));

                action(cmd);
            }
        }

        MySqlConnection NewConnection()
        {
            var c = new MySqlConnection(this.ConnectionString);
            c.Open();
            return c;
        }

        class TransactionLock : IDisposable
        {
            public TransactionLock(string key, MySqlServerStorage owner)
            {
                this.Connection = owner.NewConnection();
                this.Transaction = this.Connection.BeginTransaction(IsolationLevel.Serializable);

                this.ExecuteAcquireCommand(key);
            }

            public MySqlTransaction Transaction { get; private set; }

            public MySqlConnection Connection { get; private set; }

            public void Dispose()
            {
                try
                {
                    this.Transaction.Rollback();
                }
                finally
                {
                    this.Transaction.Dispose();
                    this.Connection.Dispose();
                }
            }

            void ExecuteAcquireCommand(string key)
            {
                var cmd = new MySqlCommand(AcquireLockQuery, this.Connection, this.Transaction)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.AddRange(new[] { CreateKeyParameter(key) });

                cmd.ExecuteNonQuery();
            }
        }
    }
}
