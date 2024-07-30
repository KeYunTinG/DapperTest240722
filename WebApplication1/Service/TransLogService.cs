using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using WebApplication1.Interface;
using WebApplication1.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Newtonsoft.Json;

namespace WebApplication1.Service
{
    public class TransLogService : ITransLogService
    {
        private readonly string _uploadsFolder;
        public TransLogService(IConfiguration configuration)
        {
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), configuration["LoggingSettings:UploadsFolder"]);
        }
        public void Log(ITransLog log)
        {
            if (!Directory.Exists(_uploadsFolder)) //確認寫入資料夾
            {
                Directory.CreateDirectory(_uploadsFolder);
            }

            string logFilePath = Path.Combine(_uploadsFolder, $"{log.traceId}" + ".txt");

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    string logTxt = JsonConvert.SerializeObject(log, Newtonsoft.Json.Formatting.Indented);
                    writer.WriteLine($" {logTxt}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log to file: {ex.Message}");
            }
        }
    }
}
