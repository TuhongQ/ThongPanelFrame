using ThongPanelFrame.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThongPanelFrame.Model
{
    public class Global
    {
       // private static readonly Lazy<Global> _singer =
       //new Lazy<Global>(() => new Global());

       // public static Global Instance
       // { get { return _singer.Value; } }

       // private Global()
       // { }
        public static List<string>? AllResult { get; set; } = new List<string>();
        public static string? GlADBDescription { get; set; }
        public static string RecipeFolderPath => @"..\SysCfg\DeviceParams\Recipe\";
        public static string RecipeConfigFilePath => @"..\SysCfg\DeviceParams\Recipe\recipe.config";

        public static bool bUpdataList=false;
       
    }
}
