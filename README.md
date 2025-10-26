# redLightVision

A minimal UI for running a Red Therapy screen session to help users rest or exercise their eyes.
Users can select a session duration (1–3 minutes) and start the session.

When started, presents a full-screen red overlay with faint, randomly appearing white circles of varying sizes and a countdown timer in the bottom-left corner.
The session runs asynchronously for the specified duration and the form closes automatically when complete.

Behavior:

Compact interface to choose and run a timed red-screen session.
Displays subtle white circles and a visible countdown timer.
The main form awaits session completion before disposing of the overlay.
Asynchronous execution maintains UI responsiveness.

Notes:

This file contains only the MainForm implementation.
Uses async/await for non-blocking UI updates.
Fixed-size, simple layout for a minimal appearance.
