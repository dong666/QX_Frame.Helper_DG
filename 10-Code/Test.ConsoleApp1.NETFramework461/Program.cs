using QX_Frame.Helper_DG;
using QX_Frame.Helper_DG.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.ConsoleApp1.NETFramework461
{
    class Program
    {

        static void Main(string[] args)
        {
            new ConfigBootStrap();//BootStrap

            //---------

            //string content = "123";
            //string sIV = "33111111";
            //string key = Encrypt_Helper_DG.MD5_Encrypt("123").Substring(0,24);

            //string encString = Encrypt_Helper_DG.DES3_Encrypt(content, key, sIV);

            //Console.WriteLine(encString);

            //Console.WriteLine("\n------------");

            //Console.WriteLine(Encrypt_Helper_DG.DES3_Decrypt(encString, key, sIV));

            string source = "123456eeasdasdas";
            string encodeString = Encrypt_Helper_DG.Base64_Encode(source);
            string decodeString = Encrypt_Helper_DG.Base64_Decode(encodeString);
            Console.WriteLine(encodeString);
            Console.WriteLine(decodeString);




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
