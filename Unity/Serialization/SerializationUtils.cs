using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Hull.Unity.Serialization {
    public static class SerializationUtils {
        private static IFormatter _formatter;

        public static IFormatter BinaryFormatter {
            get {
                if (_formatter == null) {
                    _formatter = new BinaryFormatter();
                    var surrogateSelector = new SurrogateSelector();
                    surrogateSelector.AddSurrogate(
                        typeof(Vector2),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector2SerializationSurrogate());
                    surrogateSelector.AddSurrogate(
                        typeof(Vector3),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector3SerializationSurrogate());
                    surrogateSelector.AddSurrogate(
                        typeof(Vector4),
                        new StreamingContext(StreamingContextStates.All),
                        new Vector4SerializationSurrogate());
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
