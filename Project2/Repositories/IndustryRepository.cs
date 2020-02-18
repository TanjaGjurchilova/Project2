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
using Project2.repositories.Abstract;

namespace Project2.repositories
{
    public class IndustryRepository : IIndustryRepository
    {
        private readonly Db _context;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IndustryRepository(IConfiguration configuration)
        {
            _context = new Db();
        }
        public Industry DeleteIndustry(int IndustryId)
        {
            throw new NotImplementedException();
        }

        public Industry GetIndustry(int IndustryId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Industry> IndustryList()
        {
            DataTable dt;

            try
            {
                var cmd = _context.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "SELECT * FROM public.Industrys";

                dt = _context.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<Industry> list = (from DataRow dr in dt.Rows select CreateIndustryObject(dr)).ToList();

            return list;
        }

        public bool InsertIndustry(Industry Industry)
        {
            throw new NotImplementedException();
        }

        public Industry UpdateIndustry(int IndustryId)
        {
            throw new NotImplementedException();
        }
        private static Industry CreateIndustryObject(DataRow dr)
        {
            var Industry = new Industry
            {

                Id = int.Parse(dr["ID"].ToString()),
                Type = dr["type"].ToString()

            };
            return Industry;
        }
    }
}
