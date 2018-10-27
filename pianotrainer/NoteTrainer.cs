using System;
using System.Timers;
using Midi;

namespace pianotrainer
{
    class NoteTrainer : TrainerBase
    {
        Pitch chosenPitch = 0;
        public delegate void ViewModelChangedHandler(object sender, NoteTrainerViewModel trainerViewModel);
        public event ViewModelChangedHandler ViewModelChangedEvent;

        public NoteTrainer(KeyboardController controller, KeyboardState keyboardState) : base(500, controller, keyboardState)
        {
            minimumPitch = Pitch.B1;
            maximumPitch = Pitch.D6;
        }

        public override void Start()
        {
            PickNextNote();
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        internal override void KeyboardStateChanged(object sender, EventArgs e)
        {
            CheckPressedKeys();
        }

        internal override void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            switch (gameState)
            {
                case GameState.WaitingForInput:
                    CheckPressedKeys();
                    break;
                case GameState.ProceedToNext:
                    PickNextNote();
                    break;
            }
        }

        private void PickNextNote()
        {
            base.Stop();

            chosenPitch = (new Random().Next(maximumPitch - minimumPitch)) + minimumPitch;
            var viewModel = new NoteTrainerViewModel();
            viewModel.DisplayNotes.Add(new DisplayNote { MidiPitch = chosenPitch, State = DisplayNoteState.Neutral });
            ViewModelChangedEvent?.Invoke(this, viewModel);

            gameState = GameState.WaitingForInput;

            base.Start();
        }

        private void CheckPressedKeys()
        {
            bool correctKeyPressed = false;
            int incorrectKeys = 0;
            var viewModel = new NoteTrainerViewModel();

            foreach (KeyboardKey key in keyboardState.DepressedKeys)
            {
                if (key.MidiPitch.Equals(chosenPitch))
                {
                    viewModel.DisplayNotes.Add(new DisplayNote { MidiPitch = key.MidiPitch, State = DisplayNoteState.Correct });
                    correctKeyPressed = true;
                }
                else
                {
                    viewModel.DisplayNotes.Add(new DisplayNote { MidiPitch = key.MidiPitch, State = DisplayNoteState.Incorrect });
                    incorrectKeys++;
                }
            }

            if (!correctKeyPressed)
            {
                viewModel.DisplayNotes.Insert(0, new DisplayNote { MidiPitch = chosenPitch, State = DisplayNoteState.Neutral });
            }

            ViewModelChangedEvent?.Invoke(this, viewModel);

            if (correctKeyPressed && incorrectKeys == 0)
            {
                gameState = GameState.ProceedToNext;
                ResetTimer();
            }
        }
    }
}
