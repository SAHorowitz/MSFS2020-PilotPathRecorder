# Developer Notes

## Manifest

One way to (temporarily) get past the error message:

```
Unable to find manifest signing certificate in the certificate store.
```

Is to disable the "signing" of the certificate.  E.g., With the project selected
in the _Solution Explorer_:

1. Unload the project (e.g., `Project >> Unload Project`)
1. Edit the Project File (e.g., `Project >> Edit Project File`)
1. Change the value for `SignManifests` from `true` to `false`
1. Reload the Project (e.g., `Project >> Reload Project`)

- *CAUTION*: _remember to re-enable this_ prior to release!
