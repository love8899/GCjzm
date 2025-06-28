// https://docs.telerik.com/aspnet-mvc/troubleshoot/troubleshooting-validation#globalized-dates-and-numbers-are-not-recognized-as-valid-when-using-jquery-validation
// jQuery validation does not support globalized dates and numbers
jQuery.extend(jQuery.validator.methods, {
    date: function (value, element) {
        return this.optional(element) || kendo.parseDate(value) !== null;
    },
    number: function (value, element) {
        return this.optional(element) || kendo.parseFloat(value) !== null;
    }
});

jQuery(document).ready(function ($) {
    // setup inputs
    setupInputs();
});


//
// Common
//

function doNothing() { return false; }

function scroll2top() {
    //$('html, body').animate({ scrollTop: 0 }, 'fast');
    $('html,body').scrollTop(0);
}

function showThrobberNow(message) {
    $('.throbber-header').html(message);
    $(".throbber").show();
}

function hideThrobberNow() {
    $('.throbber-header').empty();
    $(".throbber").hide();
}

function setupInputs(selector, popup) {
    // add * for required inputs
    addRequireHint(selector, undefined, undefined, undefined, undefined, popup);
    addRequireHint(selector, 'textarea', undefined, undefined, undefined, popup);
}

// kendo input combo
//var kendoInputCombo = ['combobox', 'numerictextbox', 'datepicker', 'timepicker', 'datetimepicker', 'dropdownlist'];
var kendoInputCombo = ['ComboBox', 'NumericTextBox', 'DatePicker', 'TimePicker', 'DateTimePicker', 'DropDownList'];

function addRequireHint(selector, tagName, exReadonly, exDisabled, exHidden, popup) {
    selector = selector || '';
    tagName = tagName || 'input';
    if (exReadonly === undefined) exReadonly = false;
    if (exDisabled === undefined) exDisabled = true;
    if (exHidden === undefined) exHidden = true;
    selector += ' ' + tagName + '[data-val-required]';
    if (exHidden) selector += ':not([type=hidden])';
    if (exDisabled) selector += ':not([disabled])';
    if (exReadonly) selector += ':not([readonly])';
    $(selector).each(function () {
        var input = $(this);
        var dataRole = input.data('role');
        if (dataRole) {
            var idx = getIndex(kendoInputCombo, dataRole, false);
            if (idx > -1)
                input = input.closest('.k-widget');
        }
        addRequireMark(input, popup);
    });
}

function addRequireMark(input, popup) {
    if (!popup) {   // apply Bootstrap style
        input.wrap("<div class='input-group input-group-required'></div>");
        input.after("<div class='input-group-btn'><span class=\"required\">*</span></div>");
    } else
        input.after("<span class='required'>*</span>");
}


function enableValidation(formName) {
    $.validator.unobtrusive.parse('#'+ formName);
}

function addClass2FormLines(formName, classes, rowCls) {
    rowCls = rowCls || 'form-group';
    classes = classes || ['col-sm-4', 'col-sm-8'];
    var lines = $('div.' + rowCls);
    if (formName)
        lines = $('#' + formName).find('div.' + rowCls);
    lines.each(function (i) {
        $(this).children().each(function (j) {
            if (j < 2) {    // just first 2 for label & field
                var ele = $(this);
                var cls = classes[j];
                if (!ele.hasClass(cls)) {
                    if (rowCls === 'form-group')
                        ele.removeClass().addClass(cls);
                    else    // add wrapper div
                        ele.wrap('<div class="' + cls + '"></div>');
                }
            }
        });
    });
}

function resetSearchForm(formName) {
    var form = $('form#' + formName);
    if (form) {
        form.validate().resetForm();
        //form[0].reset();    // not need if button having type='reset'
    }
}

function clearValidationError(name) {
    $('input[name="' + name + '"]').removeClass('input-validation-error');
    $('span[data-valmsg-for="' + name + '"]').empty();
    $('span[data-valmsg-for="' + name + '"]').removeClass('field-validation-error').addClass('field-validation-valid');
}

function showValidationError(name, msg) {
    $('input[name="' + name + '"]').addClass('input-validation-error');
    //$('span[data-valmsg-for="' + name + '"]').text(msg);
    $('span[data-valmsg-for="' + name + '"]').html('<span>' + msg + '</span>');
    $('span[data-valmsg-for="' + name + '"]').removeClass('field-validation-valid').addClass('field-validation-error');
    //$('span[data-valmsg-for="' + name + '"]').css("display", "inline");
}

function setReadonly(selector) {
    var ele = $(selector);
    ele.attr('readonly', 'readonly');
    ele.on("keydown", function (e) { e.preventDefault(); });
}

function enableEle(ele, enable) {
    if (enable)
        ele.removeAttr('disabled');
    else
        ele.attr('disabled', 'disabled');
}

