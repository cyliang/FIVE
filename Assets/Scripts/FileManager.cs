using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FileManager: MonoBehaviour {

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
			
		public void loadFile(string filePath) {
			path = filePath;
			fullPath = string.Format("{0}/{1}", instance.projectPath, filePath);

			Dictionary<string, object> qry = new Dictionary<string, object>();
			Dictionary<string, string> param = new Dictionary<string, string>();
			param.Add("path", filePath);
			param.Add("content", content);
			qry.Add("cmd", "loadFile");
			qry.Add("params", param);

			view.makeQuery(qry, delegate (bool success, string _) {
				if (!success)
					Debug.LogError("Query failed.");
			});
		}

		public void saveFile() {
			Dictionary<string, object> qry = new Dictionary<string, object>();
			qry.Add("cmd", "saveFile");
			qry.Add("params", new Dictionary<string, object>());

			view.makeQuery(qry, delegate (bool success, string value) {
				if (!success)
					Debug.LogError("Query failed.");
				else {
					content = (UWKJson.Deserialize(value) as IDictionary<string, string>)["content"];
				}
			});
		}
    }
    
    public string projectPath;

    [Header(" [File Browser]")]
    public Canvas FileBrowserCanvas;
    public Text title, resultPath;
    public GameObject directoryContent, fileContent;
    public Button btnConfirm, btnCancel;
    public GameObject itemPrefab, itemSpacePrefab;

	private static FileManager instance;

	void Start() {
		instance = this;

        MenuController.addBtn("Change project path", () => {
            openFileBrowser();
        });
        btnCancel.onClick.AddListener(() => {
            FileBrowserCanvas.gameObject.SetActive(false);
        });
        btnConfirm.onClick.AddListener(() => {
            /* TODO */
        });
	}

    void openFileBrowser() {
        FileBrowserCanvas.gameObject.SetActive(true);
    }
}
