using Castle.DynamicProxy;
using Nexus.Client.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nexus.Client.CLI
{
    public class GameModeInterceptorSelector : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            if (IsGetter(method))
            {
                return interceptors;
            }
            else
            {
                return null;
            }
        }

        private bool IsGetter(MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
        }

        private bool IsSetter(MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
        }
    }

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