function compare(a, b) {
    return JSON.stringify(a) === JSON.stringify(b);
}

function cloneObject(left, right) {
    //for (const p in left) {
    Object.keys(left).forEach(function (p, i) {
        right[p] = left[p];
        //}
    });
    //for (const p in right) {
    Object.keys(right).forEach(function (p, i) {
        if (!(p in left))
            delete right[p];
        //}
    });
}

function isHtml(str) { return /<\/?[a-z][\s\S]*>/i.test(str); }

function distinct(value, index, self) { return self.indexOf(value) === index; }

function factorialize(num) {
    // exclude 1 and itself
    var result = [];
    if (num < 4)
        return result;
    for (var i = num - 1; i > 1; i--) {
        if (num % i === 0)
            result.push(i);
    }
    return result.filter(distinct);
}

function getIndex(arr, v, caseSensative) {
    if (caseSensative === undefined) caseSensative = true;
    if (caseSensative)
        return arr.indexOf(v);
    else {
        var lowerCaseArr = [];
        arr.forEach(function (x) {
            lowerCaseArr.push(x.toLowerCase());
        });
        return lowerCaseArr.indexOf(v.toLowerCase());
    }
}

function getNext(arr, v) {
    var next = null;
    var idx = arr.indexOf(v);
    if (idx > -1 && idx < arr.length - 1)
        next = arr[idx + 1];
    return next;
}


//
// Notifications
//

function popupNotification(message, messagetype, modal) {
    if (message) {
        //types: success, error, warning
        var container;
        if (messagetype === 'success') {
            //success
            container = $('#dialog-notifications-success');
        }
        else if (messagetype === 'error') {
            //error
            container = $('#dialog-notifications-error');
        }
        else if (messagetype === 'warning') {
            //warning
            container = $('#dialog-notifications-warning');
        }
        else {
            //other
            container = $('#dialog-notifications-success');
        }

        //we do not encode displayed message
        var htmlcode = '';
        if (typeof message === 'string') {
            htmlcode = '<p>' + message + '</p>';
        } else {
            for (var i = 0; i < message.length; i++) {
                htmlcode = htmlcode + '<p>' + message[i] + '</p>';
            }
        }
        container.html(htmlcode);

        var isModal = (modal ? true : false);
        container.dialog({
            modal: isModal,
            width: 350,
            position: { my: "top", at: "top", of: ".content-wrapper" }
        });
    }
}

var adminNotificationTimeout;
function displayAdminNotification(message, messagetype, timeout, barId) {
    messagetype = messagetype || 'success';
    timeout = timeout || 0;
    barId = barId || '.notifications';

    clearTimeout(adminNotificationTimeout);
    var cssclass = 'alert';
    //remove previous notifications
    $('.' + cssclass).remove();
    //types: success, error, warning
    if (messagetype === 'success') {
        cssclass += ' alert-success';
    }
    else if (messagetype === 'error') {
        cssclass += ' alert-danger';
    }
    else if (messagetype === 'warning') {
        cssclass += ' alert-warning';
    }
    cssclass += ' alert-dismissable';
    //add new notifications
    var divStartTag = "<div class='" + cssclass + "'>";
    var btnClose = "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>";
    var divEndTag = "</div>";
    var htmlcode = divStartTag + btnClose;
    if (typeof message === 'string') {
        htmlcode += message;
    } else {
        for (var i = 0; i < message.length; i++) {
            var newLine = i < message.length - 1 ? '<br />' : '';
            htmlcode += message[i] + newLine;
        }
    }
    htmlcode += divEndTag;
    $(barId).prepend(htmlcode)
        .fadeIn('slow')
        .mouseenter(function () {
            console.log('clear timer as mouse entering', $(this));
            clearTimeout(adminNotificationTimeout);
        });

    $('.alert .close').unbind('click').click(function () {
        $('.alert').fadeOut('slow');
    });
    //timeout (if set)
    if (timeout > 0) {
        adminNotificationTimeout = setTimeout(function () {
            $('.alert').fadeOut('slow');
        }, timeout);
    }
}


//
// Request
//

function ajaxReq(url, type, data, onSuccess, onError) {
    $.ajax({
        url: url,
        type: type,
        data: data,
        cache: false,
        async: false,
        success: function (result) {
            if (result.Succeed === undefined || result.Succeed) {   // allow result.Succeed undefined
                if (typeof onSuccess === "function")
                    onSuccess(result);
                else
                    popupNotification("Success !", "success");
            }
            else {
                if (typeof onError === "function")
                    onError(result);
                else
                    popupNotification("Error:" + result.Error, "error");
            }
        },
        //error: function (e) { console.log(e); }
        error: function (data) { alert(data.responseText); }
    });
}

function ajaxGet(url, data, onSuccess, onError) {
    return ajaxReq(url, 'GET', data, onSuccess, onError);
}

