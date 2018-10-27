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
        private Score score = null;

        public MainWindow()
        {
            InitializeComponent();

            keyboardState = new KeyboardState();
            keyboardController = new KeyboardController(keyboardState);
            game = new NoteTrainer(keyboardController, keyboardState);
            game.ViewModelChangedEvent += Game_ViewModelChangedEvent;
            foreach (string device in keyboardController.MidiDevices)
            {
                InputDeviceCombo.Items.Add(device);
            }

            score = Score.CreateOneStaffScore(Clef.Treble, MajorScale.C);
            score.AddStaff(Clef.Bass, TimeSignature.CommonTime, Step.C, MajorAndMinorScaleFlags.MajorFlat);

            GameViewer.ScoreSource = score;
        }

        private void Game_ViewModelChangedEvent(object sender, NoteTrainerViewModel trainerViewModel)
        {
            Dispatcher.Invoke(new Action<NoteTrainerViewModel>(UpdateDisplay), trainerViewModel);
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            keyboardController.OpenDevice(InputDeviceCombo.SelectedValue.ToString());
            game.Start();
        }

        ~MainWindow()
        {
            keyboardController.CloseDevice();
        }

        internal void UpdateDisplay(NoteTrainerViewModel trainerViewModel)
        {
            while(score.FirstStaff.Elements.Count > 2)
            {
                score.FirstStaff.Elements.RemoveAt(2);
            }
            while (score.SecondStaff.Elements.Count > 2)
            {
                score.SecondStaff.Elements.RemoveAt(2);
            }

            foreach (var displayNote in trainerViewModel.DisplayNotes)
            {
                var staff = (displayNote.MidiPitch >= Midi.Pitch.C4) ? score.FirstStaff : score.SecondStaff;

                var scoreNote = new Note(Pitch.FromMidiPitch((int)displayNote.MidiPitch, Pitch.MidiPitchTranslationMode.Auto), RhythmicDuration.Quarter) { IsUpperMemberOfChord = (staff.Elements.Count > 2) };
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

            if (score.FirstStaff.Elements.Count == 2)
            {
                score.FirstStaff.Elements.Add(new Rest(RhythmicDuration.Quarter));
            }
            if (score.SecondStaff.Elements.Count == 2)
            {
                score.SecondStaff.Elements.Add(new Rest(RhythmicDuration.Quarter));
            }
        }
    }
}
