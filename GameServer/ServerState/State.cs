using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.Extensions;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState {
    [Serializable]
    public class State : ISerializable {
        private readonly Dictionary<Type, IStatePart> _parts = new Dictionary<Type, IStatePart>();
        private List<IStateChangeInfo> _changeInfo = new List<IStateChangeInfo>();

        public State() {
            BeginUpdate();
        }

        public State(SerializationInfo info, StreamingContext context) {
            _parts = (Dictionary<Type, IStatePart>)info.GetValue(
                "_parts", typeof(Dictionary<Type, IStatePart>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("_parts", _parts, typeof(Dictionary<Type, IStatePart>));
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
                throw new ArgumentException(string.Format("<{0}> should implement <{1}>", partType.Name, typeof(IStatePart).Name));
            }
            if (_parts.ContainsKey(partType)) {
                throw new ArgumentException(string.Format("State already contains part <{0}>", partType.Name));
            }
            var statePart = (IStatePart)Activator.CreateInstance(partType);
            statePart.State = this;
            _parts[partType] = statePart;
            return statePart;
        }

        public bool HasPart<T>() where T : IStatePart {
            return HasPart(typeof(T));
        }

        public bool HasPart(Type partType) {
            return _parts.ContainsKey(partType);
        }

        public ulong UpdateId { get; private set; }

        public bool IsReadonly { get; private set; }

        internal void BeginUpdate() {
            unchecked {
                UpdateId++;
            }
            IsReadonly = false;
            _changeInfo.Clear();
        }

        internal void EndUpdate() {
            IsReadonly = true;
        }
        
        private ulong _modifiedUpdateId;
        public bool IsModified { get { return _modifiedUpdateId == UpdateId; } }

        internal void Modify() {
            _modifiedUpdateId = UpdateId;
        }

        internal void AddChangeInfo(IStateChangeInfo changeInfo) {
            _changeInfo.Add(changeInfo);
        }

        public IEnumerable<IStateChangeInfo> ChangeInfo {
            get { return _changeInfo; }
        }
    }
}
