using Immersal.Samples.Mapping;
using TMPro;
using UnityEngine;

public class PasscodeManager : MonoBehaviour
{
    [SerializeField] private MappingUIManager mappingUIManager;
    [SerializeField] private WorkspaceManager oldWorkspaceManager, newWorkspaceManager;
    
    [SerializeField] private TMP_InputField passCodeInputField;
    
    public bool debugMode;
    
    [SerializeField] private GameObject passCode;

    private void Start()
    {
        passCode.SetActive(false);
    }

    public void SubmitPasscode()
    {
        if (passCodeInputField.text == "1234")
        {
            mappingUIManager.workspaceManager = oldWorkspaceManager;
            newWorkspaceManager.gameObject.SetActive(false);
            mappingUIManager.ChangeState(mappingUIManager.uiState);
            
            debugMode = !debugMode;
        }
        
        passCode.SetActive(false);
    }
}
