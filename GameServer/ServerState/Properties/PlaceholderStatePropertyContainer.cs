using System;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    internal class PlaceholderStatePropertyContainer : AbstractStatePropertyContainer {
        public delegate void ModifyChildrenDelegate(ModificationType modificationType);

        public ModifyChildrenDelegate ModifyChildrenImpl;

        public PlaceholderStatePropertyContainer() { }

        public PlaceholderStatePropertyContainer(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        protected override void ModifyChildren(ModificationType modificationType) {
            ModifyChildrenImpl(modificationType);
        }

        public new void ModifyChild(IStateProperty child, ModificationType modificationType) {
            base.ModifyChild(child, modificationType);
        }
    }
}
