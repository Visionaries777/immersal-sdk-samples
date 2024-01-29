using Immersal.Samples.Mapping;
using UnityEngine;
using UnityEngine.UI;

public class NewMappingUIComponent : MappingUIComponent
{
    private Color button_normalColor = new Color(1f, 1f, 1f, 1f);
    private Color button_disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    private Color icon_normalColor = new Color(1f, 1f, 1f, 1f);
    private Color icon_disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    public override void Activate() 
    {
        if(image != null) {
            image.color = button_normalColor;
        }
        if(button != null) {
            ColorBlock cb = button.colors;
            cb.normalColor = icon_normalColor;
            button.colors = cb;
            button.interactable = true;
        }
    }
    
    public override void Disable() {
        if (image != null) {
            image.color = button_disabledColor;
        }
        if (button != null) {
            ColorBlock cb = button.colors;
            cb.normalColor = icon_disabledColor;
            button.colors = cb;
            button.interactable = false;
        }
    }
}
