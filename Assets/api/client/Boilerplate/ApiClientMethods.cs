﻿using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace api
{
    public static partial class Client
    {
        const string SERVER_ADDR = "192.168.1.70";
        const int SERVER_PORT = 80;

        /// <summary>
        /// Starts all the necessary things and joins the server.
        /// </summary>
        public static void StartClient()
        {
            AddPlugins();
            ServerEndpoint = new IPEndPoint(IPAddress.Parse(SERVER_ADDR), SERVER_PORT);
        }
    }
}