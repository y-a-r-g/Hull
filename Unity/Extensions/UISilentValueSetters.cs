using UnityEngine.UI;

namespace Hull.Unity.Extensions {
    public static class UISilentValueSetters {
        private static readonly Slider.SliderEvent EmptySliderEvent = new Slider.SliderEvent();

        public static void SetValueSilently(this Slider instance, float value) {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = EmptySliderEvent;
            instance.value = value;
            instance.onValueChanged = originalEvent;
        }

        private static readonly Toggle.ToggleEvent EmptyToggleEvent = new Toggle.ToggleEvent();

        public static void SetValueSilently(this Toggle instance, bool value) {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = EmptyToggleEvent;
            instance.isOn = value;
            instance.onValueChanged = originalEvent;
        }

        private static readonly InputField.OnChangeEvent EmptyInputFieldEvent = new InputField.OnChangeEvent();

        public static void SetValueSilently(this InputField instance, string value) {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = EmptyInputFieldEvent;
            instance.text = value;
            instance.onValueChanged = originalEvent;
        }
    }
}
