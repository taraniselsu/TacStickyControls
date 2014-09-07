/**
 * ControlAxis.cs
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
    internal class ControlAxis
    {
        private float value = 0.0f;
        private KeyBinding decreaseKey;
        private KeyBinding increaseKey;
        private Settings settings;

        internal ControlAxis(KeyBinding decreaseKey, KeyBinding increaseKey, Settings settings)
        {
            this.decreaseKey = decreaseKey;
            this.increaseKey = increaseKey;
            this.settings = settings;
        }

        internal void Update()
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                return;
            }

            float modifier = (FlightInputHandler.fetch.precisionMode) ? settings.PrecisionControlsModifier : 1.0f;

            if (decreaseKey.GetKey())
            {
                value = Math.Max(value - (settings.Speed * Time.deltaTime * modifier), -1.0f);
            }
            if (decreaseKey.GetKeyUp())
            {
                value = Math.Max(StickyUtilities.RoundDown(value, settings.Step * modifier), -1.0f);
            }

            if (increaseKey.GetKey())
            {
                value = Math.Min(value + (settings.Speed * Time.deltaTime * modifier), 1.0f);
            }
            if (increaseKey.GetKeyUp())
            {
                value = Math.Min(StickyUtilities.RoundUp(value, settings.Step * modifier), 1.0f);
            }
        }

        internal void Zero()
        {
            value = 0.0f;
        }

        internal float GetValue()
        {
            return value;
        }
    }
}
