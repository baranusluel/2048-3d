function resized() {
  var canvas = document.getElementById("#canvas");
  var gameContainer = document.getElementById("gameContainer");
  if (canvas.width != gameContainer.clientWidth || canvas.height != gameContainer.clientHeight) {
    canvas.width = gameContainer.clientWidth;
    canvas.height = gameContainer.clientHeight;
  }
}



// Following adapted from: https://ocias.com/blog/unity-webgl-custom-progress-bar/

function init() {
    
}

function UnityProgress (dom, progress) {
  this.SetProgress = function (progress) { 
    if (progress == 1)
      document.getElementById("loadingBox").style.display = "none";
  }
  this.Update = function(progress) {
    var length = 200 * Math.min(progress, 1);
    bar = document.getElementById("progressBar")
    bar.style.width = length + "px";
  }
  this.SetProgress(progress);
  this.Update(progress);
}