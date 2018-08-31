$('.navbar-nav').on('click', 'li', function () {
    $('.navbar-nav li.active').removeClass('active');
    $(this).addClass('active');
});



//print
function PrintTable(name) {
    var tableToPrint = document.getElementById(name);
    newWin = window.open("");
    newWin.document.write(tableToPrint.outerHTML);
    newWin.print();
    newWin.Close();

}



//student Select
function apper() {
    var x = document.getElementsByClassName("select");
    var y = document.getElementsByClassName("update");
    x[0].style.display = "none";
    x[1].style.display = "none";
    y[0].style.display = "table-row";
    y[1].style.display = "table-row";
    y[2].style.display = "table-row";
}
function diapper() {

    var x = document.getElementsByClassName("select");
    var y = document.getElementsByClassName("update");
    x[0].style.display = "table-row";
    x[1].style.display = "table-row";
    y[0].style.display = "none";
    y[1].style.display = "none";
    y[2].style.display = "none";
}