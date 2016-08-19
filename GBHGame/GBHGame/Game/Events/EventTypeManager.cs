using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GBH
{
    public static class EventTypeManager
    {
        private class EventTypeData
        {
            public Type Type { get; set; }
            public int EventCode { get; set; }
        }

        private static Dictionary<int, EventTypeData> _registeredEvents = new Dictionary<int, EventTypeData>();

        static EventTypeManager()
        {
            ScanEntityTypes(Assembly.GetExecutingAssembly());
        }

        private static void ScanEntityTypes(Assembly assembly)
        {
            var eventTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GameEvent)));

            foreach (Type eventType in eventTypes)
            {
                if (!eventType.IsAbstract)
                {
                    int typeCode = eventType.GetCustomAttribute<EventTypeAttribute>().TypeCode;

                    _registeredEvents.Add(typeCode, new EventTypeData
                    {
                        Type = eventType,
                        EventCode = typeCode
                    });
                }
            }
        }

        public static GameEvent CreateEvent(int eventCode)
        {
            EventTypeData eventData;

            if (_registeredEvents.TryGetValue(eventCode, out eventData))
            {
                return (GameEvent)Activator.CreateInstance(eventData.Type);
            }

            return null;
        }
    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class EventTypeAttribute : Attribute
    {
        readonly int typeCode;

        // This is a positional argument
        public EventTypeAttribute(int typeCode)
        {
            this.typeCode = typeCode;
        }

        public int TypeCode
        {
            get { return typeCode; }
        }
    }
}
