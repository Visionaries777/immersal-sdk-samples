using System.Collections;
using System.IO;
using System.Text;
using Immersal.Samples.ContentPlacement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadItemsFromServer : MonoBehaviour
{
    private string serverDomain = "http://192.168.1.169:12345/manifest";
    [SerializeField] private string userId = "u001";
    private string filePath;

    [SerializeField] private Button uploadButton;
    
    private void Start()
    {
        uploadButton.interactable = false;
        
        filePath = Path.Combine(Application.persistentDataPath, ContentStorageManager.Instance.m_Filename);
        StartCoroutine(GetItemListFromServer());
    }

    private IEnumerator GetItemListFromServer()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(serverDomain);
        webRequest.SetRequestHeader("UID", userId);
            
        yield return webRequest.SendWebRequest();
        
        uploadButton.interactable = true;

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
        uploadButton.interactable = false;
        
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
        
        uploadButton.interactable = true;
        
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
