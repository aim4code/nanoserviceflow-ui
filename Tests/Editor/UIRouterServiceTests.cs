// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using NUnit.Framework;
using Aim4code.NanoServiceFlow;
using UnityEngine;

namespace Aim4code.NanoServiceFlow.UI.Tests
{
    public class UIRouterServiceTests
    {
        private TestUIState _state;
        private TestUIService _service;
        private UIRouterService _router;

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
        public void RegisterRoute_AddsToRoutingTable_AllowsOpenScreenTranslation()
        {
            // Act: Register a route manually
            ServiceLocator.Dispatch(new RegisterUIRouteAction("Inventory", "TestUI", "Modals"));

            // Act: Dispatch the designer's intent
            ServiceLocator.Dispatch(new OpenScreenAction("Inventory"));

            // Assert: The Router successfully translated OpenScreen -> PushPanel
            Assert.IsTrue(_state.ActivePanels.Value.Contains("Inventory"));
            Assert.AreEqual("Inventory", _state.LocationStacks["Modals"][0]);
        }

        [Test]
        public void UnregisterRoute_RemovesFromTable_PreventsTranslation()
        {
            ServiceLocator.Dispatch(new RegisterUIRouteAction("Inventory", "TestUI", "Modals"));
            ServiceLocator.Dispatch(new UnregisterUIRouteAction("Inventory"));

            // Act: Try to open an unregistered screen
            ServiceLocator.Dispatch(new OpenScreenAction("Inventory"));

            // Assert: State remains unchanged
            Assert.IsFalse(_state.ActivePanels.Value.Contains("Inventory"));
        }
    }
}