using Immersal.Samples.Mapping;

public class NewWorkspaceManager : WorkspaceManager
{
    public MappingUIComponent settingButton = null;
    
    protected override void ChangeState(UIState state)
    {
        switch (state)
        {
            case UIState.Default:
                captureButton.Activate();
                m_InfoPanel.Activate();
                m_PromptConstructMap.SetActive(false);
                m_InfoPanel.gameObject.SetActive(true);
                settingButton.Activate();
                break;
            case UIState.Options:
                break;
            case UIState.DeleteData:
                break;
            case UIState.SubmitNewMap:
                captureButton.Disable();
                m_InfoPanel.Disable();
                m_PromptConstructMap.SetActive(true);
                m_InfoPanel.gameObject.SetActive(true);
                settingButton.Disable();
                break;
            default:
                break;
        }
    }
}
