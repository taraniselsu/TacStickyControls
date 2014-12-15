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
        private float tempValue = 0.0f;
        private float timeKeyDown = 0.0f;
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
                // Ignore if the Alt key is held down, the user might be trying to set the trim
                return;
            }

            float modifier = (FlightInputHandler.fetch.precisionMode) ? settings.PrecisionControlsModifier : 1.0f;

            if (decreaseKey.GetKeyDown())
            {
                // the key was just pressed down
                timeKeyDown = Time.time;
                tempValue = value;
            }
            else if (decreaseKey.GetKey())
            {
                float delta = Time.time - timeKeyDown;
                if (delta < settings.MinTime)
                {
                    // Only move a single step
                    float temp = tempValue - (settings.Step * modifier);
                    value = Math.Max(temp, -1.0f);
                }
                else
                {
                    delta = Mathf.Pow(delta, settings.Exponent);

                    float temp = tempValue - (settings.Speed * delta * modifier);
                    temp = StickyUtilities.RoundDown(temp, settings.Step * modifier);
                    value = Math.Max(temp, -1.0f);
                }
            }

            if (increaseKey.GetKeyDown())
            {
                // the key was just pressed down
                timeKeyDown = Time.time;
                tempValue = value;
            }
            else if (increaseKey.GetKey())
            {
                float delta = Time.time - timeKeyDown;
                if (delta < settings.MinTime)
                {
                    // Only move a single step
                    float temp = tempValue + (settings.Step * modifier);
                    value = Math.Min(temp, 1.0f);
                }
                else
                {
                    delta = Mathf.Pow(delta, settings.Exponent);

                    float temp = tempValue + (settings.Speed * delta * modifier);
                    temp = StickyUtilities.RoundUp(temp, settings.Step * modifier);
                    value = Math.Min(temp, 1.0f);
                }
            }
        }

        internal void Zero()
        {
            value = 0.0f;
        }

        internal float GetValue()
        {
            if (value > settings.PositionDeadZone)
            {
                return Mathf.Pow((value - settings.PositionDeadZone) / (1 - settings.PositionDeadZone), settings.PositionExponent);
            }
            else if (value < -settings.PositionDeadZone)
            {
                return -Mathf.Pow((-value - settings.PositionDeadZone) / (1 - settings.PositionDeadZone), settings.PositionExponent);
            }
            else
            {
                return 0f;
            }
        }

        internal void SetValue(float newValue)
        {
            if (newValue > 0f)
            {
                value = Mathf.Pow(newValue, 1f / settings.PositionExponent) * (1 - settings.PositionDeadZone) + settings.PositionDeadZone;
            }
            else if (newValue < 0f)
            {
                value = -Mathf.Pow(-newValue, 1f / settings.PositionExponent) * (1 - settings.PositionDeadZone) + settings.PositionDeadZone;
            }
            else
            {
                value = 0f;
            }
        }

        internal float GetRawValue()
        {
            return value;
        }

        internal void SetRawValue(float newValue)
        {
            value = newValue;
        }
    }
}
