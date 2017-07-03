using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// This property holds a list of nested properties. Nested properties can be observed as well.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    [Serializable]
    public class ListStateProperty<TItem> : AbstractListStateProperty<TItem> where TItem : IStateProperty {
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

        public ListStateProperty() { }
        public ListStateProperty(TItem[] value, bool doNotCopyReference = false) : base(value, doNotCopyReference) { }
        public ListStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected override void BindItems(IList<TItem> items)  {
            for (var i = 0; i < items.Count; i++) {
                var item = items[i];
                if (item != null) {
                    item.Container = this;
                    items[i] = item;
                }
            }
        }

        public override void Set(IList<TItem> value, bool doNotCopyReference = false) {
            if (value != null) {
                for (var i = 0; i < value.Count; i++) {
                    var item = value[i];
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
            return new ItemsEnumerator(Value.GetEnumerator());
        }

        protected override void ModifyChildren(ModificationType modificationType) {
            if (Value != null) {
                for (var i = 0; i < Value.Count; i++) {
                    var item = Value[i];
                    ModifyChild(item, modificationType);
                    Value[i] = item;
                }
            }
        }

        public override void Add(TItem item) {
            item.Container = this;
            base.Add(item);
        }

        public override bool Remove(TItem item) {
            var result = base.Remove(item);
            if (result) {
                item.Container = null;
            }
            return result;
        }

        public override void Clear() {
            if (Value != null) {
                for (var i = 0; i < Value.Count; i++) {
                    var item = Value[i];
                    item.Container = null;
                }
            }
            base.Clear();
        }

        public override void Insert(int index, TItem item) {
            item.Container = this;
            base.Insert(index, item);
        }

        public override void RemoveAt(int index) {
            var item = Value[index];
            if (item != null) {
                item.Container = null;
            }
            base.RemoveAt(index);
        }

        protected override void SetDeserializedContainerToChildren() {
            base.SetDeserializedContainerToChildren();
            for (var i = 0; i < Value.Count; i++) {
                if (Value != null) {
                    Value[i].DeserializedContainer = this;
                }
            }
        }
    }
}
