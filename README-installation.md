# Installation Notes

These notes are aimed at answering the most frequent questions
arising for new users.

Please let us know if your question isn't answered below!

## "What are the prerequisites?"

- This version of `MSFS2020-PilotPathRecorder` has been successfully
installed on [Windows 10](https://www.microsoft.com/en-us/windows/get-windows-10),
and requires a standard installation of the 
[desktop version of Google Earth](https://www.google.com/earth/versions/)
(e.g., try version 7.3 if you're unsure) to be associated with the `.kml`
file extension.

## "How do I install and run it?"

NOTE: the notes below are addendum to the [installation instructions](README.md#instructions-for-install)
on the main README document. 

1. Download the `PilotPathRecorder*.zip` deliverable build asset from a 
   [release](https://github.com/noodnik2/MSFS2020-PilotPathRecorder/releases).
   If you haven't any reason to prefer a specific release, please 
   [choose the latest](https://github.com/noodnik2/MSFS2020-PilotPathRecorder/releases/latest). 
1. Unzip the release into a new application folder, from where you will run it.
   Note that the application will need write access to this folder (e.g., in
   order to store its state) when it runs.
1. To run the application, double-click or otherwise invoke the file
   named `PilotPathRecorder.exe`, which you'll find within the unzipped
   folder.  It can be helpful to create a link to this location (e.g.,
   on your desktop) for easier access.
   
> _NOTE: as of this writing, the releases are not "signed", meaning
you will likely receive a message from Windows advising you to
refrain from unzipping and/or running the application, as it doesn't
come from a "trusted" source.  Rest assured, there is no malware
included; you can override the warning and continue to install
and run the application, taking comfort that the full source code
and [automated build script](.github/workflows/build.yml) are
provided for [each release](https://github.com/noodnik2/MSFS2020-PilotPathRecorder/releases)._