function justGet(url, data, msg, onDone) {
    ajaxGet(url, data,
        function (result) { // on sucsess
            console.log('result:', result);
            if (msg)
                displayAdminNotification(msg, "success");
            if (typeof onDone === 'function')
                onDone(result);
        },
        function (result) { // on error
            popupNotification(result.Error, "error");
        }
    );
}

function ajaxPost(url, data, onSuccess, onError) {
    return ajaxReq(url, 'POST', data, onSuccess, onError);
}

function justPost(url, data, msg, onDone) {
    ajaxPost(url, data,
        function (result) { // on sucsess
            if (msg)
                displayAdminNotification(msg, "success");
            if (typeof onDone === 'function')
                onDone(result);
        },
        function (result) { // on error
            popupNotification(result.Error, "error");
        }
    );
}

function getJsonData(url, data) {
    var result = null;
    $.ajax({
        url: url,
        dataType: 'json',
        async: false,
        data: data,
        success: function (json) {
            result = json;
        }
    });
}

function loadFromUrl(id, url, data) {
    var result = null;
    $.ajax({
        url: url,
        dataType: 'html',
        async: false,
        data: data,
        success: function (html) {
            $(id).html(html);
        },
        error: function (e) {
            $(id).html("<p style='text-align:center;color:red;margin-top:10px'><b>Failed to load!</b></p>");
        }
    });
}

function setCollapseContent(name, url, data) {
    var id = '#' + name;
    $(id).on('show.bs.collapse', function () { loadFromUrl(id, url, data); });
    $(id).on('hidden.bs.collapse', function () { $(id).empty(); });
}


//
// Popup
//

function closeDialog(name) {
    $('#' + name + '.ui-dialog-content').dialog('close');
}

function popupContentFromUrl(id, url, title, bindEvents, modal, width, height) {
    var isModal = modal ? true : false;
    var targetWidth = width ? width : 480;
    var targetHeight = height ? height : 450;
    var maxHeight = $(window).height() - 20;

    $('<div id="' + id + '"></div>')
        .load(url, function () {
            // setup inputs
            setupInputs('#' + id, true);
            // apply auto focus
            var input = $(this).find('input[autofocus]');
            input.focus(function () {
                var input = $(this);
                // https://docs.telerik.com/kendo-ui/controls/editors/numerictextbox/how-to/select-all-on-focus
                setTimeout(function () { input[0].setSelectionRange(0, 9999); }, 0);    //setTimeout(function () { input.select(); },0);
            });
            if (input.data('role') === 'numerictextbox') {
                var kendoInput = input.data('kendoNumericTextBox');
                kendoInput.focus();
            }
            else
                input.focus();
            // bind dialog events(save, cancel, ...)
            if (typeof bindEvents === 'function')
                bindEvents($(this));
        })
        .dialog({
            modal: isModal,
            //position: { my: "center", at: "center" },
            position: { my: "top", at: "top", of: ".content-wrapper" },
            width: targetWidth,
            height: targetHeight,
            maxHeight: maxHeight,
            title: title,
            closeText: false,
            close: function (event, ui) {
                $(this).dialog('destroy').remove();
            }
        });
}

function bindDialogEvent(dialog, formName, url, onSuccess, onError, btnSave, btnCancel) {
    btnSave = btnSave || ".btn-save";
    btnCancel = btnCancel || ".btn-cancel";
    var form = $('#' + formName);
    form.find(btnCancel).click(function () {
        dialog.dialog('close');
    });
    form.find(btnSave).click(function () {
        var ready = typeof preValidate !== 'function' || preValidate();
        if (ready && form.valid()) {
            ajaxPost(url, form.serialize(),
                function (result) {
                    dialog.dialog('close');
                    if (onSuccess && typeof onSuccess === "function")
                        onSuccess(result);
                },
                onError
            );
        }
    });
}

function popupPost(dialogName, getUrl, title, width, formName, postUrl, msg, onDone) {
    popupContentFromUrl(dialogName, getUrl, title, function (dialog) {
        bindDialogEvent(dialog, formName, postUrl,
            function (result) { // on suceess
                displayAdminNotification(msg);
                if (typeof onDone === 'function')
                    onDone(result);
            },
            function (result) { // on error
                popupNotification("Error: " + result.Error, "error");
            }
        );
    }, true, width);
    return false;
}

function popupConfirm(title, text) {
    title = title || 'Confirm';
    text = text || '';
    //var url = '/yes-or-no?' + (new URLSearchParams({ text: text })).toString(); // not working in IE
    var url = '/yes-or-no?text=' + encodeURIComponent(text);
    return new $.Deferred(function (deferred) {
        popupContentFromUrl('yesorno', url, title, function (dialog) {
            var dialogId = dialog.attr('id');
            $('#' + dialogId + ' input[type=button]').click(function (e) {
                deferred.resolve({ button: this.value });
                closeDialog(dialogId);
            });
        }, true, 360);
    });
}

