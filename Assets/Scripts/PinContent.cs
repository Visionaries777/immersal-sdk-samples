using Immersal.Samples.ContentPlacement;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class PinContent : MovableContent
{
    private Camera mainCam;
    private Canvas canvas;
    private BoxCollider boxCollider;
    private LookAtConstraint lookAtConstraint;

    [SerializeField] 
    private TMP_InputField itemNameInputField;
    
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        boxCollider = GetComponent<BoxCollider>();
        lookAtConstraint = GetComponent<LookAtConstraint>();
    }
    
    protected override void Start()
    {
        base.Start();
        mainCam = Camera.main;
        LookAtCamera();
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
    
    public void UpdateName(string inputName)
    {
        StoreContent();
    }
    
    public override void ToggleContent(bool isActive)
    {
        canvas.enabled = isActive;
        boxCollider.enabled = isActive;
        lookAtConstraint.enabled = isActive;
    }
    
    public override TMP_InputField ItemNameInputField
    {
        get { return itemNameInputField; }
    }
}
