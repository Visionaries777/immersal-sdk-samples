using Immersal.Samples.ContentPlacement;
using UnityEngine;
using UnityEngine.UI;

public class ContentButtonsListener : MonoBehaviour
{
    [SerializeField] private Button addButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button uploadButton;

    private void Awake()
    {
        addButton.onClick.AddListener(AddButtonOnClick);
        deleteButton.onClick.AddListener(DeleteButtonOnClick);
        uploadButton.onClick.AddListener(UploadButtonOnClick);
    }

    private void AddButtonOnClick()
    {
        ContentStorageManager.Instance.AddContent();
    }
    
    private void DeleteButtonOnClick()
    {
        ContentStorageManager.Instance.DeleteAllContent();
    }

    private void UploadButtonOnClick()
    {
        uploadButton.interactable = false;
        ServerManager.Instance.UploadItemList(() =>
        {
            uploadButton.interactable = true;
        });
    }
}