function yesOrNo(callback, title, text) {
    $.when(popupConfirm(title, text)).done(function (response) {
        if (response.button === "Yes" && typeof callback === 'function')
            callback();
    });
}

function toDelete(callback) { yesOrNo(callback, 'Delete', 'Yes to delete, or No to cancel.'); }

function toProceed(callback) { yesOrNo(callback, 'Confirm'); }

function openKendoWin(name, title, url, bindEvents, modal, width, height) {
    var isModal = modal ? true : false;
    var targetWidth = width ? width : 480;
    var targetHeight = height ? height : 450;
    var maxHeight = $(window).height() - 20;
    $(document.body).append('<div id="' + name + '"></div>');
    $('#' + name).kendoWindow({
        title: title,
        modal: isModal,
        resizable: false,
        width: targetWidth,
        height: targetHeight,
        maxHeight: maxHeight,
        content: url,
        close: function () { setTimeout(function () { $('#' + name).kendoWindow('destroy'); }, 200); },
        refresh: function (e) {
            // setup inputs
            setupInputs('#' + name, true);
            // apply auto focus
            var input = $(this).find('input[autofocus]');
            input.focus(function () {
                var input = $(this);
                setTimeout(function () { input.select(); });
            });
            if (input.data('role') === 'numerictextbox') {
                var kendoInput = input.data('kendoNumericTextBox');
                kendoInput.focus();
            }
            else
                input.focus();
            // bind dialog events(save, cancel, ...)
            if (typeof bindEvents === 'function')
                bindEvents($(this));
        }
    }).data('kendoWindow').center();
}

function closeKendoWin(name) {
    var win = $('#' + name).data("kendoWindow");
    if (win) win.close();
}

function newWindow(url, target, w, h) {
    var x = screen.width / 2 - w / 2 + window.screenX;
    var y = screen.height / 2 - h / 2;
    return window.open(url, target, 'height=' + h + ',width=' + w + ',left=' + x + ',top=' + y + ',resizable=no,scrollbars=yes');
}


//
// Date & Time
//

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

function getDateTime(date, time) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(),
        time.getHours(), time.getMinutes(), time.getSeconds(), time.getMilliseconds());
}


//
// Candidate profile, for clients
//

// popup
function candidateProfile(candidateGuid, url) {
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
            }
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

// new window
function getCandidateInfo(candidateGuid) {
    var guid = candidateGuid;
    if (typeof guid === 'object') { // event
        guid = getGridDataItemByEvent(guid).CandidateGuid;
    }
    var url = "/Client/Common/_CandidateBasicInfo?guid=" + guid + "&viewName=CandidateInfoWin";
    //javascript: OpenWindow(url, 800, 600, true);
    newWindow(url, undefined, 800, 600);
    return false;
}


//
// Kendo Grid: Settings
//

function getScreenSize() {
    return { width: document.documentElement.clientWidth, height: document.documentElement.clientHeight };
}

function getFitPagerBtnCnt(maxCnt) {
    var btnCnt = 1;
    // for simplicity, only support factors of maxCnt (<= 10)
    if (maxCnt > 10)
        maxCnt = 10;
    var factors = factorialize(maxCnt);
    var scrSize = getScreenSize();
    //console.log('scr size:', scrSize, maxCnt);
    if (scrSize.width >= 992)
        btnCnt = maxCnt;
    else if (scrSize.width >= 768 && factors.length >= 1)
        btnCnt = factors[0];
    else if (scrSize.width >= 576 && factors.length >= 2)
        btnCnt = factors[1];
    return btnCnt;
}

function toggleGridPager(grid) {
    var maxCnt = grid.getOptions().pageable.buttonCount;
    var dataSrc = grid.dataSource;
    var total = Math.ceil(dataSrc.total() / dataSrc.pageSize());
    if (total > 1) {
        var btnCnt = getFitPagerBtnCnt(maxCnt);
        if (btnCnt <= maxCnt) {
            var curr = dataSrc.page() - 1;
            var avialableNo = Math.floor(curr / maxCnt);
            var avialableCurr = curr % maxCnt;
            var prevMore = avialableNo > 0 ? 1 : 0;
            var visibleNo = Math.floor(avialableCurr / btnCnt);
            $(grid.element[0]).find('ul.k-pager-numbers li').each(function (i) {
                var $this = $(this);
                if (i >= prevMore && i < prevMore + maxCnt) {   // other items not relevant
                    if (i < prevMore + visibleNo * btnCnt)
                        $this.hide();
                    else if (i >= prevMore + (visibleNo + 1) * btnCnt)
                        $this.hide();
                    else if ($this.is(':hidden'))
                        $this.show();
                }
            });
        }
    }
}

