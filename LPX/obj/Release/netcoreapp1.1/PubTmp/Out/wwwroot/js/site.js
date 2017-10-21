// Write your Javascript code.

$("#signupbutton" ).click(function() {

 $.ajax({
                type: "POST",
                url: "/signup/register",
                data: param = "",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: successFunc,
                error: errorFunc
            });

            function successFunc(data, status) {     
                alert(data);
            }

            function errorFunc() {
                alert('error');
            }
        });

