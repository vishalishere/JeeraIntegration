$(document).ready(function () {

    $(".ModelWrapper .close").click(function (e) {
        e.preventDefault();
        var dialog = $(this).parent().parent().parent();
        $("#ExistingStory", dialog).hide();
        $("#newStory", dialog).hide();
        hideCustomDialog(dialog);
    });

    $(".ModelWrapper .bottominfo .close").click(function (e) {
        e.preventDefault();
        var dialog = $(this).parent().parent().parent().parent().parent();
        hideCustomDialog(dialog);
    });

    $(document).keyup(function (e) {
        e.preventDefault();
        if (e.keyCode === 27)
        {
            $("#ExistingStory").hide();
            $("#newStory").hide();
            hideCustomDialog($(".ModelWrapper"));
        }
    });
});

function hideCustomDialog(dialog) {
    $('body').removeClass('stopScroll');
    $(dialog).hide()
               .find(".model").hide()
               .find(".OverlayMask").hide();
    $(".model #Loading", dialog).hide();
    var dialogEvents = $(dialog).data('events');
    if (dialogEvents != undefined && dialogEvents.isHidden != undefined) {
        $(dialog).trigger('isHidden');
        $(dialog).unbind('isHidden');
    }
}
function showCustomDialog(dialog) {
    PositionDialog(dialog);
    $(window).resize(function () {
        PositionDialog(dialog);
    });
    var top = 10;
    if (dialog.hasClass("fixHeight")) {
        top = 150;
    }

    $(dialog).show().find(".OverlayMask").show().parent().find(".model").show()
                            .css({ top: 50 })
                            .animate({ top: top }, { duration: 100, queue: false });
    $('body').addClass('stopScroll');
}
function PositionDialog(dialog) {
    var modContainer = $(".model", dialog);
    var top = 0;

    $(modContainer).css({

        left: ($(window).width() - $(modContainer).outerWidth()) / 2
    });
}