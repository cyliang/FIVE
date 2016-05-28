using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
        public TreeNode<DirStatus> treeNode;
        public GameObject gameObject;
        public DirectoryInfo dirInfo;
    }
    private TreeNode<DirStatus> fileBrowserRoot = new TreeNode<DirStatus>(new DirStatus());
    private Dictionary<GameObject, DirStatus> dirDictionary = new Dictionary<GameObject, DirStatus>();

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
            projectPath = resultPath.text;
            fileBrowserStatus = FileBrowserStatus.Closed;
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
            var child = new DirStatus {
                depth = 0,
                expanded = false,
                dirInfo = new DirectoryInfo(drive)
            };
            var childNode = fileBrowserRoot.AddChild(child);
            child.treeNode = childNode;
            if (drive == parents[0].Name)
                d = childNode;
        }
        
        foreach (DirectoryInfo p in parents.Skip(1)) {
            var ds = new DirStatus {
                depth = d.Value.depth + 1,
                expanded = false,
                dirInfo = p
            };
            d = d.AddChild(ds);
            ds.treeNode = d;
        }
        d.Value.expanded = true;

        foreach (DirectoryInfo subDir in d.Value.dirInfo.GetDirectories()) {
            var ds = new DirStatus {
                depth = d.Value.depth + 1,
                expanded = false,
                dirInfo = subDir
            };
            ds.treeNode = d.AddChild(ds);
        }
    }

    void drawBrowser(bool dirFile) {
        if (dirFile) {
            foreach (var child in dirDictionary) {
                Destroy(child.Key);
            }
            dirDictionary.Clear();
        }

        foreach (var item in fileBrowserRoot.Flatten().Skip(1)) {
            item.gameObject = addBrowserItem(dirFile, item.dirInfo.Name, item.depth);
            dirDictionary.Add(item.gameObject, item);
        }
    }

    GameObject addBrowserItem(bool dirFile, string text, int space, int index = -1) {
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

        item.GetComponent<Button>().onClick.AddListener(() => OnItemClick(item));

        return item;
    }

    public void OnItemClick(GameObject item) {
        DirStatus selectedDir;
        if (item == null || !dirDictionary.TryGetValue(item, out selectedDir))
            return;

        if (selectedDir.expanded) {
            selectedDir.treeNode.RemoveAllChildren();
            selectedDir.expanded = false;
        } else {
            foreach (var subDir in selectedDir.dirInfo.GetDirectories()) {
                if (selectedDir.treeNode.Children.Count == 0 || selectedDir.treeNode.Children.First().Value.dirInfo.Name != subDir.Name) {
                    var newChild = new DirStatus() {
                        depth = selectedDir.depth + 1,
                        expanded = false,
                        dirInfo = subDir
                    };
                    newChild.treeNode = selectedDir.treeNode.AddChild(newChild);
                }
            }
            selectedDir.expanded = true;
        }

        drawBrowser(true);
        changeBtnColor(selectedDir.gameObject.GetComponent<Button>(), BtnState.Highlighted);
        resultPath.text = selectedDir.dirInfo.FullName;
    }

    enum BtnState {
        Normal, Highlighted, Pressed, Disabled
    }
    void changeBtnColor(Button btn, BtnState state) {
        Color targetColor = new Color(1, 1, 1, 0);
        switch (state) {
            case BtnState.Highlighted:
                targetColor = btn.colors.highlightedColor;
                break;
            case BtnState.Pressed:
                targetColor = btn.colors.pressedColor;
                break;
            case BtnState.Disabled:
                targetColor = btn.colors.disabledColor;
                break;
        }

        var originalColors = btn.colors;
        originalColors.normalColor = targetColor;
        btn.colors = originalColors;
    }
}
