using System;
using System.Windows;
using Manufaktura.Controls.Model;
using Manufaktura.Controls.Primitives;
using Manufaktura.Music.Model;
using Manufaktura.Music.Model.MajorAndMinor;

namespace pianotrainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KeyboardState keyboardState = null;
        private KeyboardController keyboardController = null;
        private NoteTrainer game = null;
        private Score musicScore = null;
        private GameConfiguration gameConfiguration = null;

        public MainWindow()
        {
            InitializeComponent();

            keyboardState = new KeyboardState();
            keyboardController = new KeyboardController(keyboardState);
            game = new NoteTrainer(keyboardController, keyboardState);
            game.ViewModelChangedEvent += Game_ViewModelChangedEvent;
            InputDeviceCombo.ItemsSource = keyboardController.MidiDevices;
            HandCombo.ItemsSource = new string[] { Hands.Left.ToString(), Hands.Right.ToString(), Hands.Both.ToString() };
            HandCombo.SelectedIndex = 0;

            musicScore = Score.CreateOneStaffScore(Clef.Treble, MajorScale.C);
            musicScore.AddStaff(Clef.Bass, TimeSignature.CommonTime, Step.C, MajorAndMinorScaleFlags.MajorFlat);

            GameViewer.ScoreSource = musicScore;
        }

        private void Game_ViewModelChangedEvent(object sender, NoteTrainerViewModel trainerViewModel)
        {
            Dispatcher.Invoke(new Action<NoteTrainerViewModel>(UpdateDisplay), trainerViewModel);
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            keyboardController.OpenDevice(InputDeviceCombo.SelectedValue.ToString());
            var gameConfiguration = new GameConfiguration
            {
                Hands = (Hands)HandCombo.SelectedIndex,
                Flats = FlatsCheckbox.IsChecked.GetValueOrDefault(true),
                Sharps = SharpsCheckbox.IsChecked.GetValueOrDefault(true)
            };
            this.gameConfiguration = gameConfiguration;

            game.Start(gameConfiguration);

            FlipForm();
        }

        ~MainWindow()
        {
            keyboardController.CloseDevice();
        }

        internal void UpdateDisplay(NoteTrainerViewModel trainerViewModel)
        {
            ClearMusicScore();

            var translationMode = Pitch.MidiPitchTranslationMode.Auto;
            if (gameConfiguration.Flats && !gameConfiguration.Sharps)
            {
                translationMode = Pitch.MidiPitchTranslationMode.Flats;
            }
            else if (gameConfiguration.Sharps && !gameConfiguration.Flats)
            {
                translationMode = Pitch.MidiPitchTranslationMode.Sharps;
            }

            foreach (var displayNote in trainerViewModel.DisplayNotes)
            {
                var staff = (displayNote.MidiPitch >= Midi.Pitch.C4) ? musicScore.FirstStaff : musicScore.SecondStaff;

                var scoreNote = new Note(Pitch.FromMidiPitch((int)displayNote.MidiPitch, translationMode), RhythmicDuration.Quarter) { IsUpperMemberOfChord = (staff.Elements.Count > 2) };
                switch (displayNote.State)
                {
                    case DisplayNoteState.Correct:
                        scoreNote.CustomColor = new Color(0, 255, 0, 255);
                        break;
                    case DisplayNoteState.Incorrect:
                        scoreNote.CustomColor = Color.Red;
                        break;
                }
                staff.Add(scoreNote);
            }

            if (musicScore.FirstStaff.Elements.Count == 2)
            {
                musicScore.FirstStaff.Elements.Add(new Rest(RhythmicDuration.Quarter));
            }
            if (musicScore.SecondStaff.Elements.Count == 2)
            {
                musicScore.SecondStaff.Elements.Add(new Rest(RhythmicDuration.Quarter));
            }
        }

        private void ClearMusicScore()
        {
            while (musicScore.FirstStaff.Elements.Count > 2)
            {
                musicScore.FirstStaff.Elements.RemoveAt(2);
            }
            while (musicScore.SecondStaff.Elements.Count > 2)
            {
                musicScore.SecondStaff.Elements.RemoveAt(2);
            }
        }

        private void StopGameButton_Click(object sender, RoutedEventArgs e)
        {
            game.Stop();
            keyboardController.CloseDevice();
            ClearMusicScore();
            FlipForm();
        }

        private void FlipForm()
        {
            InputDeviceCombo.IsEnabled = !InputDeviceCombo.IsEnabled;
            HandCombo.IsEnabled = !HandCombo.IsEnabled;
            FlatsCheckbox.IsEnabled = !FlatsCheckbox.IsEnabled;
            SharpsCheckbox.IsEnabled = !SharpsCheckbox.IsEnabled;
            StartGameButton.IsEnabled = !StartGameButton.IsEnabled;
            StopGameButton.IsEnabled = !StopGameButton.IsEnabled;
        }
    }

    internal class GameConfiguration
    {
        internal Hands Hands { get; set; }
        internal bool Sharps { get; set; }
        internal bool Flats { get; set; }
    }

    internal enum Hands
    {
        Left = 0,
        Right = 1,
        Both = 2
    }
}
