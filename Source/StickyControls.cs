/**
 * StickControls.cs
 * 
 * Thunder Aerospace Corporation's Flight Computer for the Kerbal Space Program, by Taranis Elsu
 * 
 * (C) Copyright 2013, Taranis Elsu
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

using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class StickyControls : MonoBehaviour
    {
        private string configFilename;
        private MainWindow window;

        private Vessel currentVessel = null;

        internal float Yaw { get; private set; }
        internal float Pitch { get; private set; }
        internal float Roll { get; private set; }

        internal float Speed { get; set; }
        internal float Step { get; set; }
        internal float PrecisionControlsModifier { get; set; }
        internal string ZeroControlsKey { get; set; }
        internal bool Enabled { get; set; }

        StickyControls()
        {
            this.Log("Constructor");
        }

        void Awake()
        {
            this.Log("Awake");
            configFilename = IOUtils.GetFilePathFor(this.GetType(), "StickyControls.cfg");
            window = new MainWindow(this);

            // Set defaults
            Speed = 1.0f;
            Step = 0.1f;
            PrecisionControlsModifier = 0.1f;
            ZeroControlsKey = "z";
            Enabled = true;
        }

        void Start()
        {
            this.Log("Start");
            Load();
            window.SetVisible(true);
        }

        void OnDestroy()
        {
            this.Log("OnDestroy");
            Save();

            if (currentVessel != null)
            {
                Unregister(currentVessel);
            }
        }

        private void Register(Vessel vessel)
        {
            if (vessel != null)
            {
                vessel.OnFlyByWire += StickyFlyByWire;
                vessel.OnJustAboutToBeDestroyed += OnJustAboutToBeDestroyed;
            }
        }

        private void Unregister(Vessel vessel)
        {
            if (vessel != null)
            {
                vessel.OnFlyByWire -= StickyFlyByWire;
                vessel.OnJustAboutToBeDestroyed -= OnJustAboutToBeDestroyed;
            }
        }

        void Update()
        {
            try
            {
                if (!FlightGlobals.ready)
                {
                    return;
                }
                else if (FlightGlobals.ActiveVessel != currentVessel)
                {
                    Unregister(currentVessel);
                    currentVessel = FlightGlobals.ActiveVessel;
                    Register(currentVessel);

                    Yaw = 0;
                    Pitch = 0;
                    Roll = 0;
                }
                else
                {
                    if (Input.GetKeyDown(ZeroControlsKey))
                    {
                        Yaw = 0;
                        Pitch = 0;
                        Roll = 0;
                    }

                    if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(ZeroControlsKey))
                    {
                        Enabled = !Enabled;
                    }

                    if (Enabled && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
                    {
                        float modifier = (FlightInputHandler.fetch.precisionMode) ? PrecisionControlsModifier : 1.0f;

                        if (GameSettings.YAW_LEFT.GetKey())
                        {
                            Yaw = Math.Max(Yaw - (Speed * Time.deltaTime * modifier), -1.0f);
                        }
                        if (GameSettings.YAW_LEFT.GetKeyUp())
                        {
                            Yaw = Math.Max(RoundDown(Yaw, Step * modifier), -1.0f);
                        }

                        if (GameSettings.YAW_RIGHT.GetKey())
                        {
                            Yaw = Math.Min(Yaw + (Speed * Time.deltaTime * modifier), 1.0f);
                        }
                        if (GameSettings.YAW_RIGHT.GetKeyUp())
                        {
                            Yaw = Math.Min(RoundUp(Yaw, Step * modifier), 1.0f);
                        }

                        if (GameSettings.PITCH_UP.GetKey())
                        {
                            Pitch = Math.Min(Pitch + (Speed * Time.deltaTime * modifier), 1.0f);
                        }
                        if (GameSettings.PITCH_UP.GetKeyUp())
                        {
                            Pitch = Math.Min(RoundUp(Pitch, Step * modifier), 1.0f);
                        }

                        if (GameSettings.PITCH_DOWN.GetKey())
                        {
                            Pitch = Math.Max(Pitch - (Speed * Time.deltaTime * modifier), -1.0f);
                        }
                        if (GameSettings.PITCH_DOWN.GetKeyUp())
                        {
                            Pitch = Math.Max(RoundDown(Pitch, Step * modifier), -1.0f);
                        }

                        if (GameSettings.ROLL_LEFT.GetKey())
                        {
                            Roll = Math.Max(Roll - (Speed * Time.deltaTime * modifier), -1.0f);
                        }
                        if (GameSettings.ROLL_LEFT.GetKeyUp())
                        {
                            Roll = Math.Max(RoundDown(Roll, Step * modifier), -1.0f);
                        }

                        if (GameSettings.ROLL_RIGHT.GetKey())
                        {
                            Roll = Math.Min(Roll + (Speed * Time.deltaTime * modifier), 1.0f);
                        }
                        if (GameSettings.ROLL_RIGHT.GetKeyUp())
                        {
                            Roll = Math.Min(RoundUp(Roll, Step * modifier), 1.0f);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.LogError("Exception:\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void StickyFlyByWire(FlightCtrlState state)
        {
            if (Enabled && currentVessel != null)
            {
                state.yaw = Yaw;
                state.pitch = Pitch;
                state.roll = Roll;
            }
        }

        private void OnJustAboutToBeDestroyed()
        {
            Unregister(currentVessel);
        }

        private void Load()
        {
            if (File.Exists<StickyControls>(configFilename))
            {
                ConfigNode config = ConfigNode.Load(configFilename);
                window.Load(config);
                Speed = Utilities.GetValue(config, "Speed", Speed);
                Step = Utilities.GetValue(config, "Step", Step);
                Enabled = Utilities.GetValue(config, "Enabled", Enabled);
                PrecisionControlsModifier = Utilities.GetValue(config, "PrecisionControlsModifier", PrecisionControlsModifier);
                ZeroControlsKey = Utilities.GetValue(config, "ZeroControlsKey", ZeroControlsKey);
                this.Log("Load: " + config);
            }
        }

        private void Save()
        {
            ConfigNode config = new ConfigNode();
            window.Save(config);
            config.AddValue("Speed", Speed);
            config.AddValue("Step", Step);
            config.AddValue("Enabled", Enabled);
            config.AddValue("PrecisionControlsModifier", PrecisionControlsModifier);
            config.AddValue("ZeroControlsKey", ZeroControlsKey);

            config.Save(configFilename);
            this.Log("Save: " + config);
        }

        internal static float RoundUp(float value, float step)
        {
            return Mathf.Ceil(value / step) * step;
        }

        internal static float RoundDown(float value, float step)
        {
            return Mathf.Floor(value / step) * step;
        }
    }
}
