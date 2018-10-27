using Midi;

namespace pianotrainer
{
    class KeyboardKey
    {
        public bool Down { get; private set; }
        public Pitch MidiPitch { get; }

        /// <summary>
        /// Represents the state of a single key on a keyboard.
        /// </summary>
        /// <param name="midiPitch"></param>
        internal KeyboardKey(Pitch midiPitch)
        {
            MidiPitch = midiPitch;
        }

        /// <summary>
        /// Sets the key to a depressed state.
        /// </summary>
        internal void Press()
        {
            Down = true;
        }

        /// <summary>
        /// Sets the key back to its normal state.
        /// </summary>
        internal void Release()
        {
            Down = false;
        }
    }
}
