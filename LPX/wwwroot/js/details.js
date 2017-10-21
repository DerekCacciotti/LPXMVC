function ShowLogOff()
{
    var adminnav = document.getElementById("adminnav")
    adminnav.style.display = "block";
    var toolsnav = document.getElementById("toolsnav")
    toolsnav.style.display = "none"
}


document.onload = ShowLogOff();

function BacktoSearch()
{
    location.href = "/products/search";
}


function addtoCart()
{
    var LPXOrder = new Object();
    var productid = document.getElementById("productnum")
    var productname = document.getElementById("itemname")
    var brand = document.getElementById("brandlabel")
    var packsize = document.getElementById("packsize")

    LPXOrder.ID = productid.innerHTML
    LPXOrder.ProductName = productname.innerHTML
    LPXOrder.Brand = brand.innerHTML
    LPXOrder.PackSize = packsize.innerHTML

    console.log(LPXOrder)

    var json = JSON.stringify(LPXOrder)
    alert(json)


var saveData = $.ajax({
      type: 'GET',
      url: "/products/cart",
      data: LPXOrder,
      dataType: "json",
      success: function(resultData) { alert("Save Complete"); console.log(resultData); location.href = "/products/viewcart" }
});
saveData.error(function() { alert("Something went wrong"); });


    }

   


