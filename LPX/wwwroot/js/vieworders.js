function ShowLogOff()
{
    var adminnav = document.getElementById("adminnav")
    adminnav.style.display = "block";
    var toolsnav = document.getElementById("toolsnav")
    toolsnav.style.display = "none"

}

function Lookup()
{
    var LPXOrder = new Object();
   var ordernumber = document.getElementById("ordernumber").value

   LPXOrder.OrderNumber = ordernumber


     var saveData = $.ajax({
      type: 'POST',
      url: "/admin/RetreveOrder",
      data: LPXOrder,
      dataType: "json",
      success: function(resultData) { alert("Save Complete"); console.log(resultData); window.location.replace("/admin/orderdetails");}
});
saveData.error(function() { alert("Something went wrong"); });

 
}

