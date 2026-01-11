using System.Text.RegularExpressions;

namespace TestDojo.Mocking;

/// <summary>
/// 應用程式安裝器 - 協調 URL 驗證和安裝流程
/// </summary>
public class NewApplication
{
    private readonly IInstaller _installer;

    /// <summary>
    /// 建構函式 - 依賴注入安裝程式
    /// </summary>
    /// <param name="installer">實現 IInstaller 介面的安裝程式實例</param>
    public NewApplication(IInstaller installer)
    {
        _installer = installer;
    }

    /// <summary>
    /// 執行應用程式安裝流程
    /// </summary>
    /// <param name="url">應用程式的安裝網址</param>
    /// <returns>安裝是否成功</returns>
    public bool InstallApplication(string url)
    {
        // 步驟 1: 驗證 URL 格式
        if (!_installer.ValidateUrl(url)) return false;
        
        bool result;
        try
        {
            // 步驟 2: 執行安裝
            result = _installer.Install(url);
        }
        catch (Exception e)
        {
            // 步驟 3: 捕捉異常並記錄
            Console.WriteLine(e);
            throw;
        }
        return result;
    }
}

/// <summary>
/// 安裝程式介面 - 定義基礎安裝操作契約
/// </summary>
public interface IInstaller
{
    /// <summary>
    /// 執行應用程式安裝
    /// </summary>
    /// <param name="url">應用程式的網址</param>
    /// <returns>安裝是否成功</returns>
    bool Install(string url);

    /// <summary>
    /// 驗證網址格式是否有效 (https 或 http)
    /// </summary>
    /// <param name="url">要驗證的網址</param>
    /// <returns>網址是否有效</returns>
    bool ValidateUrl(string url);  
}

/// <summary>
/// 安裝程式實現 - 提供 URL 驗證和安裝功能
/// </summary>
public class Installer : IInstaller
{
    /// <summary>
    /// 執行應用程式安裝 (目前未實現)
    /// </summary>
    public bool Install(string url)
    {
        // 模擬安裝邏輯，實際實現可根據需求進行
        //Console.WriteLine($"Installing application from {url}...");
        //throw new NotImplementedException();
        return true;
    }
    
    /// <summary>
    /// 使用正則表達式驗證 URL 格式
    /// 驗證規則: https://uibakery.io/regex-library/url-regex-csharp
    /// </summary>
    /// <param name="url">待驗證的網址</param>
    /// <returns>URL 是否符合 https:// 或 http:// 格式</returns>
    public bool ValidateUrl(string url)
    {
        // 正則表達式模式: 驗證 http/https URL 格式
        var regex = new Regex(
            "^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]" +
            "{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        return regex.IsMatch(url);
    }
}