# pianotrainer
## What is it?
This app helps you practice sheet music sight reading by prompting you for a note across a set of treble and bass staves. It listens to input from any standard MIDI device, showing you which notes you are playing and comparing them against the prompted note. Any incorrect notes are shown in red. When the correct note, and only the correct note, is played, a new note is selected at random and prompted. This allows for continuous practice.
Currently the app picks any note in the range B1 to D6, including flats and sharps.

### How to use it
It's still in a very rough and ready state, so you will need to run this from Visual Studio 2017. Plug in and switch on your MIDI device before starting the application. The library used loads standard MIDI devices from Windows so ensure it is installed and working. Start the application, select your MIDI device in the drop down and click Start game. The game will continue to prompt for notes until you close it.

### The interface is a bit...
This is only a one-day hack that I've been meaning to do for months. I have not spent any time on the interface beyond putting the controls on that are required to make it work. As I expand the app I'll tidy up the UI and mkae it look a bit less... bad.

### And the code...
It's a spike, true and simple. I have mainly focused on getting something functional. I haven't applied any architectural or design patterns per se. I did have in mind ways I would like to extend it in future so where it wasn't going to add a lot of effort I have built this in an extensible way. As I figure out how I want to extend this further, over time I will evolve this to have a stronger architecture and include all the standard good practices like unit tests etc.

## The future
I have a number of ideas of things I'd like this to do in the future:
* Select a hand for one-handed practice
* Multiple notes, chords
* Simultaneous two-handed practice
* Different keys
* Generate random practice songs within a given key
* Dynamics training (ppp - fff)