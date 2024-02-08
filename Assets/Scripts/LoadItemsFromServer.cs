using System.Collections;
using System.IO;
using System.Text;
using Immersal.Samples.ContentPlacement;
using UnityEngine;
using UnityEngine.Networking;

public class LoadItemsFromServer : MonoBehaviour
{
    [SerializeField] private string serverDomain = "http://192.168.1.164:12345/manifest";
    [SerializeField] private string userId = "u001";
    private string filePath;
    
    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, ContentStorageManager.Instance.m_Filename);
        StartCoroutine(GetItemListFromServer());
    }

    private IEnumerator GetItemListFromServer()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(serverDomain);
        webRequest.SetRequestHeader("UID", userId);
            
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = webRequest.downloadHandler.text;
            File.WriteAllText(filePath, jsonResponse);
            Debug.Log("Loaded server data : " + jsonResponse);
                
            ContentStorageManager.Instance.LoadContents();
        }
        else
        {
            Debug.LogError("Error: " + webRequest.error);
        }
    }

    public void UploadItemList()
    {
        StartCoroutine(UpdateItemListFromServer());
    }

    private IEnumerator UpdateItemListFromServer()
    {
        string jsonData = File.ReadAllText(filePath);
        using UnityWebRequest webRequest = UnityWebRequest.Put(serverDomain, jsonData);
        webRequest.SetRequestHeader("UID", userId);
        
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        
        yield return webRequest.SendWebRequest();
        
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload succeed");
        }
        else
        {
            Debug.LogError("Error: " + webRequest.error);
        }
    }
}
