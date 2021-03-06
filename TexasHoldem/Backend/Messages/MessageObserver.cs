﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Obser;
using System.Net;
using System.Net.Sockets;

namespace Backend.Messages
{
    public class MessageObserver
    {

        private List<ObserverAbstract<TcpClient>> playersList;

        public MessageObserver()
        {
            playersList = new List<ObserverAbstract<TcpClient>>();
        }

        public void Subscribe(ObserverAbstract<TcpClient> p)
        {
            playersList.Add(p);
        }

        public void Unsubscribe(ObserverAbstract<TcpClient> p)
        {
            playersList.Remove(p);
        }

        public void Update(object obj)
        {
            foreach (ObserverAbstract<TcpClient> p in playersList)
            {
                p.update(obj);
            }
        }
    }

}
