// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using System.Reflection;
using NUnit.Framework;
using Aim4code.NanoServiceFlow;
using UnityEngine;

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
            // Arrange: Build a virtual Unity hierarchy in memory
            var canvasGO = new GameObject("Canvas");
            canvasGO.SetActive(false); // SUPPRESS AWAKE()

            canvasGO.AddComponent<TestUIProvider>();

            var layerGO = new GameObject("Modals");
            layerGO.transform.SetParent(canvasGO.transform);
            var locProvider = layerGO.AddComponent<UILocationProvider>();
            
            // Inject LocationName
            typeof(UILocationProvider)
                .GetField("_locationName", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(locProvider, "Modals");

            var panelGO = new GameObject("MyPanel");
            panelGO.transform.SetParent(layerGO.transform);
            var panel = panelGO.AddComponent<UIPanel>();
            
            // Inject PanelId
            typeof(UIPanel)
                .GetField("_panelId", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(panel, "ShopScreen");
            
            // Act: Reactivate to fire Awake() naturally across the hierarchy!
            canvasGO.SetActive(true);
            
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
            canvasGO.SetActive(false); // SUPPRESS AWAKE()

            canvasGO.AddComponent<TestUIProvider>();

            var layerGO = new GameObject("Modals");
            layerGO.transform.SetParent(canvasGO.transform);
            var locProvider = layerGO.AddComponent<UILocationProvider>();
            
            typeof(UILocationProvider)
                .GetField("_locationName", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(locProvider, "Modals");

            var panelGO = new GameObject("MyPanel");
            panelGO.transform.SetParent(layerGO.transform);
            var panel = panelGO.AddComponent<UIPanel>();
            
            typeof(UIPanel)
                .GetField("_panelId", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(panel, "ShopScreen");

            canvasGO.SetActive(true); // Fire Awake()

            // Act: Destroy the panel
            Object.DestroyImmediate(panelGO);

            // Need to manually dispatch Unregister here if your UIPanel.OnDestroy doesn't do it automatically yet
            // ServiceLocator.Dispatch(new UnregisterUIRouteAction("ShopScreen"));

            ServiceLocator.Dispatch(new OpenScreenAction("ShopScreen"));

            // Assert
            Assert.IsFalse(_state.ActivePanels.Value.Contains("ShopScreen"));

            Object.DestroyImmediate(canvasGO);
        }
    }
}