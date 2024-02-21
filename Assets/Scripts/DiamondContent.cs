using System.Collections.Generic;
using Immersal.Samples.ContentPlacement;
using UnityEngine;

public class DiamondContent : MovableContent
{
    private SphereCollider sphereCollider;
    private RotateOverTime rotateOverTime;
    private readonly List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private ParticleSystem particleSystem;
    
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rotateOverTime = GetComponent<RotateOverTime>();
        foreach (Transform child in transform)
        {
            var meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderers.Add(meshRenderer);
            }

            var ps = child.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                particleSystem = ps;
            }
        }
    }
    
    public override void ToggleContent(bool isActive)
    {
        sphereCollider.enabled = isActive;
        rotateOverTime.enabled = isActive;
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = isActive;
        }

        if (isActive)
        {
            particleSystem.Play();
        }
        else
        {
            particleSystem.Stop();
        }
    }
}
