# LiveCam Q & A

These notes are aimed at answering the most frequent questions
arising for new users of `PilotPathRecorder`'s "LiveCam" support.

Please let us know if your question isn't answered below!

## _What does it do?  Can I have a demo?_

Yes!  Check out this YouTube video:

- [![MSFS2020-PilotPathRecorder v1.3.0n2b Enhancement Demo](https://img.youtube.com/vi/KmmUC1Yl1oo/0.jpg)](https://www.youtube.com/watch?v=KmmUC1Yl1oo)

Also check out a [script](README-kmlcam-demo.md) that was developed
to help demonstrate these features.

## _What are the prerequisites?_

`MSFS2020-PilotPathRecorder` has been successfully
installed on [Windows 10] with a standard installation of
the Microsoft [.NET Framework 4.7.2] (runtime framework); the
desktop version of [Google Earth] (e.g., try version 7.3 if you're
unsure), and relies on its normal association with the `.kml` file
extension.

So, basically it's:
- [Windows 10]
- [.NET Framework 4.7.2]
- desktop version of [Google Earth]

## _How do I install and run it?_

See the [base installation instructions].

Basically, it's:

1. Download the `PilotPathRecorder*.zip` deliverable from [releases].
   Unless you have a specific reason not to, please choose the [latest release]. 
1. Unzip the release into a new application folder, from where you will run it.
   Note that the application will need write access to this folder (e.g., in
   order to store its state) when it runs.
1. To run the application, double-click or otherwise invoke the file
   named `PilotPathRecorder.exe`, which you'll find within the unzipped
   folder.  It can be helpful to create a link to this location (e.g.,
   on your desktop) for easier access.

> _NOTE: as of this writing, the releases are not "signed", meaning
you will likely receive a message from Windows advising you to refrain
from unzipping and/or running the application, as it doesn't come from
a "trusted" source.  Rest assured, there is no malware included; you
can override the warning and continue to install and run the application,
taking comfort that the full source code, [automated build script] and
[build logs] are provided for each release._

## _How can I create my own LiveCams?_

Another YouTube video covers the subject of how to customize
and/or create your own LiveCams:

- [![MSFS2020-PilotPathRecorder v1.3.0n2b Enhancement - LiveCam Tutorial](https://img.youtube.com/vi/qcHL9cVQe5s/0.jpg)](https://www.youtube.com/watch?v=qcHL9cVQe5s)

Time to crack open the [KML Documentation]!

## _What was your inspiration?_

I'm glad you asked :hand_over_mouth:

In addition to being inspired by [SA Horiwitz's original project], and being a fan
of moving map technology (i.e., I put 2+2 together), I found the following video
of a very relevant presentation (given at Google I/O way back in 2011), which
convinced me this could be done for simulated flights as well.  The relevant
presentation starts at offset 16 minutes 17 seconds into the video:

- [![Google I/O 2011: High Performance KML for Maps and Earth](https://img.youtube.com/vi/nIoWHlEEeNI/0.jpg)](https://www.youtube.com/watch?v=nIoWHlEEeNI&t=16m17s)

[releases]: https://github.com/noodnik2/MSFS2020-PilotPathRecorder/releases
[latest release]: https://github.com/noodnik2/MSFS2020-PilotPathRecorder/releases/latest
[automated build script]: .github/workflows/build.yml
[build logs]: https://github.com/noodnik2/MSFS2020-PilotPathRecorder/actions
[Google Earth]: https://www.google.com/earth/versions/
[Windows 10]: https://www.microsoft.com/en-us/windows/get-windows-10
[.NET Framework 4.7.2]: https://dotnet.microsoft.com/download/dotnet-framework/net472
[base installation instructions]: README.md#instructions-for-install
[KML Documentation]: https://developers.google.com/kml/documentation
[SA Horiwitz's original project]: https://github.com/SAHorowitz/MSFS2020-PilotPathRecorder