function getFirstGridName() {
    var gridName = '';
    var anyGrids = $('.k-grid');
    if (anyGrids.length)
        gridName = anyGrids[0].id;
    return gridName;
}

var $window = $(window);
var lastWindowWidth = $window.width();
function initSearchAndGrid(id, gridName, checkedIds, scroll2topOnPageChg) {
    collapseSearchBox(id);
    if (!gridName)
        gridName = getFirstGridName();
    if (gridName) {
        var grid = $('#' + gridName).data('kendoGrid');
        window.addEventListener("resize", function () {
            var windowWidth = $window.width();
            if (lastWindowWidth !== windowWidth) {
                lastWindowWidth = windowWidth;
                toggleGridPager(grid);
            }
        });
        if (scroll2topOnPageChg === undefined || scroll2topOnPageChg)
            grid.pager.bind('change', scroll2top);
        if (checkedIds)
            setGridCheckBoxHandlers(gridName, checkedIds);
    }
}

function setGridResponsive(grid) {
    hideGridColumnsIfZeroWidth(grid);
    setTimeout(function () { toggleGridPager(grid); }, 0);
}

function reloadGrid(grid, p, onReloaded) {
    if (grid) {
        if (p && grid.dataSource.page() !== p)
            grid.dataSource.page(p);
        grid.dataSource.read();
        if (onReloaded && typeof onReloaded === "function")
            onReloaded();
    }
}

function reloadGridByName(gridName, p, onReloaded) {
    gridName = gridName || getFirstGridName();
    if (gridName)
        reloadGrid($('#' + gridName).data('kendoGrid'), p, onReloaded);
}

function refreshGridByName(gridName) {
    gridName = gridName || getFirstGridName();
    if (gridName)
        $('#' + gridName).data('kendoGrid').refresh();
}


//
// Kendo Grid: Search
//

function collapseSearchBox(id) {
    id = id || "#search-box";
    $(id).click();
}

function reloadByDataRange(from, to, fromName, toName, searchBtnName, formName) {
    fromName = fromName || 'sf_From';
    toName = toName || 'sf_To';
    searchBtnName = searchBtnName || "btn-search";
    formName = formName || 'search-form';
    resetSearchForm(formName);
    $("#" + fromName).data("kendoDatePicker").value(from);
    var toPicker = $("#" + toName).data("kendoDatePicker");
    toPicker.max(to); toPicker.value(to);
    $("#" + searchBtnName).click();
    updateDateRangeDisplay(from, to);
}

function updateDateRangeDisplay(from, to) {
    var fromDisplay = $('#DateRange_From');
    if (fromDisplay) fromDisplay.val(kendo.toString(from, 'MM/dd/yyyy'));
    var toDisplay = $('#DateRange_To');
    if (toDisplay) toDisplay.val(kendo.toString(to, 'MM/dd/yyyy'));
}

function getValidFields(fields, mapping) {
    var toMap = Object.keys(mapping).length;
    return fields.reduce(function (a, x) {
        if (x.value && x.value !== '0' && x.value !== 'false') { // exclude unchecked box
            var name = x.name;
            var netName = name.substring(3);        // trim prefix 'sf_', for mapping
            if (toMap && netName in mapping)
                name = 'sf_' + mapping[netName];    // facilitate processing later
            a.push({ name: name, value: x.value });
        }
        return a;
    }, []);
}

function getFilterArray(fields, fromName, toName, nonNumFields, dateField) {
    fromName = fromName || 'sf_From';
    toName = toName || 'sf_To';
    nonNumFields = nonNumFields || [];
    dateField = dateField || "JobStartDateTime";
    var filterArray = [];
    fields.map(function (x) {
        if (x.value) {
            var netName = x.name.substring(3);  // trim prefix 'sf_', for local search
            var filter = { field: netName, operator: "contains", value: x.value };
            if (isNaN(x.value)) {
                if (x.name === fromName)    // using prefixed name!
                    filter = { field: dateField, operator: "gte", value: new Date(x.value) };
                else if (x.name === toName) // using prefixed name!
                    filter = { field: dateField, operator: "lte", value: new Date(x.value) };
            } else if (netName.endsWith("Id") && nonNumFields.length && nonNumFields.indexOf(netName) === -1) {
                filter = { field: netName, operator: "eq", value: parseInt(x.value) };
            }
            filterArray.push(filter);
        }
    });
    return filterArray;
}

