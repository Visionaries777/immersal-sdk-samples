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
    public enum ContentType
    {
        Checklist,
        Diamond,
        Dashboard,
        Firework,
        Video
    }
    
    public class ContentStorageManager : MonoBehaviour
    {
        [HideInInspector]
        public List<MovableContent> contentList = new List<MovableContent>();

        [SerializeField]
        private List<GameObject> m_ContentPrefab = null;
        [SerializeField]
        private Immersal.AR.ARSpace m_ARSpace;
        [SerializeField]
        public string m_Filename = "content.json";
        private Savefile m_Savefile;
        private List<Vector3> m_Positions = new List<Vector3>();
        private List<string> m_Names = new List<string>();
        private List<int> m_MapIds = new List<int>();
        private List<ContentType> m_Types = new List<ContentType>();

        private int lastLoadedMapId = -1;

        [System.Serializable]
        public struct Savefile
        {
            public List<Vector3> positions;
            public List<string> names;
            public List<int> mapIds;
            public List<ContentType> types;
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

        public void AddContent(ContentType type)
        {
#if UNITY_EDITOR
            if (lastLoadedMapId == -1 || !ARSpace.mapIdToMap.ContainsKey(lastLoadedMapId))
            {
                return;
            }
            ARMap map = ARSpace.mapIdToMap[lastLoadedMapId];
            Transform cameraTransform = Camera.main.transform;
            GameObject go;
            switch (type)
            {
                case ContentType.Checklist:
                    go = Instantiate(m_ContentPrefab[0], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                case ContentType.Diamond:
                    go = Instantiate(m_ContentPrefab[1], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                case ContentType.Dashboard:
                    go = Instantiate(m_ContentPrefab[2], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                case ContentType.Firework:
                    go = Instantiate(m_ContentPrefab[3], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                case ContentType.Video:
                    go = Instantiate(m_ContentPrefab[4], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var movableContent = go.GetComponent<MovableContent>();
            movableContent.mapId = lastLoadedMapId;
            movableContent.type = type;

#elif UNITY_ANDROID || UNITY_IOS
            var lastLocalizedMapId = ImmersalSDK.Instance.Localizer.lastLocalizedMapId;
            if (lastLocalizedMapId == -1 || !ARSpace.mapIdToMap.ContainsKey(lastLocalizedMapId))
            {
                return;
            }
            
            ARMap map = ARSpace.mapIdToMap[lastLocalizedMapId];
            
            Transform cameraTransform = Camera.main.transform;
            GameObject go;
            switch (type)
            {
                case ContentType.Checklist:
                    go = Instantiate(m_ContentPrefab[0], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                case ContentType.Diamond:
                    go = Instantiate(m_ContentPrefab[1], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                case ContentType.Dashboard:
                    go = Instantiate(m_ContentPrefab[2], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                case ContentType.Firework:
                    go = Instantiate(m_ContentPrefab[3], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                case ContentType.Video:
                    go = Instantiate(m_ContentPrefab[4], cameraTransform.position + cameraTransform.forward, Quaternion.identity, map.transform);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var movableContent = go.GetComponent<MovableContent>();
            movableContent.mapId = lastLocalizedMapId;
            movableContent.type = type;
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
            m_Types.Clear();
            foreach (MovableContent content in contentList)
            {
                m_Positions.Add(content.transform.localPosition);
                m_Names.Add(content.ItemNameInputField != null ? content.ItemNameInputField.text : "null");
                m_MapIds.Add(content.mapId);
                m_Types.Add(content.type);
            }
            m_Savefile.positions = m_Positions;
            m_Savefile.names = m_Names;
            m_Savefile.mapIds = m_MapIds;
            m_Savefile.types = m_Types;

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
                    GameObject go;
                    switch (loadFile.types[i])
                    {
                        case ContentType.Checklist:
                            go = Instantiate(m_ContentPrefab[0], m_ARSpace.transform);
                            break;
                        case ContentType.Diamond:
                            go = Instantiate(m_ContentPrefab[1], m_ARSpace.transform);
                            break;
                        case ContentType.Dashboard:
                            go = Instantiate(m_ContentPrefab[2], m_ARSpace.transform);
                            break;
                        case ContentType.Firework:
                            go = Instantiate(m_ContentPrefab[3], m_ARSpace.transform);
                            break;
                        case ContentType.Video:
                            go = Instantiate(m_ContentPrefab[4], m_ARSpace.transform);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    go.transform.localPosition = loadFile.positions[i];
                    var movableContent = go.GetComponent<MovableContent>();
                    if (movableContent.ItemNameInputField != null)
                    {
                        movableContent.ItemNameInputField.text = loadFile.names[i];
                    }
                    movableContent.mapId = loadFile.mapIds[i];
                    movableContent.type = loadFile.types[i];
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