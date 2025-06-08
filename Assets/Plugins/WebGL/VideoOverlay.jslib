mergeInto(LibraryManager.library, {
  PlayOverlayVideo: function (urlPtr) {
    var url = UTF8ToString(urlPtr);

    // Check if one already exists
    if (document.getElementById("unity-video-overlay")) return;

    var video = document.createElement('video');
    video.id = "unity-video-overlay";
    video.src = url;
    video.autoplay = true;
    video.controls = true;
    video.playsInline = true;  // iOS support
    video.style.position = 'absolute';
    video.style.top = '0';
    video.style.left = '0';
    video.style.width = '100%';
    video.style.height = '100%';
    video.style.zIndex = '1000';
    video.style.backgroundColor = 'black';

    document.body.appendChild(video);

    video.onended = function () {
      document.body.removeChild(video);
    };
  }
});
