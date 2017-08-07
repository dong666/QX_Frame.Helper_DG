using QX_Frame.Helper_DG;
using QX_Frame.Helper_DG.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Test.ConsoleApp1.NETFramework461.Config;
using QX_Frame.Helper_DG.Service;
using QX_Frame.Helper_DG.Log;
using System.Threading;
using ServiceStack.Redis;
using System.Linq.Expressions;
using System.Reflection;
using QX_Frame.Helper_DG.Bantina;

namespace Test.ConsoleApp1.NETFramework461
{
    class Program
    {
        static void Main(string[] args)
        {
            //new ConfigBootStrap();//BootStrap

            //---------

            TB_People people1 = new TB_People { Name = "111" };
            TB_People people2 = people1;
            people1.Name = "44";


            string str1 = "csacsacascsa";
            string str2 = "1233";

            Console.WriteLine(people1.GetHashCode());
            Console.WriteLine(people2.GetHashCode());



            //using (DB_QX_Frame_Test test = new DB_QX_Frame_Test())
            //{
            //    List<TB_People> peopleList = test.QueryEntitiesPaging<TB_People,string>(1,5,t=>t.Name, t => t.Name.StartsWith("li"),out int count,true);
            //    foreach (var item in peopleList)
            //    {
            //    Console.WriteLine(item.Uid+ " "+ item.Name);
            //    }
            //    Console.WriteLine(count);
            //}




            //---------

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }


    public class DB_QX_Frame_Test:Bantina
    {
        public DB_QX_Frame_Test() : base("data source=.;initial catalog=DB_QX_Frame_Test;persist security info=True;user id=Sa;password=Sa123456;MultipleActiveResultSets=True;App=EntityFramework") { }
       
    }

    [TableAttribute(TableName = "TB_People")]
    public class TB_People
    {
        [KeyAttribute]
        public Guid Uid { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public int Age { get; set; }
        [Column]
        public int ClassId { get; set; }

    }

}