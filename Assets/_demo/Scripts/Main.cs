using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Inspekly.IO;
using Inspekly.Utilities;
using KeLSnG.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Main : MonoBehaviour
{
    void Start()
    {
        this.Test();
    }

    async void Test()
    {
        // copy files
        /*
        async void copyFiles()
        {
            var streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "_demo/immersal/m001");

            var directory = Path.Combine(Application.persistentDataPath, "immersal/m001");
            var dirInfo = new DirectoryInfo(directory);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            const string XMLFileName = "vz_corrider_000.bytes";
            var xmlFilePath = Path.Combine(directory, XMLFileName);
            var isXMLFileDone = File.Exists(xmlFilePath);

            const string DATFileName = "vz_pantry_000.bytes";
            var datFilePath = Path.Combine(directory, DATFileName);
            var isDATFileDone = File.Exists(datFilePath);

            void checkAllFiles()
            {
                // Debug.Log($"checkAllFiles ::: isXMLFileDone[{isXMLFileDone}] / isDATFileDone[{isDATFileDone}]");

                if (isXMLFileDone && isDATFileDone)
                {
                    // this.UploadMapExample();
                }
            }

            IEnumerator getFile(string url, Action<UnityWebRequest, bool> onComplete = null)
            {
                // Debug.Log("GetFile " + url);

                UnityWebRequest www = new UnityWebRequest(new Uri(url))
                {
                    timeout = 10,
                    downloadHandler = new DownloadHandlerBuffer()
                };
                www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return null;
                }

                if (string.IsNullOrEmpty(www.error))
                {
                    onComplete?.Invoke(www, true);
                }
                else
                {
                    onComplete?.Invoke(www, false);
                }
            }

            if (!isXMLFileDone)
            {
                this.StartCoroutine(getFile(Path.Combine(streamingAssetsPath, XMLFileName), async (www, isSuccess) =>
                {
                    if (isSuccess)
                    {
                        await File.WriteAllBytesAsync(xmlFilePath, www.downloadHandler.data);
                        isXMLFileDone = File.Exists(xmlFilePath);
                        checkAllFiles();
                    }
                    else
                    {
                        Debug.LogError("failed to download xml file from streaming assets");
                    }
                }));
            }

            if (!isDATFileDone)
            {
                this.StartCoroutine(getFile(Path.Combine(streamingAssetsPath, DATFileName), async (www, isSuccess) =>
                {
                    if (isSuccess)
                    {
                        await File.WriteAllBytesAsync(datFilePath, www.downloadHandler.data);
                        isDATFileDone = File.Exists(datFilePath);
                        checkAllFiles();
                    }
                    else
                    {
                        Debug.LogError("failed to download dat file from streaming assets");
                    }
                }));
            }

            checkAllFiles();
        }
        */

        this.Example();
    }

    async Task<bool> UploadMapExample(string UID)
    {
        var tcs = new TaskCompletionSource<bool>();

        // test map name
        var mapName = "Map001";

        // test two files
        var mapFiles = new List<Inspekly.API.Immersal.Entity>();
        var file1Name = "vz_corrider_000.bytes";
        var file1Path = $"{Application.streamingAssetsPath}/immersal/m001/{file1Name}";
        if (File.Exists(file1Path))
        {
            var dataBytes = File.ReadAllBytes(file1Path);
            var mapFile1 = new Inspekly.API.Immersal.Entity()
            {
                refID = "ref001",
                size = dataBytes.Length,
                dataBytes = dataBytes,
                contentType = "application/octet-stream",
                extension = "bytes",
                fileName = file1Name,
            };

            using var md5 = MD5.Create();
            var byteHashedPassword = md5.ComputeHash(dataBytes = File.ReadAllBytes(file1Path));
            mapFile1.md5 = Convert.ToBase64String(byteHashedPassword);

            mapFiles.Add(mapFile1);
        }
        else Debug.LogError($"File 1 [{file1Path}] not exist");

        var file2Name = "vz_pantry_000.bytes";
        var file2Path = $"{Application.streamingAssetsPath}/immersal/m001/{file2Name}";
        if (File.Exists(file2Path))
        {
            var dataBytes = File.ReadAllBytes(file2Path);
            var mapFile2 = new Inspekly.API.Immersal.Entity()
            {
                refID = "ref002",
                size = dataBytes.Length,
                dataBytes = dataBytes,
                contentType = "application/octet-stream",
                extension = "bytes",
                fileName = file2Name,
            };

            using var md5 = MD5.Create();
            var byteHashedPassword = md5.ComputeHash(dataBytes = File.ReadAllBytes(file2Path));
            mapFile2.md5 = Convert.ToBase64String(byteHashedPassword);

            mapFiles.Add(mapFile2);
        }
        else Debug.LogError($"File 2 [{file2Path}] not exist");

        if (mapFiles.Count == 0)
        {
            Debug.LogError("No file to be upload");
            return tcs.TrySetResult(false);
        }

        var res = await Inspekly.API.Immersal.API.CreateMap(UID, new()
        {
            mapName = mapName,
            files = mapFiles
        });

        void uploadFiles(UnityAction<string, float> onUpdateProgress, UnityAction<bool> onUploadDone)
        {
            var isCancelled = false;

            int getCount(Inspekly.API.State state)
            {
                var reqCount = 0;
                var jLoop = res.files.Count;
                for (var j = 0; j < jLoop; j++)
                {
                    if (res.files[j].state == state)
                    {
                        reqCount++;
                    }
                }
                return reqCount;
            }

            void upload()
            {
                var iLoop = res.files.Count;
                // no files, mostly by duplicated area target
                if (iLoop == 0)
                {
                    onUploadDone(true);
                    return;
                }

                for (var i = 0; i < iLoop; i++)
                {
                    var file = res.files[i];
                    var signedURL = file.putSrc;

                    // file already handling
                    if (file.state != Inspekly.API.State.waiting)
                    {
                        Debug.Log($"fileInfo[{file.refID}] - {file.state}");
                        continue;
                    }

                    // update ref
                    file.state = Inspekly.API.State.uploading;
                    res.files[i] = file;

                    this.StartCoroutine(Web.UploadFile(signedURL, uploadHandler: new UploadHandlerRaw(file.dataBytes)
                    {
                        contentType = file.contentType
                    },
                    file.md5,
                    onUploading: (progress) =>
                    {
                        onUpdateProgress(file.refID, progress);
                        // Debug.Log($"uploading file[{file.fileName}] ({(progress * 100).ToString("0.#")})");
                    },
                    onComplete: (www, isSuccess) =>
                    {
                        var fileInfoRefIdx = res.files.FindIndex(f => f.refID == file.refID);
                        if (fileInfoRefIdx < 0)
                        {
                            return;
                        }

                        if (isSuccess)
                        {
                            // update ref
                            var fileInfoRef = res.files[fileInfoRefIdx];
                            // already canceled
                            if (fileInfoRef.state != Inspekly.API.State.cancelled)
                            {
                                fileInfoRef.state = Inspekly.API.State.done;
                                res.files[fileInfoRefIdx] = fileInfoRef;
                            }

                            var reqCount = getCount(Inspekly.API.State.uploading);
                            Debug.LogFormat($"{reqCount} req left >>> success ::: {fileInfoRef.fileName}");

                            if (reqCount <= 0)
                            {
                                // if already canceled
                                if (isCancelled)
                                {
                                    return;
                                }

                                var cancelledCount = getCount(Inspekly.API.State.cancelled);
                                // Debug.Log($"varifyFiles ::: reqCount - {areaTarget.areaTargetID} - {areaTarget.fileInfos.Count} | cancelledCount - {cancelledCount}");
                                if (cancelledCount > 0)
                                {
                                    return;
                                }

                                onUploadDone(true);
                            }
                        }
                        else
                        {
                            Debug.LogError(signedURL + "|" + www.error);
                            onUploadFailed(file);
                        }
                    }));
                }
            }

            void onUploadFailed(Inspekly.API.Immersal.Interface.PreUploadEntity file)
            {
                PopupManager.Instance.CreateUploadFileFailedPopup(
                    fileName: file.fileName,
                    file.refID,
                    onCancel: () =>
                    {
                        var fileInfoRefIdx = res.files.FindIndex(f => f.refID == file.refID);
                        if (fileInfoRefIdx < 0)
                        {
                            return;
                        }

                        // seems as cancel upload area target
                        if (!isCancelled)
                        {
                            // seems as cancel all
                            var iLoop = res.files.Count;
                            for (var i = 0; i < iLoop; i++)
                            {
                                var fileInfos = res.files[i];
                                fileInfos.state = Inspekly.API.State.cancelled;
                                res.files[i] = fileInfos;
                            }

                            // ARScannerMain.Instance.ui.localDataSetPrefabManager.OnCancelledDataset(areaTargetFiles.AreaTargetMenu.areaTargetName, areaTargetFiles.areaTargetID);
                            PopupManager.Instance.CreateAreaTargetUploadNotComplete(res.mapName);
                            isCancelled = true;
                            onUploadDone(false);
                        }
                    },
                    onRetry: () =>
                    {
                        var fileInfoRefIdx = res.files.FindIndex(f => f.refID == file.refID);
                        if (fileInfoRefIdx < 0)
                        {
                            return;
                        }

                        // update ref
                        var fileInfoRef = res.files[fileInfoRefIdx];
                        fileInfoRef.state = Inspekly.API.State.waiting;
                        res.files[fileInfoRefIdx] = fileInfoRef;

                        upload();
                    }
                );
            }

            upload();
        }


        Debug.Log($"Created map[{res.mapID}]({res.mapName})");

        uploadFiles((refID, progress) => { }, (isDone) =>
        {
            if (isDone)
            {
                Debug.Log($"Map files uploaded");
            }
            else
            {
                Debug.Log($"Map files failed");
            }

            tcs.TrySetResult(isDone);
        });

        return await tcs.Task;
    }

    async Task<string> ListMapsExample(string UID)
    {
        var mapIDs = await Inspekly.API.Immersal.API.FetchData(UID);
        var iLoop = mapIDs.childs.Count;
        for (var i = 0; i < iLoop; i++)
        {
            Debug.Log($"Map[{i}] ::: {mapIDs.childs[i]}");
        }

        return iLoop > 0 ? mapIDs.childs[0] : null;
    }

    async Task<Inspekly.API.Immersal.Interface.GetResponse> DownloadMapExample(string UID, string mapID)
    {
        Debug.Log($"Downloading map[{mapID}]");
        var mapInfo = await Inspekly.API.Immersal.API.GetMap(UID, mapID);
        Debug.Log(JsonUtility.ToJson(mapInfo));

        if (mapInfo != null)
        {
            var iLoop = mapInfo.files?.Count;
            for (var i = 0; i < iLoop; i++)
            {
                Debug.Log($"Download map[{mapID}] file[{i}] ::: {mapInfo.files[i].getSrc}");
            }
        }
        else
        {
            Debug.LogError($"Map[{mapID}] not exist");
        }

        return mapInfo;
    }

    async void Example()
    {
        // test user id
        var UID = "u001";

        await this.UploadMapExample(UID);

        var mapID = await this.ListMapsExample(UID);
        // var mapID = "M-LSBM6G0E69C217";

        if (mapID != null)
        {
            var mapInfo = await this.DownloadMapExample(UID, mapID);
            if (mapInfo != null)
            {
                // need some time to update
                await this.Timer();

                var filesCount = mapInfo.files?.Count;
                // update check based on md5, if previous files didn't exist in update request body, it will be removed
                if (filesCount > 1)
                {
                    Debug.Log($"Remove file[{mapInfo.files[0].fileName}]");
                    mapInfo.files.RemoveAt(0);
                    await Inspekly.API.Immersal.API.UpdateData(UID, mapID, JsonUtility.ToJson(mapInfo));

                    // load again
                    var mapInfo2 = await this.DownloadMapExample(UID, mapID);
                    Debug.Log($"Files changed from [{filesCount}] >> [{mapInfo2.files.Count}]");
                }

                await Inspekly.API.Immersal.API.DeleteData(UID, mapID);

                // load again
                var mapInfo3 = await this.DownloadMapExample(UID, mapID);
            }
        }
    }

    async Task<bool> Timer(float timer = 5f)
    {
        var tcs = new TaskCompletionSource<bool>();

        this.StartCoroutine(this.Timer(5f, () =>
        {
            tcs.TrySetResult(true);
        }));

        return await tcs.Task;
    }

    IEnumerator Timer(float timer, UnityAction onDone = null)
    {
        yield return new WaitForSeconds(timer);

        onDone?.Invoke();
    }
}
