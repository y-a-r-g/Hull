﻿using System;
using System.Runtime.Serialization;
using Hull.Collections;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// Property thet holds <see cref="LinearMap{T}"/> of the other state properties. 
    /// Child properties as items can be observed.  
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class LinearMapStateProperty<TValue> : LinearMap<TValue>, IStatePropertyContainer
        where TValue : IStateProperty {
        private readonly PlaceholderStatePropertyContainer _property;

        /// <summary>
        /// Creates empty property
        /// </summary>
        public LinearMapStateProperty() {
            _property = new PlaceholderStatePropertyContainer();
            _property.ModifyChildrenImpl = ModifyChildrenImpl;
        }

        protected LinearMapStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) {
            _property = (PlaceholderStatePropertyContainer)info.GetValue(
                "_property", typeof(PlaceholderStatePropertyContainer));
            _property.ModifyChildrenImpl = ModifyChildrenImpl;
            BindItems();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_property", _property, typeof(PlaceholderStatePropertyContainer));
        }

        private void BindItems() {
            for (var i = 0; i < _items.Count; i++) {
                var item = _items[i];
                if (item != null) {
                    item.Container = this;
                    _items[i] = item;
                }
            }
        }

        /// <summary>
        /// Adds new items. This property will be marked as <value>Changed</value>, added property will be marked as <value>Added</value>.
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <returns>New id assiged to added item</returns>
        public override LinearMapId Add(TValue item) {
            Modify(ModificationType.Changed);
            if (item != null) {
                item.Container = this;
            }
            return base.Add(item);
        }

        /// <summary>
        /// Removes an item with given id. This property will be marked as <value>Changed</value>, removed property will be marked as <value>Removed</value>.
        /// </summary>
        /// <param name="id">Id of the item to remove</param>
        /// <returns>New id assiged to added item</returns>
        public override void Remove(LinearMapId id) {
            var item = this[id];
            if (item != null) {
                item.Container = null;
            }
            base.Remove(id);
        }

        /// <summary>
        /// Removes all the items.
        /// </summary>
        public override void Clear() {
            Modify(ModificationType.Changed);
            base.Clear();
        }

        /// <summary>
        /// Returns or replaces an item with given id. Will throw an <value>ArgumentOutOfRangeException</value> if item with this id is not exits.
        /// </summary>
        /// <param name="id">Id of the item</param>
        public override TValue this[LinearMapId id] {
            get { return base[id]; }
            set {
                if (Contains(id)) {
                    var item = this[id];
                    item.Container = null;
                }
                Modify(ModificationType.Changed);
                base[id] = value;
                if (value != null) {
                    value.Container = this;
                }
            }
        }

        /// <summary>
        /// Holds parent property for this one.
        /// </summary>
        public IStatePropertyContainer Container {
            get { return _property.Container; }
            set { _property.Container = value; }
        }

        /// <summary>
        /// Returns <value>true</value> if this property was changed since last tick
        /// </summary>
        public bool IsModified {
            get { return _property.IsModified; }
        }

        /// <summary>
        /// Holds last modification type of this property
        /// </summary>
        public ModificationType ModificationType {
            get { return _property.ModificationType; }
        }

        /// <summary>
        /// Mark this property as modified untill next tick. Also marks the Container.
        /// </summary>
        /// <param name="modificationType">Type of the applied modification</param>
        public void Modify(ModificationType modificationType) {
            _property.Modify(modificationType);
        }

        /// <summary>
        /// Marks this property sa modified. Also marks all the items that Linear Map holds.
        /// </summary>
        /// <param name="modificationType"></param>
        public void ModifyWithChildren(ModificationType modificationType) {
            _property.ModifyWithChildren(modificationType);
        }

        private void ModifyChildrenImpl(ModificationType modificationType) {
            foreach (var kvp in this) {
                _property.ModifyChild(kvp.Value, modificationType);
            }
        }
    }
}
