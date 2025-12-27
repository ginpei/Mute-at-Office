using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mute_at_Office.Libs.UserConfig
{
    class UserConfig
    {
        public List<ZoneCondition> safeZoneConditions = [];

        public string Ssid { get; set; } = "";
        public string SpeakerName { get; set; } = "";
    }
}
