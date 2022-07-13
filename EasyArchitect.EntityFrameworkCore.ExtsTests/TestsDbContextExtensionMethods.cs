using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyArchitect.EntityFrameworkCore.Exts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;
using EasyArchitect.EntityFrameworkCore.ExtsTests.Fake.Models;

namespace EasyArchitect.EntityFrameworkCore.Exts.Tests
{
    [TestClass()]
    public class TestsDbContextExtensionMethods
    {
        private IConfigurationRoot _configuration;

        [TestMethod()]
        public async Task Test_SqlQuery()
        {
            bool actual;
            bool expected = true;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");

            _configuration = builder.Build();

            var _dbContextOptions = new DbContextOptionsBuilder<HousingLoansDbContext>()
                .UseOracle(_configuration.GetConnectionString("Orcl.Connection"))
                .Options;

            HousingLoansDbContext context = new HousingLoansDbContext(_dbContextOptions);
            string sqlText = @"SELECT T.CUSTOMERID,
    T.CHTNAME,
    T.AID,
    T.MOBILE,
    T.TEL,
    T.ADDRESS,
    T.LOCATION,
    T.EMAIL,
    T.MARRY,
    T.FAMILYNUM,
    T.EDUCATION,
    T.USERID
FROM CUSTOMERDETAILVO T";
            var result = await context.SqlQuery<Customerdetail>(sqlText, null);
            int count = result.Count();
            Assert.IsTrue(count > 0);
        }
    }
}