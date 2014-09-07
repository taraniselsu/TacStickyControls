/**
 * StickyControlSettings.cs
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

namespace Tac.StickyControls
{
    internal class Settings
    {
        internal float Speed { get; set; }
        internal float Step { get; set; }
        internal float PrecisionControlsModifier { get; set; }
        internal string ZeroControlsKey { get; set; }
        internal bool Enabled { get; set; }

        internal Settings()
        {
            // Set defaults
            Speed = 1.0f;
            Step = 0.1f;
            PrecisionControlsModifier = 0.1f;
            ZeroControlsKey = "z";
            Enabled = true;
        }

        internal void Load(ConfigNode config)
        {
            Speed = Utilities.GetValue(config, "Speed", Speed);
            Step = Utilities.GetValue(config, "Step", Step);
            Enabled = Utilities.GetValue(config, "Enabled", Enabled);
            PrecisionControlsModifier = Utilities.GetValue(config, "PrecisionControlsModifier", PrecisionControlsModifier);
            ZeroControlsKey = Utilities.GetValue(config, "ZeroControlsKey", ZeroControlsKey);
        }

        internal void Save(ConfigNode config)
        {
            config.AddValue("Speed", Speed);
            config.AddValue("Step", Step);
            config.AddValue("Enabled", Enabled);
            config.AddValue("PrecisionControlsModifier", PrecisionControlsModifier);
            config.AddValue("ZeroControlsKey", ZeroControlsKey);
        }
    }
}
