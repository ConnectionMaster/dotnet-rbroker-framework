﻿/*
 * DiscreteTaskBroker.cs
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

namespace DeployRBroker
{
    /// <summary>
    /// Represents a Discrete Broker implementation
    /// </summary>
    /// <remarks></remarks>
    public class DiscreteTaskBroker : RBrokerEngine
    {
        /// <summary>
        /// Constructor for specifying a Discrete Instance of RBroker
        /// </summary>
        /// <param name="brokerConfig">Discrete Broker Configuration object</param>
        /// <remarks></remarks>
        public DiscreteTaskBroker(DiscreteBrokerConfig brokerConfig)
            : base ((RBrokerConfig) brokerConfig)
        {
            m_rClient = RClientFactory.createClient(brokerConfig.deployrEndpoint, brokerConfig.maxConcurrentTaskLimit);

            if(brokerConfig.userCredentials != null) 
            {
                m_rUser = m_rClient.login(brokerConfig.userCredentials);
            } 
            else 
            {
                m_rUser = null;
            }

            /*
             * Prep the base RBrokerEngine.
             */

            initEngine(brokerConfig.maxConcurrentTaskLimit);

            /*
             * Initialize the resourceTokenPool with Integer
             * based resourceTokens.
             */
            for(int i=0; i< brokerConfig.maxConcurrentTaskLimit; i++) 
            {
                m_resourceTokenPool.TryAdd(i);
            }
        }

        /// <summary>
        /// Implementation of the refresh method on RBroker interface
        /// /// </summary>
        /// <param name="config">Discrete Broker Configuration object</param>
        /// <remarks></remarks>
        public new void refresh(RBrokerConfig config)
        {
            throw new Exception("DiscreteTaskBroker configuration refresh not supported.");
        }

        /// <summary>
        /// Implementation of the submit method on RBroker interface.  
        /// Allows applications to submit a new Task for execution
        /// </summary>
        /// <param name="task">RTask to be submitted for execution as a discrete task</param>
        /// <returns>RTaskToken for this RTask</returns>
        /// <remarks></remarks>
        public new RTaskToken submit(RTask task)
        {
            return submit(task, false);
        }

        /// <summary>
        /// Implementation of the submit method on RBroker interface.  
        /// Allows applications to submit a new Task for execution.
        /// </summary>
        /// <param name="task">RTask to be submitted for execution as a discrete task</param>
        /// <param name="priority">priority of task.  If TRUE, then task has a high priority</param>
        /// <returns>RTaskToken for this RTask</returns>
        /// <remarks></remarks>
        public new RTaskToken submit(RTask task, Boolean priority)
        {
            DiscreteTask discreteTask = (DiscreteTask) task;

            if(m_rUser == null && discreteTask.external != "") 
            {
                throw new Exception("External script task execution not permitted on anonymous broker.");
            }

            return submit(task, priority);
        }

        /// <summary>
        /// Returns a resource token for the task back to the token pool.  
        /// /// </summary>
        /// <param name="task">RTask submitted for execution as a background task</param>
        /// <param name="result">RTaskResult containing the results of the completed task</param>
        /// <remarks></remarks>
        public override void callback(RTask task, RTaskResult result) 
        {

            Object obj;

            m_taskResourceTokenMap.TryGetValue(task, out obj);
            int resourceToken = (int)obj;

            Boolean added = m_resourceTokenPool.TryAdd(resourceToken);

            if(!added) 
            {
                throw new Exception("DiscreteTaskBroker: callback, project could not be added back to pool, ???????.");
            }

        }

        /// <summary>
        /// createBrokerWorker override method for DiscreteTaskBroker.
        /// </summary>
        /// <remarks></remarks>
        protected override RBrokerWorker createBrokerWorker(RTask task,
                                                   long taskIndex,
                                                   Boolean isPriorityTask,
                                                   Object resourceToken,
                                                   RBrokerEngine brokerEngine) 
        {

            return new DiscreteTaskWorker((DiscreteTask) task,
                                        taskIndex,
                                        isPriorityTask,
                                        m_rClient,
                                        (int) resourceToken,
                                        (RBroker) brokerEngine);
        }

        /// <summary>
        /// cloneTask override method for DiscreteTaskBroker.
        /// </summary>
        /// <remarks></remarks>
        protected override RTask cloneTask(RTask genesis) 
        {

            DiscreteTask source  = (DiscreteTask) genesis;
            DiscreteTask clone = null;
            if(source.external != "") 
            {
                String externalURL = source.external;
                
                clone = new DiscreteTask(externalURL,
                                       source.options);
            } 
            else 
            {
                clone = new DiscreteTask(source.filename,
                                       source.directory,
                                       source.author,
                                       source.version,
                                       source.options);
            }
            clone.setToken(source.getToken());

            return clone;
        }
    
    }
}
