# Developer Notes

These notes are aimed at developers wishing to use Visual Studio 2019 (VS 2019)
or other tool to continue development on, or make enhancements to
MSFS2020-PilotPathRecorder.

Please share knowledge here that could help to make future development more pleasant
and/or less frustrating!

## Quick Links

- [Signing The Application Manifest](#signing-the-application-manifest)
- [LiveCam Webserver Tricks](#livecam-webserver-tricks)

## Signing The Application Manifest

When trying to build this project for the first time, you are likely to receive the error:

```
Unable to find manifest signing certificate in the certificate store.
```

There are (at least) two ways to get beyond this problem during development:

### Use Your Own "Test Certificate"

Using VS 2019, it's easy to create and sign with your own "test" certificate.

With the project selected in the _Solution Explorer_:

1. Select `Properties` (e.g., _`Alt + Enter`_)
1. Select `Signing`
1. Select `Create Test Certificate...`
1. Enter a new password
1. Ensure `Sign the ClickOnce manifests` is checked
1. Save the change (e.g., _`Ctrl+S`_)

### Disable Signing Altogether

Disabling the signing is appropriate for development purposes; however, it's better
to create and sign the released program with a certificate (see above.)

#### Via Manual Edit of Project (.csproj) File

You can disable the signing of the manifest file by changing the `SignManifests`
configuration value from `true` to `false`.  Within VS 2019 for example, this
can be accomplished with the project selected in the _Solution Explorer_:

1. Unload the project (e.g., `Project >> Unload Project`)
1. Edit the Project File (e.g., `Project >> Edit Project File`)
1. Change the value for `SignManifests` from `true` to `false`
1. Reload the Project (e.g., `Project >> Reload Project`)

#### Using VS 2019 Properties

Within VS 2019, you can disable signing with the project selected in
the _Solution Explorer_:

1. Select `Properties` (e.g., _`Alt + Enter`_)
1. Select `Signing`
1. Ensure neither the `Sign the ClickOnce manifests` checkbox nor the
   `Sign the assembly` checkbox are checked
1. Save the change (e.g., _`Ctrl+S`_)

Once the desired change is made, cleaning and rebuilding the project without
the error depicted above should be possible.

## LiveCam Webserver Tricks

The following additional endpoints (see [LiveCam](README-kmlcam.md)) can be helpful during
development or customization of LiveCams:

- `http://localhost:8000/eval/Model.values` (shows current real-time LiveCam values)
- `http://localhost:8000/eval/template` (renders a template `POST`ed to this endpoint)

The default handler for an unrecognized HTTP request will attempt to read and return
the resource (file) named by the URL path.  This enables built-in resources to be
accessed by liveCam templates.  As of this writing, there is no way to load new
resources from the dialog; however, they can be manually copied into the application
runtime directory, if needed.
