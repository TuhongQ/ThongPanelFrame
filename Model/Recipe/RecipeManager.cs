using ThongPanelFrame.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Linq;

namespace ThongPanelFrame.Model.Recipe
{
    public class RecipeManager
    {
        private static readonly Lazy<RecipeManager> _singer = new Lazy<RecipeManager>(() => new RecipeManager());
        public static RecipeManager Instance { get { return _singer.Value; } }
        #region 常规单例
        //public static RecipeManager Instance
        //{
        //    get
        //    {
        //        lock (_syncRoot)
        //        {
        //            if (_instance==null)
        //            {
        //                _instance = new RecipeManager();
        //                _instance.Load();
        //            }
        //            return _instance;
        //        }
        //    }
        //}
        //private static RecipeManager _instance;
        //private static readonly object _syncRoot = new object();
        #endregion
        private RecipeManager()
        {
            Load();//只调用一次
        }

        private readonly List<RecipeParam> _recipes = new List<RecipeParam>();
        private readonly object _lockRecipes = new object();
        public RecipeParam Data { get; private set; }
        public void Load()
        {
            try
            {
                lock (_lockRecipes)
                {
                    Data = null;
                    _recipes.Clear();

                    string folderPath = Global.RecipeFolderPath;

                    if (Directory.Exists(folderPath))
                    {
                        // 加载所有的Recipe文件
                        string[] files = Directory.GetFiles(folderPath, "*.xml");
                        string errFile = string.Empty;
                        foreach (string file in files)
                        {
                            try
                            {
                                RecipeParam recipe = XMLHelper.LoadObjectFromXml(file, typeof(RecipeParam)) as RecipeParam;
                                _recipes.Add(recipe);
                            }
                            catch (Exception)
                            {
                                errFile += file + "\n";
                               // ILogManagerService.Warn(GetType().Name, "Recipe文件加载失败：" + file);
                            }
                        }
                        if (errFile != string.Empty)
                            MessageBox.Show("Recipe文件加载失败：\n" + errFile, "", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // 设置使用的Recipe
                        try
                        {
                            XElement config = XElement.Load(Global.RecipeConfigFilePath);
                            string name = config.Value;
                            foreach (RecipeParam recipe in _recipes)
                            {
                                if (recipe.Name == name)
                                {
                                    Data = recipe;
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        { }

                        // 如果上面的步骤没有找到指定的Recipe
                        if (Data == null)
                        {
                            if (_recipes.Count > 0)
                            {
                                MessageBox.Show($"没有找到指定的Recipe，将自动使用:{_recipes[0].Name}", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                //ILogManagerService.Warn(GetType().Name, $"没有找到指定的Recipe，使用:{_recipes[0].Name}");

                                Data = _recipes[0];
                                XElement config = new XElement("CurrentUseRecipe", Data.Name);
                                config.Save(folderPath + "recipe.config");
                            }
                            else
                            {
                                MessageBox.Show($"Recipe为空，需要手动创建Recipe！", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                                //ILogManagerService.Warn(GetType().Name, $"Recipe为空！");
                            }
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(folderPath);

                        MessageBox.Show($"Recipe为空，需要手动创建Recipe！", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        //ILogManagerService.Warn(GetType().Name, $"Recipe为空！");
                    }
                }
                //RecipeListChanged?.Invoke(Data.Name);
            }
            catch (Exception ex)
            {
               // ILogManagerService.Warn(GetType().Name, ex.ToString());
            }
        }

        public bool Apply(RecipeParam recipe)
        {
            lock (_lockRecipes)
            {
                for (int i = 0; i < _recipes.Count; i++)
                {
                    if (recipe.Name == _recipes[i].Name)
                    {
                        _recipes[i] = recipe.Clone();
                        if (Data.Name == _recipes[i].Name)
                            Data = _recipes[i];
                        return true;
                    }
                }
                return false;
            }
        }

        public bool Save(string name)
        {
            lock (_lockRecipes)
            {
                for (int i = 0; i < _recipes.Count; i++)
                {
                    if (_recipes[i].Name == name)
                    {
                        string path = Global.RecipeFolderPath + $"{_recipes[i].Name}.xml";
                        XMLHelper.SaveObjectToXml(path, _recipes[i]);
                        return true;
                    }
                }
                return false;
            }
        }
    }
}