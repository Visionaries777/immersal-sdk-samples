using KeLSnG.Extension;
using Inspekly.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Inspekly.IO
{

    public class IOManager : MonoBehaviour
    {
        [Serializable]
        protected enum FolderType
        {
            persistentDataPath,
            streamingAssetsPath
        }
        [Tooltip("Folder type")]
        [SerializeField] protected FolderType folderType;
        [Tooltip("Main folder name, Required")]
        [SerializeField] protected string folder;

        [Tooltip("Default file name when file name is not provided, Optional")]
        [SerializeField] protected string defaultFileName;

        [Tooltip("File extenstion, Optional")]
        [SerializeField] protected string fileExtension;


        protected string directory;

        protected bool IsInit { get; set; }

        public DirectoryInfo DirectoryInfo { get; private set; }

        protected virtual void Awake()
        {
            this.Init();
        }

        protected virtual void Init()
        {
            this.defaultFileName = Ext_Data.IsNotNull(this.defaultFileName) ? this.defaultFileName : null;

            switch (this.folderType)
            {
                case FolderType.streamingAssetsPath:
                    this.directory = Path.Combine(Application.streamingAssetsPath, this.folder);
                    break;

                case FolderType.persistentDataPath:
                default:
                    this.directory = Path.Combine(Application.persistentDataPath, this.folder);
                    if (!Directory.Exists(this.directory))
                    {
                        Directory.CreateDirectory(this.directory);
                    }
                    this.DirectoryInfo = new DirectoryInfo(this.directory);
                    break;
            }

            if (Ext_Data.IsNotNull(this.fileExtension))
            {
                this.fileExtension = this.fileExtension.Contains(".") ? "" : "." + this.fileExtension;
            }
            else
            {
                this.fileExtension = "";
            }

            this.IsInit = true;
        }

        public bool IsFileExists(string fileName = null)
        {
            return File.Exists(this.GetFilePath(fileName));
        }

        public string GetFilePath(string fileName)
        {
            if (!this.IsInit)
            {
                throw new Exception($"{this.name} not initialize");
            }

            var isFileNameValid = Ext_Data.IsNotNull(fileName);
            if (!isFileNameValid && this.defaultFileName == null)
            {
                throw new Exception("File name can't be empty");
            }
            if (!isFileNameValid)
            {
                fileName = this.defaultFileName;
            }

            return Path.Combine(this.directory, fileName, this.fileExtension);
        }

        public void Save(byte[] data, string fileName = null)
        {
            if (this.folderType == FolderType.streamingAssetsPath)
            {
                throw new Exception("Streaming asset folder are read only");
            }

            File.WriteAllBytes(this.GetFilePath(fileName), data);
        }

        public async Task<bool> Save(System.Object data, string fileName = null)
        {
            if (this.folderType == FolderType.streamingAssetsPath)
            {
                throw new Exception("Streaming asset folder are read only");
            }

            using (var sw = new StreamWriter(this.GetFilePath(fileName)))
            {
                var fileTxt = JsonUtility.ToJson(data);
                await sw.WriteAsync(fileTxt);
                sw.Close();
            }

            return true;
        }

        public async Task<string> LoadText(string fileName = null)
        {
            var tcs = new TaskCompletionSource<string>();

            if (this.folderType == FolderType.streamingAssetsPath)
            {
                this.StartCoroutine(Web.GetFile(this.GetFilePath(fileName), null, onComplete: (www, isSuccess) =>
                {
                    if (isSuccess)
                    {
                        tcs.SetResult(www.downloadHandler.text);
                    }
                    else
                    {
                        throw new Exception("unable to fetch file");
                    }
                }));
            }
            else
            {
                using (var sr = new StreamReader(this.GetFilePath(fileName)))
                {
                    var fileTxt = await sr.ReadToEndAsync();
                    sr.Close();

                    tcs.SetResult(fileTxt);
                }
            }

            return await tcs.Task;
        }

        public async Task<byte[]> LoadBytes(string fileName = null)
        {
            return Convert.FromBase64String(await this.LoadText(fileName));
        }

        public void Delete(string fileName = null)
        {
            if (this.folderType == FolderType.streamingAssetsPath)
            {
                throw new Exception("Streaming asset folder are read only");
            }

            var filePath = this.GetFilePath(fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public FileInfo FileInfo(string fileName = null)
        {
            return new FileInfo(this.GetFilePath(fileName));
        }
    }
}
