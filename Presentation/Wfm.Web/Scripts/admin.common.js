/**
 *  Check Null String
*/
function isNullOrWhiteSpace(str) {
    return str === null || str.match(/^ *$/) !== null;
}

/**
 *  Pretty Phone Number
*/
function prettyPhone(n) {
    if (isNullOrWhiteSpace(n))
        return "";
    var d = String(n).replace(/\D/g, "");
    d = (({ 7: 1, 10: 1 }[d.length]) ? d.match(/\d{1,3}/g) : 0) || [];
    switch (d.length) {
        case 3: return d[0] + "-" + d[1] + d[2];
        //case 4: return "(" + d[0] + ") " + d[1] + "-" + d[2] + d[3];
        case 4: return d[0] + "-" + d[1] + "-" + d[2] + d[3];
        default: return n; //alert(n + " is not a valid phone number");
    }
    return n;
}

/**
 *  Pretty Social Insurance Number
*/
function prettySocialInsuranceNumber(n) {
    if (isNullOrWhiteSpace(n))
        return "";
    var d = String(n).replace(/\D/g, "");
    d = (({ 9: 1, 10: 1 }[d.length]) ? d.match(/\d{1,3}/g) : 0) || [];
    switch (d.length) {
        case 3: return d[0] + "-" + d[1] + "-" + d[2];
        case 4: return "(" + d[0] + ") " + d[1] + "-" + d[2] + d[3];
        default: return n; //alert(n + " is not a valid phone number");
    }
    return n;
}

/**
 *  Convert UTC date time to local date time
*/
//function utcToLocalDateTime1(n) {
//    if (isNullOrWhiteSpace(n))
//        return "";
//    var date = new Date(n + ' UTC');
//    return date.toString();
//}
//function ConvertUTCTimeToLocalTime(UTCDateString) {
//    var convertdLocalTime = new Date(UTCDateString);
//    var hourOffset = convertdLocalTime.getTimezoneOffset() / 60;
//    convertdLocalTime.setHours(convertdLocalTime.getHours() + hourOffset);
//    return convertdLocalTime;
//}


/**
 *  Format phone numbers
*/
function formatPhone(phonenum) {
    var regexObj = /^(?:\+?1[-. ]?)?(?:\(?([0-9]{3})\)?[-. ]?)?([0-9]{3})[-. ]?([0-9]{4})$/;
    if (regexObj.test(phonenum)) {
        var parts = phonenum.match(regexObj);
        var phone = "";
        if (parts[1]) { phone += "+1 (" + parts[1] + ") "; }
        phone += parts[2] + "-" + parts[3];
        return phone;
    }
    else {
        //invalid phone number
        return phonenum;
    }
}

/**
 * Format postal code
*/
function formatPostalcode(pcode) {
    var regexObj = /^\s*([a-ceghj-npr-tvxy]\d[a-ceghj-npr-tv-z])(\s)?(\d[a-ceghj-npr-tv-z]\d)\s*$/i
    if (regexObj.test(pcode)) {
        var parts = pcode.match(regexObj);
        var pc = parts[1] + " " + parts[3];
        return pc.toUpperCase();
    }
    else {
        return pcode;
    }
}

var currentModelId = "";
function closeModalWindow() {
    $('#' + currentModelId).data('tWindow').close();
}
function openModalWindow(modalId) {
    currentModelId = modalId;
    $('#' + modalId).data('tWindow').center().open();
}

function setLocation(url) {
    window.location.href = url;
}

function getE(name) {
    //Obsolete since wfmCommerce 2.60. But still here for backwards compatibility (in case of some plugin developers used it in their plugins or customized solutions)
    if (document.getElementById)
        var elem = document.getElementById(name);
    else if (document.all)
        var elem = document.all[name];
    else if (document.layers)
        var elem = document.layers[name];
    return elem;
}

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function showThrobber(message) {
    $('.throbber-header').html(message);
    window.setTimeout(function () {
        $(".throbber").show();
    }, 1000);
}

$(document).ready(function () {
    //Setup the ajax indicator
    $('#ajaxBusy').css({
        display: "none",
        margin: "0px",
        paddingLeft: "0px",
        paddingRight: "0px",
        paddingTop: "0px",
        paddingBottom: "0px",
        position: "absolute",
        right: "3px",
        top: "3px",
        width: "auto"
    });

    //for batch edit grid
    $('.k-grid-add').html('Add');
    $('.k-grid-save-changes').html('Save');
    $('.k-grid-cancel-changes').html('Cancel');
});

function gridDelete(callback) {
    $.when(kendo.ui.ExtYesNoDialog.show({
        title: "Confirm",
        message: "Are you sure you want to delete?",
        icon: "k-ext-warning"
    })).done(function (response) {
        //console.log(kendo.format("{0} button clicked", response.button));
        if (response.button == "Yes") {
            callback();
        }
    });
    return false;
}
// Ajax activity indicator bound to ajax start/stop document events
$(document).ajaxStart(function () {
    $('#ajaxBusy').show();
}).ajaxStop(function () {
    $('#ajaxBusy').hide();
});

function confirmAction(text, callback) {
    $.when(kendo.ui.ExtOkCancelDialog.show({
        title: "Confirm",
        message: text,
        icon: "k-ext-warning"
    })).done(function (response) {
        //console.log(kendo.format("{0} button clicked", response.button));
        if (response.button == "OK") {
            callback();
        }
    });
    return false;
}

$(document).ready(function () {
    // disable submit button on form submission
    // Avoiding Duplicate form submission
    // Find ALL <form> tags on your page
    $(document).on('invalid-form.validate', 'form', function () {
        var button = $(this).find('input[type="submit"]');
        setTimeout(function () {
            button.removeAttr('disabled');
        }, 1);
    });
    $(document).on('submit', 'form', function () {
        var button = $(this).find('input[type="submit"]');
        setTimeout(function () {
            button.attr('disabled', 'disabled');
        }, 0);
    });


});

//Display Employee basic information pop-up
function candidateProfile(candidateGuid,url) {   
    var win = $("#candidate-profile").data("kendoWindow");
    if (!win) {
        $("#candidate-profile").kendoWindow({
            title: "Employee Profile",
            modal: true,
            resizable: true,
            width: 1200,
            height: 500,
            maxHeight: 1000,
            minHeight: 500,
            minWidth: 1000,
            maxWidth: 1500,
            close: function () {
                setTimeout(function () {
                    $('#candidate-profile').kendoWindow('close');
                }, 200);
            },
            refresh: function (e) {
                win.center().open();
            },
        });
        win = $("#candidate-profile").data("kendoWindow");
    }
    else {
        win.open();
    }
    win.refresh({
        url: url,
        data: { guid: candidateGuid }
    });
    win.center();
    return false;
}

function error_handler(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
    }
}

function refreshGrid(gridControl, pageNumber) {
    var grid = $(gridControl).data('kendoGrid');
    pageNumber = pageNumber || 1;
    grid.dataSource.page(pageNumber);
    grid.dataSource.read();
}

$(document).ready(function() {
    kendo.ui.Grid.fn.options.columnMenuInit = function(e){
        var menu = e.container.find(".k-menu").data("kendoMenu");
        menu.bind('activate', function(e){
            if(e.item.is(':last-child')){
                // if an element in the submenu is focused first, the issue is not observed
                e.item.find('span.k-dropdown.k-header').first().focus();
                // e.item.find('input').first().focus();
            }
        });
    }
})