using Microsoft.Extensions.Configuration;
using Project2.Helpers;
using Project2.Infrastructure;
using Project2.Models;
using Project2.Repositories.Abstract;
using NLog;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

namespace Project2.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly Db _context;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UserRepository(IConfiguration configuration)
        {
            _context = new Db();
        }



        public IEnumerable<User> UserList()
        {
            DataTable dt;

            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT *, s.type FROM public.users a INNER JOIN public.roles s ON s.id = a.fk_role ORDER bY a.id DESC";

                dt = _context.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<User> list = (from DataRow dr in dt.Rows select CreateUserObject(dr)).ToList();

            return list;
        }

        public Task<User> ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
        private static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Changes password hash.
        /// </summary>
        /// <param name="pwd">The password</param>
        /// <param name="salt">The salt</param>
        /// <returns>SHA1 of the password</returns>
        public static string CreatePasswordHash(string pwd, string salt)
        {
            var hashedPwd = HashHelper.Create(pwd, salt);
            return hashedPwd;
        }

        private static User CreateUserObject(DataRow dr)
        {
            var user = new User
            {
                Id = int.Parse(dr["ID"].ToString()),
                FullName = dr["fullname"].ToString(),
                Username = dr["username"].ToString(),
                Firstname = dr["firstname"].ToString(),
                Surname = dr["surname"].ToString(),
                Address = dr["address"].ToString(),
                City = dr["city"].ToString(),
                Phone = dr["phone"].ToString(),
                Email = dr["email"].ToString(),
                Active = bool.Parse(dr["active"].ToString()),
                Appruved = bool.Parse(dr["appruved"].ToString()),
                CompanyUser = bool.Parse(dr["company_user"].ToString()),
                UserRole = new Role
                {
                    Id = int.Parse(dr["ID"].ToString()),
                    Type = dr["type"].ToString()
                }
            };
      
            //user.Appruved = false;
            //user.CompanyUser = true;
            return user;
        }

        public User GetUser(int UserId)
        {
            DataTable dt;

            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "SELECT * FROM public.users a INNER JOIN public.roles s ON s.\"ID\" = a.\"FK_Role\"  WHERE u.id=:id;";
                _context.CreateParameterFunc(cmd, "@id", UserId, NpgsqlDbType.Integer);
                dt = _context.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return CreateUserObject(dt.Rows[0]);
        }

        public bool InsertUser(User User)
        {
            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT * FROM public.users WHERE LOWER(email)=LOWER(:email);";
                _context.CreateParameterFunc(cmd, "@email", User.Email, NpgsqlDbType.Text);

                var dt = _context.ExecuteSelectCommand(cmd);

                if (dt.Rows.Count == 0)
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                    {
                        cmd.Connection.Open();
                    }
                    cmd.CommandText = "SELECT * FROM public.users WHERE LOWER(username)=LOWER(:un);";
                    _context.CreateParameterFunc(cmd, "@un", User.Username, NpgsqlDbType.Text);

                    dt = _context.ExecuteSelectCommand(cmd);

                    if (dt.Rows.Count == 0)
                    {
                        if (cmd.Connection.State != ConnectionState.Open)
                        {
                            cmd.Connection.Open();
                        }

                        var salt = CreateSalt(8);
                    
                        cmd.CommandText =
                            "INSERT INTO public.users (username, password, \"FK_Role\", fullname, firstname, surname, city, phone, email, salt, address, \"FK_Country\", \"FK_Company\", \"FK_Industry\", appruved)"
                            + "VALUES (:un1, :pass, :rid, :fn, :fin, :sn, :c, :phone, :email, :salt, :a, :counid, :comid, :iid, :apr);";
                        _context.CreateParameterFunc(cmd, "@rid", User.RoleId, NpgsqlDbType.Integer);
                        _context.CreateParameterFunc(cmd, "@pass", CreatePasswordHash(User.Password, salt), NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@fn", User.FullName, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@fin", User.Firstname, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@sn", User.Surname, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@c", User.City, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@un1", User.Username, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@phone", User.Phone, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@salt", salt, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@a", User.Address, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@email", User.Email, NpgsqlDbType.Text);
                        _context.CreateParameterFunc(cmd, "@iid", User.IndustryId, NpgsqlDbType.Integer);
                        _context.CreateParameterFunc(cmd, "@comid", User.CompanyId, NpgsqlDbType.Integer);
                        _context.CreateParameterFunc(cmd, "@counid", User.CountryId, NpgsqlDbType.Integer);
                        _context.CreateParameterFunc(cmd, "@apr", User.Appruved, NpgsqlDbType.Boolean);

                        var rowsAffected = _context.ExecuteNonQuery(cmd);
                        return rowsAffected == 1;
                    }
                    throw new Exception("Постои такво корисничко име");
                }
                throw new Exception("Постои таков емаил");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public User DeleteUser(int UserId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(User User)
        {
            int rowsAffected;
            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "UPDATE public.users SET fullname=:fn, city=:c, phone=:phone, email=:email, phone=:phone, address=:address, firstname=:fin, surname=:sn  WHERE id=:id;";
                _context.CreateParameterFunc(cmd, "@id", User.Id, NpgsqlDbType.Integer);
                _context.CreateParameterFunc(cmd, "@rid", User.RoleId, NpgsqlDbType.Integer);
                _context.CreateParameterFunc(cmd, "@fn", User.FullName, NpgsqlDbType.Text);
                _context.CreateParameterFunc(cmd, "@c", User.City, NpgsqlDbType.Text);
                _context.CreateParameterFunc(cmd, "@phone", User.Phone, NpgsqlDbType.Text);
                _context.CreateParameterFunc(cmd, "@address", User.Address, NpgsqlDbType.Text);
                _context.CreateParameterFunc(cmd, "@fin", User.Firstname, NpgsqlDbType.Text);
                _context.CreateParameterFunc(cmd, "@sn", User.Surname, NpgsqlDbType.Text);
                //_context.CreateParameterFunc(cmd, "@iid", User.IndustryId, NpgsqlDbType.Integer);
                //_context.CreateParameterFunc(cmd, "@comid", User.CompanyId, NpgsqlDbType.Integer);
                //_context.CreateParameterFunc(cmd, "@conid", User.CountryId, NpgsqlDbType.Integer);

                rowsAffected = _context.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return rowsAffected == 1;
        }

        User IUserRepository.ValidateUser(string emailORusername, string password)
        {
            DataTable dt;
            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT salt FROM public.users WHERE username=@unORem1 OR email=@unORem1";
                _context.CreateParameterFunc(cmd, "@unORem1", emailORusername, NpgsqlDbType.Text);
                var salt = _context.ExecuteScalar(cmd);
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                    "SELECT * FROM public.users u INNER JOIN public.roles s ON s.\"ID\" = u.\"FK_Role\"  WHERE (lower(u.username)=lower(@unORem) OR lower(u.email)=lower(@unORem)) AND u.password= @p;";
                _context.CreateParameterFunc(cmd, "@unORem", emailORusername, NpgsqlDbType.Text);
                _context.CreateParameterFunc(cmd, "@p", CreatePasswordHash(password, salt), NpgsqlDbType.Text);
                dt = _context.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                //throw new NotFoundException(string.Format("Wrong username or password"), ErrorCodes.ErrorCodeItemNotFound);
                return null;
            }
            return CreateUserObject(dt.Rows[0]);
        }

        public bool ChangeUserPassword(int userId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public string CreateResetPasswordToken(string usermail, string path)
        {
            DataTable dt;
            int userID;
            string userName;
            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "SELECT u.\"ID\", u.firstname FROM public.users u WHERE u.email like @u;";

                _context.CreateParameterFunc(cmd, "@u", usermail, NpgsqlDbType.Text);
                dt = _context.ExecuteSelectCommand(cmd);

                userID = int.Parse(dt.Rows[0]["ID"].ToString());
                userName = dt.Rows[0]["firstname"].ToString();
                //userID = Convert.ToInt32(_context.ExecuteScalar(cmd));
            }
            catch (Exception ex)
            {
                //  Logger.Error(ex);
                //throw new Exception(ex.Message);
                return "";
            }
            if (dt == null || dt.Rows.Count == 0)
            {
                return "";
            }

            //var userObj = CreateUserObject(dt.Rows[0]);

            //try
            //{
            //    var cmd = _context.CreateCommand();
            //    if (cmd.Connection.State != ConnectionState.Open)
            //    {
            //        cmd.Connection.Open();
            //    }
            //    cmd.CommandText = "DELETE FROM public.users u WHERE u.\"ID\" = @u;";
            //    _context.CreateParameterFunc(cmd, "@u", userID, NpgsqlDbType.Integer);
            //    _context.ExecuteNonQuery(cmd);
            //}
            //catch (Exception ex)
            //{
            //    // Logger.Error(ex);
            //    //throw new Exception(ex.Message);
            //    return "";
            //}
            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText = @"INSERT INTO public.recovery(fk_user, token, validthrough) values (@u, @t, @v);";
                string token = Guid.NewGuid().ToString();
                _context.CreateParameterFunc(cmd, "@u", userID, NpgsqlDbType.Integer);
                _context.CreateParameterFunc(cmd, "@t", token, NpgsqlDbType.Text);
                _context.CreateParameterFunc(cmd, "@v", DateTime.Now.AddDays(1), NpgsqlDbType.Date);
                dt = _context.ExecuteSelectCommand(cmd);
                // var recoveryObj = CreateRecoveryObject(dt.Rows[0]);
                var rootDir = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                string msgBody = String.Format(
                    "{1}. To reset your pasword click on the next link  {0}",
                    HttpUtility.HtmlEncode(path+ "Account/AddNewPassword?token=" +
                        token), HttpUtility.HtmlEncode(userName));
                //var result = Mail.SendMail(userObj.Email, ConfigurationManager.AppSettings["mailUser"],
                //    "Recovery password", msgBody);
                //return result;
                return msgBody;
            }
            catch (Exception ex)
            {
                //Logger.Error(ex);
                //throw new Exception(ex.Message);
                return "";
            }
        }

        public bool ResetUserPassword(int userId, out string pass)
        {
            throw new NotImplementedException();
        }
        public User LoginUserWithoutHash(string email, string password)
        {
            DataTable dt;
            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                // Scenario with roles table
                //cmd.CommandText =
                //    "SELECT u.*, r.role_name FROM public.user u INNER JOIN roles r ON u.role_id = r.role_id WHERE lower(u.email)=lower(:email) AND u.pass= :p AND u.active=1;";

                // Test example
                cmd.CommandText =
                    "SELECT * FROM public.users a INNER JOIN public.roles s ON s.\"ID\" = a.\"FK_Role\"  WHERE lower(a.email)=lower(:email) AND a.password= :p ;";

                _context.CreateParameterFunc(cmd, "@email", email, NpgsqlDbType.Text);
                _context.CreateParameterFunc(cmd, "@p", password, NpgsqlDbType.Text);

                dt = _context.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex, new object[] { email, password });
                throw new Exception(ex.Message);
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                //throw new NotFoundException(string.Format("Wrong username or password"), ErrorCodes.ErrorCodeItemNotFound);
                return null;
            }
            return CreateUserObject(dt.Rows[0]);
        }

    }

}
