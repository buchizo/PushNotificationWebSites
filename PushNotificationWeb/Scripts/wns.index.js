function selectorEscape(text) {
    return text.replace(/[!"#$%&'()*+,./:;<=>?@@[\]^`{|}~|#\s]/g, "\\$&");
}

function showTileImage(selection, tile) {
    var uri = selection.options[selection.selectedIndex].value;
    if (!uri) {
        uri = "/Content/images/emptyimage.png";
    }        
    $("#"+tile, selection.parentNode).attr("src", uri);
} 

function toggleAudioSelector() {
    $('#AudioSelector')[0].disabled = !$('#AudioEnabled')[0].checked;
}

function UpdateNotificationStatus(data, status, xhr, id) {
    $("#result_"+id).html("<div>" + data.Status + "</div><div>Device Status: " + data.DeviceConnectionStatus + "</div><div>Notification Status: " + data.NotificationStatus + "</div>");
    $("#sending_"+id).html();
    setTimeout("clearResultMessages('" + "#result_" + id + "')", 5000);
}

function clearNotificationsForm() {
    $('#notificationTemplate').empty();
    $('#notificationType').val(0);
    $('#notificationTemplateType').empty();
}

function clearResultMessages(id) {
    $(id).fadeOut('slow', function() { $(this).html('&nbsp;').show(); });
}

$(document).ready(function () {
    $('.createNotification').click(function() {
        var id = $(this).attr('id');
        var url = $("[id='url_" + id.replace('?', '_') + "']").text();

        $('#itemIdentifier').val(id);
        $('#itemUrl').val(url);

        clearNotificationsForm();
        
        $("#templateWindow").dialog({
            height: 500,
            width: 650,
            modal: true,
            title: 'Push Notification',
            buttons: [{
                text: 'Send',
                style: 'display:none;',
                click: function() {
                    $('form').submit();
                    $(this).dialog("close");
                }},
                { text: 'Cancel',  click: function() { $(this).dialog("close"); } }
               ]
        });
    });
    
    $('.notificationType').change(function () {
        var selectedValue = $(this).val();
        
        $('#notificationTemplate').empty();
        $('#notificationTemplateType').empty();
        $('.ui-dialog-buttonset button:first').hide();
        
        if (selectedValue <= 0)
            return;
        
        $.getJSON('/PushNotification/GetNotificationTypes', { notificationType: selectedValue }, function (notificationTypes) {
            var notificationSelect = $('.notificationTemplateType');
            notificationSelect.empty();
            //add the elements to the drop down
            notificationSelect.append($('<option/>').attr('value', '0').text('--- Select Value ---'));
            $.each(notificationTypes, function (index, type) {
                notificationSelect.append($('<option/>').attr('value', type.Id).text(type.Name));
            });
        });
    });
    
    $('.notificationTemplateType').change(function () {
        var notificationType = $(this).prev().val();
        var notificationTemplateType = $(this).val();

        $('#notificationTemplate').empty();
        $('.ui-dialog-buttonset button:first').hide();
        if (notificationTemplateType <= 0)
            return;
        
        $('#notificationTemplate').text('loading...');
        
        //find partition key and row key
        var ids = $('#itemIdentifier').val().split('?');
        var pk = selectorEscape(ids[0]);
        var rk = selectorEscape(ids[1]);

        //find previous select element value
        var url = $('#itemUrl').val();

        var data = {
            NotificationType: notificationType,
            NotificationTemplateType: notificationTemplateType,
            ApplicationId: pk,
            DeviceId: rk,
            ChannelUrl: jQuery.trim(url)
        };
                   
        $.ajax({
            type: 'POST',
            url: '/PushNotification/GetSendTemplate',
            data: data,
            dataType: 'html',
            success: function (result) {
                $('#notificationTemplate').html(result);
                $('.ui-dialog-buttonset button:first').show();
            }
        });
    });
});