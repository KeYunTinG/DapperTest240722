using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApplication1.Models;
using Dapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Numerics;
using WebApplication1.Interface;
using System.Xml.Linq;
using System.Diagnostics.Metrics;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly string _connectString;
        private readonly string _uploadsFolder;
        private readonly ITransLogService _transLogService;

        public MembersController(IConfiguration configuration , ITransLogService transLogService)
        {
            _connectString = configuration.GetConnectionString("DefaultConnection");
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), configuration["LoggingSettings:UploadsFolder"]);
            _transLogService = transLogService;
        }
        // GET: api/<MembersController>
        [HttpGet]
        public IEnumerable<Member> Get()
        {
            using (var conn = new SqlConnection(_connectString))
                return conn.Query<Member>("SELECT * FROM Members");
        }
        // GET api/<MembersController>/5
        [HttpGet("{id}")]
        public IEnumerable<Member> Get(int id)
        {
            using (var conn = new SqlConnection(_connectString))
                return conn.Query<Member>("SELECT * FROM Members WHERE Id = @Id", new { Id = id });
        }

        [HttpPost]
        public IActionResult Post(int id, [FromBody] string phone)
        {
            using (var conn = new SqlConnection(_connectString))
            {
                string sql = "UPDATE Members SET Phone = @Phone WHERE Id = @Id";

                int count = conn.Execute(sql, new { Id = id, Phone = phone });
                if (count == 0)
                {
                    LogToFileFail();
                    return NotFound("Member not found");
                }

                LogToFileSuccess(conn, id);
                return Ok("完成");
            }
        }
        private void LogToFileSuccess(SqlConnection conn, int id) //生成內容
        {
            string sqlSelect = "SELECT Id,Name,Phone FROM Members WHERE Id = @Id";
            var member = conn.QueryFirstOrDefault<Member>(sqlSelect, new { Id = id });
            string transTime = "TS" + DateTime.Now.ToString("yyyyMMddHHmm");
            var guid = Guid.NewGuid().ToString();
            string traceCode = transTime + guid.Substring(guid.Length-12);
            var log = new ITransLog
            {
                traceId = traceCode,
                rtnCode = "0000",
                msg = "成功",
                info = new Info
                {
                    Id = member.Id,
                    Name = member.Name,
                    Phone = member.Phone
                }
            };

            _transLogService.Log(log);
        }
        private void LogToFileFail()
        {
            string transTime = "TS" + DateTime.Now.ToString("yyyyMMddHHmm");
            var guid = Guid.NewGuid().ToString();
            string traceCode = transTime + guid.Substring(guid.Length - 12);
            var log = new ITransLog
            {
                traceId = traceCode,
                rtnCode = "9099",
                msg = "系統錯誤",
            };

            _transLogService.Log(log);
        }
    }
}
