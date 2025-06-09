mergeInto(LibraryManager.library, {
  PlayOverlayVideo: function (urlPtr) {
    var url = UTF8ToString(urlPtr);

    var video = document.createElement('video');
    video.id = "unity-video-overlay";
    video.src = url;
    video.autoplay = true;
    video.controls = false; // Disable controls
    video.playsInline = true;
    video.muted = false;
    video.style.position = 'absolute';
    video.style.top = '0';
    video.style.left = '0';
    video.style.width = '100%';
    video.style.height = '100%';
    video.style.zIndex = '1000';
    video.style.backgroundColor = 'black';
    video.style.objectFit = 'cover';

    document.body.appendChild(video);

    video.onended = function () {
      document.body.removeChild(video);
    };
  }
});
