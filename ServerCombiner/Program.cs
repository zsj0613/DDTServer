using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Admin;
 
namespace ServerCombiner
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Please input the name of database of the main server.");
            var str = Console.ReadLine();
            //var db1 = new Entities($"Data Source=.;Initial Catalog={str};Integrated Security=SSPI");
            var db1 = new Entities($"Data Source=www.hqgddt.com;Initial Catalog={str};User Id=sa;Password=5214.lsj.5214");
            Console.WriteLine("Please input the name of database of the second server.");


            str = Console.ReadLine();
            //var db2 = new Entities($"Data Source=.;Initial Catalog={str};Integrated Security=SSPI");
            var db2 = new Entities($"Data Source=www.hqgddt.com;Initial Catalog={str};User Id=sa;Password=5214.lsj.5214");
            Console.WriteLine("Please make sure all the data like activities, goods templates and more are the same in the databases. And Make Sure to Have Backed Up the Data. Press Any Key to Continue.");
            Console.ReadKey();


 







        }
    }
}
