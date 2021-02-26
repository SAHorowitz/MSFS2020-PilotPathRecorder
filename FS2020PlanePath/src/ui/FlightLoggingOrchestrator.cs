using System;
using System.Diagnostics;

namespace FS2020PlanePath
{

    /// <summary>
    /// Class containing the logic to "orchestrate" the logging of flight data,
    /// in order to more clearly define its behavior and separate it from the UI,
    /// and to facilitate testing.
    /// </summary>
    public class FlightLoggingOrchestrator
    {

        /// <summary>
        /// Identifier of the source triggering a particular action being taken
        /// </summary>
        public enum TriggerSource
        {
            StartStop,      // "Start / Stop" (toggle) button
            PauseResume,    // "Pause / Resume" (toggle) button
            Initialization,
            Reset
        }

        /// <summary>
        /// Reflects current state of "automatic logging" - true if enabled
        /// </summary>
        public bool IsAutomatic { get; set; }

        /// <summary>
        /// Model of the "Start / Stop" (toggle) button
        /// </summary>
        public IButtonStateModel<ToggleState> StartButton { get; private set; }

        /// <summary>
        /// Model of the "Pause / Resume" (toggle) button
        /// </summary>
        public IButtonStateModel<ToggleState> PauseButton { get; private set; }

        /// <summary>
        /// Action taken to initialize logging
        /// </summary>
        public Action<TriggerSource> InitializeLoggingAction { get; private set; }

        /// <summary>
        /// Action taken to enable logging
        /// </summary>
        public Action<TriggerSource> EnableLoggingAction { get; private set; }

        /// <summary>
        /// Action taken to disable logging
        /// </summary>
        public Action<TriggerSource> DisableLoggingAction { get; private set; }

        /// <summary>
        /// Action taken to flush (write out any unwritten) log entries
        /// </summary>
        public Action<TriggerSource> FlushLoggingAction { get; private set; }

        private bool userStarted;
        private bool thresholdReached;

        public FlightLoggingOrchestrator(
            IButtonStateModel<ToggleState> enableButton,
            IButtonStateModel<ToggleState> pauseButton,
            Action<TriggerSource> initializeLoggingAction,
            Action<TriggerSource> enableLoggingAction,
            Action<TriggerSource> disableLoggingAction,
            Action<TriggerSource> flushLoggingAction
        )
        {
            this.StartButton = enableButton;
            this.PauseButton = pauseButton;
            this.InitializeLoggingAction = initializeLoggingAction;
            this.EnableLoggingAction = enableLoggingAction;
            this.DisableLoggingAction = disableLoggingAction;
            this.FlushLoggingAction = flushLoggingAction;
            InitializeLoggingAction(TriggerSource.Initialization);
        }

        /// <summary>
        /// Called when the logging "threshold" is reached, meaning that logging
        /// should be started, if "automatic" mode is enabled.
        /// </summary>
        public void ThresholdReached()
        {
            if (IsAutomatic && !thresholdReached && StartButton.State == ToggleState.Out)
            {
                StartAction(false);
            }
            thresholdReached = true;
        }

        /// <summary>
        /// Called when the logging "threshold" is missed (i.e., was reached but
        /// has become no longer reached), meaning that logging should be stopped,
        /// if "automatic" mode is enabled, so long as the user did not start it.
        /// </summary>
        public void ThresholdMissed()
        {
            if (
                IsAutomatic 
             && !userStarted 
             && thresholdReached
             && StartButton.State == ToggleState.In
             && PauseButton.State == ToggleState.Out
            )
            {
                StopAction();
            }
            thresholdReached = false;
        }

        /// <summary>
        /// Called when the user wants to start logging.
        /// </summary>
        public void Start()
        {
            StartAction(true);
        }

        /// <summary>
        /// Called when the user wants to stop logging.
        /// </summary>
        public void Stop()
        {
            StopAction();
        }

        /// <summary>
        /// Called when the user wants to pause logging.
        /// </summary>
        public void Pause()
        {
            PauseAction();
        }

        /// <summary>
        /// Called when the user wants to resume logging.
        /// </summary>
        public void Resume()
        {
            ResumeAction();
        }

        /// <summary>
        /// Resets the logger to initial state
        /// </summary>
        public void Reset()
        {
            thresholdReached = false;
            userStarted = false;
            InitializeLoggingAction(TriggerSource.Reset);
            FlushLoggingAction(TriggerSource.Reset);
        }

        private void StartAction(bool fromUi)
        {
            Debug.Assert(StartButton.State == ToggleState.Out);
            EnableLoggingAction(TriggerSource.StartStop);
            StartButton.State = ToggleState.In;
            PauseButton.State = ToggleState.Out;
            PauseButton.IsEnabled = true;
            userStarted = fromUi;
        }

        private void StopAction()
        {
            Debug.Assert(StartButton.State == ToggleState.In);
            DisableLoggingAction(TriggerSource.StartStop);
            FlushLoggingAction(TriggerSource.StartStop);
            StartButton.State = ToggleState.Out;
            PauseButton.State = ToggleState.Out;
            PauseButton.IsEnabled = false;
        }

        private void PauseAction()
        {
            Debug.Assert(StartButton.State == ToggleState.In);
            Debug.Assert(PauseButton.State == ToggleState.Out);
            DisableLoggingAction(TriggerSource.PauseResume);
            PauseButton.State = ToggleState.In;
        }

        private void ResumeAction()
        {
            Debug.Assert(StartButton.State == ToggleState.In);
            Debug.Assert(PauseButton.State == ToggleState.In);
            EnableLoggingAction(TriggerSource.PauseResume);
            PauseButton.State = ToggleState.Out;
        }

    }

}
