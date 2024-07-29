using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApplication1.Models;
using Dapper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using System.Numerics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly string _connectString = @"Server=UT11302111;Database=mygoDB;Trusted_Connection=True;";
        private static readonly string UploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "logs");

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

        // POST api/<MembersController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT api/<MembersController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string phone)
        {
            using (var conn = new SqlConnection(_connectString))
            {
                string sql = "UPDATE Members SET Phone = @Phone WHERE Id = @Id";

                int count = conn.Execute(sql, new { Id = id, Phone = phone });
                if (count == 0)
                {
                    return NotFound("Member not found");
                }

                LogToFile(conn, id);
                return Ok("完成");
            }
        }

        // DELETE api/<MembersController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        private void LogToFile(SqlConnection conn, int id) //生成內容
        {
            string sqlSelect = "SELECT Id,Name,Phone FROM Members WHERE Id = @Id";
            var member = conn.QueryFirstOrDefault<Member>(sqlSelect, new { Id = id });
            string TransID = $"{TransactionId()}";
            string logTxt = $"TransID: {TransID}," +
                $"ID: {member.Id}," +
                $"姓名: {member.Name}," +
            $"電話: {member.Phone}";

            WriteLogToFile(LogFile(TransID), logTxt);
        }
        private void WriteLogToFile(string logFilePath, string logMessage) //寫入log
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($" {logMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log to file: {ex.Message}");
            }
        }
        private string TransactionId() //TS碼生成
        {
            DateTime now = DateTime.Now;
            string transTime = "TS" + now.ToString("yyyyMMddHHmm");
            string query = "SELECT TOP 1 * FROM LogMembers ORDER BY Id DESC";
            string TransID = transTime + "0001"; 

            using (SqlConnection conn = new SqlConnection(_connectString))
            {
                SqlCommand command = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string lastRecordName = reader["FileAddress"].ToString();
                        if (lastRecordName.Contains(transTime))
                        {
                            string numberPart = lastRecordName.Replace(transTime, "");
                            int fileAddressNumber;
                            if (int.TryParse(numberPart, out fileAddressNumber))
                            {
                                TransID = transTime + (fileAddressNumber + 1).ToString("D4"); 
                            }
                        }
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("資料庫連線或查詢失敗: " + ex.Message);
                }

                string sql = "INSERT INTO LogMembers (FileAddress) VALUES (@FileAddress)";

                int count = conn.Execute(sql, new { FileAddress = TransID });
                if (count == 0)
                {
                    Console.WriteLine("Failed to create LogMembers");
                }
            }

            return TransID;
        }
        private string LogFile(string TransID) //確認寫入資料夾
        {
            if (!Directory.Exists(UploadsFolder))
            {
                Directory.CreateDirectory(UploadsFolder);
            }
            string logFilePath = Path.Combine(UploadsFolder, $"{TransID}" + ".txt");
            return  logFilePath;
        }

    }
}
