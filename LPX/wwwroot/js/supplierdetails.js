function ShowLogOff()
{
    var adminnav = document.getElementById("adminnav")
    adminnav.style.display = "block";
    var toolsnav = document.getElementById("toolsnav")
    toolsnav.style.display = "none"

}



document.onload = ShowLogOff();

function returntoLookup()
{
     location.href = "/suppliers"
}







function submitbacktocustomer()
{
var finalorders = [] // storage array
// temp object 
var finalfurfilledorders = {}; // post object
   var table = $("#order-table")
   table.find('tr').each(function(i, el) {
   var furfilledorder = new Object(); 
   var tds = $(this).find('td');
   var productname = tds.eq(0).text();
   console.log(productname);
   var brand = tds.eq(1).text();
   var amount = tds.eq(2).text();
   var packsize = tds.eq(3).text();
   var sellerprice = tds.eq(4).find('input').val();
   console.log(brand)
   console.log(amount)
   console.log(packsize)
   console.log(sellerprice)

   var sellerpricefloat = parseFloat(sellerprice)

   furfilledorder.ProductName = productname
   furfilledorder.Brand = brand
   furfilledorder.Amount = parseInt(amount)
   furfilledorder.PackSize = packsize
   furfilledorder.SellerPrice = sellerpricefloat.toFixed(2)

   console.log(furfilledorder)
   finalorders.push(furfilledorder)



  











   })

   console.log(finalorders)

  var cleandata = finalorders.slice(1)

  console.log(cleandata)

 



  finalfurfilledorders.OrderNumber = localStorage.getItem("ordernumber")
  finalfurfilledorders.OrderDate = ""
  finalfurfilledorders.Orderdata = JSON.stringify(cleandata)
  finalfurfilledorders.RespondingSupplier = localStorage.getItem("rsupplier")
  finalfurfilledorders.Suppliers = ""
  console.log(finalfurfilledorders)





  var saveData = $.ajax({
      type: 'POST',
      url: "/suppliers/CreateFulfilledOrder",
      data: finalfurfilledorders,
      dataType: "json",
      success: function(resultData) { alert("Save Complete"); console.log(resultData); }
});
saveData.error(function() { alert("Something went wrong"); });




 }

