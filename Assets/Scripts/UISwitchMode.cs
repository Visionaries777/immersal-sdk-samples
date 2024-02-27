using Immersal.Samples.Mapping;
using TMPro;
using UnityEngine;

public class UISwitchMode : MonoBehaviour
{
    [SerializeField] private WorkspaceManager workspaceManager;
    
    [SerializeField] private GameObject workSpaceOldUI, workSpaceNewUI;
    [SerializeField] private GameObject visualizeOldUI, visualizeNewUI;

    [SerializeField] private GameObject passCode;
    [SerializeField] private TMP_InputField passCodeInputField;
    private bool debugMode;

    [SerializeField] private TMP_InputField oldUIMapName;
    [SerializeField] private TMP_InputField newUIMapName;

    private void Start()
    {
        workSpaceOldUI.SetActive(false);
        workSpaceNewUI.SetActive(true);
        
        passCode.SetActive(false);

        workspaceManager.newMapName = newUIMapName;
    }
    
    public void SubmitPasscode()
    {
        if (passCodeInputField.text == "1234")
        {
            workSpaceOldUI.SetActive(!debugMode);
            workSpaceNewUI.SetActive(debugMode);
            
            visualizeOldUI.SetActive(!debugMode);
            visualizeNewUI.SetActive(debugMode);
            
            workspaceManager.enabled = !debugMode;
            
            workspaceManager.newMapName = oldUIMapName;

            debugMode = !debugMode;
        }
        
        passCode.SetActive(false);
    }
}
