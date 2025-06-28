/* https://stackoverflow.com/questions/13800644/ssrs-javascript-report-load-event */
Sys.Application.add_load(function () {
    loaded = true;
    var onLoaded = function () {
        $(parent.document).find('.throbber').hide();
        loaded = true;
    };
    var viewerReference = $find("ReportViewer");
    check_load = function () {
        var loading = viewerReference.get_isLoading();
        if (loading)
            loaded = false;
        else if (!loaded)
            onLoaded();
        setTimeout(check_load, 100);    /* recall itself every 100 miliseconds */
    };
    check_load();
});
