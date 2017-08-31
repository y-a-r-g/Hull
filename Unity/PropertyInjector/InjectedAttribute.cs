using System;

namespace Hull.Unity.PropertyInjector {
    /// <summary>
    /// Marks property of the MonoBehaviour which value should be injected from config.
    /// Supported property types: string, int, float, bool ("false" is false, rest values are true)
    /// <seealso cref="PropertyInjectorCore"/> 
    /// </summary>
    public class InjectedAttribute : Attribute {
        /// <summary>
        /// Group of the property. If group is null - MonoBehaviour name will be used as group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Key of the property. If key is null - property name will be used as key
        /// </summary>
        public string Key { get; set; }

        public InjectedAttribute() { }
    }
}