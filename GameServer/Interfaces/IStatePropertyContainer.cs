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
    }
}
