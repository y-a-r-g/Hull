using System.Runtime.Serialization;

namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// State property interface. State can hold only members that implement this interface.
    /// Implementations should be marked as [Serializable]
    /// </summary>
    public interface IStateProperty : ISerializable {
        /// <summary>
        /// Parent property for this property.
        /// </summary>
        IStatePropertyContainer Container { get; set; }

        /// <summary>
        /// Sets container without children modification. Containers should set container to children
        /// </summary>
        IStatePropertyContainer DeserializedContainer { set; }

        /// <summary>
        /// Returns <code>true</code> if this property was changed since last tick.
        /// </summary>
        bool IsModified { get; }

        /// <summary>
        /// Hold type of last property modification.
        /// </summary>
        ModificationType ModificationType { get; }

        /// <summary>
        /// Property should mark itself as modified and call this method of its <see cref="Container"/>.
        /// </summary>
        /// <param name="modificationType">This value should be returned by <see cref="IStateProperty.ModificationType"/></param>
        void Modify(ModificationType modificationType);

        /// <summary>
        /// Unique id of the property
        /// </summary>
        ulong UniqueId { get; }
    }
}