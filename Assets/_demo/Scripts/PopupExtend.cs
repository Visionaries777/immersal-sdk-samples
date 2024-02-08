using KeLSnG.UI;
using UnityEngine.Events;
using static KeLSnG.UI.PopupManager;

public static class PopupExtend
{
    public static void CreateAreaTargetUploadNotComplete(this PopupManager self, string areaTargetName)
    {
        self.CreatePopup(new PopupConfig(
            new PopupEvent()
            {
                option = "OK"
            }
        )
        {
            title = "ERROR",
            noticeStr = $"Area target [{areaTargetName}] upload not complete"
        });
    }

    public static void CreateUploadFileFailedPopup(this PopupManager self, string fileName, string logID, UnityAction onCancel, UnityAction onRetry)
    {
        self.CreatePopup(new PopupConfig(
            new PopupEvent()
            {
                option = "Cancel",
                onClick = onCancel,
            },
            new PopupEvent()
            {
                option = "Retry",
                onClick = onRetry,
            }
        )
        {
            title = "ERROR",
            noticeStr = $"Upload failed [{fileName}]",
        });
    }
}