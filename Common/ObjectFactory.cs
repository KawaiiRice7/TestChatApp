using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Common
{
    public class ObjectFactory
    {
        private static UnityContainer container = new UnityContainer();

        public static void RegisterInstance<TInterface>(TInterface instance)
        {
            container.RegisterInstance<TInterface>(instance);
        }

        public static TInterface Resolve<TInterface>()
        {
            if (container.IsRegistered<TInterface>())
                return container.Resolve<TInterface>();

            else
                throw new Exception();
        }
    }
}
