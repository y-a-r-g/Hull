using System;
using System.Collections.Generic;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;
#if UNITY_5
using UnityEngine;

#endif

namespace Hull.GameServer {
    public class GameProcessor {
        private readonly Dictionary<Type, IRequestProcessor> _requestProcessors =
            new Dictionary<Type, IRequestProcessor>();

        private readonly Queue<RequestLiteItem> _requestsLine = new Queue<RequestLiteItem>();
        private readonly State _state;
        private readonly IServerRuntime _runtime;
        private readonly List<IUpdater> _updaters = new List<IUpdater>();

        public event Action<State> StateChanged;

#if !UNITY_5
        private const float dt = 1f / 30;
#endif

        public GameProcessor(State initialState, IServerRuntime runtime) {
            if (initialState == null) {
                throw new ArgumentNullException();
            }

            initialState.ForceModify();
            initialState.EndUpdate();
            _state = initialState;
            _runtime = runtime;

#if UNITY_5
            var updater = new GameObject().AddComponent<UnityUpdater>();
            updater.StartAction = Start;
            updater.FixedUpdateAction = FixedUpdate;
#else
            var timer = new Timer {
                Interval = dt,
                AutoReset = true,
                Enabled = true
            };
            var initialized = false;

            timer.Elapsed += (sender, e) => {
                if (!initialized) {
                    Start();
                    initialized = true;
                }
                Update();
            };
#endif
        }

        private void Start() {
            SendStateChange();
        }

        private void FixedUpdate() {
            _state.BeginUpdate();
            while (_requestsLine.Count > 0) {
                var item = _requestsLine.Dequeue();
                IRequestProcessor processor;
                if (!_requestProcessors.TryGetValue(item.Request.GetType(), out processor)) {
                    throw new ArgumentOutOfRangeException(
                        string.Format("Processor for <{0}> request is not registered", item.Request.GetType()));
                }
                processor.ProcessRequest(item.Request, item.Player, _state, _runtime);
            }
#if UNITY_5
            var dt = Time.deltaTime;
#endif
            foreach (var updater in _updaters) {
                updater.Update(_state, _runtime, dt);
            }
            _state.EndUpdate();

            SendStateChange();
        }

        public void RegisterProcessor<TRequest>(IRequestProcessor processor) where TRequest : IRequest {
            if (processor == null) {
                throw new ArgumentNullException();
            }
            _requestProcessors[typeof(TRequest)] = processor;
        }

        public void ProcessRequest(IRequest request, IPlayer player) {
            if (request == null) {
                throw new ArgumentNullException();
            }
            _requestsLine.Enqueue(new RequestLiteItem {Request = request, Player = player});
        }

        public void AddUpdater(IUpdater updater) {
            if (updater == null) {
                throw new ArgumentNullException();
            }
            _updaters.Add(updater);
        }

        private void SendStateChange() {
            if (_state.IsModified) {
                if (StateChanged != null) {
                    StateChanged(_state);
                }
            }
        }
    }
}
