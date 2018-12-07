'use strict'; $(document).ready(function () {
    $("#basic-forms").steps({ headerTag: "h3", bodyTag: "fieldset", transitionEffect: "slideLeft", autoFocus: true });
    $("#verticle-wizard").steps({ headerTag: "h3", bodyTag: "fieldset", transitionEffect: "slide", stepsOrientation: "vertical", autoFocus: true }); $("#design-wizard").steps({ headerTag: "h3", bodyTag: "fieldset", transitionEffect: "slideLeft", autoFocus: true });
    var form = $("#example-advanced-form").show();
    form.steps({
        headerTag: "h3", bodyTag: "fieldset", transitionEffect: "slideLeft",
        onStepChanging: function (event, currentIndex, newIndex) {
    if (currentIndex > newIndex) { return true; }
if(newIndex===3&&Number($("#age-2").val())<18){return false;}
if(currentIndex<newIndex){form.find(".body:eq("+newIndex+") label.error").remove();form.find(".body:eq("+newIndex+") .error").removeClass("error");}
            form.validate().settings.ignore = ":disabled,:hidden"; return form.valid();
        },
        onStepChanged: function (event, currentIndex, priorIndex) {
            if (currentIndex === 2 && Number($("#age-2").val()) >= 18) { form.steps("next"); }
            if (currentIndex === 2 && priorIndex === 3) { form.steps("previous"); }
        },
        onFinishing: function (event, currentIndex) { form.validate().settings.ignore = ":disabled"; return form.valid(); },
        onFinished: function (event, currentIndex) {
            $.ajax({
                type: "POST",
                url: '<%= Url.Action("Admin", "Register") %>',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    alert("Hello: " + response.Name + " .\nCurrent Date and Time: " + response.DateTime);
                },
                failure: function (response) {
                    alert("data Sent");
                },
                error: function (response) {
                    alert("not working");
                }
            });
            alert("Submitted!"); 
        }
    }).validate({ errorPlacement: function errorPlacement(error, element) { element.before(error); }, rules: { confirm: { equalTo: "#password-2" } } });
});