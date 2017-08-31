using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// Inherit this class to create a property that can hold other properties.
    /// </summary>
    [Serializable]
    public abstract class AbstractStatePropertyContainer : AbstractStateProperty, IStatePropertyContainer {
        protected AbstractStatePropertyContainer() { }

        protected AbstractStatePropertyContainer(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public sealed override IStatePropertyContainer Container {
            get { return base.Container; }
            set {
                if (base.Container != value) {
                    base.Container = value;
                    if (base.Container != null) {
                        ModifyChildren(ModificationType.Added);
                    }
                    else {
                        ModifyChildren(ModificationType.Removed);
                    }
                }
            }
        }

        /// <summary>
        /// Marks this property sa modified. Also marks all the holding items as modified.
        /// </summary>
        /// <param name="modificationType"></param>
        public void ModifyWithChildren(ModificationType modificationType) {
            Modify(modificationType);
            ModifyChildren(modificationType);
        }

        public abstract IEnumerator<IStateProperty> GetChildrenEnumerator();

        public virtual IStateProperty GetChildProperty(ulong uniqueId) {
            using (var e = GetChildrenEnumerator()) {
                while (e.MoveNext()) {
                    if (e.Current.UniqueId == uniqueId) {
                        return e.Current;
                    }
                }
            }

            return default(IStateProperty);
        }

        protected void ModifyChild(IStateProperty child, ModificationType modificationType) {
            if (child != null) {
                child.Container = this;
                var container = child as IStatePropertyContainer;
                if (container != null) {
                    container.ModifyWithChildren(modificationType);
                }
                else {
                    child.Modify(modificationType);
                }
            }
        }

        /// <summary>
        /// In this methoud property should iterate over the children and call <see cref="ModifyChild"/> for each item.
        /// </summary>
        /// <param name="modificationType"></param>
        protected abstract void ModifyChildren(ModificationType modificationType);

        protected abstract void SetDeserializedContainerToChildren();

        public override IStatePropertyContainer DeserializedContainer {
            set {
                base.DeserializedContainer = value;
                SetDeserializedContainerToChildren();
            }
        }
    }
}