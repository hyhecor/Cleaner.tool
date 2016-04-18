using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cleaner
{
    class Program
    {
        /*
        const string CONFIG_PATH = "cleaner.ini";
        const string CONFIG_CONTEXT =
              "\r\n" + "[CONFIG]"
            + "\r\n" + "TARGET="
            + "\r\n" + "DESTINATION="
            + "\r\n" + "DURATION=";
        */
        const string usag =
            "Cleaner.exe [TARGET] [DESTINATION] [DURATION] \n"
          + "            [TARGET]      : 백업 대상 폴더 \n"
          + "            [DESTINATION] : 백업 목적 폴더 \n"
          + "            [DURATION]    : 파일 유지 기간(일자) \n";

        static string _targetDir;
        static string _destinationDir;
        static int    _duration;

        static void Main(string[] args)
        {
            /*
            INI.iniUtil ini = new INI.iniUtil(AppDomain.CurrentDomain.BaseDirectory + CONFIG_PATH);

            _targetDir = ini.GetValue("CONFIG", "TARGET", "");
            _destinationDir = ini.GetValue("CONFIG", "DESTINATION", "");
            _duration = int.Parse(ini.GetValue("CONFIG", "DURATION", "5"));
            */

            if (args.Length != 3)
            { 
                Console.WriteLine(usag);
                return;
            }
            
            _targetDir = args[0];
            _destinationDir = args[1];
            _duration = int.Parse(args[2]);

            //Console.WriteLine("Load {0}", CONFIG_PATH);
            Console.WriteLine("TARGET {0}", _targetDir);
            Console.WriteLine("DESTINATION {0}", _destinationDir);
            Console.WriteLine("DURATION {0}", _duration);

            // 설정파일이 올바르지 않다면,
            if (_targetDir.Length == 0 || _destinationDir.Length == 0 )
            {
                /*
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(ini.GetPath()))
                {
                    writer.WriteLine(CONFIG_CONTEXT); 
                }

                ini.SetValue("CONFIG", "TARGET", "[TARGET DIR]");
                ini.SetValue("CONFIG", "DESTINATION", "[DESTINATION DIR]");
                ini.SetValue("CONFIG", "DURATION", "5");
                */

                Console.WriteLine(usag);

                return; // 프로그램 종료
            }

            List <System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            DateTime date = DateTime.Now.Date;
            date = date.AddDays(_duration * -1);

            try {

                // 파일 찾기 
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(_targetDir);
                foreach (System.IO.FileInfo file in dir.GetFiles())
                {
                    if (file.LastWriteTime.Date < date)
                        files.Add(file);
                }

                Console.WriteLine("Count files; {0}", files.Count);

                // 리스트 검사
                if (files.Count == 0)
                {
                    Console.WriteLine("No File");
                    return;
                }

                // 파일 복사 
                foreach (System.IO.FileInfo file in files)
                {
                    string path = _destinationDir + "\\" + file.LastWriteTime.Date.ToString("yyyy-MM-dd") + "\\";

                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

                    if (di.Exists == false)
                    {
                        Console.WriteLine("Create directory; {0}", path);
                        di.Create();
                    }

                    Console.WriteLine("Copy to; {0}", path + file.Name);
                    file.CopyTo(path + file.Name, true);
                }


                // 파일 삭제
                foreach (System.IO.FileInfo file in files)
                {
                    Console.WriteLine("Delete; {0}", file.Name);
                    file.Delete();
                }

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
