var events = [];
$(".events").each(function () {
    var patientUsername = $(".patientUsername", this).text().trim();
    var staffUsername = $(".staffUsername", this).text().trim();
    var appointmentTime = $(".appointmentTime", this).text().trim();
    var appointmentDate = $(".appointmentDate", this).text().trim();
    var event = {
        "patientUserName": patientUsername,
        "staffUserName": staffUsername,
        "appointmentDate": appointmentDate,
        "appointmentTime": appointmentTime
    };
    events.push(event);
});
$("#calendar").fullCalendar({
    locale: 'au',
    events: events,
    dayClick: function (date, allDay, jsEvent, view) {
        var d = new Date(date);
        var m = moment(d).format("YYYY-MM-DD");
        m = encodeURIComponent(m);
        var uri = "/Events/Create?date=" + m;
        $(location).attr('href', uri);
    }
});