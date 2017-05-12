using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Hull.Serialization {
    public static class SerializationUtils {
        private static BinaryFormatter _formatter;

        public static BinaryFormatter BinaryFormatter {
            get {
                if (_formatter == null) {
                    _formatter = new BinaryFormatter();
                    var surrogateSelector = new SurrogateSelector();
                    surrogateSelector.AddSurrogate(
                        typeof(Vector3),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector3SerializationSurrogate());
                    surrogateSelector.AddSurrogate(
                        typeof(Quaternion),
                        new StreamingContext(StreamingContextStates.All),
                        new QuaternionSerializationSurrogate());
                    surrogateSelector.AddSurrogate(
                        typeof(Bounds),
                        new StreamingContext(StreamingContextStates.All),
                        new BoundsSerializationSurrogate());
                    _formatter.SurrogateSelector = surrogateSelector;
                }
                return _formatter;
            }
        }

        static SerializationUtils() {
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        }
    }
}
