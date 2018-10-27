using System;
using System.Timers;
using Midi;

namespace pianotrainer
{
    /// <summary>
    /// Base class for trainer games.
    /// </summary>
    abstract class TrainerBase
    {
        private Timer timer = null;
        internal KeyboardController keyboardController = null;
        internal KeyboardState keyboardState = null;
        internal Pitch minimumPitch = Pitch.A0;
        internal Pitch maximumPitch = Pitch.C7;
        internal GameState gameState = GameState.Stopped;

        /// <summary>
        /// Creates a new game.
        /// </summary>
        /// <param name="timerInterval">The duration of a game cycle in miliseconds.</param>
        /// <param name="controller">A keyboard controller that will process the input device's messages.</param>
        /// <param name="state">The keyboard state that will represent the current state of the input device's keyboard.</param>
        protected TrainerBase(double timerInterval, KeyboardController controller, KeyboardState state)
        {
            keyboardController = controller;
            keyboardController.KeyboardStateChanged += KeyboardStateChanged;

            keyboardState = state;

            timer = new Timer(timerInterval);
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true;
        }

        /// <summary>
        /// Event handler called when MIDI input causes the KeyboardState to change.
        /// </summary>
        /// <param name="sender">The object causing the change.</param>
        /// <param name="e">Blank event arguments.</param>
        abstract internal void KeyboardStateChanged(object sender, EventArgs e);

        /// <summary>
        /// Event handler called when the game timer has elapsed a cycle.
        /// </summary>
        /// <param name="sender">The timer causing the event.</param>
        /// <param name="e">Timer event arguments.</param>
        abstract internal void TimerElapsed(object sender, ElapsedEventArgs e);

        /// <summary>
        /// Starts the game.
        /// </summary>
        virtual public void Start()
        {
            timer.Start();
        }

        /// <summary>
        /// Stops the game.
        /// </summary>
        virtual public void Stop()
        {
            timer.Stop();
        }

        /// <summary>
        /// Resets the timer but keeps it running.
        /// </summary>
        internal void ResetTimer()
        {
            timer.Stop();
            timer.Start();
        }
    }
}
