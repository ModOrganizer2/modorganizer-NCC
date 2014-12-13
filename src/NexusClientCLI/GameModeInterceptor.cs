using Castle.DynamicProxy;
using Nexus.Client.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.Client.CLI
{
    [Serializable]
    class GameModeInterceptor : IInterceptor
    {
        private List<string> m_AdditionalWritablePaths;
        private Version m_ExtenderVersion;

        public GameModeInterceptor(List<string> additionalWritablePaths, Version extenderVersion)
        {
            m_AdditionalWritablePaths = additionalWritablePaths;
            m_ExtenderVersion = extenderVersion;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            if (invocation.Method.Name == "get_WritablePaths")
            {
                IEnumerable<string> temp = (IEnumerable<string>)invocation.ReturnValue;
                invocation.ReturnValue = temp.Concat(m_AdditionalWritablePaths);
            }
            else if (invocation.Method.Name == "get_ScriptExtenderVersion")
            {
                if (invocation.ReturnValue == null)
                    invocation.ReturnValue = m_ExtenderVersion;
            }
        }
    }
}
