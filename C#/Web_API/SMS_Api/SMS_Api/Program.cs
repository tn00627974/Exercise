using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;

class Program
{
    static async Task Main(string[] args)
    {
        string url = "https://smsapi.mitake.com.tw/api/mtk/SmSend?CharsetURL=UTF-8"; 
        string username = "24785797SMS";
        string password = "Yi0417Ent0054Di";
        string dstaddr = "0952739894";
        string smbody = "測試訊息";

        var client = new HttpClient();

        var parameters = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password },
            { "dstaddr", dstaddr },
            { "smbody", smbody }
        };

        var content = new FormUrlEncodedContent(parameters);
        var response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"回應內容: {responseContent}");
        }
        else
        {
            Console.WriteLine($"錯誤代碼: {response.StatusCode}");
            Console.WriteLine("無法發送簡訊，請檢查帳號或 API 路徑設定。");
        }
    }
}
