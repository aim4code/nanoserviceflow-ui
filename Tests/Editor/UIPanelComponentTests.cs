// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Reflection;
using NUnit.Framework;
using Aim4code.NanoServiceFlow;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;

namespace Aim4code.NanoServiceFlow.UI.Tests
{
    public class UIPanelComponentTests
    {
        private TestUIState _state;
        private TestUIService _service;
        private UIRouterService _router; // Needed to catch the auto-registration

        [SetUp]
        public void Setup()
        {
            _state = new TestUIState();

            ServiceLocator.RegisterState(_state);
            ServiceLocator.RegisterService<TestUIService>();
            ServiceLocator.RegisterService<UIRouterService>();
            ServiceLocator.InitializeAll();
            
            _service = ServiceLocator.Get<TestUIService>();
            _router = ServiceLocator.Get<UIRouterService>();
        }

        [TearDown]
        public void Teardown()
        {
            // Crucial: Clear the static locator after every test to ensure isolation
            ServiceLocator.ClearAll();
        }

        [Test]
        public void UIPanel_Awake_AutoRegistersRoute_BasedOnHierarchy()
        {
            // Arrange
            var canvasGO = new GameObject("Canvas");
            canvasGO.SetActive(false); // Suppress automatic Awake on AddComponent

            var rootProvider = canvasGO.AddComponent<TestUIProvider>();
            typeof(UIRootProvider).GetField("_rootKey", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(rootProvider, "TestUI");

            var layerGO = new GameObject("Modals");
            layerGO.transform.SetParent(canvasGO.transform);
            var locProvider = layerGO.AddComponent<UILocationProvider>();
            typeof(UILocationProvider).GetField("_locationName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(locProvider, "Modals");

            var panelGO = new GameObject("MyPanel");
            panelGO.transform.SetParent(layerGO.transform);
            var panel = panelGO.AddComponent<UIPanel>();
            typeof(UIPanel).GetField("_panelId", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(panel, "ShopScreen");

            // 1. CRITICAL FIX: Reactivate the hierarchy so GetComponentInParent can see them!
            canvasGO.SetActive(true);

            // 2. Act: Now trigger Awake manually while they are active
            typeof(UIPanel).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(panel, null);

            // Attempt to open the screen
            ServiceLocator.Dispatch(new OpenScreenAction("ShopScreen"));

            // Assert
            Assert.IsTrue(_state.ActivePanels.Value.Contains("ShopScreen"));

            Object.DestroyImmediate(canvasGO);
        }

        [Test]
        public void UIPanel_OnDestroy_UnregistersRoute()
        {
            // Arrange
            var canvasGO = new GameObject("Canvas");
            canvasGO.SetActive(false); 

            var rootProvider = canvasGO.AddComponent<TestUIProvider>();
            typeof(UIRootProvider).GetField("_rootKey", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(rootProvider, "TestUI");

            var layerGO = new GameObject("Modals");
            layerGO.transform.SetParent(canvasGO.transform);
            var locProvider = layerGO.AddComponent<UILocationProvider>();
            typeof(UILocationProvider).GetField("_locationName", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(locProvider, "Modals");

            var panelGO = new GameObject("MyPanel");
            panelGO.transform.SetParent(layerGO.transform);
            var panel = panelGO.AddComponent<UIPanel>();
            typeof(UIPanel).GetField("_panelId", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(panel, "ShopScreen");

            // 1. Reactivate the hierarchy
            canvasGO.SetActive(true); 

            // 2. Fire Awake manually
            typeof(UIPanel).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(panel, null);

            // 3. Fire OnDestroy manually to guarantee it executes in Edit Mode!
            typeof(UIPanel).GetMethod("OnDestroy", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(panel, null);

            // Act: Destroy the panel
            Object.DestroyImmediate(panelGO);

            // EXPECT THE ERROR: Tell Unity we expect a log about ShopScreen missing
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(".*Cannot open 'ShopScreen'.*"));

            // Attempt to open the screen after it was destroyed
            ServiceLocator.Dispatch(new OpenScreenAction("ShopScreen"));

            // Assert: The route was unregistered, so the Router blocked the request, leaving the state clean!
            Assert.IsFalse(_state.ActivePanels.Value.Contains("ShopScreen"));

            Object.DestroyImmediate(canvasGO);
        }
    }
}