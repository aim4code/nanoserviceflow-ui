// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Aim4code.NanoServiceFlow.UI.Editor
{
    public abstract class UIRoutingDropdownDrawer : PropertyDrawer
    {
        private string[] _options = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [Dropdown] with strings only.");
                return;
            }

            if (_options == null)
            {
                _options = LoadOptionsFromDatabase();
            }

            if (_options == null || _options.Length == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            int currentIndex = Mathf.Max(0, System.Array.IndexOf(_options, property.stringValue));
            currentIndex = EditorGUI.Popup(position, label.text, currentIndex, _options);
            property.stringValue = _options[currentIndex];
        }

        private string[] LoadOptionsFromDatabase()
        {
            string[] guids = AssetDatabase.FindAssets("t:UIRoutingDatabase");
            if (guids.Length == 0) return null;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var database = AssetDatabase.LoadAssetAtPath<UIRoutingDatabase>(path);

            if (database == null) return null;

            // Call the abstract method implemented by the child classes
            return GetOptions(database);
        }

        // The single responsibility handed down to the specific drawers
        protected abstract string[] GetOptions(UIRoutingDatabase database);
    }

    [CustomPropertyDrawer(typeof(UILocationDropdownAttribute))]
    public class UILocationDropdownDrawer : UIRoutingDropdownDrawer
    {
        protected override string[] GetOptions(UIRoutingDatabase database) => database.Locations.ToArray();
    }

    [CustomPropertyDrawer(typeof(UIPanelDropdownAttribute))]
    public class UIPanelDropdownDrawer : UIRoutingDropdownDrawer
    {
        protected override string[] GetOptions(UIRoutingDatabase database) => database.Panels.ToArray();
    }

    [CustomPropertyDrawer(typeof(UIRootDropdownAttribute))]
    public class UIRootDropdownDrawer : UIRoutingDropdownDrawer
    {
        protected override string[] GetOptions(UIRoutingDatabase database) => database.Roots.ToArray();
    }
}
#endif