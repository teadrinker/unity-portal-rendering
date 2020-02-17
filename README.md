# unity-portal-rendering
Portal rendering using projection mapping

This was designed for an experiment where a large screen and VR tracking technology can be used to simulate an environment for film/video production. A low budget alternative to [something like this](https://www.youtube.com/watch?v=bErPsq5kPzE).


Note that:
 * You should be able to copy the PortalRendering folder into any Unity project (2018 or later)
 * You can drop the prefab PortalRendering/PortalRendering into a Unity scene
 * Portal is always square in the scene, but screen export camera can be of any aspect ratio.
 * The camera "ExportToScreen" by default outputs to Display 3 (you probably want to set it up with multiple screens so you can run unity on one and film another at the same time)
 * Material: RealtimeUVProjection has a slight green tint for debugging purposes, remove this before filming
 * You can increase the rendering resolution (render texture named PortalRT) 
