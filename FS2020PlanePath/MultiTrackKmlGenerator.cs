using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using System.Collections.Generic;

namespace FS2020PlanePath
{

    // noodnik2 TODO - use this in a LiveCam script & then delete this hard-coded class

    public class MultiTrackKmlGenerator
    {

        public string GetMultiTrackKml()
        {
            trackNo = trackNo + 1;
            Kml kml = NewGxKml();
            Document updatedDocument = GetUpdatedDocument();

            if (trackNo == 1)
            {
                kml.Feature = updatedDocument;
                return StringOf(kml);
            }

            CreateCollection createCollection = new CreateCollection();
            createCollection.Add(updatedDocument);

            Update update = new Update();
            update.AddUpdate(createCollection);

            NetworkLinkControl networkLinkControl = new NetworkLinkControl();
            networkLinkControl.Cookie = $"seq={lastSeq}";
            networkLinkControl.Update = update;

            kml.NetworkLinkControl = networkLinkControl;
            return StringOf(kml);
        }

        public void AddKmlCameraParameterValues(KmlCameraParameterValues kmlCameraParameterValues)
        {
            this.kmlCameraParameterValuesList.Add(kmlCameraParameterValues);
        }

        private Document GetUpdatedDocument()
        {
            MultipleTrack multipleTrack = new MultipleTrack();
            multipleTrack.Id = "sean";

            Track track = new Track();
            track.Id = $"sean_t{trackNo}";
            foreach (KmlCameraParameterValues kmlCameraParameterValues in kmlCameraParameterValuesList.FindAll(e => e.seq > lastSeq))
            {
                track.AddCoordinate(
                    new Vector
                    {
                        Altitude = kmlCameraParameterValues.altitude,
                        Longitude = kmlCameraParameterValues.longitude,
                        Latitude = kmlCameraParameterValues.latitude
                    }
                );
                track.AddAngle(
                    new Angle {
                        Heading = kmlCameraParameterValues.heading,
                        Pitch = kmlCameraParameterValues.tilt,
                        Roll = kmlCameraParameterValues.roll
                    }
                );
                lastSeq = kmlCameraParameterValues.seq;
            }
            multipleTrack.AddTrack(track);

            Placemark placemark = new Placemark();
            placemark.Geometry = multipleTrack;

            Document document = new Document();
            document.AddFeature(placemark);
            return document;
        }

        private string StringOf(Kml kml)
        {
            Serializer serializer = new Serializer();
            serializer.Serialize(kml);
            return serializer.Xml;
        }

        private static Kml NewGxKml()
        {
            Kml kml = new Kml();
            kml.AddNamespacePrefix(KmlNamespaces.GX22Prefix, KmlNamespaces.GX22Namespace);
            return kml;
        }

        private List<KmlCameraParameterValues> kmlCameraParameterValuesList = new List<KmlCameraParameterValues>();
        private int trackNo;
        private long lastSeq;

    }

}
