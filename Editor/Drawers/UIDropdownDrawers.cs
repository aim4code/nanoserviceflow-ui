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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize();
            if (_database == null) return EditorGUIUtility.singleLineHeight * 2.5f; 
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

            // --- Calculate Layout Space ---
            float buttonWidth = 45f;
            // Shrink the popup width to leave room for the button
            Rect popupRect = new Rect(position.x, position.y, position.width - buttonWidth - 2f, position.height);
            // Place the button at the far right
            Rect buttonRect = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);

            // --- Draw the Shortcut Button ---
            if (GUI.Button(buttonRect, new GUIContent("Edit", "Selects the Routing Database in the Project Window")))
            {
                // Changes the Inspector to show the Database
                Selection.activeObject = _database;
                // Makes the asset flash yellow in the Project window hierarchy
                EditorGUIUtility.PingObject(_database);
            }

            string[] options = GetOptions(_database);
        
            if (options == null || options.Length == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.Popup(popupRect, label.text, 0, new[] { "[ List is Empty ]" });
                EditorGUI.EndDisabledGroup();
                return;
            }

            int currentIndex = Mathf.Max(0, System.Array.IndexOf(options, property.stringValue));
        
            // Use the shrunken popupRect here instead of position!
            currentIndex = EditorGUI.Popup(popupRect, label.text, currentIndex, options);
            property.stringValue = options[currentIndex];
        }

        private void DrawMissingDatabaseError(Rect position, GUIContent label)
        {
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            float remainingWidth = position.width - EditorGUIUtility.labelWidth;
            float buttonWidth = 80f;

            Rect helpBoxRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, remainingWidth - buttonWidth - 5, position.height);
            float buttonHeight = EditorGUIUtility.singleLineHeight * 1.5f;
            Rect buttonRect = new Rect(position.x + position.width - buttonWidth, position.y + (position.height - buttonHeight) / 2f, buttonWidth, buttonHeight);

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