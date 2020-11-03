window.jsErrors = [];
window.onerror = function (errorMessage, url, lineNumber) {
    window.jsErrors.push('Message: ' + errorMessage + ', url: ' + url + ', line:' + lineNumber);
    return false;
}

function getErrors() {
    return window.jsErrors;
}