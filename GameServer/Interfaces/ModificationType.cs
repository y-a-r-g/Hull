namespace Hull.GameServer.Interfaces {
    /// <summary>
    /// Type of the property modification since last tick.
    /// </summary>
    public enum ModificationType {
        /// <summary>
        /// Propery value was changed
        /// </summary>
        Changed,

        /// <summary>
        /// Property was added to parent
        /// </summary>
        Added,

        /// <summary>
        /// Property was removed from parent
        /// </summary>
        Removed
    }
}