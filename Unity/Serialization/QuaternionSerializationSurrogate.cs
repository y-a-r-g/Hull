using System.Runtime.Serialization;
using UnityEngine;
using Object = System.Object;

namespace Hull.Serialization {
    public sealed class QuaternionSerializationSurrogate : ISerializationSurrogate {
        public void GetObjectData(Object obj, SerializationInfo info, StreamingContext context) {
            var quaternion = (Quaternion)obj;
            info.AddValue("x", quaternion.x);
            info.AddValue("y", quaternion.y);
            info.AddValue("z", quaternion.z);
            info.AddValue("w", quaternion.w);
        }

        public Object SetObjectData(
            Object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector) {
            var quaternion = (Quaternion)obj;
            quaternion.x = (float)info.GetValue("x", typeof(float));
            quaternion.y = (float)info.GetValue("y", typeof(float));
            quaternion.z = (float)info.GetValue("z", typeof(float));
            quaternion.w = (float)info.GetValue("w", typeof(float));
            return quaternion;
        }
    }
}
