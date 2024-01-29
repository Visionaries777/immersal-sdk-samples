using UnityEngine;
using UnityEngine.Animations;

public class LookAtCamera : MonoBehaviour
{
    private LookAtConstraint lookAtConstraint;
    
    private Transform cameraTransform;

    private void Start()
    {
        lookAtConstraint = GetComponent<LookAtConstraint>();
        
        if (lookAtConstraint == null)
        {
            lookAtConstraint = gameObject.AddComponent<LookAtConstraint>();
        }
        
        cameraTransform = Camera.main.transform;
        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = cameraTransform,
            weight = 1
        };
        lookAtConstraint.AddSource(source);
        lookAtConstraint.rotationOffset = new Vector3(0, -180, 0);
        lookAtConstraint.constraintActive = true;
    }
}
