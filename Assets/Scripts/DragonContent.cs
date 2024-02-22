using Immersal.Samples.ContentPlacement;
using UnityEngine;

public class DragonContent : MovableContent
{
    private Animator animator;
    private GameObject childGo;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        childGo = transform.GetChild(0).gameObject;
    }
    
    public override void ToggleContent(bool isActive)
    {
        animator.enabled = isActive;
        childGo.SetActive(isActive);
    }
}
