function ShowLogOff()
{
    var adminnav = document.getElementById("adminnav")
    adminnav.style.display = "block";
    var toolsnav = document.getElementById("toolsnav")
    toolsnav.style.display = "none"
}


document.onload = ShowLogOff();


function test()
{
      $('#tablediv').on('click', 'table tr', function() {
    console.log("Clicked", this);
    var selectedrow = this;
    console.log(selectedrow.cells[0]);
    //alert(this)
    var html = selectedrow.cells[0].innerHTML
    var productnumber = parseInt(html)
    alert(html)
    location.href = "/products/details?id=" + productnumber;


});
}