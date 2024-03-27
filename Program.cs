using LiteDB;
using System.Linq;

// 定義一個POCO類來表示集合中的文檔
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        // 使用 "Filename" 參數指定資料庫檔案的位置
        using (var db = new LiteDatabase(@"MyData.db"))
        {
            // 獲取或創建一個名為 "persons" 的集合
            var col = db.GetCollection<Person>("persons");

            // 創建一個新的Person物件並插入集合中
            var person = new Person { Name = "John Doe", Age = 30, Address = "USA" };
            col.Insert(person);

            // 使用LINQ查詢文檔
            var result =from c in col.FindAll() where c.Age > 20 select c; 

            foreach (var item in result)
            {
                Console.WriteLine($"找到 {item.Id} 人員：{item.Name}, 年齡：{item.Age}, address：{item.Address}");
            }
        }
    }
}
