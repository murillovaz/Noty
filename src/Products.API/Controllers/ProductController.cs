using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Noty;
using Noty.MySql;
using Noty.SqlServer;
using Products.API.Entities;

namespace Products.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly SqlServerContext _ctx;
        private readonly MySqlContext _ctxMysql;
        public ProductController(SqlServerContext ctx, MySqlContext mysql)
        {
            _ctx = ctx;
            _ctxMysql = mysql;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var valuesSqlServerNonQuery = await _ctx.ExecuteStoredProcedureNonQuery("Products_Insert", 
                "name".WithValue("Banana Nanica"),
                "price".WithValue(100.00)
                );

            var valuesSqlServer = await _ctx.ExecuteStoredProcedureCollectionReader<Product>("Products_Get", 
                "@id".WithValue(1));

            var valuesMySql = await _ctxMysql.ExecuteStoredProcedureCollectionReader<Product>("Products_Get", "v_id".WithValue(1));

            return Ok(JsonConvert.SerializeObject(new { valuesMySql = valuesMySql, valuesSqlServer = valuesSqlServer, valuesSqlServerNonQuery = valuesSqlServerNonQuery }, Formatting.Indented));
        }
    }
}
