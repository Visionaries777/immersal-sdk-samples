using System.IO;
using Immersal.Samples.ContentPlacement;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Video;

public class VideoContent : MovableContent
{
    private Camera mainCam;
    private Canvas canvas;
    private BoxCollider boxCollider;
    private LookAtConstraint lookAtConstraint;
    private VideoPlayer videoPlayer;
    private string videoName = "demo.mp4";
    
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        boxCollider = GetComponent<BoxCollider>();
        lookAtConstraint = GetComponent<LookAtConstraint>();
        videoPlayer = GetComponentInChildren<VideoPlayer>();
    }
    
    protected override void Start()
    {
        base.Start();
        mainCam = Camera.main;
        LookAtCamera();
        
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, videoName);
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = streamingAssetsPath;
    }
    
    private void LookAtCamera()
    {
        if (lookAtConstraint == null)
        {
            lookAtConstraint = gameObject.AddComponent<LookAtConstraint>();
        }
        
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = mainCam.transform,
            weight = 1
        };
        lookAtConstraint.AddSource(source);
        lookAtConstraint.rotationOffset = new Vector3(0, -180, 0);
        lookAtConstraint.constraintActive = true;
    }
    
    public override void ToggleContent(bool isActive)
    {
        canvas.enabled = isActive;
        boxCollider.enabled = isActive;
        lookAtConstraint.enabled = isActive;
        videoPlayer.enabled = isActive;
    }
}
