
using System;

namespace signalR
{
    public class HubConnectionBuilder
    {
        private string v;

        public HubConnectionBuilder()
        {
            //var connection = signalR.HubConnectionBuilder("/chat").build();
        }

        //public object WithUrl(string v, Action<object> value)
        //{
        //    throw new NotImplementedException();
        //}

        public object WithUrl(string v)
        {
            this.v = v;
            return this;
        }
    }
}