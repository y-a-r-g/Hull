using System;
using System.Collections.Generic;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;
#if UNITY_5_3_OR_NEWER
using UnityEngine;

#endif

namespace Hull.GameServer {
    /// <summary>
    /// Core part of the game server. Controls the launching of Request Processors and Updaters. Raises event when state changed.
    ///
    /// Every tick:
    /// All requests are processed in order they we received.
    /// All updaters are called in order they were added.
    /// <code>StateChanged</code> event is triggered, if state changed.
    /// 
    /// Request are not processed imppediately, they are stored in queue and called at the beginning of the next tick.
    ///
    /// </summary>
    /// <typeparam name="TState">Type of the server <see cref="State"/></typeparam>
    /// <typeparam name="TRuntime">Type of the server <see cref="IServerRuntime{TState}"/></typeparam>
    public class GameProcessor<TState, TRuntime> : IRequestReceiver<TState>
        where TState : State
        where TRuntime : IServerRuntime<TState> {
        private readonly Dictionary<Type, RequestProcessorItem> _requestProcessors =
            new Dictionary<Type, RequestProcessorItem>();

        private readonly Queue<RequestQueueItem<TState>> _requestsQueue =
            new Queue<RequestQueueItem<TState>>();

        private readonly TRuntime _runtime;
        private readonly List<IUpdater<TState, TRuntime>> _updaters = new List<IUpdater<TState, TRuntime>>();
        private TState _state;

#if !UNITY_5_3_OR_NEWER
        private const float dt = 1f / 30;
#endif

        /// <summary>
        /// Creates new processor with gigen state and runtime.
        /// </summary>
        /// <param name="initialState">Initial game state</param>
        /// <param name="runtime">Server runtime</param>
        /// <exception cref="ArgumentNullException">State or runtime is null</exception>
        public GameProcessor(TState initialState, TRuntime runtime) {
            if (initialState == null) {
                throw new ArgumentNullException("initialState");
            }
            if (runtime == null) {
                throw new ArgumentNullException("runtime");
            }

            initialState.ModifyWithChildren(ModificationType.Added);
            initialState.EndUpdate();
            State = initialState;
            _runtime = runtime;

            foreach (var player in runtime.Players) {
                player.OnRegister(this);
            }

#if UNITY_5_3_OR_NEWER
            var updater = new GameObject("Hull.UnityUpdater").AddComponent<UnityUpdater>();
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
            State.BeginUpdate();
            while (_requestsQueue.Count > 0) {
                var item = _requestsQueue.Dequeue();
                RequestProcessorItem processor;
                if (!_requestProcessors.TryGetValue(item.Request.GetType(), out processor)) {
                    throw new ArgumentOutOfRangeException(
                        string.Format("Processor for <{0}> request is not registered", item.Request.GetType()));
                }
                try {
                    processor.ProcessMethod.Invoke(
                        processor.RequestProcessor, new object[] {item.Request, item.Player, State, _runtime});
                }
                catch (Exception ex) {
                    Debug.Log(ex);
                }
            }
#if UNITY_5_3_OR_NEWER
            var dt = Time.deltaTime;
#endif
            foreach (var updater in _updaters) {
                updater.Update(State, _runtime, dt);
            }

            _runtime.UpdateCoroutines();

            State.EndUpdate();

            SendStateChange();
        }

        /// <summary>
        /// Associates a Request Processor with a Request. One Request may have only one Request Processor associated with.
        /// When Game Processor receives the request it will find associated Request Processor and all its <code>ProcessRequest</code> method. 
        /// </summary>
        /// <param name="processor">Associated Request Processor</param>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <exception cref="ArgumentNullException">Processor is null</exception>
        public void RegisterProcessor<TRequest>(IRequestProcessor<TState, TRuntime, TRequest> processor)
            where TRequest : IRequest {
            if (processor == null) {
                throw new ArgumentNullException("processor");
            }
            _requestProcessors[typeof(TRequest)] = new RequestProcessorItem {
                RequestProcessor = processor,
                ProcessMethod = processor.GetType().GetMethod("ProcessRequest")
            };
        }

        /// <summary>
        /// Adds a request to the queue. This request will be processed in the beginning of the next tick.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="player"></param>
        /// <exception cref="ArgumentNullException">Request is null</exception>
        public void ProcessRequest(IRequest request, IPlayer<TState> player) {
            if (request == null) {
                throw new ArgumentNullException("request");
            }
            _requestsQueue.Enqueue(new RequestQueueItem<TState> {Request = request, Player = player});
        }

        /// <summary>
        /// Adds an updater. Every tick its <code>Update</code> method will be called. Updaters are called in the order they were added.
        /// </summary>
        /// <param name="updater"></param>
        /// <exception cref="ArgumentNullException">Updater is null</exception>
        public void AddUpdater(IUpdater<TState, TRuntime> updater) {
            if (updater == null) {
                throw new ArgumentNullException("updater");
            }
            _updaters.Add(updater);
        }

        private void SendStateChange() {
            if (State.IsModified) {
                foreach (var player in _runtime.Players) {
                    player.OnStateChange(State);
                }
            }
        }

        /// <summary>
        /// Holds current state. If state replaced - all players will be notified about state change
        /// </summary>
        public TState State {
            get { return _state; }
            set {
                var replaced = _state != null;
                _state = value;
                if (replaced) {
                    value.ModifyWithChildren(ModificationType.Changed);
                    SendStateChange();
                }
            }
        }
    }
}
