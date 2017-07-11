using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameClient.Observers;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState.Properties;

namespace Hull.GameServer.ServerState {
    /// <summary>
    /// State is a main thing of the client-server interaction. State contains all the information to restore game in any moment.
    /// Only server can change the state. Client should send requests to the server to ask server to change the state.
    /// Since state is <see cref="AbstractComplexStateProperty"/> it can hold only fields that implement <see cref="IStateProperty"/> interface.
    /// To create your game state - inherit it from this class and add all required fields.
    /// Any state properrty, when changed, will be marked as changes and also will mark as changed its container and container of container etc., up to state. So if any state was changed - state will be marked as changed.
    /// Observers can be used to react on property changes on client side. <seealso cref="StatePropertyObserver{TState,TProperty}"/>
    /// </summary>
    [Serializable]
    public abstract class State : AbstractComplexStateProperty, IDeserializationCallback {
        private readonly List<IStateChangeInfo> _changeInfo = new List<IStateChangeInfo>();
        private ulong _stateUpdateId;

        /// <summary>
        /// Initialized the new state
        /// </summary>
        public State() {
            Container = this;
        }

        protected State(SerializationInfo info, StreamingContext context) : base(info, context) {
            _changeInfo = (List<IStateChangeInfo>)info.GetValue("_changeInfo", typeof(List<IStateChangeInfo>));
            _stateUpdateId = (ulong)info.GetValue("_stateUpdateId", typeof(ulong));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_changeInfo", _changeInfo, typeof(List<IStateChangeInfo>));
            info.AddValue("_stateUpdateId", _stateUpdateId, typeof(ulong));
        }

        /// <summary>
        /// This field holds server's tick number from the start of the game. Note: it can be used aoly to compare to other UpdateId's since it can be reset at any time. 
        /// </summary>
        public new ulong UpdateId {
            get { return _stateUpdateId; }
        }

        /// <summary>
        /// Returns <value>true</value> if state is modified since last tick.
        /// </summary>
        public override bool IsModified {
            get { return base.UpdateId == UpdateId; }
        }

        /// <summary>
        /// Returns <value>true</value> if modification of the state is prohibited. 
        /// It will be always <value>false</value> when <see cref="IUpdater{TState,TRuntime}.Update"/> or <see cref="IRequestProcessor{TState,TRuntime,TRequest}.ProcessRequest"/> method called.
        /// It will be always <value>true</value> for client. 
        /// </summary>
        public bool IsReadonly { get; private set; }

        internal void BeginUpdate() {
            unchecked {
                _stateUpdateId++;
            }
            IsReadonly = false;
            _changeInfo.Clear();
        }

        internal void EndUpdate() {
            IsReadonly = true;
        }

        /// <summary>
        /// Server can add any additional information when changed the state. 
        /// This ingormation will be whiped at the beginning of the next tick. 
        /// </summary>
        /// <param name="changeInfo"></param>
        /// <exception cref="InvalidOperationException">Trying to modify state in readonly phase</exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddChangeInfo(IStateChangeInfo changeInfo) {
            if (IsReadonly) {
                throw new InvalidOperationException("State is readonly");
            }
            if (changeInfo == null) {
                throw new ArgumentNullException("changeInfo");
            }
            _changeInfo.Add(changeInfo);
        }

        /// <summary>
        /// All change info items that was added since last tick
        /// </summary>
        public IEnumerable<IStateChangeInfo> ChangeInfo {
            get { return _changeInfo; }
        }

        protected override State CurrentState {
            get { return this; }
        }

        public void OnDeserialization(object sender) {
            DeserializedContainer = this;
        }
    }
}
