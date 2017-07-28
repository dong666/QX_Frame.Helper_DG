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

namespace Test.ConsoleApp1.NETFramework461
{
    class Program
    {
        static void Main(string[] args)
        {
            new ConfigBootStrap();//BootStrap

            //---------

            Console.WriteLine(Cache_Helper_DG.CacheCount);
            Cache_Helper_DG.Cache_Add("1", "111");
            Cache_Helper_DG.Cache_Add("2", "222");
            Cache_Helper_DG.Cache_Add("3", "333");
            Cache_Helper_DG.Cache_Add("4", "444");
            Cache_Helper_DG.Cache_Add("5", "555");
            Cache_Helper_DG.Cache_Add("6", "666");
            Cache_Helper_DG.Cache_Add("7", "777");
            Cache_Helper_DG.Cache_Add("8", "888");
            Cache_Helper_DG.Cache_Add("9", "999");

            Cache_Helper_DG.Cache_Add("a", "aaa");
            Cache_Helper_DG.Cache_Add("b", "bbb");
            Cache_Helper_DG.Cache_Add("c", "ccc");

            Console.WriteLine(Cache_Helper_DG.CacheCount);





            //---------

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
    public class Student
    {
        public int StuId { get; set; }
        public string StuName { get; set; }

        public void SetStudentInfo() => Console.WriteLine("...stu info ...");

        public Student GetStudentInfo() => new Student { StuId = 1, StuName = "zhangsan" };
    }
}