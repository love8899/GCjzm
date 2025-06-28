jQuery(document).ready(function ($) {

    this.tooltip = function () {
        /* CONFIG */
        var xOffset = 10;
        var yOffset = 20;
        // these 2 variable determine popup's distance from the cursor
        // you might want to adjust to get the right result
        /* END CONFIG */

        $("a.tooltip").hover(function(e){
            this.t = this.title;
            this.title = "";
            $("body").append("<p id='tooltip'>"+ this.t +"</p>");
            $("#tooltip")
                .css("top",(e.pageY - xOffset) + "px")
                .css("left",(e.pageX + yOffset) + "px")
                .fadeIn("fast");
        },

        function(){
            this.title = this.t;
            $("#tooltip").remove();
        });
        $("a.tooltip").mousemove(function(e){
            $("#tooltip")
                .css("top",(e.pageY - xOffset) + "px")
                .css("left",(e.pageX + yOffset) + "px");
        });
    };

});  //End Document

function GenerateCanonicalUrl(txtSrc)
{
    var output = txtSrc.replace(/[^a-zA-Z0-9]/g, ' ').replace(/\s+/g, "-").toLowerCase();
    /* remove first dash */
    if (output.charAt(0) == '-') output = output.substring(1);
    /* remove last dash */
    var last = output.length - 1;
    if (output.charAt(last) == '-') output = output.substring(0, last);
    return output;
}
// end of helper