function anyChanges(left, right) {
    var result = [];
    //for (const p in right) {
    Object.keys(right).forEach(function (p, i) {
        if (p in left) {
            var rightValue = right[p];
            var leftValue = left[p];
            // for Date string compare
            var rightDate = Date.parse(right[p]);
            var leftDate = Date.parse(left[p]);
            if (!isNaN(rightDate) && !isNaN(leftDate)) {
                rightValue = rightDate;
                leftValue = leftDate;
            }
            if (rightValue > leftValue)
                result.push({ name: p, left: left[p], right: right[p], chg: "up" });
            if (rightValue < leftValue)
                result.push({ name: p, left: left[p], right: right[p], chg: "down" });
        }
        else
            result.push({ name: p, left: null, right: right[p], chg: "add" });
    //}
    });
    //for (const p in left) {
    Object.keys(left).forEach(function (p, i) {
        if (!(p in right))
            result.push({ name: p, left: left[p], right: null, chg: "remove" });
    //}
    });
    return result;
}

function applyRemoteSearch(grid, remoteSearch, fields, serverOperation, fromName, toName) {
    var toApply = false;
    var newSearch = fields.reduce(function (a, x) { a[x.name] = x.value; return a; }, {});
    //console.log('remote search:', remoteSearch);
    //console.log('new search:', newSearch);
    var changes = anyChanges(remoteSearch, newSearch);
    if (changes.length) {
        //console.log("remote changes:", changes);
        var remoteFrom = Date.parse(remoteSearch[fromName]);
        var remoteTo = Date.parse(remoteSearch[toName]);
        var from = Date.parse(newSearch[fromName]);
        var to = Date.parse(newSearch[toName]);
        var withinRemoteDateRange = from >= remoteFrom && to <= remoteTo;
        if (serverOperation || !withinRemoteDateRange) {
            remoteSearch[fromName] = newSearch[fromName];
            remoteSearch[toName] = newSearch[toName];
            toApply = true;
        }
        var otherChanges = changes.filter(function (x) { return !(x.name === fromName || x.name === toName); });
        if (otherChanges.length) {
            //var lastRemoteSearch = { ...remoteSearch };
            var lastRemoteSearch = {}; cloneObject(remoteSearch, lastRemoteSearch);
            otherChanges.map(function (x) {
                if (x.chg === "remove")
                    delete remoteSearch[x.name];
                else if (serverOperation || x.chg !== "add") {
                    remoteSearch[x.name] = x.right;
                }
            });
            toApply = toApply || anyChanges(lastRemoteSearch, remoteSearch).length;
        }
        //console.log('new remote search:', remoteSearch);
    }
    if (toApply) {
        reloadGrid(grid, 1);
    }
    //else
    //    console.log("No remote changes to apply");
}

function applyLocalSearch(grid, localSearch, fields, fromName, toName, nonNumFields, dateField) {
    var newSearch = fields.reduce(function (a, x) { a[x.name] = x.value; return a; }, {});
    //console.log('local search:', localSearch);
    //console.log('new search:', newSearch);
    var changes = anyChanges(localSearch, newSearch);
    if (changes.length) {
        cloneObject(newSearch, localSearch);    // clone i.s.o. re-assign it, to avoid lossing reference
        //console.log('new local search:', localSearch);
        var filters = getFilterArray(fields, fromName, toName, nonNumFields, dateField);
        //console.log('local filters:', filters);
        grid.dataSource.filter({
            logic: "and",
            filters: filters
        });
    }
    //else
    //    console.log("No local changes to apply");
}

function applySearch(gridName, remoteSearch, localSearch, form, nonNumFields, dateField, fromName, toName) {
    fromName = fromName || 'sf_From';
    toName = toName || 'sf_To';
    var from = new Date($("#" + fromName).val());
    var to = new Date($("#" + toName).val());
    if (from > to)
        popupNotification("'To' must not be less than 'From' !", "error");
    else {
        var grid = $("#" + gridName).data("kendoGrid");
        var serverOperation = grid.dataSource.options.serverFiltering;
        //console.log("Server Operation:", serverOperation);
        var fields = getValidFields(form.serializeArray(), mapping);
        //console.log("search fields:", fields);
        applyRemoteSearch(grid, remoteSearch, fields, serverOperation, fromName, toName);
        if (serverOperation === false)
            applyLocalSearch(grid, localSearch, fields, fromName, toName, nonNumFields, dateField);
    }
}

//
// Kendo Grid: Check Boxes
//

function changeRowCheckBoxes(gridName, checked, updateMaster) {
    if (updateMaster === undefined)
        updateMaster = false;
    $('#' + gridName).find('.checkboxGroups').attr('checked', checked).change();
    if (updateMaster)
        updateMasterCheckbox(gridName);
}

function updateMasterCheckbox(gridName) {
    var gridDiv = $('#' + gridName);
    var numChkBoxes = gridDiv.find('tbody input[type=checkbox]').length;
    var numChkBoxesChecked = gridDiv.find('tbody input[type=checkbox]:checked').length;
    gridDiv.find('thead .mastercheckbox').attr('checked', numChkBoxes === numChkBoxesChecked && numChkBoxes > 0);
    //console.log("page checked:", gridDiv.data("kendoGrid").dataSource.page(), numChkBoxes, numChkBoxesChecked);
    //console.log("Ids on updating master:", checkedIds);
}

