using Synapse.Config;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace MapEditor
{
    public class Config : IConfigSection
    {
        [Description("The Name of the loaded Map")]
        public List<string> MapsLoaded = new List<string>()
        { 
            Plugin.MapNone
        };

        [Description("Key for change the \"Ammot\" of the tool")]
        public KeyCode KeyUp = KeyCode.Keypad1;
        public KeyCode KeyDown = KeyCode.Keypad2;
    }
}