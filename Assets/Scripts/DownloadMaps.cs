using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Immersal.AR;
using Immersal.Samples.ContentPlacement;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class Map
{
    public string fileName;
    public int mapId;
    public string mapName;
}

public class DownloadMaps : MonoBehaviour
{
    [SerializeField] private Toggle mapTogglePrefab;

    private void Start()
    {
        StartCoroutine(ParseJsonFileFromUrl());
    }

    private IEnumerator ParseJsonFileFromUrl()
    {
        var request = UnityWebRequest.Get("https://tools.inspekly.com/alpha/ICC01.json");
        yield return request.SendWebRequest();
        if (request.isDone && request.result != UnityWebRequest.Result.ConnectionError && request.responseCode == 200)
        {
            var json = request.downloadHandler.text;
            var maps = JsonConvert.DeserializeObject<List<Map>>(json);

            foreach (var map in maps)
            {
                var toggle = Instantiate(mapTogglePrefab, transform);
                toggle.onValueChanged.AddListener(value => LoadAndInstantiateARMap(map.fileName, map.mapId, toggle));
                var textMeshPro = toggle.GetComponentInChildren<TextMeshProUGUI>();
                textMeshPro.text = map.mapName;
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file: " + request.error);
        }
    }

    private async void LoadAndInstantiateARMap(string fileName, int mapId, Selectable toggle)
    {
        if (ARSpace.mapIdToMap.ContainsKey(mapId))
        {
            ARMap arMap = ARSpace.mapIdToMap[mapId];
            ContentStorageManager.Instance.HideContents(arMap);
            arMap.FreeMap(true);
            return;
        }
        
        var filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            GameObject go = new GameObject(string.Format("AR Map {0}-{1}", mapId, fileName));
            
            ARMap arMap = go.AddComponent<ARMap>();
            arMap.pointColor = ARMap.pointCloudColors[UnityEngine.Random.Range(0, ARMap.pointCloudColors.Length)];
            arMap.renderMode = ARMap.RenderMode.EditorAndRuntime;
            
            var mapData = await File.ReadAllBytesAsync(filePath);
            await arMap.LoadMap(mapData, mapId);
            
            ContentStorageManager.Instance.RepositionContents(mapId);
        }
        else
        {
            toggle.interactable = false;
            StartCoroutine(DownloadMapFile(fileName, mapId, toggle));
        }
    }

    private IEnumerator DownloadMapFile(string fileName, int mapId, Selectable toggle)
    {
        var url = "https://tools.inspekly.com/alpha/" + fileName;
        UnityWebRequest request = new UnityWebRequest(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var data = request.downloadHandler.data;
            File.WriteAllBytes(Application.persistentDataPath + "/" + fileName, data);
            Debug.Log("Downloaded map: " + fileName);
            toggle.interactable = true;
            LoadAndInstantiateARMap(fileName, mapId, toggle);
        }
        else
        {
            Debug.LogError("Download map failed: " + request.error);
        }
    }
}
