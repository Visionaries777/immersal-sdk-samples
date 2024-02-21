/*===============================================================================
Copyright (C) 2023 Immersal - Part of Hexagon. All Rights Reserved.

This file is part of the Immersal SDK.

The Immersal SDK cannot be copied, distributed, or made available to
third-parties for commercial purposes without written permission of Immersal Ltd.

Contact sales@immersal.com for licensing requests.
===============================================================================*/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

namespace Immersal.Samples.ContentPlacement
{
    public class MovableContent : MonoBehaviour
    {
        [SerializeField]
        private float m_ClickHoldTime = 0.1f;
        private float m_timeHold = 0f;

        private bool m_EditingContent = false;

        private Transform m_CameraTransform;
        private float m_MovePlaneDistance;

        //public TMP_InputField itemName;
        public int mapId;
        public ContentType type;
        
        /*private LookAtConstraint lookAtConstraint;
    
        private Transform cameraTransform;

        private Canvas canvas;
        private BoxCollider boxCollider;*/

        /*private void Awake()
        {
            canvas = GetComponent<Canvas>();
            boxCollider = GetComponent<BoxCollider>();
            lookAtConstraint = GetComponent<LookAtConstraint>();
        }*/

        protected virtual void Start()
        {
            m_CameraTransform = Camera.main.transform;
            StoreContent();
            //LookAtCamera();
        }

        /*private void LookAtCamera()
        {
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
        }*/

        private void Update()
        {
            if (m_EditingContent)
            { 
                Vector3 projection = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_MovePlaneDistance));
                transform.position = projection;
            }
        }

        protected void StoreContent()
        {
            if (!ContentStorageManager.Instance.contentList.Contains(this))
            {
                ContentStorageManager.Instance.contentList.Add(this);
            }
            ContentStorageManager.Instance.SaveContents();
        }

        public void RemoveContent()
        {
            if (ContentStorageManager.Instance.contentList.Contains(this))
            {
                ContentStorageManager.Instance.contentList.Remove(this);
            }
            ContentStorageManager.Instance.SaveContents();
            Destroy(gameObject);
        }

        private void OnMouseDrag()
        {
            m_timeHold += Time.deltaTime;

            if (m_timeHold >= m_ClickHoldTime && !m_EditingContent)
            {
                m_MovePlaneDistance = Vector3.Dot(transform.position - m_CameraTransform.position, m_CameraTransform.forward) / m_CameraTransform.forward.sqrMagnitude;
                m_EditingContent = true;
            }
        }

        private void OnMouseUp()
        {
            StoreContent();
            m_timeHold = 0f;
            m_EditingContent = false;
        }

        /*public void UpdateName(string inputName)
        {
            StoreContent();
        }*/

        public virtual void ToggleContent(bool isActive)
        {
            /*canvas.enabled = isActive;
            boxCollider.enabled = isActive;
            lookAtConstraint.enabled = isActive;*/
        }
        
        public virtual TMP_InputField ItemNameInputField
        {
            get { return null; }
        }
    }
}