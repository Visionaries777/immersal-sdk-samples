using Immersal.Samples.ContentPlacement;
using UnityEngine;

public class FireworkContent : MovableContent
{
    private Animator animator;
    private SphereCollider sphereCollider;
    private GameObject childGo;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
        childGo = transform.GetChild(0).gameObject;
    }
    
    public override void ToggleContent(bool isActive)
    {
        animator.enabled = isActive;
        sphereCollider.enabled = isActive;
        childGo.SetActive(isActive);
    }
}
