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

namespace Project2.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly Db _context;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public RoleRepository(IConfiguration configuration)
        {
            _context = new Db();
        }

        public Role DeleteRole(int RoleId)
        {
            throw new NotImplementedException();
        }

        public Role GetRole(int RoleId)
        {
            throw new NotImplementedException();
        }

        public bool InsertRole(Role Role)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Role> RoleList()
        {
            DataTable dt;

            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT * FROM public.roles";

                dt = _context.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<Role> list = (from DataRow dr in dt.Rows select CreateRoleObject(dr)).ToList();

            return list;
        }

        public Role UpdateRole(int RoleId)
        {
            throw new NotImplementedException();
        }
        private static Role CreateRoleObject(DataRow dr)
        {
            var role = new Role
            {
               
                Id = int.Parse(dr["ID"].ToString()),
                Type = dr["type"].ToString()
               
            };
            return role;
        }
    }
}
