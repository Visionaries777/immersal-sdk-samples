/*===============================================================================
Copyright (C) 2023 Immersal - Part of Hexagon. All Rights Reserved.

This file is part of the Immersal SDK.

The Immersal SDK cannot be copied, distributed, or made available to
third-parties for commercial purposes without written permission of Immersal Ltd.

Contact sales@immersal.com for licensing requests.
===============================================================================*/

using System;
using UnityEngine;
using UnityEngine.UI;
using Immersal.Samples.Util;
using TMPro;

namespace Immersal.Samples.Mapping
{
	public class MappingUIManager : MonoBehaviour
	{
		private CanvasGroup canvasGroup;
		public event SwitchModeEvent OnSwitch = null;
		public delegate void SwitchModeEvent();
	    
        public WorkspaceManager workspaceManager;
        public VisualizeManager visualizeManager;
        public TextMeshProUGUI locationText = null;
        public TextMeshProUGUI vLocationText = null;

        public Toggle gpsToggle = null;
        //public Toggle nearbyMapsToggle = null;
        //public Toggle serverLocalizeToggle = null;

        [SerializeField]
        private HorizontalProgressBar m_ProgressBar = null;
        
		private enum UIState {Workspace, Visualize};
		private UIState uiState = UIState.Workspace;

		[SerializeField] private TextMeshProUGUI loggedInAsText = null;
		
		public void SwitchMode() {
			if (uiState == UIState.Workspace) {
				uiState = UIState.Visualize;
			} else {
				uiState = UIState.Workspace;
			}		
			ChangeState(uiState);
		}

        public void ShowProgressBar()
        {
            m_ProgressBar.transform.GetComponent<Fader>().FadeIn();
        }

        public void HideProgressBar()
        {
            m_ProgressBar.transform.GetComponent<Fader>().FadeOut();
        }

        public void SetProgress(int value)
        {
            m_ProgressBar.currentValue = value;
        }
        
        public void ShowDebugConsole(bool value)
        {
	        DebugConsole.Instance.Show(value);
        }

        public void LogoutButtonPressed()
        {
	        visualizeManager.ResetMaps();
	        LoginManager.Instance.Logout();
        }

		private void ChangeState(UIState state) {
			switch (state) {
				case UIState.Workspace:
                    workspaceManager.gameObject.SetActive(true);
                    visualizeManager.gameObject.SetActive(false);
                    break;
				case UIState.Visualize:
                    workspaceManager.gameObject.SetActive(false);
                    visualizeManager.gameObject.SetActive(true);
                    break;
				default:
					break;
			}
		}

		private void OnEnable()
		{
			loggedInAsText.text = string.Format("Logged in as {0}", PlayerPrefs.GetString("login"));
		}

		private void OnDisable()
		{
			loggedInAsText.text = string.Empty;
		}

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start()
        {
            //ChangeState(uiState);
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            m_ProgressBar.minValue = 0;
            m_ProgressBar.maxValue = 100;
            m_ProgressBar.currentValue = 0;
        }
		
		public void ChangeMode(bool isScan)
		{
			canvasGroup.alpha = 1;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			
			ChangeState(isScan ? UIState.Workspace : UIState.Visualize);
		}

		public void BackToChooseMode()
		{
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			
			OnSwitch?.Invoke();
		}
	}
}