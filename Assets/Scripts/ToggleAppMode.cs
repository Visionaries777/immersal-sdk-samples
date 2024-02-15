using Immersal.Samples.Mapping;
using TMPro;
using UnityEngine;

public class ToggleAppMode : MonoBehaviour
{
    [SerializeField] private GameObject appModePanel;
    
    public GameObject mappingUIPrefab;
    private GameObject m_MappingUI;

    private MappingUIManager mappingUIManager;
    
    [SerializeField] private TMP_InputField serverField;
    private readonly string serverDomain = "http://192.168.1.171:12345";

    void Start()
    {
        LoginManager loginManager = LoginManager.Instance;

        if (loginManager != null)
        {
            loginManager.OnLogin += EnableMappingMode;
            loginManager.OnLogout += DisableMappingMode;
        }
        
        if (PlayerPrefs.HasKey("serverDomain"))
        {
            serverField.text = PlayerPrefs.GetString("serverDomain");
        }
        else
        {
            PlayerPrefs.SetString("serverDomain", serverDomain);
        }
    }

    public void SetServerDomain(string s)
    {
        if (s.Length == 0)
        {
            s = serverDomain;
            serverField.text = s;
        }
        else if (s[s.Length - 1] == '/')
        {
            s = s.Substring(0, s.Length - 1);
            serverField.text = s;
        }
        
        PlayerPrefs.SetString("serverDomain", serverField.text);
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
