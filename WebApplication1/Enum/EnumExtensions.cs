using System.ComponentModel;

namespace WebApplication1.Enum
{
        public enum statusCode
        {
            #region 系統x0xx
            [Description("0000")]
            成功 = 0000,
            [Description("9001")]
            超過每日上線 = 9001,
            [Description("9002")]
            驗證失敗或效期失效 = 9002,
            [Description("9003")]
            重複驗證 = 9003,
            [Description("9099")]
            系統錯誤 = 9099,
            #endregion

            #region 信箱x2xx
            [Description("9201")]
            重發信箱失敗 = 9201,
            [Description("9202")]
            驗證效期過期無法重複發送 = 9202,
            [Description("9203")]
            客戶端已驗證過無法重複發送 = 9203,
            #endregion

            #region 三竹x3xx
            [Description("9301")]
            發送三竹簡訊失敗 = 9301,
            #endregion
        }
        public static class EnumExtensions
        {
            public static string ToDescriptionString(this statusCode val)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])val
                   .GetType()
                   .GetField(val.ToString())
                   .GetCustomAttributes(typeof(DescriptionAttribute), false);
                return attributes.Length > 0 ? attributes[0].Description : string.Empty;
            }
        }
}
