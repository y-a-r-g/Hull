using System.Runtime.Serialization;
using UnityEngine;
using Object = System.Object;

namespace Hull.Unity.Serialization {
    public sealed class Vector4SerializationSurrogate : ISerializationSurrogate {
        public void GetObjectData(Object obj, SerializationInfo info, StreamingContext context) {
            var vector = (Vector4)obj;
            info.AddValue("x", vector.x);
            info.AddValue("y", vector.y);
            info.AddValue("z", vector.z);
            info.AddValue("w", vector.w);
        }

        public Object SetObjectData(
            Object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector) {
            var vector = (Vector4)obj;
            vector.x = (float)info.GetValue("x", typeof(float));
            vector.y = (float)info.GetValue("y", typeof(float));
            vector.z = (float)info.GetValue("z", typeof(float));
            vector.w = (float)info.GetValue("w", typeof(float));
            return vector;
        }
    }
}