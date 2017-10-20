using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Hull.Extensions;
using Hull.Unity.Serialization;
using UnityEngine;

namespace Hull.Unity.PropertyInjector {
    /// <summary>
    /// Singleton class will be created automatically.
    /// Usage:
    /// 1) set PropertyInjectorCore.SpreadsheetKey
    /// Also share spreadsheet with url to read and find key in url: https://docs.google.com/spreadsheets/d/{THIS_IS_THE_KEY}/edit#gid=0.
    /// 
    /// 2) Add PropertyInjector to GameObjects to mark that rest of MonoBehaviours on that object should be initialized form config
    /// 
    /// 3) Mark properties with [Inject] or [InjectArray] attributes to mark properties as injectable from config.
    /// 
    /// Spreadsheet format.
    /// First column of spreadsheet is treated as key, rest non-empty columns are joined with "," and treated as value.
    /// If both key and value is empty - row is ignored.
    /// If key is empty, value is treated as group name. All next key-value pairs will be assigned to that group, until group not changed.
    /// Initial group is empty string.
    /// 
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PropertyInjectorCore : MonoBehaviour {
        public static string FileName = "Hull.PropertyInjector.config";

        public static event Action PropertiesInjected;

        private static string _spreadsheetKey;

        public static string SpreadsheetKey {
            get { return _spreadsheetKey; }
            set {
                _spreadsheetKey = value;
                if (_spreadsheetKey != null) {
                    if (!_instance) {
                        Instance.GetHashCode();
                    }
                    else {
                        Instance.StartCoroutine(Instance.DownloadCsv(_spreadsheetKey));
                    }
                }
            }
        }

        private static PropertyInjectorCore _instance;

        public static PropertyInjectorCore Instance {
            get {
                if (!_instance) {
                    _instance = FindObjectOfType<PropertyInjectorCore>();
                }
                if (!_instance) {
                    _instance = new GameObject("Hull.PropertyInjector").AddComponent<PropertyInjectorCore>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                if (_instance._values == null) {
                    _instance.InitFromCache();
                    if (SpreadsheetKey != null) {
                        _instance.StartCoroutine(_instance.DownloadCsv(SpreadsheetKey));
                    }
                }
                return _instance;
            }
            private set { _instance = value; }
        }

        private Dictionary<string, List<KeyValuePair<string, string>>> _values;

        private void Awake() {
            Instance = this;

            if (_values == null) {
                InitFromCache();
                if (SpreadsheetKey != null) {
                    StartCoroutine(DownloadCsv(SpreadsheetKey));
                }
            }
        }

        private void InitFromCache() {
            string fileName = null;
            var persistentLevel = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(persistentLevel)) {
                fileName = persistentLevel;
            }

            var streamingLevel = Path.Combine(Application.streamingAssetsPath, FileName);
            if (File.Exists(streamingLevel)) {
                fileName = streamingLevel;
            }

            if ((fileName != null) && File.Exists(fileName)) {
                try {
                    using (var stream = new FileStream(fileName, FileMode.Open)) {
                        _values = (Dictionary<string, List<KeyValuePair<string, string>>>)SerializationUtils.BinaryFormatter
                            .Deserialize(stream);
                    }
                }
                catch { }
            }

            if (_values == null) {
                _values = new Dictionary<string, List<KeyValuePair<string, string>>>();
            }

            ReinjectAll();
        }

        private IEnumerator DownloadCsv(string spreadsheetKey) {
            var url = string.Format("https://docs.google.com/spreadsheets/d/{0}/export?format=csv", spreadsheetKey);
            var www = new WWW(url);
            yield return www;

            _values = new Dictionary<string, List<KeyValuePair<string, string>>>();
            var lines = www.text.Split('\n');
            var group = "";
            foreach (var line in lines) {
                var trimmed = line.Trim();
                var comma = trimmed.IndexOf(',');
                if (comma != -1) {
                    var key = trimmed.Substring(0, comma).Trim();
                    var value = trimmed.Substring(comma + 1).Trim();
                    if (key == "") {
                        if (value != "") {
                            group = value;
                        }
                    }
                    else {
                        if (!_values.ContainsKey(group)) {
                            _values[group] = new List<KeyValuePair<string, string>>();
                        }
                        _values[group].Add(new KeyValuePair<string, string>(key, value));
                    }
                }
            }

#if UNITY_EDITOR
            var path = Path.Combine(Application.streamingAssetsPath, FileName);
            Directory.CreateDirectory(Application.streamingAssetsPath);
#else
            var path = Path.Combine(Application.persistentDataPath, FileName);
            Directory.CreateDirectory(Application.persistentDataPath);
#endif

            using (var stream = new FileStream(path, FileMode.Create)) {
                SerializationUtils.BinaryFormatter.Serialize(stream, _values);
            }

            ReinjectAll();
        }

        public static void UpdateSpreadsheet() {
            Instance.StartCoroutine(Instance.DownloadCsv(_spreadsheetKey));
        }

        public static void ReinjectAll() {
            FindObjectsOfType<PropertyInjector>().ForEach(PropertyInjector.Inject);
            if (PropertiesInjected != null) {
                PropertiesInjected();
            }
        }

        public static void InitializeField(FieldInfo fieldInfo, object component) {
            var group = component.GetType().Name;
            var key = fieldInfo.Name;

            foreach (var fieldInfoCustomAttribute in fieldInfo.GetCustomAttributes(true)) {
                var injectedArray = fieldInfoCustomAttribute as InjectedArrayAttribute;
                List<KeyValuePair<string, string>> keys;
                if (injectedArray != null) {
                    group = injectedArray.Group;
                    if (Instance._values.TryGetValue(group, out keys)) {
                        var elementType = fieldInfo.FieldType.GetElementType();
                        var array = Array.CreateInstance(elementType, keys.Count);
                        var index = 0;
                        foreach (KeyValuePair<string, string> kvp in keys) {
                            var value = kvp.Value;
                            if (elementType == typeof(int)) {
                                array.SetValue(int.Parse(value, CultureInfo.InvariantCulture), index);
                            }
                            else if (elementType == typeof(float)) {
                                array.SetValue(float.Parse(value, CultureInfo.InvariantCulture), index);
                            }
                            else if (elementType == typeof(string)) {
                                array.SetValue(value, index);
                            }
                            else if (elementType == typeof(bool)) {
                                array.SetValue(value != "false", index);
                            }
                            else {
                                throw new ArrayTypeMismatchException();
                            }

                            index++;
                        }

                        fieldInfo.SetValue(component, array);
                    }
                }

                var injected = fieldInfoCustomAttribute as InjectedAttribute;
                if (injected != null) {
                    if (injected.Group != null) {
                        group = injected.Group;
                    }

                    if (Instance._values.TryGetValue(group, out keys)) {
                        if (injected.Key != null) {
                            key = injected.Key;
                        }

                        foreach (var keyValuePair in keys) {
                            if (keyValuePair.Key == key) {
                                var value = keyValuePair.Value;
                                if (fieldInfo.FieldType == typeof(int)) {
                                    fieldInfo.SetValue(component, int.Parse(value));
                                }
                                else if (fieldInfo.FieldType == typeof(float)) {
                                    fieldInfo.SetValue(component, float.Parse(value));
                                }
                                else if (fieldInfo.FieldType == typeof(string)) {
                                    fieldInfo.SetValue(component, value);
                                }
                                else if (fieldInfo.FieldType == typeof(bool)) {
                                    fieldInfo.SetValue(component, value != "false");
                                }
                                else {
                                    throw new TypeLoadException();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
