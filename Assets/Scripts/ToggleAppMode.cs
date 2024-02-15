using Immersal.Samples.Mapping;
using UnityEngine;

public class ToggleAppMode : MonoBehaviour
{
    [SerializeField] private GameObject appModePanel;
    
    public GameObject mappingUIPrefab;
    private GameObject m_MappingUI;

    private MappingUIManager mappingUIManager;

    void Start()
    {
        LoginManager loginManager = LoginManager.Instance;

        if (loginManager != null)
        {
            loginManager.OnLogin += EnableMappingMode;
            loginManager.OnLogout += DisableMappingMode;
        }
    }
    
    public void ImmersalServerMode()
    {
        appModePanel.SetActive(false);
        
        mappingUIManager.ChangeMode(false);
    }
    
    public void ScanMode()
    {
        appModePanel.SetActive(false);

        mappingUIManager.ChangeMode(true);
    }

    private void EnableMappingMode()
    {
        if (m_MappingUI == null)
        {
            m_MappingUI = Instantiate(mappingUIPrefab);
            mappingUIManager = m_MappingUI.GetComponent<MappingUIManager>();
            mappingUIManager.OnSwitch += ShowAppModelPanel;
        }
        else
        {
            m_MappingUI.SetActive(true);
        }
    }
    
    public void DisableMappingMode()
    {
        if (m_MappingUI != null)
        {
            m_MappingUI.SetActive(false);
        }
    }

    private void ShowAppModelPanel()
    {
        appModePanel.SetActive(true);
    }
}
