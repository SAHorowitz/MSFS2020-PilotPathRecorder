using System.Collections.Generic;
using Xunit;

namespace FS2020PlanePath.XUnitTests
{

    public class FlightLoggingTestButtonModel : IButtonStateModel<ToggleState>
    {

        public FlightLoggingTestButtonModel(bool initialEnablement)
        {
            State = ToggleState.Out;
            IsEnabled = initialEnablement;
        }

        public bool IsEnabled { get; set; }

        public ToggleState State { get; set; }

    }

    public class FlightLoggingOrchestratorTests
    {

        public FlightLoggingOrchestratorTests()
        {
            actionNames = new List<string>();
            actionSources = new List<int>();
            orchestrator = new FlightLoggingOrchestrator(
                new FlightLoggingTestButtonModel(true),
                new FlightLoggingTestButtonModel(false),
                s => registerAction(s, "i"),
                s => registerAction(s, "e"),
                s => registerAction(s, "d"),
                s => registerAction(s, "f")
            );
        }

        private void registerAction(FlightLoggingOrchestrator.TriggerSource actionSource, string actionName)
        {
            actionNames.Add(actionName);
            actionSources.Add((int) actionSource);
        }

        private FlightLoggingOrchestrator orchestrator;
        private List<string> actionNames;
        private List<int> actionSources;

        /// <summary>
        /// Tests expected initial configuration.
        /// </summary>
        [Fact]
        public void StartupState()
        {
            Assert.False(orchestrator.IsAutomatic);
            AssertInitialButtonState();
            AssertActions("i");
        }

        /// <summary>
        /// Arrival at either threshold ignored while not in automatic mode.
        /// Also tests start, stop, pause & resume actions.
        /// </summary>
        [Fact]
        public void NonAutomaticFullNormalCase()
        {
            orchestrator.IsAutomatic = false;
            AssertActions("i");
            orchestrator.ThresholdReached();
            orchestrator.ThresholdMissed();
            AssertActions("i");
            Assert.False(orchestrator.IsAutomatic);
            AssertInitialButtonState();
            orchestrator.Start();
            AssertActions("i,e");
            orchestrator.Pause();
            AssertButtonEnabledState(true,  true);
            AssertActions("i,e,d");
            orchestrator.Resume();
            AssertButtonEnabledState(true, true);
            AssertActions("i,e,d,e");
            orchestrator.Stop();
            AssertActions("i,e,d,e,d,f");
            AssertSources("2,0,1,1,0,0");
            AssertInitialButtonState();
        }

        /// <summary>
        /// Switching off automatic mode while in flight disables close & flush of log on threshold missed.
        /// </summary>
        [Fact]
        public void AutomaticToNonAutomaticSimpleCase()
        {
            orchestrator.IsAutomatic = true;
            AssertInitialButtonState();
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            orchestrator.IsAutomatic = false;
            orchestrator.ThresholdMissed();
            AssertActions("i,e");
            AssertSources("2,0");
            AssertButtonEnabledState(true, true);
        }

        /// <summary>
        /// "If you manually start logging then you must manually stop it."
        /// </summary>
        [Fact]
        public void AutomaticUserStartedThresholdReachedThenMissedThenUserStopped()
        {
            orchestrator.IsAutomatic = true;
            Assert.True(orchestrator.IsAutomatic);
            orchestrator.Start();
            AssertActions("i,e");
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            orchestrator.ThresholdMissed();
            AssertActions("i,e");
            AssertButtonEnabledState(true, true);
            orchestrator.Stop();
            AssertInitialButtonState();
            AssertActions("i,e,d,f");
            AssertSources("2,0,0,0");
        }

        /// <summary>
        /// "If you manually start logging then you must manually stop it."
        /// </summary>
        [Fact]
        public void NonAutomaticUserStartedToAutomaticThresholdMissed()
        {
            orchestrator.IsAutomatic = false;
            Assert.False(orchestrator.IsAutomatic);
            AssertInitialButtonState();
            orchestrator.Start();
            AssertActions("i,e");
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            AssertButtonEnabledState(true, true);
            orchestrator.IsAutomatic = true;
            orchestrator.ThresholdMissed();
            AssertActions("i,e");
            AssertSources("2,0");
            AssertButtonEnabledState(true, true);
        }

