﻿/*
 * Program.cs
 *
 * Copyright (C) 2010-2015 by Microsoft Corporation
 *
 * This program is licensed to you under the terms of Version 2.0 of the
 * Apache License. This program is distributed WITHOUT
 * ANY EXPRESS OR IMPLIED WARRANTY, INCLUDING THOSE OF NON-INFRINGEMENT,
 * MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE. Please refer to the
 * Apache License 2.0 (http://www.apache.org/licenses/LICENSE-2.0) for more details.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeployR;

namespace Background
{
    class Program
    {
        public const String AUTH_USER_NAME = "testuser";
        public const String AUTH_PASSWORD = "changeme";
        public const String DEPLOYR_ENDPOINT = "http://localhost:7400/deployr";

        static void Main(string[] args)
        {
            //RClientFactory.setDebugMode(true);  //un-comment this line to see the HTTP calls on the API
            BackgroundBasics.Execute();
        }
    }
}
