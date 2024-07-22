using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApplication1.Models;
using Dapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly string _connectString = @"Server=(LocalDB)\MSSQLLocalDB;Database=myDB;Trusted_Connection=True;";

        public MembersController(string connectString)
        {
            _connectString = connectString;
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
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MembersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<MembersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MembersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
