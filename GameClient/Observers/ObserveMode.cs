namespace Hull.GameClient.Observers {
    /// <summary>
    /// Observing mode for multiple-properties observer.
    /// </summary>
    public enum ObserveMode {
        /// <summary>
        /// Triggers if all properties were changed
        /// </summary>
        Any,
        /// <summary>
        /// Triggers if any property was changed
        /// </summary>
        All
    }
}
