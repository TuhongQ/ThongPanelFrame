using ThongPanelFrame.Base;
using ThongPanelFrame.Model.Recipe;
using System.Collections.Generic;
using System.Threading;

namespace ThongPanelFrame.Model.Process
{
    public class ProcStage
    {
        public ProcStage()
        {
           
        }

        public void Load()
        {
            RecipeParam recipeParam = RecipeManager.Instance.Data;
            for (int i = 0; i < recipeParam.ADBParams.Count; i++)
            {
                if (!string.IsNullOrEmpty(recipeParam.ADBParams[i].ADBSTR))
                {
                    Global.AllResult[i] = CMDManager.Instance.ExecuteAdbCommand(recipeParam.ADBParams[i].ADBSTR);
                    Global.GlADBDescription = recipeParam.ADBParams[i].Description;
                }
                Thread.Sleep(1000);
            }
        }
    }
}