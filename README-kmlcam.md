# KML "Live Camera"

## Description

You can configure a [Network Link] to periodically "call back" to MSFS2020-PilotPathRecorder in order
to synchronize Google Earth's view with the simulator's view, in "real-time", using a [KML Camera].

- _*Note: as of this writing, this feature is only usable in the "desktop" version of "Google Earth"*_

## Setup

Once this feature is enabled within MSFS2020-PilotPathRecorder, configure and activate the Network Link
within Google Earth, as described below:

### MSFS2020-PilotPathRecorder

The "Live Camera" checkbox can be used to enable or disable support for this feature
within MSFS2020-PilotPathRecorder.  When checked, an internal "webserver" listener
will be activated (at the location specified in the "Network Link" textbox, just to the right
of the checkbox), enabling Google Earth to periodically retrieve the current position
of the simulated flight.

Though the default value provided for the Network Link URL (e.g., `http://localhost:8000/kmlcam`)
should work in most cases, you can change it if necessary prior to enabling the feature.

- _Note: this URL value should correspond to the one used for the "Network Link" within
Google Earth (see below)._

Here's an example of what it might look like right after being enabled:

![MSFS2020-PilotPathRecorder - "Live Camera Listener Started"](docs/images/PPRv13n2lcls.jpg)

### Google Earth

Once the Live Camera is enabled in MSFS2020-PilotPathRecorder, the Network Link can be installed
and/or activated within Google Earth, as illustrated in the example below.  This can be done
through using the `Link` button on the dialog (see "Extras", below), or manually within Google
Earth.  One way to bring up the "Edit Network Link" dialog within Google Earth is through its
menu item: `Add >> Network Link`.

The configured Network Link should look something like this:

![Google Earth - "Edit Network Link" Dialog](docs/images/GoogleEarthKmlCamNetlink.jpg)

The important configuration items are:

- `Link` - points Google Earth to MSFS2020-PilotPathRecorder for the updated, "live camera view";
    this should be set to the same value as configured in the "Network Link" text box within
    the MSFS2020-PilotPathRecorder dialog, as described in the previous section

- `Refresh` - sets the frequency with which the updates will take place; the more frequent the
    update, the "smoother" should be the apparent "movement" of the aircraft within Google Earth.
    Either supported "Refresh Mode" can be used (i.e., refresh as soon as the previous refresh
    completes, or every so often...)
 
- `Fly to View on Refresh` - tells Google Earth to "fly" to the newly updated "camera" position;
    always use this if you want Google Earth's view to follow the aircraft's position.

Once properly activated, the Network Link will periodically refresh the "Camera" view within
Google Earth to match the "straight ahead" view from within the simulator's cockpit.  

Should Google Earth be unable to reach MSFS2020-PilotPathRecorder's "Live Camera" through
the Network Link for an extended period of time, it may be necessary to manually "Refresh"
it from within Google Earth to restore synchronization once the link becomes reachable again.

## Extras

Two extra buttons appearing to the right of the Network Link URL can be used to customize
the "Live Camera", as follows:

- Use the `Edit` button to customize the templates used to build the "Network Link" or
    the "Camera" KML documents.  

- Use `Link` to install the Network Link directly into Google Earth (presuming it's installed
    and is the handler for `.kml` files), which will be launched if not currently running.

- _See the [KML Reference](https://developers.google.com/kml/documentation/kmlreference) for details_

### Examples

Instead of showing the view from the simulated airplane's cockpit, for example, you could use the
following "Camera Template" to display your airplane as a moving icon, as viewed from above:

```
<?xml version='1.0' encoding='UTF-8'?>
<kml
    xmlns = 'http://www.opengis.net/kml/2.2'
    xmlns:gx='http://www.google.com/kml/ext/2.2'
    xmlns:kml='http://www.opengis.net/kml/2.2'
    xmlns:atom='http://www.w3.org/2005/Atom'
>
  <Document>
    <Style id="planeIcon">
      <IconStyle>
        <scale>1.5</scale>
        <Icon>
          <href>
            https://findicons.com/files/icons/599/transport_for_vista/48/airplane.png
          </href>
        </Icon>
      </IconStyle>
    </Style>
    <Placemark>
      <name>Me</name>
      <description>Just flying around</description>
      <styleUrl>#planeIcon</styleUrl>
      <Point>
        <altitudeMode>absolute</altitudeMode>
        <coordinates>{longitude},{latitude},{altitude}</coordinates>
        <altitudeMode>absolute</altitudeMode>
      </Point>
    </Placemark>
  </Document>
</kml>
```

There are potentially countless creative ways this experimental feature can be used to track your flight.

If you come up with something interesting or that you'd like to share, please let us know!


[KML Camera]: https://developers.google.com/kml/documentation/cameras
[Network Link]: https://developers.google.com/kml/documentation/updates