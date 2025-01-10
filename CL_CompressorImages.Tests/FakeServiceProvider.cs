using System;

using Microsoft.Xrm.Sdk;

namespace CL_CompressorImages.Tests
{

    public class FakeServiceProvider : IServiceProvider
    {
        private readonly IPluginExecutionContext _pluginExecutionContext;

        public FakeServiceProvider(IPluginExecutionContext pluginExecutionContext)
        {
            _pluginExecutionContext = pluginExecutionContext;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IPluginExecutionContext))
            {
                return _pluginExecutionContext;
            }

            // Puedes extenderlo para manejar otros servicios si es necesario
            return null;
        }
    }

}
