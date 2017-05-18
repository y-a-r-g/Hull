﻿using System;
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
                fieldInfo.SetValue(this, info.GetValue(fieldInfo.Name, fieldInfo.FieldType));
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            foreach (var fieldInfo in _fields) {
                info.AddValue(fieldInfo.Name, fieldInfo.GetValue(this), fieldInfo.FieldType);
            }
        }

        public override IStatePropertyContainer Container {
            get { return base.Container; }
            set {
                base.Container = value;
                if (_fields != null) {
                    foreach (var field in _fields) {
                        var prop = ((IStateProperty)field.GetValue(this));
                        if (prop != null) {
                            prop.Container = this;
                        }
                    }
                }
            }
        }

        public override void ForceModify() {
            base.ForceModify();
            foreach (var field in _fields) {
                var prop = ((IStateProperty)field.GetValue(this));
                prop.ForceModify();
            }
        }
    }
}