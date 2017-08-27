using System.Reflection;
using Hull.Extensions;
using UnityEngine;

namespace Hull.Unity.Types {
    public class CachedComponentsBehaviour : MonoBehaviour, ISerializationCallbackReceiver {
        public virtual void OnBeforeSerialize() {
            FillFields();
        }

        public virtual void OnAfterDeserialize() { }

        protected virtual void Awake() {
            FillFields();
        }

        protected void FillFields() {
            foreach (var fieldInfo in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                if (fieldInfo.GetCustomAttributes(true).Contains(element => element.GetType() == typeof(CacheComponentAttribute))) {
                    if (fieldInfo.GetValue(this) == null) {
                        fieldInfo.SetValue(this, gameObject.GetComponent(fieldInfo.FieldType));
                    }
                }
            }
        }
    }
}
