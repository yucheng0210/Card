  將此段程式碼加入到DataManager.cs
 private string dialogDataListPath = "Assets/MyAssets/Data/DialogData";
 public Dictionary<string, List<Dialog>> DialogList { get; set; }
  protected override void Awake()
    {
        DialogList = new Dictionary<string, List<Dialog>>();
    }
    private void LoadData()
    {
 #region 對話列表
        foreach (string file in Directory.GetFiles(dialogDataListPath, "*.csv"))
        {
            lineData = File.ReadAllLines(file);
            List<Dialog> dialogs = new List<Dialog>();
            for (int i = 1; i < lineData.Length; i++)
            {
                string[] row = lineData[i].Split(',');
                Dialog dialog = new Dialog();
                dialog.Branch = row[0];
                dialog.Type = row[1];
                dialog.TheName = row[2];
                dialog.Order = row[3];
                dialog.Content = row[4];
                dialogs.Add(dialog);
            }
            string fileName = Path.GetFileNameWithoutExtension(file);
            DialogList.Add(fileName, dialogs);
        }
        #endregion
    }