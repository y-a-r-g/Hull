using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    internal class PlaceholderStatePropertyContainer : AbstractStatePropertyContainer {
        public delegate void ModifyChildrenDelegate(ModificationType modificationType);

        public delegate IEnumerator<IStateProperty> GetChildrenEnumeratorDelegate();

        public ModifyChildrenDelegate ModifyChildrenImpl;
        public GetChildrenEnumeratorDelegate GetChildrenEnumeratorImpl;

        public PlaceholderStatePropertyContainer() { }

        public PlaceholderStatePropertyContainer(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        protected override void ModifyChildren(ModificationType modificationType) {
            ModifyChildrenImpl(modificationType);
        }

        public override IEnumerator<IStateProperty> GetChildrenEnumerator() {
            return GetChildrenEnumeratorImpl();
        }

        public new void ModifyChild(IStateProperty child, ModificationType modificationType) {
            base.ModifyChild(child, modificationType);
        }
    }
}
