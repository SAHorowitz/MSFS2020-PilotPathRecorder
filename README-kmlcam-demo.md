# Feb 2021 KmlCam Demonstration

These notes guide a demonstration of the "LiveCam" enhancements as of 
February 2021.

## Intro

One of the nicest add-ons I've come across for Microsoft Flight Simulator is
a nifty tool named `PilotPathRecorder`, created by SA Horowitz, available as
a downloadable ZIP file from GitHub.  I love how it can help me post-analyze
and visualize flights I've taken within Google Earth's simulated 3D world.

Being a huge fan of moving map technology to help gain real-time situational
awareness from a secondary device, I thought it would be worthwhile to enhance
`PilotPathRecorder` to enable visualization in Google Earth while flying, in
near "real time" - not just retrospectively, after the flight completes.

This demo is a short presentation of the enhancements made so far to add this
type of functionality to `PilotPathRecorder`.

*\<\<bring up the app\>\>*

## Feature Presentations

Three enhancements made to the user interface are:

- Two new "Connection Types" for providing flight path data input
- Addition of "LiveCams" for providing "near real-time" display in Google Earth
- A new "Launch KML File" button

### Connections

When developing new features, it's important to have a reliable and abundant
source of test data.  To this end, I introduced the ability to generate two
new streams of flight path data:

- `Replay` - replays a previously logged stream of flight data
- `RandomWalk` - generates a new, random stream of flight data

I found it very cumbersome without these enhancements to develop LiveCams,
since otherwise I would have needed to fly while developing.  With these
new features, I can test LiveCams while either replaying a previously 
recorded flight, or while playing a new, randomly generated flight.

### LiveCams

I use the term "LiveCam" to describe a new view that `PilotPathRecorder` 
can give the user of a flight path it's receiving.

I've considered two of the LiveCam definitions I've experimented with so far
worthy of inclusion as "builtin" to the application - they are:

- `Cockpit` - representing a view from the "cockpit" of the simulated flight
- `Tagalong` - where the "camera" is moved only manually by the user

*\<\<show these in the "LiveCam" checkbox and drop-down\>\>*

#### Demonstration

Here's a demonstration of some of these ideas:

##### Random Walk

1. *\<\<start the app\>\>*
1. *\<\<point out `Automatic Logging`\>\>*
1. *\<\<start a `RandomWalk`\>\>*
1. *\<\<point out logging started\>\>*

##### Cockpit LiveCam

1. *\<\<select the `Cockpit` LiveCam\>\>*
1. *\<\<start `Live Camera`\>\>*
1. *\<\<use `Link` to start it in Google Earth\>\>*
1. *\<\<point out how it's tracking the `Cockpit` view\>\>*

##### Tagalong LiveCam

1. *\<\<select the `Tagalong` LiveCam\>\>*
1. *\<\<use `Link` to start it in Google Earth\>\>*
1. *\<\<point out it's tracking <u>both</u> `Tagalong` and `Cockpit`\>\>*
1. *\<\<point out the KML "Placeholder" representing the airplane\>\>*
1. *\<\<click on the Placeholder to retrieve the current data\>\>*
1. *\<\<disable the `Cockpit` view in Google Earth\>\>*
1. *\<\<point out how in `Tagalong` user controls the vantage\>\>*
1. *\<\<point out how the track line is left in `Tagalong`\>\>*

#### LiveCam Development

LiveCam definitions were kept external to `PilotPathRecorder`, and are
stored in JSON files containing what are called "Lens templates."  Using
the `Edit` button, you can customize built-in LiveCams or create your
own.  As an example, I'll modify the `Tagalong` LiveCam to un-comment
a block of KML which was disabled in the built-in template because it
sometimes caused Google Earth to crash.

1. *\<\<select `Tagalong` and press `Edit`\>\>*
1. *\<\<introduce the three `Lens` tabs\>\>*
1. *\<\<briefly show the templates and their inter-lens references\>\>*
1. *\<\<un-comment the `Model` tag\>\>*
1. *\<\<show the reference to the 3D model\>\>*
1. *\<\<change the `Scale` tag to 15 times\>\>*
1. *\<\<save the updated LiveCam definition\>\>*

This change will take effect right away, and you'll see the 3D airplane
model in addition to the airplane Placemark.  

*\<\<point out the 3D airplane model, and that it's 15x normal size\>\>*

The modified version of this LiveCam will now be used instead of the builtin
version you started with.

### Launch KML Button

I also found myself wanting a "shortcut" to invoke Google Earth directly
from within `PilotPathRecorder`, so I created the `Launch KML File`.
Pressing this button is the same as pressing the existing `Create KML File`
button, then loading the newly created file manually within Google Earth.

### Another Cool Idea

Just before I created this video, I happened upon another really cool resource
for helping me to use Google Earth as a "moving map" for tracking my flight
progress.  At the bottom of [this chartbundle.com page](http://www.chartbundle.com/charts/),
find the link to the file [chartbundle_aero.kml](http://www.chartbundle.com/charts/kml/chartbundle_aero.kml),
which I used to overlay FAA aeronautical charts directly onto Google Earth,
on top of which to project my flight path with `PilotPathRecorder'.

*\<\<demonstration\>\>*
