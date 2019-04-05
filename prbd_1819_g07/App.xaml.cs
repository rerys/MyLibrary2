using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static readonly string IMAGE_PATH = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../images");
        public App()
        {
#if MSSQL
            var type = DbType.MsSQL;
#else
            var type = DbType.MySQL;
#endif
            //using (var model = Model.CreateModel(type)) {
            // model.ClearDatabase();

            // model.CreateTestData();
            // }

            var test = new TestDatas(type);
            test.Run();

        }
    }
}
