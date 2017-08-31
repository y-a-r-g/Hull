using System;
using System.Collections.Generic;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState;

namespace Hull.GameClient.Observers {
    /// <summary>
    /// Utility mehtod to find property by path combined from unique ids.
    /// </summary>
    public static class PropertyFinder {
        /// <summary>
        /// Builds property path combined from unit ids of all its parents. Path goes from state (uniqueId not included) to property (unique id included)
        /// </summary>
        /// <param name="state">State that holds property</param>
        /// <param name="propertyUniqueId">Unique id of the property to build path to</param>
        /// <returns>Path to the property</returns>
        public static IEnumerable<ulong> GetPropertyPath(State state, ulong propertyUniqueId) {
            var path = new LinkedList<ulong>();
            GetPropertyPath(state, propertyUniqueId, path);
            var result = new ulong[path.Count];
            var index = 0;
            foreach (var id in path) {
                result[index++] = id;
            }

            return result;
        }

        private static bool GetPropertyPath(
            IStatePropertyContainer container, ulong propertyUniqueId, LinkedList<ulong> path) {
            using (var e = container.GetChildrenEnumerator()) {
                while (e.MoveNext()) {
                    if (e.Current.UniqueId == propertyUniqueId) {
                        path.AddFirst(propertyUniqueId);
                        return true;
                    }
                }

                e.Reset();
                while (e.MoveNext()) {
                    var childContainer = e.Current as IStatePropertyContainer;
                    if (childContainer != null) {
                        if (GetPropertyPath(childContainer, propertyUniqueId, path)) {
                            path.AddFirst(childContainer.UniqueId);
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Returns property instance by given path
        /// </summary>
        /// <param name="state">State that holds property</param>
        /// <param name="path">Path to the property</param>
        /// <returns>Property instance or null if not found</returns>
        public static IStateProperty FindProperty(State state, IEnumerable<ulong> path) {
            if (state == null) {
                throw new ArgumentNullException("state");
            }
            if (path == null) {
                throw new ArgumentNullException("path");
            }

            var property = (IStateProperty)state;
            foreach (var uid in path) {
                if (property == null) {
                    break;
                }

                property = ((IStatePropertyContainer)property).GetChildProperty(uid);
            }

            return property;
        }
    }
}