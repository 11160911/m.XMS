﻿/*
(function ($, sr) {
    // debouncing function from John Hann
    // http://unscriptable.com/index.php/2009/03/20/debouncing-javascript-methods/
    var debounce = function (func, threshold, execAsap) {
        var timeout;

        return function debounced() {
            var obj = this, args = arguments;
            function delayed() {
                if (!execAsap)
                    func.apply(obj, args);
                timeout = null;
            }

            if (timeout)
                clearTimeout(timeout);
            else if (execAsap)
                func.apply(obj, args);

            timeout = setTimeout(delayed, threshold || 100);
        };
    };

    // smartresize 
    jQuery.fn[sr] = function (fn) { return fn ? this.bind('resize', debounce(fn)) : this.trigger(sr); };

})(jQuery, 'smartresize');
*/
(function ($) {
    var dtFun;
    var LogOut = false;
    
    var Initdoc = function () {
        
        UU = sessionStorage.getItem('token');
        PostToWebApi({ url: "api/GetMenuInit", success: AfterInit, complete: GetHeads });
        
        //$('#imglogo').css('cursor', 'pointer');

    };

    var AfterInit = function (data) {
        
        if (ReturnMsg(data, 0) != "GetMenuInitOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            
            $("a[href='Login']").attr("href", "Login" + sessionStorage.getItem('isamcomp'));

            $('.right_col').mousedown(function (e) {
                Sidebar_Close();
            })

            var dtEmployeeSV = data.getElementsByTagName('dtEmployee');
            $('#navbarDropdown').text(GetNodeValue(dtEmployeeSV[0], 'ST_SName') + ' - ' + GetNodeValue(dtEmployeeSV[0], 'Man_Name'));
            dtFun = data.getElementsByTagName('dtAllFunction');
            SetMenu();
            init_sidebar();
            return;
        }

        if (window.PageDashboard == undefined)
            $.getScript('dashboard.js',
                function () {
                    $('#FunctionDesc').text('');
                    PageDashboard($("#app"));
                    $('#MenuLinkHome,#imglogo').click(function () {
                        $('article').remove();
                        $('#FunctionDesc').text('');
                        PageDashboard($("#app"));
                    });
                }
            );
        else {
            $('#FunctionDesc').text('');
            PageDashboard($("#app"));
        }


    };

    let LogOutX = function () {
        
        var cData = {
        }
        PostToWebApi({ url: "api/UpdateLogOutX", data: cData, success: AfterUpdateLogOutX });

    };

    let AfterUpdateLogOutX = function (data) {
        
        if (ReturnMsg(data, 0) != "UpdateLogOutXOK") {
            DyAlert(ReturnMsg(data, 1));
        }
    };

    var GetHeads = function (jqxhr) {
       
        var headers = {};
        jqxhr.getAllResponseHeaders()
            .trim()
            .split(/[\r\n]+/)
            .map(value => value.split(/: /))
            .forEach(keyValue => {
                headers[keyValue[0].trim()] = keyValue[1].trim();
            });
        UU = headers.authorization;
    };

    var Timerstart = function () {
        var timer = $('#Timer');
        var number = 10;
        timer.text(number);
        LogOutTimer = setInterval(function () {
            number--;
            if (number <= 0) {
                number = 0;
                window.location.href = "Login" + sessionStorage.getItem('isamcomp');
            }
            timer.text(number + 0);
           
        }, 1000);
    }

    var lastMenu = null;

    var menu = $('#sidebar-menu .side-menu');

    var SetMenu = function () {
        var strCat = "";

        for (var i = 0; i < dtFun.length; i++) {
            var icat = GetNodeValue(dtFun[i], "Category");
            var licat;
            var liul;
            /*
                  <li><a><i class="fa fa-home"></i> Home <span class="fa fa-chevron-down"></span></a>
                    <ul class="nav child_menu">
                      <li><a href="index.html">Dashboard</a></li>
                      <li><a href="index2.html">Dashboard2</a></li>
                      <li><a href="index3.html">Dashboard3</a></li>
                    </ul>
                  </li>


*/
            if (icat != strCat) {
                strCat = icat;
                //var strLi = '<li><a><i class="fa ' + GetNodeValue(dtFun[i], 'icon') + '"></i> ' + GetNodeValue(dtFun[i], 'CategoryC') + ' <span class="fa fa-chevron-down"></span></a>';
                //strLi += '<ul class="nav child_menu">';
                //strLi += "</ul></li>";
                var strLi = '<li><a><i class="fa ' + GetNodeValue(dtFun[i], 'icon') + '"></i> ' + GetNodeValue(dtFun[i], 'CategoryC') + ' </a>';
                strLi += "</li>";
                licat = $(strLi);
                var apg = licat.find('a');

                apg.prop('Page', GetNodeValue(dtFun[i], "Page"));
                apg.prop('ItemCode', GetNodeValue(dtFun[i], "ItemCode"));
                apg.prop('Description', GetNodeValue(dtFun[i], "Description"));
                apg.prop('SECU_PERMIT', GetNodeValue(dtFun[i], "SECU_PERMIT"));
                apg.prop('href', '#' + GetNodeValue(dtFun[i], "Page"));

                menu.append(licat);
            }
            //strLi = '<li><a href="#">' + GetNodeValue(dtFun[i], 'Description') + '</a></li>';
            var liFunc = $(strLi);
            //licat.find('.child_menu').append(liFunc);
            var apg = liFunc.find('a');

            //apg.prop('Page', GetNodeValue(dtFun[i], "Page"));
            //apg.prop('ItemCode', GetNodeValue(dtFun[i], "ItemCode"));
            //apg.prop('Description', GetNodeValue(dtFun[i], "Description"));
            //apg.prop('SECU_PERMIT', GetNodeValue(dtFun[i], "SECU_PERMIT"));
            //apg.prop('href', '#' + GetNodeValue(dtFun[i], "Page"));
            apg.click(function () { click_menu(this); });
          

        }
    };

    var click_menu = function (menuitem) {
        
        $('#FunctionDesc').text($(menuitem).prop('Description'));
        //$('#TimerLbl').hide();
        //$('#Timer').hide();
        //TimerReset(sessionStorage.getItem('isamcomp'),"");

        Timerset(sessionStorage.getItem('isamcomp'));
        $MENU_TOGGLE.click();
        OpenPage(menuitem);


    }

    var clickCategory = function (catitem) {
        if ($(catitem).hasClass('open')) {
            closeCategory(catitem);
        }
        else {
            openCategory(catitem);
        }
    }

    var openCategory = function (catitem) {
        
        $(catitem).addClass('open');
        var itemA = $(catitem).prop('CatItemA');
        $(itemA).attr('aria-expanded', 'true');
        var itemUL = $(catitem).prop('CatItemUL');
        $(itemUL).addClass('in');
        $(itemUL).css('height', 'auto');
    }

    var closeCategory = function (catitem) {
        $(catitem).removeClass('open');
        var itemA = $(catitem).prop('CatItemA');
        $(itemA).attr('aria-expanded', 'false');
        var itemUL = $(catitem).prop('CatItemUL');
        $(itemUL).removeClass('in');
        $(itemUL).css('height', '0px');
        //$(itemUL).slideToggle("slow", function () {
        //    $(itemUL).css('height', '0px');
        //});
    }

    var OpenPage = function (menuitem) {
        
        menu.find('.active').removeClass('active');
        lastMenu = menuitem;
        $('article').remove();

        var pg = $(menuitem).prop('Page');
        var auth = $(menuitem).prop('SECU_PERMIT');

        //2021-04-29 
        var Description = $(menuitem).prop('Description');
        $('.right_col .page-title .title_left h3').text(Description);

        auth = "Y";
        if (pg == "SysChangePWD")
            auth = "Y";
        if (auth == "Y") {

            //2021-04-29 Debug用，按F12後，在主控台內會顯示內容
            console.log(pg);
            //alert(pg);

            if (pg == "msSA101") {
                if (window.PagemsSA101 == undefined)
                    $.getScript('SystemSetup/msSA101.js',
                        function () {
                            PagemsSA101($(".workarea"));
                        }
                    );
                else {
                    PagemsSA101($(".workarea"));
                }
            }

            else if (pg == "msDM101") {
                if (window.PagemsDM101 == undefined) {
                    $.getScript('SystemSetup/msDM101.js',
                        function () {
                            PagemsDM101($(".workarea"));
                        }
                    );
                }
                    
                else {
                    PagemsDM101($(".workarea"));
                }
            }

            else if (pg == "ISAMDelData") {
                if (window.PageISAMDelData == undefined)
                    $.getScript('SystemSetup/ISAMDelData.js',
                        function () {
                            PageISAMDelData($(".workarea"));
                        }
                    );
                else {
                    PageISAMDelData($(".workarea"));
                }
            }

            else if (pg == "ISAMToFTP") {
                if (window.PageISAMToFTP == undefined)
                    $.getScript('SystemSetup/ISAMToFTP.js',
                        function () {
                            PageISAMToFTP($(".workarea"));
                        }
                    );
                else {
                    PageISAMToFTP($(".workarea"));
                }
            }

            else if (pg == "ISAMWHSET") {
                if (window.PageISAMWHSET == undefined)
                    $.getScript('SystemSetup/ISAMWhSet.js',
                        function () {
                            PageISAMWHSET($(".workarea"));
                        }
                    );
                else {
                    PageISAMWHSET($(".workarea"));
                }
            }

            else if (pg == "ISAMQFTPREC") {
                if (window.PageISAMQFTPREC == undefined)
                    $.getScript('SystemSetup/ISAMQFTPREC.js',
                        function () {
                            PageISAMQFTPREC($(".workarea"));
                        }
                    );
                else {
                    PageISAMQFTPREC($(".workarea"));
                }
            }
            else if (pg == "ISAM02") {
                if (window.PageISAM02 == undefined)
                    $.getScript('SystemSetup/ISAM02.js',
                        function () {
                            PageISAM02($(".workarea"));
                        }
                    );
                else {
                    PageISAM02($(".workarea"));
                }
            }
            else if (pg == "ISAM03") {
                if (window.PageISAM03 == undefined)
                    $.getScript('SystemSetup/ISAM03.js',
                        function () {
                            PageISAM03($(".workarea"));
                        }
                    );
                else {
                    PageISAM03($(".workarea"));
                }
            }

            //2022-07-27 larry
            else if (pg == "ISAMQPLU") {
                //alert("ISAMQPLU");
                if (window.PageISAMQPLU == undefined)

                    $.getScript('SystemSetup/ISAMQPLU.js',
                        function () {
                            PageISAMQPLU($(".workarea"));
                        }
                    );
                else {
                    PageISAMQPLU($(".workarea"));
                }
            }

            else if (pg == "SysChangePWD") {
                $('#OLD_PWD').val("");
                $('#NEW_PWD').val("");
                $('#CFN_PWD').val("");
                $('#modal_changePWD').modal('show');
            }
            else {
                var under_construction = '<article class="content dashboard-page">';
                under_construction += '<section class="col-md-10"><div class="card">';
                under_construction += '<img src="images/Website-Under-Construction.jpg" />';
                under_construction += '</div></section>';
                under_construction += '</article>';
                $(under_construction).insertAfter('.app-title');
            }
        }
        else {
            var under_construction = '<article class="content dashboard-page">';
            under_construction += '<section class="section"><div class="block">';
            under_construction += '<img src="images/unauthorized.jpg" />';
            under_construction += '</div></section>';
            under_construction += '</article>';
            $(under_construction).insertAfter('.mobile-menu-handle');
        }

    }

    //var beforeBtnClick = function () {
    //    TimerReset(sessionStorage.getItem('isamcomp'), "");
    //    TimerReset(sessionStorage.getItem('isamcomp'));
    //};

     //Sidebar
    var init_sidebar = function () {
        
        // TODO: This is some kind of easy fix, maybe we can improve this
        var setContentHeight = function () {
            // reset height
            $RIGHT_COL.css('min-height', $(window).height());

            var bodyHeight = $BODY.outerHeight(),
                footerHeight = $BODY.hasClass('footer_fixed') ? -10 : $FOOTER.height(),
                leftColHeight = $LEFT_COL.eq(1).height() + $SIDEBAR_FOOTER.height(),
                contentHeight = bodyHeight < leftColHeight ? leftColHeight : bodyHeight;

            // normalize content
            contentHeight -= $NAV_MENU.height() + footerHeight;

            $RIGHT_COL.css('min-height', contentHeight);
        };

        var openUpMenu = function () {
            $SIDEBAR_MENU.find('li').removeClass('active active-sm');
            $SIDEBAR_MENU.find('li ul').slideUp();
        }

        $SIDEBAR_MENU.find('a').on('click', function () {
            //ChkLogOut(sessionStorage.getItem('isamcomp'))
            click_menu(this);
        }
        );

        $MENU_TOGGLE.on('click', function () {
            //ChkLogOut(sessionStorage.getItem('isamcomp'))
            if ($BODY.hasClass('nav-md')) {
                $SIDEBAR_MENU.find('li.active ul').hide();
                $SIDEBAR_MENU.find('li.active').addClass('active-sm').removeClass('active');
            } else {
                $SIDEBAR_MENU.find('li.active-sm ul').show();
                $SIDEBAR_MENU.find('li.active-sm').addClass('active').removeClass('active-sm');
            }

            $BODY.toggleClass('nav-md nav-sm');

            setContentHeight();

            $('.dataTable').each(function () { $(this).dataTable().fnDraw(); });
        });

        this.Sidebar_Close = function () {
            if ($BODY.hasClass('nav-md')) {
            } else {
                $BODY.toggleClass('nav-md nav-sm');
            }
        }

        //// check active menu
        //$SIDEBAR_MENU.find('a[href="' + CURRENT_URL + '"]').parent('li').addClass('current-page');

        $SIDEBAR_MENU.find('a').filter(function () {
            return this.href == CURRENT_URL;
        }).parent('li').addClass('current-page').parents('ul').slideDown(function () {
            setContentHeight();
        }).parent().addClass('active');

        Timerset(sessionStorage.getItem('isamcomp'));
        // recompute content when resizing
        $(window).smartresize(function () {
            setContentHeight();
        });

        setContentHeight();

        // fixed sidebar
        if ($.fn.mCustomScrollbar) {
            $('.menu_fixed').mCustomScrollbar({
                autoHideScrollbar: false,
                theme: 'minimal',
                mouseWheel: { preventDefault: true }
            });
        }
        
    }

    var init_autosize = function () {

        if (typeof $.fn.autosize !== 'undefined') {

            autosize($('.resizable_textarea'));

        }

    };

    var CURRENT_URL = window.location.href.split('#')[0].split('?')[0],
        $BODY = $('body'),
        $MENU_TOGGLE = $('#menu_toggle'),
        $SIDEBAR_MENU = $('#sidebar-menu'),
        $SIDEBAR_FOOTER = $('.sidebar-footer'),
        $LEFT_COL = $('.left_col'),
        $RIGHT_COL = $('.right_col'),
        $NAV_MENU = $('.nav_menu'),
        $FOOTER = $('footer');

    Initdoc();


})(jQuery);

