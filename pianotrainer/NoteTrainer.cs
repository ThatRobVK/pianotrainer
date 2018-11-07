using System;
using System.Collections.Generic;
using System.Timers;
using Midi;

namespace pianotrainer
{
    class NoteTrainer : TrainerBase
    {
        private Pitch chosenPitch = 0;
        public delegate void ViewModelChangedHandler(object sender, NoteTrainerViewModel trainerViewModel);
        public event ViewModelChangedHandler ViewModelChangedEvent;
        private const Pitch LeftHandMinPitch = Pitch.B1;
        private const Pitch LeftHandMaxPitch = Pitch.C4;
        private const Pitch RightHandMinPitch = Pitch.C4;
        private const Pitch RightHandMaxPitch = Pitch.D6;
        private List<int> SharpOctavePositions = new List<int> { 1, 3, 6, 8, 10 };

        public NoteTrainer(KeyboardController controller, KeyboardState keyboardState) : base(500, controller, keyboardState)
        { }

        public override void Start(GameConfiguration gameConfiguration)
        {
            switch (gameConfiguration.Hands)
            {
                case Hands.Both:
                    minimumPitch = LeftHandMinPitch;
                    maximumPitch = RightHandMaxPitch;
                    break;
                case Hands.Left:
                    minimumPitch = LeftHandMinPitch;
                    maximumPitch = LeftHandMaxPitch;
                    break;
                case Hands.Right:
                    minimumPitch = RightHandMinPitch;
                    maximumPitch = RightHandMaxPitch;
                    break;
            }

            base.Start(gameConfiguration);

            PickNextNote();
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

            // If flats and sharps are disabled, and the chosen pitch is either, drop it by 1 to the nearest natural
            if (!GameConfiguration.Sharps && !GameConfiguration.Flats)
            {
                var position = chosenPitch.PositionInOctave();
                if (SharpOctavePositions.Contains(position))
                    chosenPitch--;
            }

            var viewModel = new NoteTrainerViewModel();
            viewModel.DisplayNotes.Add(new DisplayNote { MidiPitch = chosenPitch, State = DisplayNoteState.Neutral });
            ViewModelChangedEvent?.Invoke(this, viewModel);

            gameState = GameState.WaitingForInput;

            base.Resume();
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
