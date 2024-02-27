using System;
using System.Collections;
using System.IO;
using System.Text;
using Immersal.Samples.ContentPlacement;
using Immersal.Samples.Mapping;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    public string userId = "u001";
    private string filePath;

    private bool serverItemIsLoad;
    private Coroutine getItemListFromServerCoroutine;
    
    public static ServerManager Instance
    {
        get
        {
#if UNITY_EDITOR
            if (instance == null && !Application.isPlaying)
            {
                instance = FindObjectOfType<ServerManager>();
            }
#endif
            if (instance == null)
            {
                Debug.LogError("No LoadItemsFromServer instance found. Ensure one exists in the scene.");
            }
            return instance;
        }
    }

    private static ServerManager instance = null;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (instance != this)
        {
            Debug.LogError("There must be only one LoadItemsFromServer object in a scene.");
            DestroyImmediate(this);
        }
    }
    
    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, ContentStorageManager.Instance.m_Filename);
        //StartCoroutine(GetItemListFromServer());
    }

    public void GetItemsFromServer()
    {
        if (getItemListFromServerCoroutine != null)
        {
            StopCoroutine(getItemListFromServerCoroutine);
            getItemListFromServerCoroutine = null;
        }
        
        if (!serverItemIsLoad)
        {
            getItemListFromServerCoroutine = StartCoroutine(GetItemListFromServer());
        }
    }

    private IEnumerator GetItemListFromServer()
    {
        var itemUrl = Path.Combine(PlayerPrefs.GetString("serverDomain"), "json?fileName=content");
        
        using UnityWebRequest webRequest = UnityWebRequest.Get(itemUrl);
        webRequest.SetRequestHeader("uid", userId);
            
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = webRequest.downloadHandler.text;
            File.WriteAllText(filePath, jsonResponse);
            //Debug.Log("Loaded server data : " + jsonResponse);
            NotificationManager.Instance.GenerateSuccess("Downloaded Contents!");

            serverItemIsLoad = true;
                
            ContentStorageManager.Instance.LoadContents();
        }
        else
        {
            Debug.LogError("Error: " + webRequest.error);
            NotificationManager.Instance.GenerateError("Download Contents Failed!");
        }
    }

    public void UploadItemList(Action<bool> uploaded)
    {
        StartCoroutine(UpdateItemListFromServer(uploaded));
    }

    private IEnumerator UpdateItemListFromServer(Action<bool> uploaded)
    {
        var itemUrl = Path.Combine(PlayerPrefs.GetString("serverDomain"), "json?fileName=content");
        
        string jsonData = File.ReadAllText(filePath);
        using UnityWebRequest webRequest = UnityWebRequest.Put(itemUrl, jsonData);
        webRequest.SetRequestHeader("uid", userId);
        
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        
        yield return webRequest.SendWebRequest();
        
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload succeed");
            uploaded.Invoke(true);
        }
        else
        {
            Debug.LogError("Error: " + webRequest.error);
            uploaded.Invoke(false);
        }
    }
}
