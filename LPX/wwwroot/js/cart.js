function ShowLogOff()
{
    var adminnav = document.getElementById("adminnav")
    adminnav.style.display = "block";
    var toolsnav = document.getElementById("toolsnav")
    toolsnav.style.display = "none"

}

var productnums = []
document.onload = ShowLogOff();
var orderarray = []
var amountarray = []
var LPXOrder = new Object();
var supplierarray = []




function test()
{
    alert("click")
    location.href = "/products/savecart"




   

}

function prepareorder()
{
    var formarea = document.getElementById("formarea")
    formarea.style.display = ""

    var saveData = $.ajax({
      type: 'POST',
      url: "/products/getsuppliers",
      dataType: "json",
      success: function(resultData) 
      {

        alert("Save Complete"); 
        console.log(resultData);
        var select = document.getElementById("select")
        for(var i = 0; i < resultData.length;i++)
        {
            var obj = resultData[i]
            var optiontag = document.createElement("option");
            optiontag.value = resultData[i].supplierEmail
            optiontag.innerHTML = resultData[i].supplierName
            select.appendChild(optiontag)

        }

      }
});
saveData.error(function() {alert("error");});


}




function CloseSellerPane()
{
    if(formarea.style.display == "")
    {
            formarea.style.display = "none"
    }
    else
    {
        
            formarea.style.display = "none"
    }

}




function randomString(length, chars) {
    var result = '';
    for (var i = length; i > 0; --i) result += chars[Math.floor(Math.random() * chars.length)];
    return result;

    }

// green submit button clicked
 function sendorder()
 {
                  



                     $('#ordertable').find('input[type="checkbox"]:checked').each(function (index) {
                       var Details = new Object()


                    var productname = document.getElementsByClassName("productname")[index].innerHTML
                    console.log(productname)

                    var brand = document.getElementsByClassName("brand")[index].innerHTML
                    var packsize = document.getElementsByClassName("packsize")[index].innerHTML

                    Details.ProductName = productname
                    Details.Brand = brand
                    Details.PackSize = packsize
                    orderarray.push(Details)

                     })
    
       
              




            







             



             
    $('input[type=number]').each(function(){
        amountarray.push($(this).val());
       

        });






 console.log(orderarray)


  console.log(amountarray)
  var orderdetails = new Object();
  orderdetails.data = orderarray
  orderdetails.amounts = amountarray.toString()

  LPXOrder.OrderNumber = randomString(10, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")
  LPXOrder.PlacedBy = "<%=currentuser%>"
  LPXOrder.OrderDetails = JSON.stringify(orderdetails)


   var select = document.getElementById("select")

   var selectedvalues = $('#select').val();
   supplierarray.push(selectedvalues)
       /* for (var i = 0; i < select.options.length; i++) {
           if(select.options[i].selected ==true){
                console.log(select.options[i].innerHTML);
                //console.log(select.option[i].value)
                supplierarray.push(select.options[i].innerHTML)
            }
            */

            console.log(supplierarray)

            LPXOrder.Suppliers = supplierarray.toString();

  console.log(LPXOrder)




  var saveData = $.ajax({
      type: 'POST',
      url: "/products/createorder",
      data: LPXOrder,
      dataType: "json",
      success: function(resultData) { alert("Save Complete"); console.log(resultData) }
});
saveData.error(function() { alert("Something went wrong"); });







              

    
      
 }






   














