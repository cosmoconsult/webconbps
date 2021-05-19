window.dkrueger = window.dkrueger || {}
window.dkrueger.debugModeActive = new URLSearchParams(document.location.search).get("debug") == 1;
window.dkrueger.focusOnError = window.dkrueger.focusOnError  || {}
window.dkrueger.focusOnError.requiredAndEmptyExp = new RegExp("(.*_.*) isRequiredAndEmpty")
window.dkrueger.focusOnError.lastExecutionTime = null;
window.dkrueger.focusOnError.setFocusOnMissingRequiredField = function() {
    var errors = $(".form-errors-panel__errors-container__error");
    for (var i = 0; i < errors.length; i++){
        var currentItem = errors[i];
        var keyValue = currentItem.attributes["data-key"].value;
        var likelyErrorFieldName ;
        if (keyValue.endsWith(":")){
            likelyErrorFieldName =  keyValue.substring(0,keyValue.length-1);
        }
        if (keyValue.indexOf(", row")> -1){
            likelyErrorFieldName =  keyValue.substring(0,keyValue.indexOf(", row"));
        }
        var control = initialModel.Controls.filter(
            function(value){
                if (value.displayName ==likelyErrorFieldName){
                    return value
                }
            });
            
        
        if (control != null && control.length == 1)
        {
            console.log(`Identified field name '${control[0].fieldName}' for display name '${likelyErrorFieldName}'.`)
            window.webcon.businessRules.setFocus(control[0].fieldName);
        } 
    }
}

window.dkrueger.focusOnError.consoleLogMock = window.dkrueger.focusOnError.consoleLogMock || console.log;
console.log = function(...args){
    if (window.dkrueger.debugModeActive) {
        debugger;
    }    
    window.dkrueger.focusOnError.consoleLogMock(...args);
    if (arguments.length == 1) {
        var message = arguments[0]
        var result =  window.dkrueger.focusOnError.requiredAndEmptyExp.exec(message)    
        if (result != null && result.length == 2) {
            // Assumption there may be multiple missing required fields
            // We want to set the focus on the first one
            // It's unlikely that the user will take less than 250 ms for 
            // closing the error dialog and define a value.           
            if (window.dkrueger.focusOnError.lastExecutionTime+250 < Date.now()){
                // If the field name is a details column, we need retrieve the parent field name
                // Since there could be multiple item list we must retrieve the correct one from the error dialog
                if (result[1].indexOf("DET_")== 0){
                    setTimeout(window.dkrueger.focusOnError.setFocusOnMissingRequiredField, 250);                    
                }
                else {
                    window.webcon.businessRules.setFocus(result[1])
                    window.dkrueger.focusOnError.lastExecutionTime = Date.now()
                }
            }
        }
    }
}