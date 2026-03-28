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
        private UIRoutingDatabase _database;
        private bool _isInitialized = false;

        private void Initialize()
        {
            if (_isInitialized) return;

            string[] guids = AssetDatabase.FindAssets("t:UIRoutingDatabase");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _database = AssetDatabase.LoadAssetAtPath<UIRoutingDatabase>(path);
            }

            _isInitialized = true;
        }

        // Tell the Unity Inspector how much vertical space we need
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize();

            // If the database is missing, we request 2.5 lines of space to fit the Error Box and Button
            if (_database == null)
            {
                return EditorGUIUtility.singleLineHeight * 2.5f;
            }

            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [Dropdown] with strings only.");
                return;
            }

            Initialize();

            if (_database == null)
            {
                DrawMissingDatabaseError(position, label);
                return;
            }

            string[] options = GetOptions(_database);
            if (options == null || options.Length == 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            int currentIndex = Mathf.Max(0, System.Array.IndexOf(options, property.stringValue));
            currentIndex = EditorGUI.Popup(position, label.text, currentIndex, options);
            property.stringValue = options[currentIndex];
        }

        private void DrawMissingDatabaseError(Rect position, GUIContent label)
        {
            // 1. Draw the standard property label on the left so it aligns with everything else
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            // 2. Calculate the remaining space for the error and button
            float remainingWidth = position.width - EditorGUIUtility.labelWidth;
            float buttonWidth = 80f;

            Rect helpBoxRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y,
                remainingWidth - buttonWidth - 5, position.height);

            // Center the button vertically in the allocated space
            float buttonHeight = EditorGUIUtility.singleLineHeight * 1.5f;
            Rect buttonRect = new Rect(position.x + position.width - buttonWidth,
                position.y + (position.height - buttonHeight) / 2f, buttonWidth, buttonHeight);

            // 3. Draw the Error Box
            EditorGUI.HelpBox(helpBoxRect, "Routing DB missing!", MessageType.Error);

            // 4. Draw the Create Button
            if (GUI.Button(buttonRect, "Create DB"))
            {
                CreateDatabase();
            }
        }

        private void CreateDatabase()
        {
            // Open a save dialogue so the developer can choose exactly which folder to put it in!
            string path = EditorUtility.SaveFilePanelInProject(
                "Create UI Routing Database",
                "UIRoutingDatabase",
                "asset",
                "Choose a location to save the central UI Routing Database."
            );

            if (!string.IsNullOrEmpty(path))
            {
                var db = ScriptableObject.CreateInstance<UIRoutingDatabase>();

                AssetDatabase.CreateAsset(db, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Cache it instantly so the Inspector updates without needing a recompile
                _database = db;
                _isInitialized = true;
            }
        }

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