jQuery(document).ready(function ($) {

    // append span for required inputs, for style purpose
    $('input[data-val-required]:not([type=hidden])').each(function () {
        $(this).after('<span class="required">*</span>');
    });
    // disbale Chrome autofill
    setTimeout(function () {
        $('input[type][autocomplete=off]').each(function () {
            $(this).attr('autocomplete', 'new-password');
        });
    }, 0);
    // restore masks of masked text boxes
    setTimeout(function () {
        $('input[data-role=maskedtextbox]:not([type])').change(function () {
            var mtb = $(this).data("kendoMaskedTextBox");
            mtb.setOptions(mtb.options);        // TODO: avoid duplicate fires in Chrome
        });
    }, 0);

});

function popupContentFromUrl(id, url, title, modal, width, height) {
    var isModal = modal ? true : false;
    var targetWidth = width ? width : 550;
    var targetHeight = height ? height : 550;
    var maxHeight = $(window).height() - 20;

    $('<div id="'+ id + '"></div>').load(url)
        .dialog({
            modal: isModal,
            //position: { my: "center", at: "center" },
            width: targetWidth,
            height: targetHeight,
            maxHeight: maxHeight,
            title: title,
            close: function (event, ui) {
                $(this).dialog('destroy').remove();
            }
        });
}

var notificationTimeout;
function displayNotification(message, messagetype, timeout, barId) {
    messagetype = messagetype || 'success';
    timeout = timeout || 0;
    barId = barId || '#notification-wrapper';

    clearTimeout(notificationTimeout);
    //types: success, error, warning
    var cssclass = 'success';
    if (messagetype === 'success') {
        cssclass = 'success';
    }
    else if (messagetype === 'error') {
        cssclass = 'error';
    }
    else if (messagetype === 'warning') {
        cssclass = 'warning';
    }
    //remove previous CSS classes and notifications
    $(barId)
        .removeClass('success')
        .removeClass('error')
        .removeClass('warning');
    $(barId + ' .content').remove();
    //add new notifications
    var htmlcode = '';
    if ((typeof message) === 'string') {
        htmlcode = '<p class="content">' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p class="content">' + message[i] + '</p>';
        }
    }
    $(barId).append(htmlcode)
        .addClass(cssclass)
        .fadeIn('slow')
        .mouseenter(function () {
            clearTimeout(notificationTimeout);
        });

    $(barId + ' .close').unbind('click').click(function () {
        $(barId).fadeOut('slow');
    });

    //timeout (if set)
    if (timeout > 0) {
        notificationTimeout = setTimeout(function () {
            $(barId).fadeOut('slow');
        }, timeout);
    }
}


// setup column classes for repsonsive

function getGridColumnIndices(grid, fields) {
    var cols = [];
    for (i = 0; i < grid.columns.length; i++) {
        var idx = fields.indexOf(grid.columns[i].field);
        if (idx > -1)
            cols.push(i + 1);
    }
    return cols;
}

function getGridGroupFieldIndices(grid) {
    var grpFields = [];
    var grpCells = grid.dataSource.options.group;
    //grpFields = grpCells.map(a => a.field); // not work for IE
    grpCells.each(function (item) {
        grpFields.push(item.field);
    });
    return getGridColumnIndices(grid, grpFields).sort();
}

function getHiddenFieldIndices(grid) {
    var hiddenFieldIndices = [];
    grid.thead.find("th[style*='display:none']").each(function () {
        hiddenFieldIndices.push($(this).data("index") + 1);
    });
    return hiddenFieldIndices;
}

function getColumnRelativeLoc(hiddenIndices, cols) {
    var result = [];
    for (i = 0; i < cols.length; i++) {
        var loc = 0;
        var adj = 0;
        var moreAdj = false;
        for (j = 0; j < hiddenIndices.length; j++) {
            if (cols[i] <= hiddenIndices[j]) {
                //if (cols[i] == hiddenIndices[j])
                //    moreAdj = true; // adj + 1 if the col is also a grouping col
                break;
            }
            else
                loc++;
        }
        result.push(loc + adj);
        if (moreAdj)
            adj++;
    }
    return result;
}

function setGridColumnClassByIndex(grid, hiddenIndices, cols, classes, header) {
    header = header || true;

    //var grpLevel = grpIndices.length;
    var relLocs = getColumnRelativeLoc(hiddenIndices, cols);
    //console.log('rel loc:', hiddenIndices, cols, relLocs);
    for (i = 0; i < cols.length; i++) {
        // nth-child regardless of class, therefore not working
        //grid.tbody.find("tr:not('.k-grouping-row') td:nth-child(" + (cols[i] + grpLevel) + ")").addClass(classes);
        grid.tbody.find("tr:not('.k-grouping-row')").each(function () {
            $(this).find("td:not('.k-group-cell')").eq(cols[i] - 1).addClass(classes);
        });
        if (header) {
            // hidden columns not in colgroup
            grid.table.find("colgroup col:not('.k-group-col')").eq(cols[i] - 1 - relLocs[i]).addClass(classes);
            grid.thead.find("th:not('.k-group-cell')").eq(cols[i] - 1).addClass(classes);
            grid.footer.find("td:not('.k-group-cell')").eq(cols[i] - 1).addClass(classes);
        }
    }
}

function setGridColumnClass(grid, fields, classes, header) {
    header = header || true;

    //console.log('fields:', fields);
    var cols = getGridColumnIndices(grid, fields);
    var hiddenFieldIndices = getHiddenFieldIndices(grid);
    setGridColumnClassByIndex(grid, hiddenFieldIndices, cols, classes, header);
}

// not used, to be removed
function skipGroupCells(grid) {
    grid.tbody.find("td[colspan]").each(function () {
        $(this).attr('colspan', 15);
    });
}



// Date & Time related

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

function getDateTime(date, time) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(),
        time.getHours(), time.getMinutes(), time.getSeconds(), time.getMilliseconds());
}
