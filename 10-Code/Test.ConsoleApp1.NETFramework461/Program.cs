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

namespace Test.ConsoleApp1.NETFramework461
{
    class Program
    {
        static void Main(string[] args)
        {
            new ConfigBootStrap();//BootStrap

            //---------


            Log_MQ_Service log = new Log_MQ_Service();

            while (true)
            {
                Console.WriteLine("write to mq ...");
                log.Log(LogType.error, "log test");
                Thread.Sleep(10000);
            }


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