        /// <summary>
        /// "If you manually stop an automatically started log then once you go below the threshold speed
        /// and then above that speed again it will start an automatic log as a new flight."
        /// </summary>
        [Fact]
        public void AutomaticThresholdReachedUserStoppedThresholdMissedThenReached()
        {
            orchestrator.IsAutomatic = true;
            Assert.True(orchestrator.IsAutomatic);
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            orchestrator.Stop();
            AssertActions("i,e,d,f");
            orchestrator.ThresholdReached();
            AssertActions("i,e,d,f");
            orchestrator.ThresholdMissed();
            AssertActions("i,e,d,f");
            orchestrator.ThresholdReached();
            AssertActions("i,e,d,f,e");
            AssertSources("2,0,0,0,0");
        }

        /// <summary>
        /// "Above that value, logging starts and below that value, it stops and the flight log is saved."
        /// </summary>
        [Fact]
        public void AutomaticThresholdReachedThenMissed()
        {
            orchestrator.IsAutomatic = true;
            Assert.True(orchestrator.IsAutomatic);
            AssertActions("i");
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            orchestrator.ThresholdMissed();
            AssertActions("i,e,d,f");
            AssertSources("2,0,0,0");
        }

        /// <summary>
        /// "If you pause a log and then go below the threshold speed you must manually stop the log."
        /// </summary>
        [Fact]
        public void AutomaticThresholdReachedUserPausedThresholdMissed()
        {
            orchestrator.IsAutomatic = true;
            Assert.True(orchestrator.IsAutomatic);
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            AssertButtonEnabledState(true, true);
            orchestrator.Pause();
            AssertButtonEnabledState(true, true);
            AssertActions("i,e,d");
            orchestrator.ThresholdMissed();
            AssertActions("i,e,d");
            AssertButtonEnabledState(true, true);
            orchestrator.Stop();
            AssertInitialButtonState();
            AssertActions("i,e,d,d,f");
            AssertSources("2,0,1,0,0");
        }

        /// <summary>
        /// "If you pause a log and then go below the threshold speed you must manually stop the log."
        /// (... but, after you resume then go below threshold again, it will close & flush...)
        /// </summary>
        [Fact]
        public void AutomaticThresholdReachedUserPausedThresholdMissedUserResumed()
        {
            orchestrator.IsAutomatic = true;
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            orchestrator.Pause();

            // while paused, threshold calls should be ignored

            AssertActions("i,e,d");
            orchestrator.ThresholdMissed();
            AssertActions("i,e,d");
            orchestrator.ThresholdReached();
            AssertActions("i,e,d");
            orchestrator.ThresholdMissed();
            AssertActions("i,e,d");

            orchestrator.Resume();

            // resume always re-enables logging, regardless of threshold

            AssertActions("i,e,d,e");
            orchestrator.ThresholdReached();    // does nothing since already enabled
            AssertActions("i,e,d,e");
            orchestrator.ThresholdMissed();
            AssertActions("i,e,d,e,d,f");
            AssertInitialButtonState();
            AssertSources("2,0,1,1,0,0");
        }

        /// <summary>
        /// After user starts while in automatic mode, automatic operation is restored in the next cycle.
        /// </summary>
        [Fact]
        public void AutomaticOperationRestoredAfterUserStop()
        {
            orchestrator.IsAutomatic = true;
            orchestrator.Start();
            AssertActions("i,e");
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            orchestrator.ThresholdMissed();
            AssertActions("i,e");
            orchestrator.ThresholdReached();
            AssertActions("i,e");
            orchestrator.Stop();
            AssertActions("i,e,d,f");
            AssertInitialButtonState();
            orchestrator.ThresholdMissed();
            AssertActions("i,e,d,f");
            AssertInitialButtonState();
            orchestrator.ThresholdReached();
            AssertButtonEnabledState(true, true);
            AssertActions("i,e,d,f,e");
            orchestrator.ThresholdMissed();
            AssertActions("i,e,d,f,e,d,f");
            AssertInitialButtonState();
            AssertSources("2,0,0,0,0,0,0");
        }

        private void AssertActions(string actions)
        {
            Assert.Equal(actions, string.Join(",", actionNames));
        }

        private void AssertSources(string sources)
        {
            Assert.Equal(sources, string.Join(",", actionSources));
        }

        private void AssertInitialButtonState()
        {
            AssertButtonEnabledState(true, false);
        }

        private void AssertButtonEnabledState(bool start, bool pause)
        {
            Assert.Equal(start, orchestrator.StartButton.IsEnabled);
            Assert.Equal(pause, orchestrator.PauseButton.IsEnabled);
        }

    }

}
