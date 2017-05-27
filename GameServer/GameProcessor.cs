﻿using System;
using System.Collections.Generic;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;
#if UNITY_5
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
    /// Request are not processed imppediately, they are stored in line and called at the beginning of the next tick.
    ///
    /// </summary>
    /// <typeparam name="TState">Type of the server <see cref="State"/></typeparam>
    /// <typeparam name="TRuntime">Type of the server <see cref="IServerRuntime"/></typeparam>
    public class GameProcessor<TState, TRuntime> where TState : State where TRuntime : IServerRuntime {
        private readonly Dictionary<Type, IRequestProcessor<TState, TRuntime>> _requestProcessors =
            new Dictionary<Type, IRequestProcessor<TState, TRuntime>>();

        private readonly Queue<RequestLineItem> _requestsLine = new Queue<RequestLineItem>();
        private readonly TState _state;
        private readonly TRuntime _runtime;
        private readonly List<IUpdater<TState, TRuntime>> _updaters = new List<IUpdater<TState, TRuntime>>();

        /// <summary>
        /// When state was changes this event will be triggered. It happens once per tick if state changed. 
        /// </summary>
        public event Action<TState> StateChanged;

#if !UNITY_5
        private const float dt = 1f / 30;
#endif

        /// <summary>
        /// Creates new processor with gigen state and runtime.
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="runtime"></param>
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
                IRequestProcessor<TState, TRuntime> processor;
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

            _runtime.UpdateCoroutines();

            _state.EndUpdate();

            SendStateChange();
        }

        /// <summary>
        /// Associates a Request Processor with a Request. One Request may have only one Request Processor associated with.
        /// When Game Processor receives the request it will find associated Request Processor and all its <code>ProcessRequest</code> method. 
        /// </summary>
        /// <param name="processor">Associated Request Processor</param>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <exception cref="ArgumentNullException">Processor is null</exception>
        public void RegisterProcessor<TRequest>(IRequestProcessor<TState, TRuntime> processor)
            where TRequest : IRequest {
            if (processor == null) {
                throw new ArgumentNullException("processor");
            }
            _requestProcessors[typeof(TRequest)] = processor;
        }

        /// <summary>
        /// Adds a request to the line. This request will be processed in the beginning of the next tick.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="player"></param>
        /// <exception cref="ArgumentNullException">Request is null</exception>
        public void ProcessRequest(IRequest request, IPlayer player) {
            if (request == null) {
                throw new ArgumentNullException("request");
            }
            _requestsLine.Enqueue(new RequestLineItem {Request = request, Player = player});
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
            if (_state.IsModified) {
                if (StateChanged != null) {
                    StateChanged(_state);
                }
            }
        }
    }
}