function setGridCheckBoxHandlers(gridName, checkedIds) {
    var gridDiv = $('#' + gridName);

    gridDiv.find('.mastercheckbox').click(function () {
        changeRowCheckBoxes(gridName, $(this).is(':checked'));
    });

    gridDiv.find('tbody').on('change', 'input[type=checkbox]', function (e) {
        var chkBox = $(this);
        var checkStatus = chkBox.is(":checked");
        var checkedId = chkBox.val();
        var idx = checkedIds.indexOf(checkedId);
        if (checkStatus === true) {
            if (idx === -1)
                checkedIds.push(checkedId);
        }
        else if (idx > -1)
            checkedIds.splice(idx, 1);
        updateMasterCheckbox(gridName);
    });
}

function setGridCheckBoxStatus(grid, checkedIds) {
    //console.log('checked on bound:', checkedIds);
    grid.tbody.find('input[type=checkbox]').each(function () {
        $(this).attr('checked', checkedIds.indexOf($(this).val()) > -1);
    });
    updateMasterCheckbox(grid.element[0].id);
}

//
// Kendo Grid: Responsive Columns
//

function getGridColumnIndices(grid, fields) {
    var cols = [];
    if (fields === undefined || fields.length === 0) {
        cols.push(grid.columns.length);     // for command column, always the last column
    } else {
        for (i = 0; i < grid.columns.length; i++) {
            var idx = fields.indexOf(grid.columns[i].field);
            if (idx > -1)
                cols.push(i + 1);       // starting from 1
        }
    }
    return cols;
}

function getHiddenFieldIndices(grid) {
    var hiddenFieldIndices = [];
    grid.thead.find("th[style*='display:none']").each(function () {
        hiddenFieldIndices.push($(this).data("index") + 1);
    });
    return hiddenFieldIndices;
}

function getColumnIndexAdj(hiddenIndices, cols) {
    var result = [];
    for (i = 0; i < cols.length; i++) {
        var loc = 0;
        for (j = 0; j < hiddenIndices.length; j++) {
            if (cols[i] <= hiddenIndices[j])
                break;
            else
                loc++;
        }
        result.push(loc);
    }
    return result;
}

function setGridColumnClassByIndex(grid, hiddenIndices, cols, classes, header) {
    if (header === undefined)
        header = true;

    var Adj = getColumnIndexAdj(hiddenIndices, cols);
    //console.log('index adj:', hiddenIndices, cols, Adj);
    for (i = 0; i < cols.length; i++) {
        grid.tbody.find("tr:not('.k-grouping-row')").each(function () {
            $(this).find("td:not('.k-group-cell'):not('.k-hierarchy-cell')").eq(cols[i] - 1).addClass(classes);
        });
        if (header) {
            // hidden columns not in colgroup
            grid.table.find("colgroup col:not('.k-group-col'):not('.k-hierarchy-col')").eq(cols[i] - 1 - Adj[i]).addClass(classes);
            grid.thead.find("th:not('.k-group-cell'):not('.k-hierarchy-cell')").eq(cols[i] - 1).addClass(classes);
            grid.footer.find("td:not('.k-group-cell'):not('.k-hierarchy-cell')").eq(cols[i] - 1).addClass(classes);
        }
    }
}

function setGridColumnClass(grid, fields, classes, header) {
    var cols = getGridColumnIndices(grid, fields);
    //console.log('fields:', fields, cols);
    var hiddenFieldIndices = getHiddenFieldIndices(grid);
    setGridColumnClassByIndex(grid, hiddenFieldIndices, cols, classes, header);
}

// hide column (including header) by applying class
function hideGridCol(grid, idx, classes) {
    var newClasses = classes;
    ['xxs', 'xs', 'sm', 'md', 'lg'].forEach(function (x) {
        newClasses = newClasses.replace('col-' + x + '-0', 'hidden-' + x);
    });
    // header & footer
    grid.thead.find('th[data-index="' + idx + '"]').addClass(newClasses);
    grid.footer.find("td:not('.k-group-cell'):not('.k-hierarchy-cell')").eq(idx).addClass(newClasses);
    // row
    grid.tbody.find("tr:not('.k-grouping-row')").each(function () {
        $(this).find("td:not('.k-group-cell'):not('.k-hierarchy-cell')").eq(idx).removeClass(classes).addClass(newClasses);
    });
}

// hide columns (including header) if width = 0
function hideGridColumnsIfZeroWidth(grid) {
    //console.log('grid cols:', grid.columns);
    for (var i = 0; i < grid.columns.length; i++) {
        var attributes = grid.columns[i].attributes;
        if (attributes && attributes.class)
            hideGridCol(grid, i, attributes.class);
    }
}

