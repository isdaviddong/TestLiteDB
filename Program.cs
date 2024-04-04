using LiteDB;
using System.Diagnostics.Tracing;
using System.Linq;
using Newtonsoft.Json;

// 定義一個POCO類來表示集合中的文檔
public class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Address { get; set; }
    //刪除資料

}

class Program
{
    const string filename = "MyData.db";

    static void Main(string[] args)
    {
        //建立一個 loop 只有當用戶輸入 Q 才離開
        while (true)
        {
            //顯示選單 
            Console.Clear();

            Console.WriteLine("0. 列出所有資料");
            Console.WriteLine("1. 新增資料");
            Console.WriteLine("2. 查詢資料");
            Console.WriteLine("3. 刪除資料");
            Console.WriteLine("Q. 離開");
            Console.WriteLine("------------------------");
            Console.Write("請選擇功能：");
            string? input = Console.ReadLine();

            switch (input.ToUpper())
            {
                case "0":
                    ListAllData();
                    break;
                case "1":
                    InsertData();
                    break;
                case "2":
                    QueryData();
                    break;
                case "3":
                    DeleteData();
                    break;
                case "Q":
                    return;
                default:
                    Console.WriteLine("無效的選擇");
                    break;
            }

            Console.WriteLine("\n\n按任意鍵繼續...");
            Console.ReadLine();
        }
    }

    private static void ListAllData()
    {
        // 使用 "Filename" 參數指定資料庫檔案的位置
        using (var db = new LiteDatabase(filename))
        {
            // 獲取或創建一個名為 "persons" 的集合
            var col = db.GetCollection<Person>("persons");

            // 使用LINQ查詢文檔
            var result = from c in col.FindAll() select c;

            foreach (var item in result)
            {
                Console.WriteLine($"編號: {item.Id} 人員：{item.Name}, 年齡：{item.Age}, address：{item.Address}");
            }
        }
    }

    //新增資料
    static void InsertData()
    {
        // 使用 "Filename" 參數指定資料庫檔案的位置
        using (var db = new LiteDatabase(filename))
        {
            // 獲取或創建一個名為 "persons" 的集合
            var col = db.GetCollection<Person>("persons");
            // 創建一個新的Person物件並插入集合中
            var person = GetUserData();
            var ret = col.Insert(person);
            Console.WriteLine($"資料已新增 編號: {ret.AsInt32} ：{person.Name}, 年齡：{person.Age}, address：{person.Address}");
        }
    }


    //查詢資料
    static void QueryData()
    {
        // 使用 "Filename" 參數指定資料庫檔案的位置
        using (var db = new LiteDatabase(filename))
        {
            // 獲取或創建一個名為 "persons" 的集合
            var col = db.GetCollection<Person>("persons");

            //輸入要查詢的關鍵字
            Console.Write("請輸入要查詢的關鍵字：");
            var word = Console.ReadLine();
            if (word != null)
            {
                // 使用LINQ查詢文檔
                var result = from c in col.FindAll() where c.Name.Contains(word) select c;
                Console.WriteLine($"\n找到{result.Count()}筆資料.\n");

                foreach (var item in result)
                {
                    Console.WriteLine($"編號: {item.Id} 人員：{item.Name}, 年齡：{item.Age}, address：{item.Address}");
                }
            }
        }
    }

    static Person GetUserData()
    {
        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = httpClient.GetAsync("https://randomuser.me/api/").Result;
                var json = response.Content.ReadAsStringAsync().Result;

                //反序列化 json 取得 dynamic物件
                dynamic userData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                if (userData != null)
                {
                    var name = userData.results[0].name.first + " " + userData.results[0].name.last;
                    var address = userData.results[0].location.street.number + " " + userData.results[0].location.street.name + ", " + userData.results[0].location.city + ", " + userData.results[0].location.country;
                    var age = userData.results[0].dob.age;

                    //回傳一個物件，包含
                    return new Person { Name = name, Age = age, Address = address };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
        return null;
    }

    static void DeleteData()
    {
        // 使用 "Filename" 參數指定資料庫檔案的位置
        using (var db = new LiteDatabase(filename))
        {
            // 獲取或創建一個名為 "persons" 的集合
            var col = db.GetCollection<Person>("persons");

            //輸入要刪除的編號
            Console.Write("請輸入要刪除的編號：");
            var id = Convert.ToInt32(Console.ReadLine());

            // 刪除指定編號的資料
            var result = col.Delete(id);

            if (result)
            {
                Console.WriteLine($"編號:{id} 資料已刪除");
            }
            else
            {
                Console.WriteLine($"找不到指定編號 '{id}' 的資料");
            }
        }
    }
}
