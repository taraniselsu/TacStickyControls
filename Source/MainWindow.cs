/**
 * MainWindow.cs
 * 
 * Thunder Aerospace Corporation's Flight Computer for the Kerbal Space Program, by Taranis Elsu
 * 
 * (C) Copyright 2014, Taranis Elsu
 * 
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 * 
 * This code is licensed under the Apache License Version 2.0. See the LICENSE.txt and NOTICE.txt
 * files for more information.
 * 
 * Note that Thunder Aerospace Corporation is a ficticious entity created for entertainment
 * purposes. It is in no way meant to represent a real entity. Any similarity to a real entity
 * is purely coincidental.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac.StickyControls
{
    internal class MainWindow : Window<MainWindow>
    {
        private readonly StickyControls main;
        private readonly Settings settings;
        private readonly string version;

        private GUIStyle labelStyle;
        private GUIStyle valueStyle;
        private GUIStyle buttonStyle;
        private GUIStyle versionStyle;

        private bool showSettings = false;

        internal MainWindow(StickyControls main, Settings settings)
            : base("TAC Sticky Controls", 155, 100)
        {
            base.HideCloseButton = true;
            base.Resizable = false;
            base.HideWhenPaused = false;

            this.main = main;
            this.settings = settings;
            this.version = Utilities.GetDllVersion(this);

            this.Log(this.GetType().Assembly.Location);
        }

        protected override void ConfigureStyles()
        {
            base.ConfigureStyles();

            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.alignment = TextAnchor.MiddleLeft;
                labelStyle.fontStyle = FontStyle.Normal;
                labelStyle.normal.textColor = Color.white;
                labelStyle.margin.top = 0;
                labelStyle.margin.bottom = 0;
                labelStyle.padding.top = 0;
                labelStyle.padding.bottom = 1;
                labelStyle.wordWrap = false;

                valueStyle = new GUIStyle(labelStyle);
                valueStyle.alignment = TextAnchor.MiddleRight;
                valueStyle.stretchWidth = true;

                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.alignment = TextAnchor.MiddleCenter;
                buttonStyle.fontStyle = FontStyle.Normal;
                buttonStyle.normal.textColor = Color.white;
                buttonStyle.padding = new RectOffset(6, 2, 4, 2);
                buttonStyle.wordWrap = false;

                versionStyle = Utilities.GetVersionStyle();
            }
        }

        protected override void DrawWindowContents(int windowId)
        {
            settings.Enabled = GUILayout.Toggle(settings.Enabled, "Enabled");

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("Yaw", labelStyle);
            GUILayout.Label("Pitch", labelStyle);
            GUILayout.Label("Roll", labelStyle);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(main.GetYaw().ToString("0.000"), valueStyle);
            GUILayout.Label(main.GetPitch().ToString("0.000"), valueStyle);
            GUILayout.Label(main.GetRoll().ToString("0.000"), valueStyle);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            bool newShowSettings = GUILayout.Toggle(showSettings, "Settings", buttonStyle, GUILayout.ExpandWidth(true));
            if (newShowSettings != showSettings)
            {
                showSettings = newShowSettings;
                this.SetSize(155, 100);
            }

            if (showSettings)
            {
                float newFloat;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Speed", labelStyle, GUILayout.ExpandHeight(true));
                if (float.TryParse(GUILayout.TextField(settings.Speed.ToString("0.000"), GUILayout.ExpandWidth(true)), out newFloat))
                {
                    settings.Speed = newFloat;
                }
                GUILayout.EndHorizontal();
                settings.Speed = StickyUtilities.RoundUp(GUILayout.HorizontalSlider(settings.Speed, 0.025f, 1.0f, GUILayout.ExpandWidth(true)), 0.025f);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Step", labelStyle, GUILayout.ExpandHeight(true));
                if (float.TryParse(GUILayout.TextField(settings.Step.ToString("0.000"), GUILayout.ExpandWidth(true)), out newFloat))
                {
                    settings.Step = newFloat;
                }
                GUILayout.EndHorizontal();
                settings.Step = StickyUtilities.RoundUp(GUILayout.HorizontalSlider(settings.Step, 0.005f, 0.25f, GUILayout.ExpandWidth(true)), 0.005f);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Precision Controls Modifier", labelStyle, GUILayout.ExpandHeight(true));
                if (float.TryParse(GUILayout.TextField(settings.PrecisionControlsModifier.ToString("0.000"), GUILayout.ExpandWidth(true)), out newFloat))
                {
                    settings.PrecisionControlsModifier = newFloat;
                }
                GUILayout.EndHorizontal();
                settings.PrecisionControlsModifier = StickyUtilities.RoundUp(GUILayout.HorizontalSlider(settings.PrecisionControlsModifier, 0.005f, 0.25f, GUILayout.ExpandWidth(true)), 0.005f);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Zero Controls key", labelStyle);
                settings.ZeroControlsKey = GUILayout.TextField(settings.ZeroControlsKey, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
                GUI.Label(new Rect(4, windowPos.height - 13, windowPos.width - 20, 12), "TAC Sticky Controls v" + version, versionStyle);
            }
        }
    }
}