// kendo media properties, working, not used though
var xs_visible = '(min-width: 576px)';
var sm_visible = '(min-width: 768px)';
var md_visible = '(min-width: 992px)';
var lg_visible = '(min-width: 1200px)';
function setResponsiveColumnsByVisibility(options, columnVisibilities) {
    if (columnVisibilities && columnVisibilities.length) {
        //for (const c of options.columns) {
        for (var i = 0; i < options.columns.length; i++) {
            var c = options.columns[i];
            var v = columnVisibilities.find(function (x) { return x.field === c.field; });
            if (c.title === 'Action')
                v = columnVisibilities.find(function (x) { return x.field === c.title; });
            if (v) {
                c.media = v.media;
            }
        }
    }
}


//
// Kendo Grid: Utilities
//

function getGridDataItemByEvent(e, grid) {
    var row = $(e.target).closest('tr');
    grid = grid || row.closest('.k-grid').data('kendoGrid');
    return grid.dataItem(row);
}

function getColumnByName(gridName, columnName) {
    var column = $.grep($(gridName).data('kendoGrid').columns, function (item, index) {
        return item.field === columnName;
    });
    return column[0];
}

function gridDelete(callback) {
    $.when(kendo.ui.ExtYesNoDialog.show({
        title: "Confirm",
        message: "Are you sure you want to delete?",
        icon: "k-ext-warning",
        resizable: false
    })).done(function (response) {
        if (response.button === "Yes") {
            callback();
        }
    });
    return false;
}

function confirmAction(text, callback) {
    $.when(kendo.ui.ExtOkCancelDialog.show({
        title: "Confirm",
        message: text,
        icon: "k-ext-warning"
    })).done(function (response) {
        if (response.button === "OK") {
            callback();
        }
    });
    return false;
}

function markEditableCells(grid, row, cls) {
    cls = cls || 'editable';
    for (var i = 0; i < grid.columns.length; i++) {
        if (grid.columns[i].editor)
            row.find('td').eq(i+1).addClass(cls);
    }
}


//
// Bootstrap Accordion Wizard
//

function initAccWizard(steps, onNext, onBack, beforeNext, beforeBack, addButtons) {
    if (addButtons === undefined)
        addButtons = true;  // critical if not form submission per step
    $('.acc-wizard').accwizard({
        addButtons: addButtons,
        beforeNext: beforeNext || function (e) { return _beforeNext(steps); },
        onNext: function (e) { return _onNext(steps, onNext); },
        beforeBack: beforeBack || function (e) { return _beforeBack(); },
        onBack: function (e) { return _onBack(steps, onBack); }
    });
    //var wizard = $('.acc-wizard').data('plugin_accwizard');
    //console.log('wizard:', wizard, wizard.option('addButtons'));

    $('.acc-wizard-todo > a').on('click', function (e) {
        if ($(this).is('[disabled]'))
            e.preventDefault();
        else
            disableRestIfBack(steps, $(this).attr('href'));
    });

    $('[data-toggle=collapse]').on('click', function (e) {
        var nextStep = $(this).attr('data-target');
        var sbLink = $('.acc-wizard-todo > a[href=' + nextStep + ']');
        sbLink[0].click();  // always navigate via sidebar, thus below return false
        return false;
    });
}

function _beforeNext(steps) {
    var isValid = false;
    //var visibleForm = form.find('.in[aria-hidden=false] input');
    var visibleForm = $('.in input');
    if (visibleForm.length > 0) {
        visibleForm.validate().settings.ignore = ':disabled';
        isValid = visibleForm.valid();
    }
    enableStep(getNext(steps, getCurrStep()), isValid);  // enable / disbale next step
    return isValid;
}

function _onNext(steps, onNext) {
    setTimeout(function () {    // step changing may not finished yet
        var step = getCurrStep();
        if (typeof onNext === 'function')
            onNext(steps, step);
    }, 0);
}

function _beforeBack() {
    enableStep(getCurrStep(), false);   // disable this step
}

function _onBack(steps, onBack) {
    setTimeout(function () {    // step changing may not finished yet
        var step = getCurrStep();
        if (typeof onBack === 'function')
            onBack(steps, step);
    }, 0);
}

function getCurrStep() {
    return $('.acc-wizard-active > a').attr('href').slice(1);
    //return $(location).attr('hash').slice(1);
}

function enableStep(step, enable) {
    enableEle($('.acc-wizard-todo > a[href=#' + step + ']'), enable);
    enableEle($('[data-toggle=collapse]' + '[data-target=#' + step + ']'), enable);
}

function disableRestIfBack(steps, nextStep) {
    var curr = steps.indexOf(getCurrStep());
    var next = steps.indexOf(nextStep.slice(1));
    if (next < curr) {  // if back, disable later steps
        for (var i = next + 1; i < steps.length; i++)
            enableStep(steps[i], false);
    }
}
