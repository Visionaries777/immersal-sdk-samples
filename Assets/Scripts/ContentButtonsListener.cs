using Immersal.Samples.ContentPlacement;
using Immersal.Samples.Mapping;
using UnityEngine;
using UnityEngine.UI;

public class ContentButtonsListener : MonoBehaviour
{
    [SerializeField] private Button addButtonChecklist;
    [SerializeField] private Button addButtonDiamond;
    [SerializeField] private Button addButtonDashboard;
    [SerializeField] private Button addButtonFirework;
    [SerializeField] private Button addButtonVideo;
    [SerializeField] private Button addButtonDragon;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button uploadButton;

    private void Awake()
    {
        addButtonChecklist.onClick.AddListener(() => AddButtonOnClick(ContentType.Checklist));
        addButtonDiamond.onClick.AddListener(() => AddButtonOnClick(ContentType.Diamond));
        addButtonDashboard.onClick.AddListener(() => AddButtonOnClick(ContentType.Dashboard));
        addButtonFirework.onClick.AddListener(() => AddButtonOnClick(ContentType.Firework));
        addButtonVideo.onClick.AddListener(() => AddButtonOnClick(ContentType.Video));
        addButtonDragon.onClick.AddListener(() => AddButtonOnClick(ContentType.Dragon));
        deleteButton.onClick.AddListener(DeleteButtonOnClick);
        uploadButton.onClick.AddListener(UploadButtonOnClick);
    }

    private void AddButtonOnClick(ContentType type)
    {
        ContentStorageManager.Instance.AddContent(type);
    }
    
    private void DeleteButtonOnClick()
    {
        ContentStorageManager.Instance.DeleteAllContent();
    }

    private void UploadButtonOnClick()
    {
        uploadButton.interactable = false;
        ServerManager.Instance.UploadItemList(isSucceed =>
        {
            if (isSucceed)
            {
                NotificationManager.Instance.GenerateSuccess("Upload Succeed!");
            }
            else
            {
                NotificationManager.Instance.GenerateError("Upload Failed!");
            }
            
            uploadButton.interactable = true;
        });
    }
}
