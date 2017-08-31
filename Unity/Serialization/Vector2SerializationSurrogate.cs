using System.Runtime.Serialization;
using UnityEngine;
using Object = System.Object;

namespace Hull.Unity.Serialization {
    public sealed class Vector2SerializationSurrogate : ISerializationSurrogate {
        public void GetObjectData(Object obj, SerializationInfo info, StreamingContext context) {
            var vector = (Vector2)obj;
            info.AddValue("x", vector.x);
            info.AddValue("y", vector.y);
        }

        public Object SetObjectData(
            Object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector) {
            var vector = (Vector2)obj;
            vector.x = (float)info.GetValue("x", typeof(float));
            vector.y = (float)info.GetValue("y", typeof(float));
            return vector;
        }
    }
}