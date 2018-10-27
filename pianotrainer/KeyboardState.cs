using Midi;
using System.Collections.Generic;
using System.Linq;

namespace pianotrainer
{
    /// <summary>
    /// Maintains the state of a 127-key keyboard, which is the number of notes supported by MIDI.
    /// </summary>
    class KeyboardState
    {
        private readonly KeyboardKey[] keyboardKeys = new KeyboardKey[127];

        /// <summary>
        /// Creates a new keyboard state with all keys in the neutral position.
        /// </summary>
        public KeyboardState()
        {
            for (int index = 0; index < keyboardKeys.Length; index++)
            {
                keyboardKeys[index] = new KeyboardKey((Pitch)index);
            }
        }

        /// <summary>
        /// Updates the keyboard state to set a key to depressed.
        /// </summary>
        /// <param name="midiPitch">The MIDI pitch of the key.</param>
        public void PressKey(Pitch midiPitch)
        {
            keyboardKeys[(int)midiPitch].Press();
        }

        /// <summary>
        /// Updates the keyboard state to set a given key back to normal.
        /// </summary>
        /// <param name="midiPitch">The MIDI pitch of the key.</param>
        public void ReleaseKey(Pitch midiPitch)
        {
            keyboardKeys[(int)midiPitch].Release();
        }

        /// <summary>
        /// Returns a list of keys that are currently depressed.
        /// </summary>
        public IEnumerable<KeyboardKey> DepressedKeys
        {
            get
            {
                return keyboardKeys.Where(k => k.Down);
            }
        }
    }
}
