using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.Extensions;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState.Properties;

namespace Hull.GameServer.ServerState {
    [Serializable]
    public class State : AbstractStatePropertyContainer {
        private readonly StateParts _parts = new StateParts();
        private readonly List<IStateChangeInfo> _changeInfo = new List<IStateChangeInfo>();
        private ulong _stateUpdateId;

        public State() {
            _parts.CurrentState = this;
        }

        protected State(SerializationInfo info, StreamingContext context) : base(info, context) {
            _parts = (StateParts)info.GetValue("_parts", typeof(StateParts));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_parts", _parts, typeof(StateParts));
        }

        public T GetPart<T>() where T : IStatePart {
            return (T)GetPart(typeof(T));
        }

        public IStatePart GetPart(Type partType) {
            return _parts[partType];
        }

        public T AddPart<T>() where T : IStatePart {
            if (IsReadonly) {
                throw new AccessViolationException();
            }
            return (T)AddPart(typeof(T));
        }

        public IStatePart AddPart(Type partType) {
            if (partType.GetInterfaces().IndexOf(typeof(IStatePart)) == -1) {
                throw new ArgumentException(
                    string.Format("<{0}> should implement <{1}>", partType.Name, typeof(IStatePart).Name));
            }
            if (_parts.ContainsKey(partType)) {
                throw new ArgumentException(string.Format("State already contains part <{0}>", partType.Name));
            }
            var statePart = (IStatePart)Activator.CreateInstance(partType);
            _parts[partType] = statePart;
            statePart.Container = this;
            return statePart;
        }

        public bool HasPart<T>() where T : IStatePart {
            return HasPart(typeof(T));
        }

        public bool HasPart(Type partType) {
            return _parts.ContainsKey(partType);
        }

        public new ulong UpdateId {
            get { return _stateUpdateId; }
        }

        public override bool IsModified {
            get { return base.UpdateId == UpdateId; }
        }

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

        internal void AddChangeInfo(IStateChangeInfo changeInfo) {
            _changeInfo.Add(changeInfo);
        }

        public IEnumerable<IStateChangeInfo> ChangeInfo {
            get { return _changeInfo; }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            _parts.CurrentState = this;
        }

        protected override State CurrentState {
            get { return this; }
        }

        public override void ForceModify() {
            foreach (var kvp in _parts) {
                kvp.Value.ForceModify();
            }
        }
    }
}
