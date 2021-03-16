# Screenoinator
A simple, Windows Forms application allowing for:
- Cropping images in bulk
- Automated saving of selected screen region
- Manual screen part saving

### How to use the application
#### Image cropping
1. Select the **Cropping** tab
2. Presse the **Select files** and choose the images you want to save a region from
*(images must be the same size)*
3. Select some part of the screen by dragging the selection in the overview on the right. You can alternatively provide exact values in the input fields on the left.
4. Select the output folder.
5. Process the files.

#### Automated screenshot taking
1. Take a callibration screenshot.
2. Select some part of the screen by dragging the selection in the overview on the right.
3. Optional: Change the interval between taking screenshots and the difference treshold\*.
4. Select the output folder.
5. Begin the process. Optional: You can take the screenshot manualy.

#### Automated process
Screenshost are taken in provided intervals. A pilot screeshot is taken when the **Begin** button is pressed.
An automatically taken screenshot is downsampled 50 times for simillarity comparison. This means that for an image of size 600x400 the compared images are 12x8.
All corresponding pixels are compared. For simplicity even the sightest difference is accounted for, i.e. ``#ff0000 != #ff0001``. If percent of differing pixels is greater or equal that the ``Treshold`` the oryginmal screenshot is considered different and consequently saved.

