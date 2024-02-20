using Immersal.Samples.ContentPlacement;
using UnityEngine;

public class DiamondContent : MovableContent
{
    private SphereCollider sphereCollider;
    private RotateOverTime rotateOverTime;
    
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rotateOverTime = GetComponent<RotateOverTime>();
    }
    
    public override void ToggleContent(bool isActive)
    {
        sphereCollider.enabled = isActive;
        rotateOverTime.enabled = isActive;
    }
}
