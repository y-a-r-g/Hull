using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Hull.Extensions;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public abstract class AbstractComplexStateProperty : AbstractStatePropertyContainer {
        private readonly FieldInfo[] _fields;

        private struct FieldsEnumerator : IEnumerator<IStateProperty> {
            private readonly IEnumerator<FieldInfo> _enumerator;
            private readonly AbstractComplexStateProperty _container;

            public FieldsEnumerator(IEnumerator<FieldInfo> enumerator, AbstractComplexStateProperty container) {
                _enumerator = enumerator;
                _container = container;
            }

            public void Dispose() {
                _enumerator.Dispose();
            }

            public bool MoveNext() {
                return _enumerator.MoveNext();
            }

            public void Reset() {
                _enumerator.Reset();
            }

            public IStateProperty Current {
                get { return (IStateProperty)_enumerator.Current.GetValue(_container); }
            }

            object IEnumerator.Current {
                get { return Current; }
            }
        }

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

        public override IEnumerator<IStateProperty> GetChildrenEnumerator() {
            return new FieldsEnumerator(((IEnumerable<FieldInfo>)_fields).GetEnumerator(), this);
        }
    }
}
