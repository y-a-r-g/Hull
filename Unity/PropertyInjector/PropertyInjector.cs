using System.Reflection;
using UnityEngine;

namespace Hull.Unity.PropertyInjector {
    /// <summary>
    /// Add this component to the GameObject to inject properties to other MonoBehaviours of that object from config
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Hull/Property Injector")]
    public class PropertyInjector : MonoBehaviour {
        private void Awake() {
            Inject(this);
        }

        public static void Inject(PropertyInjector pi) {
            foreach (var component in pi.gameObject.GetComponents<MonoBehaviour>()) {
                foreach (var fieldInfo in component.GetType().GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                    PropertyInjectorCore.InitializeField(fieldInfo, component);
                }
            }
        }
    }
}
