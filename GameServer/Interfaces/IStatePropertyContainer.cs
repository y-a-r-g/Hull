using System.Collections.Generic;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// Interface for the property that can hold other nested properties
    /// </summary>
    public interface IStatePropertyContainer : IStateProperty {
        /// <summary>
        /// When this method called property should mark itself and all its child properties as modified.
        /// </summary>
        /// <param name="modificationType"></param>
        void ModifyWithChildren(ModificationType modificationType);

        /// <summary>
        /// Should return enumerator through all child properties.
        /// </summary>
        /// <returns></returns>
        IEnumerator<IStateProperty> GetChildrenEnumerator();

        /// <summary>
        /// Should return child or null if container does not contain child whith given id
        /// </summary>
        /// <param name="uniqueId">Child id</param>
        /// <returns>Child with given id or null</returns>
        IStateProperty GetChildProperty(ulong uniqueId);
    }
}
