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
	private Camera mainCam;

    [Header(" [File Browser]")]
    public Canvas FileBrowserCanvas;
    public Text title, resultPath;
    public GameObject directoryContent, fileContent;
    public Button btnConfirm, btnCancel;
    public GameObject itemDirPrefab, itemFilePrefab, itemSpacePrefab;

    public enum FileBrowserStatus {
        Closed, ProjectPath, File
    }
    private FileBrowserStatus _fileBrowserStatus;
    public static FileBrowserStatus fileBrowserStatus {
        get {
            return instance._fileBrowserStatus;
        }
        set {
			instance._fileBrowserStatus = value;
            switch (value) {
                case FileBrowserStatus.Closed:
					instance.showBrowser = false;
                    break;
                case FileBrowserStatus.ProjectPath:
					instance.showBrowser = true;
                    instance.initBrowserUntilRoot(instance.projectPathInfo);
                    break;
                case FileBrowserStatus.File:
					instance.showBrowser = true;
                    instance.initBrowser();
                    break;
            }
        }
    }
	private bool showBrowser {
		set {
			if (value)
				transform.eulerAngles = new Vector3(0f, mainCam.transform.eulerAngles.y, 0f);
			FileBrowserCanvas.gameObject.SetActive (value);
		}
	}
	public delegate void FileBrowserCallback(string fileName);
	public static FileBrowserCallback fileBrowserCallback { get; set; }

    private static FileManager instance;

    private DirectoryInfo projectPathInfo;

    private class DirStatus {
        public int depth;
        public bool expanded;
        public TreeNode<DirStatus> treeNode;
        public GameObject gameObject;
        public DirectoryInfo dirInfo;
    }
    private TreeNode<DirStatus> dirRoot = new TreeNode<DirStatus>(new DirStatus());
    private Dictionary<GameObject, DirStatus> dirDictionary = new Dictionary<GameObject, DirStatus>();
    private FileInfo[] fileList;
    private Dictionary<GameObject, FileInfo> fileDictionary = new Dictionary<GameObject, FileInfo>();

	void Start() {
		instance = this;
        projectPath = Directory.GetCurrentDirectory();
        projectPathInfo = new DirectoryInfo(projectPath);
        mainCam = Camera.main;

        MenuController.addBtn("Change project path", () => {
            fileBrowserStatus = FileBrowserStatus.ProjectPath;
        });
        btnCancel.onClick.AddListener(() => {
            fileBrowserStatus = FileBrowserStatus.Closed;
        });
        btnConfirm.onClick.AddListener(() => {
            if (fileBrowserStatus == FileBrowserStatus.ProjectPath)
                projectPath = resultPath.text;
			else if (fileBrowserCallback != null) {
				fileBrowserCallback(resultPath.text);
				fileBrowserCallback = null;
            }
            fileBrowserStatus = FileBrowserStatus.Closed;
        });

        fileBrowserStatus = FileBrowserStatus.ProjectPath;
    }

    void initBrowser() {
        dirRoot.RemoveAllChildren();
        DirStatus d = new DirStatus {
            depth = 0,
            expanded = true,
            dirInfo = new DirectoryInfo(projectPath)
        };
        d.treeNode = dirRoot.AddChild(d);

        foreach (DirectoryInfo subDir in d.dirInfo.GetDirectories()) {
            var ds = new DirStatus {
                depth = d.depth + 1,
                expanded = false,
                dirInfo = subDir
            };
            ds.treeNode = d.treeNode.AddChild(ds);
        }

        fileList = d.dirInfo.GetFiles();
        resultPath.text = "";
        drawBrowser();
    }

    void initBrowserUntilRoot(DirectoryInfo dir) {
        dirRoot.RemoveAllChildren();
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
            var childNode = dirRoot.AddChild(child);
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

        fileList = null;
        resultPath.text = projectPath;
        drawBrowser();
    }

    void drawBrowser() {
        foreach (var obj in dirDictionary.Keys.Concat(fileDictionary.Keys)) {
            Destroy(obj);
        }
        dirDictionary.Clear();
        fileDictionary.Clear();
        foreach (var item in dirRoot.Flatten().Skip(1)) {
            item.gameObject = addBrowserDirItem(item.dirInfo.Name, item.depth);
            dirDictionary.Add(item.gameObject, item);
        }

        if (fileList == null)
            return;
        foreach (var item in fileList) {
            fileDictionary.Add(addBrowserFileItem(item.Name, item.LastWriteTime), item);
        }
    }

    GameObject addBrowserDirItem(string text, int space, int index = -1) {
        GameObject item = Instantiate(itemDirPrefab);
        item.transform.SetParent(directoryContent.transform, false);
        item.GetComponentInChildren<Text>().text = text;
        if (index != -1)
            item.transform.SetSiblingIndex(index);

        while (space-- > 0) {
            GameObject itemSpace = Instantiate(itemSpacePrefab);
            itemSpace.transform.SetParent(item.transform, false);
            itemSpace.transform.SetAsFirstSibling();
        }

        item.GetComponent<Button>().onClick.AddListener(() => OnDirItemClick(item));

        return item;
    }

    GameObject addBrowserFileItem(string name, System.DateTime updateTime) {
        GameObject item = Instantiate(itemFilePrefab);
        item.transform.SetParent(fileContent.transform, false);
        var texts = item.GetComponentsInChildren<Text>();
        texts[0].text = name;
        texts[1].text = updateTime.ToString("yyyy/MM/dd HH:mm:ss");

        item.GetComponent<Button>().onClick.AddListener(() => OnFileItemClick(item));

        return item;
    }

    public void OnDirItemClick(GameObject item) {
        DirStatus selectedDir;
        if (item == null || !dirDictionary.TryGetValue(item, out selectedDir))
            return;

        if (selectedDir.expanded) {
            selectedDir.treeNode.RemoveAllChildren();
            selectedDir.expanded = false;
        } else {
            foreach (DirectoryInfo subDir in selectedDir.dirInfo.GetDirectories()) {
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
            fileList = fileBrowserStatus == FileBrowserStatus.File ? selectedDir.dirInfo.GetFiles() : null;
        }

        drawBrowser();
        changeBtnColor(selectedDir.gameObject.GetComponent<Button>(), BtnState.Highlighted);
        resultPath.text = fileBrowserStatus == FileBrowserStatus.File ? selectedDir.dirInfo.FullName.Substring(projectPath.Length).TrimStart(@"/\".ToCharArray()) : selectedDir.dirInfo.FullName;
    }

    public void OnFileItemClick(GameObject item) {
        FileInfo selectedFile;
        if (item == null || !fileDictionary.TryGetValue(item, out selectedFile))
            return;
        
        resultPath.text = selectedFile.FullName.Substring(projectPath.Length + 1);
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
