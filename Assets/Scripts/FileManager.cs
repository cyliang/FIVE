using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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

    public enum FileBrowserStatus {
        Closed, ProjectPath, File
    }
    private FileBrowserStatus _fileBrowserStatus;
    public FileBrowserStatus fileBrowserStatus {
        get {
            return _fileBrowserStatus;
        }
        set {
            _fileBrowserStatus = value;
            switch (value) {
                case FileBrowserStatus.Closed:
                    FileBrowserCanvas.gameObject.SetActive(false);
                    break;
                case FileBrowserStatus.ProjectPath:
                    FileBrowserCanvas.gameObject.SetActive(true);
                    initBrowserUntilRoot(projectPathInfo);
                    drawBrowser(true);
                    break;
                case FileBrowserStatus.File:
                    FileBrowserCanvas.gameObject.SetActive(true);
                    break;
            }
        }
    }

    private static FileManager instance;

    private DirectoryInfo projectPathInfo;

    private class DirStatus {
        public int depth;
        public bool expanded;
        public DirectoryInfo dirInfo;
    }
    private TreeNode<DirStatus> fileBrowserRoot = new TreeNode<DirStatus>(new DirStatus());

	void Start() {
		instance = this;
        projectPath = Directory.GetCurrentDirectory();
        projectPathInfo = new DirectoryInfo(projectPath);

        MenuController.addBtn("Change project path", () => {
            fileBrowserStatus = FileBrowserStatus.ProjectPath;
        });
        btnCancel.onClick.AddListener(() => {
            fileBrowserStatus = FileBrowserStatus.Closed;
        });
        btnConfirm.onClick.AddListener(() => {
            /* TODO */
        });
        initBrowserUntilRoot(projectPathInfo);
        drawBrowser(true);
    }

    void initBrowserUntilRoot(DirectoryInfo dir) {
        List<DirectoryInfo> parents = new List<DirectoryInfo>();
        while (dir != null) {
            parents.Add(dir);
            dir = dir.Parent;
        }
        parents.Reverse();

        TreeNode<DirStatus> d = null;
        foreach (string drive in Directory.GetLogicalDrives()) {
            var child = fileBrowserRoot.AddChild(new DirStatus {
                depth = 0,
                expanded = false,
                dirInfo = new DirectoryInfo(drive)
            });
            if (drive == parents[0].Name)
                d = child;
        }
        
        foreach (DirectoryInfo p in parents.Skip(1)) {
            d = d.AddChild(new DirStatus {
                depth = d.Value.depth + 1,
                expanded = false,
                dirInfo = p
            });
        }

        foreach (DirectoryInfo subDir in d.Value.dirInfo.GetDirectories()) {
            d.AddChild(new DirStatus {
                depth = d.Value.depth + 1,
                expanded = false,
                dirInfo = subDir
            });
        }
    }

    void drawBrowser(bool dirFile) {
        foreach (var item in fileBrowserRoot.Flatten().Skip(1)) {
            addBrowserItem(dirFile, item.dirInfo.Name, item.depth);
        }
    }

    int addBrowserItem(bool dirFile, string text, int space, int index = -1) {
        GameObject item = Instantiate(itemPrefab);
        item.transform.SetParent((dirFile ? directoryContent : fileContent).transform, false);
        item.GetComponentInChildren<Text>().text = text;
        if (index != -1)
            item.transform.SetSiblingIndex(index);

        while (space-- > 0) {
            GameObject itemSpace = Instantiate(itemSpacePrefab);
            itemSpace.transform.SetParent(item.transform, false);
            itemSpace.transform.SetAsFirstSibling();
        }

        return item.transform.GetSiblingIndex();
    }
}
