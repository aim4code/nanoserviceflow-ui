// ============================================================================
// Copyright (c) 2026 Daniel Conde Linares
// Licensed under the MIT License. See LICENSE file in the project root.
// ============================================================================

using NUnit.Framework;

namespace Aim4code.NanoServiceFlow.UI.Tests
{
    public class UIServiceTests
    {
        private TestUIState _state;
        private TestUIService _service;

        [SetUp]
        public void Setup()
        {
            _state = new TestUIState();

            ServiceLocator.RegisterState(_state);
            ServiceLocator.RegisterService<TestUIService>();
            ServiceLocator.InitializeAll();
            
            _service = ServiceLocator.Get<TestUIService>();
        }

        [TearDown]
        public void Teardown()
        {
            // Crucial: Clear the static locator after every test to ensure isolation
            ServiceLocator.ClearAll();
        }

        [Test]
        public void PushPanel_AddsToActivePanels_AndLocationStack()
        {
            ServiceLocator.Dispatch(new PushPanelAction("TestUI", "MainStack", "MainMenu"));

            Assert.IsTrue(_state.ActivePanels.Value.Contains("MainMenu"));
            Assert.IsTrue(_state.LocationStacks.ContainsKey("MainStack"));
            Assert.AreEqual(1, _state.LocationStacks["MainStack"].Count);
            Assert.AreEqual("MainMenu", _state.LocationStacks["MainStack"][0]);
        }

        [Test]
        public void PushPanel_HidesPreviousTop_InSameLocation()
        {
            ServiceLocator.Dispatch(new PushPanelAction("TestUI", "MainStack", "MainMenu"));
            ServiceLocator.Dispatch(new PushPanelAction("TestUI", "MainStack", "SettingsMenu"));

            // Settings should be active, MainMenu should be hidden
            Assert.IsTrue(_state.ActivePanels.Value.Contains("SettingsMenu"));
            Assert.IsFalse(_state.ActivePanels.Value.Contains("MainMenu"));
            
            // Stack should contain both
            Assert.AreEqual(2, _state.LocationStacks["MainStack"].Count);
        }

        [Test]
        public void PopPanel_RemovesCurrentTop_AndRestoresPrevious()
        {
            ServiceLocator.Dispatch(new PushPanelAction("TestUI", "MainStack", "MainMenu"));
            ServiceLocator.Dispatch(new PushPanelAction("TestUI", "MainStack", "SettingsMenu"));
            
            // Act: Pop the settings menu
            ServiceLocator.Dispatch(new PopPanelAction("TestUI", "MainStack"));

            Assert.IsFalse(_state.ActivePanels.Value.Contains("SettingsMenu"));
            Assert.IsTrue(_state.ActivePanels.Value.Contains("MainMenu"));
            Assert.AreEqual(1, _state.LocationStacks["MainStack"].Count);
        }

        [Test]
        public void ToggleOverlay_ModifiesActivePanels_WithoutAffectingStack()
        {
            ServiceLocator.Dispatch(new ToggleOverlayAction("TestUI", "LoadingScreen", true));

            Assert.IsTrue(_state.ActivePanels.Value.Contains("LoadingScreen"));
            Assert.IsFalse(_state.LocationStacks.ContainsKey("Overlays"), "Overlays should not create a stack entry");

            ServiceLocator.Dispatch(new ToggleOverlayAction("TestUI", "LoadingScreen", false));
            
            Assert.IsFalse(_state.ActivePanels.Value.Contains("LoadingScreen"));
        }
    }
}