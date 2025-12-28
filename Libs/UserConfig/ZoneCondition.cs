using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mute_at_Office.Libs.UserConfig
{
    public class ZoneCondition(string id,  string speakerName, string ssid)
    {
        public string ID { get; } = id;
        public string SpeakerName { get; set; } = speakerName;
        public string Ssid { get; set; } = ssid;
    }
}
