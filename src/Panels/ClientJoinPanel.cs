﻿using ColossalFramework.Threading;
using ColossalFramework.UI;
using CSM.Helpers;
using CSM.Networking;
using System;
using System.Threading;
using UnityEngine;

namespace CSM.Panels
{
    public class ClientJoinPanel : UIPanel
    {
        private UILabel _statusLabel;

        private UILabel _progress;

        private UIButton _cancelButton;

        public bool IsSelf { get; set; }

        public bool IsFirstJoin { get; set; }

        public string JoiningUsername { get; set; }

        public override void Start()
        {
            backgroundSprite = "GenericPanel";
            name = "ClientJoinPanel";
            color = new Color32(110, 110, 110, 220);

            relativePosition = new Vector3(0, 0);

            width = Screen.width;
            height = Screen.height;

            // Connecting Status
            _statusLabel = this.CreateTitleLabel("", new Vector2(0, 0));

            // Canel button only while first join
            _cancelButton = this.CreateButton("Cancel", new Vector2((Screen.width / 2f) - 170f, -(Screen.height / 2f) - 60f));
            _cancelButton.eventClick += OnCancelButtonClick;
            _cancelButton.isVisible = false;

            // Transfer progress
            _progress = this.CreateTitleLabel("0%", new Vector2(0, 0));
            _progress.position = new Vector3((width / 2f) - _progress.width, -(height / 2f) + 20f);
            _progress.textAlignment = UIHorizontalAlignment.Center;
        }

        public void ShowPanel()
        {
            UpdateJoinPanel();
            isVisible = true;
            Focus();
        }

        public void HidePanel(bool RemoveFromUI = false)
        {
            isVisible = false;
            if (RemoveFromUI)
                RemoveUIComponent(this);
        }

        private void OnCancelButtonClick(UIComponent uiComponent, UIMouseEventParameter eventParam)
        {
            MultiplayerManager.Instance.CurrentClient.Disconnect();
            MultiplayerManager.Instance.CurrentClient.StopMainMenuEventProcessor();
            HidePanel(true);
        }

        public override void Update()
        {
            int percent = (int)(SaveHelpers.CurrentMaxProcess / 100f * SaveHelpers.CurrentProcess);
            percent = Mathf.Clamp(percent, 0, 100);
            _progress.text = percent + "%";
            base.Update();
        }

        private void UpdateJoinPanel()
        {
            new Thread(() =>
            {
                // Waiting for _statusLabel and _cancelButton being created
                while (!_statusLabel || !_cancelButton)
                {
                    Thread.Sleep(50);
                }

                // Update _statusLabel and _cancelButton
                ThreadHelper.dispatcher.Dispatch(() =>
                {
                    _statusLabel.position = new Vector2(0, 60);
                    _statusLabel.text = GetStatusMessage();
                    float w = _statusLabel.width;
                    _statusLabel.position = new Vector2((width - w) / 2f, -(height / 2f) + 60f);
                    if (IsFirstJoin)
                    {
                        _cancelButton.isVisible = true;
                    }

                    _progress.isVisible = IsFirstJoin || IsSelf;
                });
            }).Start();
        }

        private string GetStatusMessage()
        {
            if (IsFirstJoin)
            {
                return "Downloading save game...";
            }
            else if (IsSelf)
            {
                return "Re-downloading save game...";
            }
            else
            {
                return JoiningUsername + " is joining...";
            }
        }
    }
}
