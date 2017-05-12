using System;
using System.Runtime.Serialization;
using Hull.Extensions;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    public abstract class AbstractComplexStateProperty : AbstractStatePropertyContainer {
        public override State State {
            protected get { return base.State; }
            set {
                base.State = value;

                var type = GetType();
                foreach (var field in type.GetFields()) {
                    if (!field.IsStatic) {
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
                        var prop = ((IStateProperty)field.GetValue(this));
                        prop.State = value;
                        prop.Container = this;
                    }
                }
            }
        }

        public AbstractComplexStateProperty() {
            var type = GetType();
            foreach (var field in type.GetFields()) {
                if (!field.IsStatic) {
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
        }

        public AbstractComplexStateProperty(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
