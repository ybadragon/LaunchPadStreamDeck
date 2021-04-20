using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LaunchPadStreamDeck.API.Classes;
using LaunchPadStreamDeck.API.Enums;
using LaunchPadStreamDeck.API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LaunchPadStreamDeck.API.Services
{
    public class EventService
    {
        private List<IEvent> events = new List<IEvent>();
        private readonly string keyboardEventPath;
        private readonly string processEventPath;
        private readonly string localAppData;
        public EventService()
        {
            localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            keyboardEventPath = Path.Combine(localAppData, "LaunchPadMidiController", "KeyboardEvents.json");
            processEventPath = Path.Combine(localAppData, "LaunchPadMidiController", "ProcessEvent.json");
            LoadEvents();
        }

        public IEvent GetEvent(EventOption option)
        {
            if (option is NamedEventOption)
            {
                return GetNamedEvent(option as NamedEventOption);
            }

            return GetPositionEvent(option as PositionEventOption);
        }

        private IEvent GetNamedEvent(NamedEventOption option)
        {
            return events.Where(x => x.ProfileName == option.ProfileName)
                .FirstOrDefault(x => x.EventName == option.EventName);
        }

        private IEvent GetPositionEvent(PositionEventOption option)
        {
            return events.Where(x => x.ProfileName == option.ProfileName)
                .FirstOrDefault(x => x.ButtonX == option.ButtonX && x.ButtonY == option.ButtonY);
        }

        public void SaveEvent<T, K>(T IEvent, K option, bool shouldOverwrite = false) 
            where T : IEvent
            where K : EventOption
        {
            var lEvent = (T)GetEvent(option);

            if (option is PositionEventOption && lEvent != null && !shouldOverwrite)
                throw new AlreadyExistsException($"Shortcut already exists at position X: {lEvent.ButtonX} Y:{lEvent.ButtonY}");

            if (option is NamedEventOption && lEvent != null && !shouldOverwrite)
                throw new AlreadyExistsException($"Shortcut with that name already exists at position X: {lEvent.ButtonX} Y:{lEvent.ButtonY}");

            RemoveEvent(option);
            events.Add(IEvent);
            SaveEvents();
        }

        public void RemoveEvent(EventOption option)
        {
            if (option is NamedEventOption)
            {
                RemoveNamedEvent(option as NamedEventOption);
            }
            else
            {
                RemovePositionEvent(option as PositionEventOption);
            }
        }

        private void RemoveNamedEvent(NamedEventOption option)
        {
            events.ToList().Remove(events.FirstOrDefault(x =>
                x.ProfileName == option.ProfileName && x.EventName == option.EventName));
        }

        private void RemovePositionEvent(PositionEventOption option)
        {
            events.ToList().Remove(events.FirstOrDefault(x =>
                x.ProfileName == option.ProfileName && x.ButtonX == option.ButtonX && x.ButtonY == option.ButtonY));
        }

        private void SaveEvents()
        {
            if (!Directory.Exists(Path.GetDirectoryName(keyboardEventPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(keyboardEventPath));

            if (!Directory.Exists(Path.GetDirectoryName(processEventPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(processEventPath));

            using (var sw = new StreamWriter(keyboardEventPath, false))
            {
                var keyboardEvents = events.Where(x => x.EventType == EventType.Keyboard);
                sw.WriteLine(JsonConvert.SerializeObject(keyboardEvents));
            }

            using (var sw = new StreamWriter(processEventPath, false))
            {
                var processEvents = events.Where(x => x.EventType == EventType.Process);
                sw.WriteLine(JsonConvert.SerializeObject(processEvents));
            }
        }

        private void LoadEvents()
        {
            if (File.Exists(keyboardEventPath))
            {
                using (var sr = new StreamReader(keyboardEventPath))
                {
                    var json = sr.ReadToEnd();
                    var tempEvents = JsonConvert.DeserializeObject<List<KeyboardEvent>>(json);
                    events.AddRange(tempEvents);
                }
            }

            if (File.Exists(processEventPath))
            {
                using (var sr = new StreamReader(processEventPath))
                {
                    var json = sr.ReadToEnd();
                    var tempEvents = JsonConvert.DeserializeObject<List<ProcessEvent>>(json);
                    events.AddRange(tempEvents);
                }
            }
        }
    }
}
