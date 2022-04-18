using Synapse.Config;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace MapEditor
{
    public class Config : IConfigSection
    {
        [Description("The Name of the loaded Map")]
        public List<string> MapsLoaded { get; set; } = new List<string>()
        { 
            Plugin.MapNone
        };

        [Description("Key for change the \"Ammot\" of the tool")]
        public KeyCode KeyUp { get; set; } = KeyCode.Keypad8;
        public KeyCode KeyDown { get; set; } = KeyCode.Keypad2;
        public KeyCode KeyReset { get; set; } = KeyCode.Keypad5;
    }
}