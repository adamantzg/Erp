$(function () {
    $('[id$="_ddlHour"]').each(function () {
        if ($(this).val() == 'closed') {
            $(this).next('select').hide();
            $(this).css('width', '80px');
        }
    });

    $('[id$="_ddlHour"]').change(function () {
        var minuteSelect = $(this).next('select');
        if ($(this).val() == 'closed') {
            minuteSelect.hide();
            $(this).css('width', '80px');
        }
        else {
            minuteSelect.show();
            $(this).css('width', '');
        }
        $(this).prev('input').val($(this).val() + ':' + minuteSelect.val());
    });

    $('[id$="_ddlMinute"]').change(function () {
        var hourSelect = $(this).prev('select');
        $(this).prevAll('input').val(hourSelect.val() + ':' + $(this).val());
    });
});