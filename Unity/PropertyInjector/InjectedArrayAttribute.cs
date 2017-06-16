using System;

namespace Hull.Unity.PropertyInjector {
    /// <summary>
    /// Marks property of the MonoBehaviour which value should be injected from config.
    /// Property should have array type and will be populated from full config group.
    /// Supported property types: string[], int[], float[], bool[] ("false" is false, rest values are true)
    /// <seealso cref="PropertyInjectorCore"/>
    /// </summary>
    public class InjectedArrayAttribute : Attribute {
        /// <summary>
        /// Config group to populate property value. If not specified - name of the property will be used as group name
        /// </summary>
        public string Group { get; set; }

        public InjectedArrayAttribute() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group">Config group to populate property value.</param>
        public InjectedArrayAttribute(string group) {
            Group = group;
        }
    }
}
