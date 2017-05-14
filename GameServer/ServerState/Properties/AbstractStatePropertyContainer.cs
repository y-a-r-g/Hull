using System;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public abstract class AbstractStatePropertyContainer : AbstractStateProperty, IStatePropertyContainer {
        protected AbstractStatePropertyContainer() { }

        protected AbstractStatePropertyContainer(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        protected void BindItems<TItem>(TItem[] items) where TItem : IStateProperty {
            for (var i = 0; i < items.Length; i++) {
                var item = items[i];
                if (item != null) {
                    item.Container = this;
                    items[i] = item;
                }
            }
        }

        public new void Modify() {
            base.Modify();
        }
    }
}