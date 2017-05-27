using System;
using System.Reflection;
using System.Runtime.Serialization;
using Hull.Extensions;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public abstract class AbstractComplexStateProperty : AbstractStatePropertyContainer {
        private FieldInfo[] _fields;

        protected AbstractComplexStateProperty() {
            var type = GetType();
            _fields = type.GetFields().Filter(filed => !filed.IsStatic);

            foreach (var field in _fields) {
                if (field.FieldType.GetInterfaces().IndexOf(typeof(IStateProperty)) == -1) {
                    throw new ArgumentException(
                        string.Format(
                            "{0}.{1} should implement IStateProperty interface.",
                            type.Name,
                            field.Name));
                }
                if (!field.IsInitOnly) {
                    throw new ArgumentException(
                        string.Format("{0}.{1} should be readonly.", type.Name, field.Name));
                }
            }
        }

        protected AbstractComplexStateProperty(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            _fields = GetType().GetFields().Filter(filed => !filed.IsStatic);
            foreach (var fieldInfo in _fields) {
                object value;
                try {
                    value = info.GetValue(fieldInfo.Name, fieldInfo.FieldType);
                }
                catch (SerializationException) {
                    value = Activator.CreateInstance(fieldInfo.FieldType);
                }
                fieldInfo.SetValue(this, value);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            foreach (var fieldInfo in _fields) {
                info.AddValue(fieldInfo.Name, fieldInfo.GetValue(this), fieldInfo.FieldType);
            }
        }

        protected override void ModifyChildren(ModificationType modificationType) {
            if (_fields != null) {
                foreach (var fieldInfo in _fields) {
                    var field = (IStateProperty)fieldInfo.GetValue(this);
                    ModifyChild(field, modificationType);
                }
            }
        }
    }
}
