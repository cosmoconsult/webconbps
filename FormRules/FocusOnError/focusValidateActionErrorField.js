window.dkrueger = window.dkrueger || {}
window.dkrueger.debugModeActive = new URLSearchParams(document.location.search).get("debug") == 1;
window.dkrueger.focusOnError = window.dkrueger.focusOnError  || {}
window.dkrueger.focusOnError.validationActionErrorExp = new RegExp("Error WFD_([^:]*)")
window.dkrueger.focusOnError.focusOnValidationActionError = function () {
    if (window.dkrueger.debugModeActive) {
        debugger;
    } 
    var fieldName;
    var errorControl = $(".form-errors-panel__errors-container__error__message")
    if (errorControl != null && errorControl.length > 0) {
        var message = errorControl[0].textContent
        var result = window.dkrueger.focusOnError.validationActionErrorExp.exec(message)
        if (result != null && result.length == 2) {
            fieldName = result[1];
        }
    }
    if (fieldName != null){
        window.webcon.businessRules.setFocus(fieldName)
    }
}

window.dkrueger.focusOnError.fetchMock = window.dkrueger.focusOnError.fetchMock  || window.fetch;
window.fetch = function () {
    console.log(arguments);
    return new Promise((resolve, reject) => {
        window.dkrueger.focusOnError.fetchMock.apply(this, arguments)
            .then((response) => {
                if (response.url.indexOf("/goToNextStep") > -1 || response.url.indexOf("/SaveElement") > -1 ) {
                    setTimeout(window.dkrueger.focusOnError.focusOnValidationActionError , 500);
                }
                resolve(response);
            })
            .catch((error) => {
                reject(response);
            })
    });
}