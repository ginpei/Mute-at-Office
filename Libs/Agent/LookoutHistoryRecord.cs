using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mute_at_Office.Libs.Agent;

public enum LookoutEventType
{
    MuteAtOffice,
    WiFi,
    Audio
}

public record LookoutHistoryRecord
{
    public Guid Id { get; init; }
    public DateTime DateTime { get; init; }
    public LookoutEventType EventType { get; init; }
    public string Message { get; init; }

    public LookoutHistoryRecord(LookoutEventType eventType, string message, DateTime? dateTime = null, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        DateTime = dateTime ?? DateTime.Now;
        EventType = eventType;
        Message = message;
    }

    public string GetEventTypeDisplayName()
    {
        return EventType switch
        {
            LookoutEventType.MuteAtOffice => "Mute-at-Office",
            LookoutEventType.WiFi => "WiFi",
            LookoutEventType.Audio => "Audio",
            _ => EventType.ToString()
        };
    }

    public string GetFormattedDateTime()
    {
        return DateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
