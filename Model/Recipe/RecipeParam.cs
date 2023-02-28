using ThongPanelFrame.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThongPanelFrame.Model.Recipe
{
    public class RecipeParam
    {
        public string? Name { get; set; } = "";
        public List<ADBParam>? ADBParams { get; set; } = new List<ADBParam>();

        public RecipeParam Clone()
        {
            string xml = XmlConvertor.ObjectToXml(this, false);
            RecipeParam recipe = XmlConvertor.XmlToObject(xml, GetType()) as RecipeParam;
            return recipe;
        }

    }
    public class ADBParam
    {
        public string? Description { get; set; } 
        public string? ADBSTR { get; set; }
    } 

}
