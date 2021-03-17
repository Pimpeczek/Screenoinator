# Screenoinator
A simple, Windows Forms application allowing for:
- Cropping images in bulk
- Automated saving of selected screen region
- Manual screen part saving

### How to use the application
#### Image cropping
1. Select the **Cropping** tab
2. Press the **Select files** and choose the images you want to save a region from
*(images must be the same size)*
3. Select some part of the screen by dragging the selection in the overview on the right. You can alternatively provide exact values in the input fields on the left.
4. Select the output folder.
5. Process the files.

#### Automated screenshot taking
1. Take a calibration screenshot.
2. Select some part of the screen by dragging the selection in the overview on the right.
3. Optional: Change the interval between taking screenshots and the difference threshold\*.
4. Select the output folder.
5. Begin the process. Optional: You can take the screenshot manually.

#### Automated process
Screenshots are taken in provided intervals. A pilot screenshot is taken when the **Begin** button is pressed.
An automatically taken screenshot is downsampled 50 times for similarity comparison. This means that for an image of size 600x400 the compared images are 12x8.
All corresponding pixels are compared. For simplicity even the slightest difference is accounted for, i.e. ``#ff0000 != #ff0001``. If percent of differing pixels is greater or equal that the ``Threshold`` the original screenshot is considered different and consequently saved.
