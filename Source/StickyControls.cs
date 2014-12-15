/**
 * StickControls.cs
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

using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tac.StickyControls
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class StickyControls : MonoBehaviour
    {
        private Settings settings = new Settings();
        private string configFilename;
        private MainWindow window;

        private Vessel currentVessel = null;

        private ControlAxis yaw;
        private ControlAxis pitch;
        private ControlAxis roll;

        private const string lockName = "TacStickyControls";
        private const ControlTypes desiredLock = ControlTypes.YAW | ControlTypes.PITCH | ControlTypes.ROLL;

        public StickyControls()
        {
            this.Log("Constructor");
            configFilename = IOUtils.GetFilePathFor(this.GetType(), "StickyControls.cfg");
            window = new MainWindow(this, settings);
            yaw = new ControlAxis(GameSettings.YAW_LEFT, GameSettings.YAW_RIGHT, settings);
            pitch = new ControlAxis(GameSettings.PITCH_DOWN, GameSettings.PITCH_UP, settings);
            roll = new ControlAxis(GameSettings.ROLL_LEFT, GameSettings.ROLL_RIGHT, settings);
        }

        void Awake()
        {
            this.Log("Awake");
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

            // Make sure we remove our locks
            if (InputLockManager.GetControlLock(lockName) == desiredLock)
            {
                InputLockManager.RemoveControlLock(lockName);
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

                    yaw.Zero();
                    pitch.Zero();
                    roll.Zero();
                }
                else
                {
                    foreach (var entry in InputLockManager.lockStack)
                    {
                        if ((entry.Value & (ulong)desiredLock) != 0 && entry.Key != lockName)
                        {
                            // Something else locked out the controls, so do not accept any input
                            return;
                        }
                    }

                    if (Input.GetKeyDown(settings.ZeroControlsKey) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
                    {
                        yaw.Zero();
                        pitch.Zero();
                        roll.Zero();
                    }

                    if (Input.GetKeyDown(settings.SetControlsKey) && !Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.RightAlt))
                    {
                        yaw.SetValue(currentVessel.ctrlState.yaw);
                        pitch.SetValue(currentVessel.ctrlState.pitch);
                        roll.SetValue(currentVessel.ctrlState.roll);
                    }

                    if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(settings.ZeroControlsKey))
                    {
                        settings.Enabled = !settings.Enabled;
                    }

                    if (settings.Enabled)
                    {
                        yaw.Update();
                        pitch.Update();
                        roll.Update();

                        if (InputLockManager.GetControlLock(lockName) != desiredLock)
                        {
                            InputLockManager.SetControlLock(desiredLock, lockName);
                        }
                    }
                    else
                    {
                        if (InputLockManager.GetControlLock(lockName) == desiredLock)
                        {
                            InputLockManager.RemoveControlLock(lockName);
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
            if (settings.Enabled && currentVessel != null)
            {
                state.yaw = yaw.GetValue();
                state.pitch = pitch.GetValue();
                state.roll = roll.GetValue();

                state.wheelSteer = -state.yaw;
                state.wheelThrottle = -state.pitch;
            }
        }

        internal float GetYaw()
        {
            return yaw.GetValue();
        }

        internal float GetRawYaw()
        {
            return yaw.GetRawValue();
        }

        internal float GetPitch()
        {
            return pitch.GetValue();
        }

        internal float GetRawPitch()
        {
            return pitch.GetRawValue();
        }

        internal float GetRoll()
        {
            return roll.GetValue();
        }

        internal float GetRawRoll()
        {
            return roll.GetRawValue();
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
                settings.Load(config);
                window.Load(config);
                this.Log("Load: " + config);
            }
        }

        private void Save()
        {
            ConfigNode config = new ConfigNode();
            settings.Save(config);
            window.Save(config);

            config.Save(configFilename);
            this.Log("Save: " + config);
        }
    }
}
