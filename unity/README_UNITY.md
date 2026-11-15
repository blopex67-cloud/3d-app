# Unity Android prototype

Separate Unity project in `unity/`. It leaves the Three.js/Cordova game and current workflow intact. Phase 1 is a self-contained 3D pizzeria blockout, first-person mobile camera, touch look/movement, stage animatronic placeholders, reusable vent hiding UI, flashlight and a 30-second chase placeholder. Original imported models, level parity and full enemy AI are not migrated yet.

## Unity Build Automation target
- Repository: `blopex67-cloud/3d-app`, branch `main`.
- Project subfolder: exactly `unity`.
- Platform: Android APK, landscape.
- Unity version: 6000.0.38f1 (or compatible Unity 6 editor in the dashboard).
- Keep auto-build disabled during first validation; trigger a manual build and confirm checked-out commit + project subfolder in the log.
- The current root Capacitor workflow is left untouched.

Cloud build success does not prove playability; the APK must still be tested on the phone before adding more features.
