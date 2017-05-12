using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    public abstract class AbstractStatePropertyContainer : AbstractStateProperty, IStatePropertyContainer {
        public AbstractStatePropertyContainer() { }

        public AbstractStatePropertyContainer(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public new void Modify() {
            base.Modify();
            if (State != null) {
                State.Modify();
            }
        }

        protected void BindItems<TItem>(TItem[] items) where TItem : IStateProperty {
            for (var i = 0; i < items.Length; i++) {
                var item = items[i];
                if (item != null) {
                    item.State = State;
                    item.Container = this;
                    items[i] = item;
                }
            }
        }
        
        protected void BindItems<TItem>(List<TItem> items) where TItem : IStateProperty {
            for (var i = 0; i < items.Count; i++) {
                var item = items[i];
                if (item != null) {
                    item.State = State;
                    item.Container = this;
                    items[i] = item;
                }
            }
        }
    }
}
