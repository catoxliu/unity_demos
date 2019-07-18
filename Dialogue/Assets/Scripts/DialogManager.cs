using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

    public DialogWindowManager WindowManager;
    public Transform Layout;
    public Button ButtonPrefab;

    private List<SerializableDialogue> Dialogues;
    private static string DialogAssetBundleName = "dialogue_content";
    private static string UISpriteAssetBundleName = "ui_dialog_characters";

    // Use this for initialization
    void Start () {
        LoadAssetBundle();
		foreach (var dialog in Dialogues)
        {
            var btn = Instantiate<Button>(ButtonPrefab);
            btn.transform.SetParent(Layout);
            btn.transform.localScale = Vector3.one;
            btn.GetComponentInChildren<Text>().text = dialog.name;
            btn.onClick.AddListener(delegate { OnClickButton(dialog); });
        }
        WindowManager.DialogFinishAction += ShowLayout;
    }

    private void OnDestroy()
    {
        WindowManager.DialogFinishAction -= ShowLayout;
    }

    public void OnClickButton(SerializableDialogue dialog)
    {
        WindowManager.StartDialog(dialog);
        Layout.gameObject.SetActive(false);
    }

    void ShowLayout()
    {
        Layout.gameObject.SetActive(true);
    }

    void LoadAssetBundle()
    {
        string AssetBundlePath =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        Application.dataPath + "/AssetBundles/StandaloneWindows/";
#elif UNITY_ANDROID
        "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
        Application.dataPath + "/Raw/";
#else
        string.Empty;
#endif

        //should load UI sprites first due to dependency.
        var uiAssetBundle = AssetBundle.LoadFromFile(
            AssetBundlePath + UISpriteAssetBundleName);
        if (uiAssetBundle == null)
        {
            Debug.LogError("Failed to load ui asset bundle!");
            return;
        }

        var sprites = uiAssetBundle.LoadAllAssets<Sprite>();

        Dialogues = new List<SerializableDialogue>();
        var dialogAssetBundle = AssetBundle.LoadFromFile(
            AssetBundlePath + DialogAssetBundleName);
        if (dialogAssetBundle == null)
        {
            Debug.LogError("Failed to load dialog asset bundle!");
            return;
        }

        var names = dialogAssetBundle.GetAllAssetNames();
        for (int i=0, max= names.Length; i < max; i++)
        {
            Debug.Log(names[i]);
            var dialog = dialogAssetBundle.LoadAsset<SerializableDialogue>(names[i]);
            Dialogues.Add(dialog);
        }

        uiAssetBundle.Unload(false);
        dialogAssetBundle.Unload(false);
    }
}
