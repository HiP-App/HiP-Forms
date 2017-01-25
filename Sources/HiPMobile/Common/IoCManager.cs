// /*
//  * Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  *
//  * Licensed under the Apache License, Version 2.0 (the "License");
//  * you may not use this file except in compliance with the License.
//  * You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0
//  *
//  * Unless required by applicable law or agreed to in writing, software
//  * distributed under the License is distributed on an "AS IS" BASIS,
//  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  * See the License for the specific language governing permissions and
//  * limitations under the License.
//  */


using System;
using System.Linq;
using Microsoft.Practices.Unity;

namespace de.upb.hip.mobile.pcl.Common {
    /// <summary>
    /// This class manages the access to the Inversion of Control container for the application.
    /// </summary>
    public static class IoCManager {

        static IoCManager()
        {
            Instance = new UnityContainer();
        }

        private static UnityContainer Instance { get; set; }

        public static void RegisterType<TSuper, TSub>() where TSub : TSuper
        {
            Instance.RegisterType<TSuper, TSub>();
        }

        public static void RegisterInstance(Type interf, object impl)
        {
            Instance.RegisterInstance(interf, impl);
        }

        public static T Resolve<T>()
        {
            return Instance.Resolve<T>();
        }

        public static void Clear ()
        {
            foreach (var registration in Instance.Registrations
                                                 .Where (p => p.RegisteredType == typeof (object)
                                                              && p.LifetimeManagerType == typeof (ContainerControlledLifetimeManager)))
            {
                registration.LifetimeManager.RemoveValue ();
            }
        }


    }
}