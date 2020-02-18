using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Web;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Infrastructure
{
    public class Db
    {

        private readonly string _connectionString;
        private static Logger _logger;

        public Db()
        {
            //var configuration = new ConfigurationBuilder().AddJsonFile("wwwroot/appsettingsDB.json").Build();

            var builder = new ConfigurationBuilder()                
                .AddJsonFile("appsettingsDB.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            _connectionString = configuration["DBInfo:DefaultConnection"];
            _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        // execute an update, delete, or insert command 
        // and return the number of affected rows
        public int ExecuteNonQuery(NpgsqlCommand command, bool closeConn = true)
        {
            // The number of affected rows 
            int affectedRows = -1;
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Execute the command and get the number of affected rows
                command.CommandTimeout = 90;
                affectedRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                if (closeConn)
                {
                    // Close the connection
                    command.Connection.Close();
                }
            }
            // return the number of affected rows
            return affectedRows;
        }
        // execute an update, delete, or insert command 
        // and return the number of affected rows
        public async Task<int> ExecuteNonQueryAsync(NpgsqlCommand command, bool closeConn = true)
        {
            // The number of affected rows 
            var affectedRows = -1;
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Execute the command and get the number of affected rows
                command.CommandTimeout = 90;
                affectedRows = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                if (closeConn)
                {
                    // Close the connection
                    command.Connection.Close();
                }
            }
            // return the number of affected rows
            return affectedRows;
        }

        // execute a select command and return a single result as a string
        public string ExecuteScalar(NpgsqlCommand command, bool closeConn = true)
        {
            // The value to be returned 
            string value = "";
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the connection of the command
                //command.Connection.Open();
                // Execute the command and get the number of affected rows
                //value = command.ExecuteScalar().ToString();
                command.CommandTimeout = 90;
                var c = command.ExecuteScalar();
                if (c != null)
                {
                    value = c.ToString();
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                if (closeConn)
                {
                    // Close the connection
                    command.Connection.Close();
                }
            }
            // return the result
            return value;
        }

        // execute a select command and return a single result as a string
        public async Task<string> ExecuteScalarAsync(NpgsqlCommand command, bool closeConn = true)
        {
            // The value to be returned 
            var value = "";
            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the connection of the command
                //command.Connection.Open();
                // Execute the command and get the number of affected rows
                //value = command.ExecuteScalar().ToString();
                command.CommandTimeout = 90;
                var c = await command.ExecuteScalarAsync();
                if (c != null)
                {
                    value = c.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                if (closeConn)
                {
                    // Close the connection
                    command.Connection.Close();
                }
            }
            // return the result
            return value;
        }

        /// <summary>
        /// Method that executes select command against database.
        /// </summary>
        /// <param name="command">The command to be executed</param>
        /// <returns>DataTable with results or null if exception has occured</returns>
        /// <exception cref="Exception">Throws Exception if something went wrong</exception>
        /// //[Obsolete("Use method ExecuteSelectCommandAsync")]
        public DataTable ExecuteSelectCommand(NpgsqlCommand command, bool closeConn = true)
        {
            // The DataTable to be returned 
            DataTable table;

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the data connection 

                //iskomentirano e zosto ne progashe so nego
                //command.Connection.Open(); 

                command.CommandTimeout = 190;
                // Execute the command and save the results in a DataTable
                DbDataReader reader = command.ExecuteReader();
                table = new DataTable("myData");
                table.Load(reader);

                // Close the reader 
                reader.Close();
            }
            catch (Exception ex)
            {

                //logger.Error(ex.Message, ex);
                throw;
            }
            finally
            {
                if (closeConn)
                {
                    // Close the connection
                    command.Connection.Close();
                }
            }
            return table;
        }

        public async Task<DataTable> ExecuteSelectCommandAsync(NpgsqlCommand command, bool closeConn = true)
        {
            // The DataTable to be returned 
            DataTable table;

            // Execute the command making sure the connection gets closed in the end
            try
            {
                // Open the data connection 

                //iskomentirano e zosto ne progashe so nego
                //command.Connection.Open(); 
                command.CommandTimeout = 190;
                // Execute the command and save the results in a DataTable
                var reader = await command.ExecuteReaderAsync();
                table = new DataTable("myData");
                table.Load(reader);

                // Close the reader 
                reader.Close();
            }
            catch (Exception ex)
            {

                _logger.Error(ex.Message, ex);
                throw;
            }
            finally
            {
                if (closeConn)
                {
                    // Close the connection
                    command.Connection.Close();
                }
            }
            return table;
        }

        //[Obsolete("Use method ExecuteSelectCommandWithoutConnCloseAsync", false)]
        //public static DataTable ExecuteSelectCommandWithoutConnClose(DbCommand command)
        //{
        //    // The DataTable to be returned 
        //    DataTable table;

        //    // Execute the command making sure the connection gets closed in the end
        //    try
        //    {
        //        // Open the data connection 

        //        //iskomentirano e zosto ne progashe so nego
        //        //command.Connection.Open(); 

        //        command.CommandTimeout = 190;
        //        // Execute the command and save the results in a DataTable
        //        var reader = command.ExecuteReader();
        //        table = new DataTable("myData");
        //        table.Load(reader);

        //        // Close the reader 
        //        reader.Close();
        //    }
        //    catch (Exception ex)
        //    {

        //        logger.Error(ex.Message, ex);
        //        throw;
        //    }
        //    finally
        //    {
        //        command.Parameters.Clear();
        //    }
        //    return table;
        //}

        //public static async Task<DataTable> ExecuteSelectCommandWithoutConnCloseAsync(DbCommand command)
        //{
        //    // The DataTable to be returned 
        //    DataTable table;

        //    // Execute the command making sure the connection gets closed in the end
        //    try
        //    {
        //        // Open the data connection 

        //        //iskomentirano e zosto ne progashe so nego
        //        //command.Connection.Open(); 

        //        command.CommandTimeout = 90;
        //        // Execute the command and save the results in a DataTable
        //        var reader = await command.ExecuteReaderAsync();
        //        table = new DataTable("myData");
        //        table.Load(reader);

        //        // Close the reader 
        //        reader.Close();
        //    }
        //    catch (Exception ex)
        //    {

        //        logger.Error(ex.Message, ex);
        //        throw;
        //    }
        //    finally
        //    {
        //        command.Parameters.Clear();
        //    }
        //    return table;
        //}

        // creates and prepares a new DbCommand object on a new connection
        public NpgsqlCommand CreateCommand()
        {
            // Obtain the database connection string
            //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //string connectionString = "Server=localhost; Port = 5434; Database=core_db_template; User ID=postgres; Password=Veko321@; Pooling=false; CommandTimeout=60;";
            string connectionString = _connectionString;
            // Obtain a database specific connection object
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            // Create a database specific command object
            NpgsqlCommand comm = conn.CreateCommand();
            // Set the command type to stored procedure
            comm.CommandType = CommandType.Text;
            // Return the initialized command object
            return comm;
            return null;
        }



        public void CreateParameterFunc(NpgsqlCommand cmd, string name, ParameterDirection direction, object value, NpgsqlDbType type)
        {
            NpgsqlParameter param = cmd.CreateParameter();

            param.ParameterName = name;
            param.Direction = direction;
            param.NpgsqlValue = value;
            param.NpgsqlDbType = type;
            cmd.Parameters.Add(param);
        }

        public void CreateParameterFunc(NpgsqlCommand cmd, string name, object value, NpgsqlDbType type)
        {
            NpgsqlParameter param = cmd.CreateParameter();

            param.ParameterName = name;
            param.Direction = ParameterDirection.Input;
            param.NpgsqlValue = value;
            param.NpgsqlDbType = type;
            cmd.Parameters.Add(param);
        }
    }
}
