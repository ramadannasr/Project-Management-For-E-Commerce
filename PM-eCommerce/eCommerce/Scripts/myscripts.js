$(function () {

        /*start scroll up*/
        var scrollbutton = $("#scroll-up");
        $(window).scroll(function () {
            if ($(this).scrollTop() >= 250) {
                scrollbutton.show();
            } else {
                scrollbutton.hide();
            }
        });
        scrollbutton.click(function () {
            $("html,body").animate({
                scrollTop: 0
            }, 800);
        });

        /*end scroll up*/


    /*start scroll smoth*/
    $(document).on('click', '.smoth_down', function (event) {
        event.preventDefault();

        $('html, body').animate({
            scrollTop: $($.attr(this, 'href')).offset().top
        }, 1000);
    });
    /*end scroll smoth*/

    /*start popups*/

    /*start login popup*/

    // Get the modal
    var login_dialog = document.getElementById('login_dialog');

    // Get the button that opens the modal
    var login_btn = document.getElementById("login_btn");

    // Get the <span> element that closes the modal
    var login_close = document.getElementsByClassName("login_close")[0];

    // When the user clicks the button, open the modal
    login_btn.onclick = function () {
        login_dialog.style.display = "block";
    }

    // When the user clicks on <span> (x), close the modal
    login_close.onclick = function () {
        login_dialog.style.display = "none";
    }

    //// When the user clicks anywhere outside of the modal, close it
    //Window.onclick = function (event) {
    //    if (event.target == login_dialog) {
    //        login_dialog.style.display = "none";
    //    }
    //}
    /*end login popup*/

    //when project run the login popup appeare
    //window.onload = function load() {
    //    var flag = '@Session["test"]';
    //    if (flag != null)
    //        document.getElementById('reg_dialog').style.display = "block";
    //    //alert("my scripts");
    //    else
    //    document.getElementById('login_dialog').style.display = "block";
    //}

    
    /*start reg popup*/

    // Get the modal
    var reg_dialog = document.getElementById('reg_dialog');

    // Get the button that opens the modal
    var reg_btn = document.getElementById("reg_btn");

    // Get the <span> element that closes the modal
    var reg_close = document.getElementsByClassName("reg_close")[0];

    // When the user clicks the button, open the modal
    reg_btn.onclick = function () {
        reg_dialog.style.display = "block";
    }

    // When the user clicks on <span> (x), close the modal
    reg_close.onclick = function () {
        reg_dialog.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        var x = event.target;
        if (event.target == reg_dialog) {
            reg_dialog.style.display = "none";
        }
    }

    /*end reg popup*/

    /*-- when click esc button on keyboard the modal close --*/
    $(document).keyup(function (event) {
        if (event.which === 27) {
            $('#login_dialog').hide();
            $('#reg_dialog').hide();
        }
    });
    /*end*/

    /* end popups*/

});
