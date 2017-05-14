using System.Runtime.Serialization;
using UnityEngine;
using Object = System.Object;

namespace Hull.Unity.Serialization {
    public sealed class BoundsSerializationSurrogate : ISerializationSurrogate {
        public void GetObjectData(Object obj, SerializationInfo info, StreamingContext context) {
            var bounds = (Bounds)obj;
            info.AddValue("center", bounds.center);
            info.AddValue("size", bounds.size);
        }

        public Object SetObjectData(
            Object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector) {
            var bounds = (Bounds)obj;
            bounds.center = (Vector3)info.GetValue("center", typeof(Vector3));
            bounds.size = (Vector3)info.GetValue("size", typeof(Vector3));
            return bounds;
        }
    }
}
