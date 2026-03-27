// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

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
            canvasGO.AddComponent<TestUIProvider>();

            var layerGO = new GameObject("Modals");
            layerGO.transform.SetParent(canvasGO.transform);
            var locProvider = layerGO.AddComponent<UILocationProvider>();
            locProvider.LocationName = "Modals";

            var panelGO = new GameObject("MyPanel");
            panelGO.transform.SetParent(layerGO.transform);
            
            // Act: AddComponent automatically calls Awake() in EditMode
            var panel = panelGO.AddComponent<UIPanel>();
            panel.PanelId = "ShopScreen";
            
            // Trigger the open action. If Awake() worked, the router knows about 'ShopScreen'
            ServiceLocator.Dispatch(new OpenScreenAction("ShopScreen"));

            // Assert: The panel successfully wired itself up to the system
            Assert.IsTrue(_state.ActivePanels.Value.Contains("ShopScreen"));

            // Cleanup
            Object.DestroyImmediate(canvasGO);
        }

        [Test]
        public void UIPanel_OnDestroy_UnregistersRoute()
        {
            // Arrange
            var canvasGO = new GameObject("Canvas");
            canvasGO.AddComponent<TestUIProvider>();

            var layerGO = new GameObject("Modals");
            layerGO.transform.SetParent(canvasGO.transform);
            var locProvider = layerGO.AddComponent<UILocationProvider>();
            locProvider.LocationName = "Modals";

            var panelGO = new GameObject("MyPanel");
            panelGO.transform.SetParent(layerGO.transform);
            
            var panel = panelGO.AddComponent<UIPanel>();
            panel.PanelId = "ShopScreen";

            // Act: Destroy the panel (simulating scene unload)
            Object.DestroyImmediate(panelGO);

            // Attempt to open the screen after it was destroyed
            ServiceLocator.Dispatch(new OpenScreenAction("ShopScreen"));

            // Assert: The screen should not be added to the state, as its route was unregistered
            Assert.IsFalse(_state.ActivePanels.Value.Contains("ShopScreen"));

            // Cleanup
            Object.DestroyImmediate(canvasGO);
        }
    }
}