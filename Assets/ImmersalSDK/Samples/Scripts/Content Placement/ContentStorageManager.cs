/*===============================================================================
Copyright (C) 2023 Immersal - Part of Hexagon. All Rights Reserved.

This file is part of the Immersal SDK.

The Immersal SDK cannot be copied, distributed, or made available to
third-parties for commercial purposes without written permission of Immersal Ltd.

Contact sales@immersal.com for licensing requests.
===============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Immersal.AR;

namespace Immersal.Samples.ContentPlacement
{
    public class ContentStorageManager : MonoBehaviour
    {
        [HideInInspector]
        public List<MovableContent> contentList = new List<MovableContent>();

        [SerializeField]
        private GameObject m_ContentPrefab = null;
        [SerializeField]
        private Immersal.AR.ARSpace m_ARSpace;
        [SerializeField]
        public string m_Filename = "content.json";
        private Savefile m_Savefile;
        private List<Vector3> m_Positions = new List<Vector3>();
        private List<string> m_Names = new List<string>();
        private List<int> m_MapIds = new List<int>();

        private int lastLoadedMapId = -1;

        [System.Serializable]
        public struct Savefile
        {
            public List<Vector3> positions;
            public List<string> names;
            public List<int> mapIds;
        }

        public static ContentStorageManager Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null && !Application.isPlaying)
                {
                    instance = UnityEngine.Object.FindObjectOfType<ContentStorageManager>();
                }
#endif
                if (instance == null)
                {
                    Debug.LogError("No ContentStorageManager instance found. Ensure one exists in the scene.");
                }
                return instance;
            }
        }

        private static ContentStorageManager instance = null;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            if (instance != this)
            {
                Debug.LogError("There must be only one ContentStorageManager object in a scene.");
                UnityEngine.Object.DestroyImmediate(this);
                return;
            }

            if (m_ARSpace == null)
            {
                m_ARSpace = GameObject.FindObjectOfType<Immersal.AR.ARSpace>();
            }
        }

        private void Start()
        {
            contentList.Clear();
            //LoadContents();
        }

        public void AddContent()
        {
#if UNITY_EDITOR
            if (lastLoadedMapId == -1 || !ARSpace.mapIdToMap.ContainsKey(lastLoadedMapId))
            {
                return;
            }
            ARMap map = ARSpace.mapIdToMap[lastLoadedMapId];
            Transform cameraTransform = Camera.main.transform;
            GameObject go = Instantiate(m_ContentPrefab, cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
            go.GetComponent<MovableContent>().mapId = lastLoadedMapId;
            
#elif UNITY_ANDROID || UNITY_IOS
            var lastLocalizedMapId = ImmersalSDK.Instance.Localizer.lastLocalizedMapId;
            if (lastLocalizedMapId == -1 || !ARSpace.mapIdToMap.ContainsKey(lastLocalizedMapId))
            {
                return;
            }
            
            ARMap map = ARSpace.mapIdToMap[lastLocalizedMapId];
            
            Transform cameraTransform = Camera.main.transform;
            GameObject go = Instantiate(m_ContentPrefab, cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
            
            go.GetComponent<MovableContent>().mapId = lastLocalizedMapId;
#endif
        }

        public void DeleteAllContent()
        {
            List<MovableContent> copy = new List<MovableContent>();

            foreach (MovableContent content in contentList)
            {
                copy.Add(content);
            }

            foreach(MovableContent content in copy)
            {
                content.RemoveContent();
            }
        }

        public void SaveContents()
        {
            m_Positions.Clear();
            m_Names.Clear();
            m_MapIds.Clear();
            foreach (MovableContent content in contentList)
            {
                m_Positions.Add(content.transform.localPosition);
                m_Names.Add(content.itemName.text);
                m_MapIds.Add(content.mapId);
            }
            m_Savefile.positions = m_Positions;
            m_Savefile.names = m_Names;
            m_Savefile.mapIds = m_MapIds;

            string jsonstring = JsonUtility.ToJson(m_Savefile, true);
            string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
            File.WriteAllText(dataPath, jsonstring);
        }

        public void LoadContents()
        {
            string dataPath = Path.Combine(Application.persistentDataPath, m_Filename);
            Debug.LogFormat("Trying to load file: {0}", dataPath);

            try
            {
                Savefile loadFile = JsonUtility.FromJson<Savefile>(File.ReadAllText(dataPath));

                // foreach (Vector3 pos in loadFile.positions)
                // {
                //     GameObject go = Instantiate(m_ContentPrefab, m_ARSpace.transform);
                //     go.transform.localPosition = pos;
                // }

                for (int i = 0; i < loadFile.positions.Count; i++)
                {
                    GameObject go = Instantiate(m_ContentPrefab, m_ARSpace.transform);
                    go.transform.localPosition = loadFile.positions[i];
                    var movableContent = go.GetComponent<MovableContent>();
                    movableContent.itemName.text = loadFile.names[i];
                    movableContent.mapId = loadFile.mapIds[i];
                    movableContent.ToggleContent(false);
                }

                Debug.Log("Successfully loaded file!");
            }
            catch (FileNotFoundException e)
            {
                Debug.LogWarningFormat("{0}\n.json file for content storage not found. Created a new file!", e.Message);
                File.WriteAllText(dataPath, "");
            }
            catch (NullReferenceException err)
            {
                Debug.LogWarningFormat("{0}\n.json file for content storage not found. Created a new file!", err.Message);
                File.WriteAllText(dataPath, "");
            }
        }

        public void RepositionContents(int mapId)
        {
            ARMap map = ARSpace.mapIdToMap[mapId];
            
            foreach (var movableContent in contentList)
            {
                if (movableContent.mapId == mapId)
                {
                    movableContent.transform.SetParent(map.transform.parent,false);
                    movableContent.ToggleContent(true);
                }
            }
            
#if UNITY_EDITOR
            lastLoadedMapId = mapId;
#endif
        }

        public void HideContents(ARMap arMap)
        {
            foreach (var movableContent in contentList)
            {
                if (movableContent.mapId == arMap.mapId)
                {
                    movableContent.transform.SetParent(arMap.transform.parent,false);
                    movableContent.ToggleContent(false);
                }
            }
        }
    }
}