using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// This property holds an array of nested properties. Nested properties can be observed as well.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    [Serializable]
    public class ArrayStateProperty<TItem> : AbstractArrayStateProperty<TItem> where TItem : IStateProperty {
        private struct ItemsEnumerator : IEnumerator<IStateProperty> {
            private readonly IEnumerator<TItem> _enumerator;

            public ItemsEnumerator(IEnumerator<TItem> enumerator) {
                _enumerator = enumerator;
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
                get { return _enumerator.Current; }
            }

            object IEnumerator.Current {
                get { return Current; }
            }
        }

        public ArrayStateProperty() { }
        public ArrayStateProperty(TItem[] value, bool doNotCopyReference = false) : base(value, doNotCopyReference) { }
        public ArrayStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected override void BindItems(TItem[] items) {
            for (var i = 0; i < items.Length; i++) {
                var item = items[i];
                if (item != null) {
                    item.Container = this;
                    items[i] = item;
                }
            }
        }

        public override void Set(TItem[] value, bool doNotCopyReference = false) {
            if (Value != null) {
                for (var i = 0; i < Value.Length; i++) {
                    var item = Value[i];
                    if (item != null) {
                        item.Container = null;
                    }
                }
            }

            base.Set(value, doNotCopyReference);
        }

        public override TItem this[int index] {
            get { return base[index]; }
            set {
                if (Value[index] != null) {
                    Value[index].Container = null;
                }
                value.Container = this;
                base[index] = value;
            }
        }

        public override IEnumerator<IStateProperty> GetChildrenEnumerator() {
            return new ItemsEnumerator(((IEnumerable<TItem>)Value).GetEnumerator());
        }

        protected override void ModifyChildren(ModificationType modificationType) {
            if (Value != null) {
                for (var i = 0; i < Value.Length; i++) {
                    var item = Value[i];
                    ModifyChild(item, modificationType);
                    Value[i] = item;
                }
            }
        }

        protected override void SetDeserializedContainerToChildren() {
            base.SetDeserializedContainerToChildren();
            for (var i = 0; i < Value.Length; i++) {
                if (Value != null) {
                    Value[i].DeserializedContainer = this;
                }
            }
        }
    }
}