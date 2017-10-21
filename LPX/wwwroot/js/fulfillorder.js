function Lookup()
{
var LPXOrder = {}
LPXOrder.OrderNumber = document.getElementById("ordernumber").value
LPXOrder.Suppliers = document.getElementById("companyemail").value
localStorage.setItem("rsupplier",document.getElementById("companyemail").value )
localStorage.setItem("ordernumber", document.getElementById("ordernumber").value )
  var saveData = $.ajax({
      type: 'POST',
      url: "/suppliers/GetOrderDeatils",
      data: LPXOrder,
      dataType: "json",
      success: function(resultData) { alert("Save Complete"); console.log(resultData); window.location = "/suppliers/ShowDetails" }
});
saveData.error(function() { alert("Something went wrong"); });


}
