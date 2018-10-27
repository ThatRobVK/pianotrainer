using System;
using System.Collections.Generic;
using System.Linq;
using Midi;

namespace pianotrainer
{
    internal class KeyboardController
    {
        internal event EventHandler KeyboardStateChanged;

        private readonly KeyboardState state;
        private InputDevice inputDevice = null;

        /// <summary>
        /// Maintains a keyboard state based on input from a given MIDI device.
        /// </summary>
        /// <param name="keyboardState">The keyboard state to update based on the MIDI device.</param>
        internal KeyboardController(KeyboardState keyboardState)
        {
            state = keyboardState;
        }

        /// <summary>
        /// Ensures the MIDI device is closed.
        /// </summary>
        ~KeyboardController()
        {
            CloseDevice();
        }

        /// <summary>
        /// Returns a list of available MIDI devices.
        /// </summary>
        internal IEnumerable<string> MidiDevices
        {
            get
            {
                return InputDevice.InstalledDevices.Select(i => i.Name);
            }
        }

        /// <summary>
        /// Opens a MIDI device and starts listening for input.
        /// </summary>
        /// <param name="midiDevice">The name of a MIDI device to open.</param>
        internal void OpenDevice(string midiDevice)
        {
            CloseDevice();

            inputDevice = InputDevice.InstalledDevices.Where(d => d.Name.Equals(midiDevice)).First();
            if (inputDevice != null)
            {
                inputDevice.Open();

                inputDevice.NoteOn += InputDevice_NoteOn;
                inputDevice.NoteOff += InputDevice_NoteOff;

                inputDevice.StartReceiving(null);
            }
        }

        /// <summary>
        /// Event handler for MIDI NoteOff messages.
        /// </summary>
        /// <param name="msg">The message from the MIDI device.</param>
        private void InputDevice_NoteOff(NoteOffMessage msg)
        {
            UpdateState(msg.Pitch, false);
        }

        /// <summary>
        /// Event handler for MIDI NoteOn messages.
        /// </summary>
        /// <param name="msg">The message from the MIDI device.</param>
        private void InputDevice_NoteOn(NoteOnMessage msg)
        {
            // Some devices signal NoteOff by sending a NoteOn message with a Velocity of 0
            UpdateState(msg.Pitch, (msg.Velocity > 0));
        }

        /// <summary>
        /// Updates the keyboard state for a given MIDI pitch.
        /// </summary>
        /// <param name="midiPitch">The MIDI pitch of the key to update.</param>
        /// <param name="depressed">True if the key was pressed, false if it was released.</param>
        private void UpdateState(Pitch midiPitch, bool depressed)
        {
            if (depressed)
            {
                state.PressKey(midiPitch);
            }
            else
            {
                state.ReleaseKey(midiPitch);
            }

            // Raise an event to allow others to update
            KeyboardStateChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Closes the current MIDI device.
        /// </summary>
        internal void CloseDevice()
        {
            if (inputDevice != null && inputDevice.IsOpen)
            {
                if (inputDevice.IsReceiving)
                {
                    inputDevice.StopReceiving();
                }

                inputDevice.RemoveAllEventHandlers();
                inputDevice.Close();
            }
        }
    }

}
