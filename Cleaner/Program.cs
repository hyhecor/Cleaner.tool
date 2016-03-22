using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cleaner
{
    class Program
    {
        const string CONFIG_PATH = "cleaner.ini";
        const string CONFIG_CONTEXT =
              "\r\n" + "[CONFIG]"
            + "\r\n" + "TARGET="
            + "\r\n" + "DESTINATION="
            + "\r\n" + "DURATION=";

        static string _targetDir;
        static string _destinationDir;
        static int    _duration;

        static void Main(string[] args)
        {

            INI.iniUtil ini = new INI.iniUtil(AppDomain.CurrentDomain.BaseDirectory + CONFIG_PATH);

            _targetDir = ini.GetValue("CONFIG", "TARGET", "");
            _destinationDir = ini.GetValue("CONFIG", "DESTINATION", "");
            _duration = int.Parse(ini.GetValue("CONFIG", "DURATION", "5"));

            // 설정파일이 올바르지 않다면,
            if (_targetDir.Length == 0 || _destinationDir.Length == 0 )
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(ini.GetPath()))
                {
                    writer.WriteLine(CONFIG_CONTEXT); 
                }

                ini.SetValue("CONFIG", "TARGET", "[TARGET DIR]");
                ini.SetValue("CONFIG", "DESTINATION", "[DESTINATION DIR]");
                ini.SetValue("CONFIG", "DURATION", "5");

                return;
            }

            List <System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            DateTime date = DateTime.Now.Date;
            date = date.AddDays(-1 * _duration);

            // 파일 찾기 
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(_targetDir);
            foreach (System.IO.FileInfo file in dir.GetFiles())
            {
                if (file.LastWriteTime.Date < date)
                    files.Add(file);
            }

            // 리스트 검사
            if (files.Count == 0)
                return;


            // 파일 복사 
            foreach (System.IO.FileInfo file in files)
            {
                string path = _destinationDir + "\\" + file.LastWriteTime.Date.ToString("yyyy-MM-dd") + "\\";

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                file.CopyTo(path + file.Name, true);
            }


            // 파일 삭제
            foreach (System.IO.FileInfo file in files)
            {
                file.Delete();
            }


        }
    }
}
