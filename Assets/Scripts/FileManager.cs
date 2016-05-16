using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class FileManager {

    public class File {
        public ViewBehavior view;
        public string path, fullPath;

        public string content {
            get {
                return System.IO.File.ReadAllText(fullPath);
            }
            set {
                System.IO.File.WriteAllText(fullPath, value);
            }
        }
    }
    
    public static string projectPath;
        
    public static void loadFile(File file, string filePath) {
        file.path = filePath;
        file.fullPath = string.Format("{0}/{1}", projectPath, filePath);

        Dictionary<string, object> qry = new Dictionary<string, object>();
        Dictionary<string, string> param = new Dictionary<string, string>();
        param.Add("path", filePath);
        param.Add("content", file.content);
        qry.Add("cmd", "loadFile");
        qry.Add("params", param);

        file.view.makeQuery(qry, delegate (bool success, string _) {
            if (!success)
                Debug.LogError("Query failed.");
        });
    }

    public static void saveFile(File file) {
        Dictionary<string, object> qry = new Dictionary<string, object>();
        qry.Add("cmd", "saveFile");
        qry.Add("params", new Dictionary<string, object>());

        file.view.makeQuery(qry, delegate (bool success, string value) {
            if (!success)
                Debug.LogError("Query failed.");
            else {
                file.content = (UWKJson.Deserialize(value) as IDictionary<string, string>)["content"];
            }
        });
    }
}
