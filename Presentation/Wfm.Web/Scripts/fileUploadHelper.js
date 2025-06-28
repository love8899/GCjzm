function validateFileSize(_file)
{
    if (_file.size > 15 * 1024 * 1024) {
        alert(_file.name + " is too big!");
        return false;
    }
    return true;
}

function onUpload(e) {
    var files = e.files;
    $.each(files, function () {
        if (!validateFileSize(this)) {
            e.preventDefault(); // This cancels the upload for the file
        }
    });
}

function onComplete() {
    $(".k-widget.k-upload").find("ul").remove();
    $(".k-upload-files.k-reset").find("li").remove();
}

function refreshPager() { $(".k-pager-refresh").trigger("click"); }

function onSuccess(e) { refreshPager(); }

function onError(e) {
    refreshPager();
    // Array with information about the uploaded files
    var files = e.files;
    if (e.operation == "upload") {
        if (e.XMLHttpRequest.responseText != "") {
            alert(e.XMLHttpRequest.responseText);
        }
        else {
            alert("Failed to upload " + files.length + " files");
        }
    }
}
