using System.Reflection;

namespace Hull.GameServer {
    internal sealed class RequestProcessorItem {
        public object RequestProcessor;
        public MethodInfo ProcessMethod;
    }
}
