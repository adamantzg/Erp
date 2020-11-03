// http://stackoverflow.com/questions/1933969/sound-effects-in-javascript-html5
var camera = ((new Audio()).canPlayType("audio/ogg; codecs=vorbis") === "") ? 'https://s3-us-west-2.amazonaws.com/s.cdpn.io/9729/79190__nathan-lomeli__iphone-camera-click.wav' : 'https://s3-us-west-2.amazonaws.com/s.cdpn.io/9729/79190__nathan-lomeli__iphone-camera-click.ogg';
var snd = new Audio(camera); // buffers automatically when created
function snapshoot() {
    snd.play();
}