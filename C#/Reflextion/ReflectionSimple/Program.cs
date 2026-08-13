using System;
using System.Reflection;

public class UserProfile
{
    public string Name { get; set; }
    public void Greet(string message)
    {
        Console.WriteLine($"哈囉 {Name}, {message}");
    }
}

class Program
{
    static void Main()
    {
        var userProfile = new UserProfile();

        Console.WriteLine("=== 反射 (Reflection) 範例 ===");
        Console.WriteLine("Type is {0}", userProfile.GetType());
        // The example displays the following output:
        // Type is UserProfile

        int n1 = 12;
        int n2 = 82;
        long n3 = 12;
        Console.WriteLine("n1 and n2 are the same type: {0}",
                  Object.ReferenceEquals(n1.GetType(), n2.GetType()));
        Console.WriteLine("n1 and n3 are the same type: {0}",
                          Object.ReferenceEquals(n1.GetType(), n3.GetType()));
        // The example displays the following output:
        //       n1 and n2 are the same type: True
        //       n1 and n3 are the same type: False


        Assembly info = typeof(int).Assembly;
        Console.WriteLine(info);
        // The example displays the following output:
        // System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e



        //// 1. 取得類別的 Type 資訊
        Type type = typeof(UserProfile);
        Console.WriteLine("類別名稱: " + type.Name);

        // 2. 動態建立實例 (相當於 new UserProfile())
        object obj = Activator.CreateInstance(type);

        // 3. 動態設定屬性值 (Name = "子文")
        PropertyInfo prop = type.GetProperty("Name");
        prop.SetValue(obj, "子文"); // SetValue 修改屬性內容值

        // 4. 動態呼叫方法 (Greet("事事順利"))
        MethodInfo method = type.GetMethod("Greet");
        method.Invoke(obj, new object[] { "事事順利！" });

        // 5. 列出所有屬性 (常用於資料庫對應)
        Console.WriteLine("\n列出所有屬性：");
        foreach (var p in type.GetProperties()) // GetProperties 是 Type 類別的方法，用來取得類別的所有屬性資訊
        {
            Console.WriteLine($"- 屬性名: {p.Name}, 類型: {p.PropertyType}");
        }
    }
}
