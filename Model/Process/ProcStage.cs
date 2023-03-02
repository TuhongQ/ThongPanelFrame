using ThongPanelFrame.Base;
using ThongPanelFrame.Model.Recipe;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System;

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
                    double dbTemp = ((i + 1) * 100) / (recipeParam.ADBParams.Count);
                    for (double j = Machine.Instance.MachineProcess; j < dbTemp; j++)
                    {
                        Machine.Instance.MachineProcess++;
                        Thread.Sleep(30);
                    }
                    Global.AllResult[i] = CMDManager.Instance.ExecuteAdbCommand(recipeParam.ADBParams[i].ADBSTR);
                    Thread.Sleep(100);
                }
            }
        }
    }
}