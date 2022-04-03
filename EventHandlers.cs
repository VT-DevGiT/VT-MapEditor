using Synapse;
using System;

namespace MapEditor
{
    public class EventHandlers
    {
        public EventHandlers()
        {
            Server.Get.Events.Round.WaitingForPlayersEvent += OnWhaiting;
        }

        private void OnWhaiting()
        {
            
        }
    }
}