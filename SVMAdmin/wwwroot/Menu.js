/*
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

    var Initdoc = function () {
        UU = sessionStorage.getItem('token');
        PostToWebApi({ url: "api/GetMenuInit", success: AfterInit, complete: GetHeads });
        
        //$('#imglogo').css('cursor', 'pointer');

    };

    var AfterInit = function (data) {
        if (ReturnMsg(data, 0) != "GetMenuInitOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            var dtEmployeeSV = data.getElementsByTagName('dtEmployeeSV');
            $('#navbarDropdown').text(GetNodeValue(dtEmployeeSV[0], 'ChineseName') + '-' + GetNodeValue(dtEmployeeSV[0], 'Man_Name'));
            dtFun = data.getElementsByTagName('dtAllFunction');
            SetMenu();
            init_sidebar();

            //init_autosize();
            //$.getScript('js/custom.js', function (data, textStatus, jqxhr) { });

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
                var strLi = '<li><a><i class="fa ' + GetNodeValue(dtFun[i], 'icon') + '"></i> ' + GetNodeValue(dtFun[i], 'CategoryC') + ' <span class="fa fa-chevron-down"></span></a>';
                strLi += '<ul class="nav child_menu">';
                strLi += "</ul></li>";
                licat = $(strLi);
                menu.append(licat);
            }
            strLi = '<li><a href="#">' + GetNodeValue(dtFun[i], 'Description') + '</a></li>';
            var liFunc = $(strLi);
            licat.find('.child_menu').append(liFunc);
            var apg = liFunc.find('a');

            apg.click(function () { click_menu(this); });
            apg.prop('Page', GetNodeValue(dtFun[i], "Page"));
            apg.prop('ItemCode', GetNodeValue(dtFun[i], "ItemCode"));
            apg.prop('Description', GetNodeValue(dtFun[i], "Description"));
            apg.prop('SECU_PERMIT', GetNodeValue(dtFun[i], "SECU_PERMIT"));
            apg.prop('href', '#' + GetNodeValue(dtFun[i], "Page"));

        }
    };

    var click_menu = function (menuitem) {
        $('#FunctionDesc').text($(menuitem).prop('Description'));
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

            if (pg == "GMMacPLUSet") {
                if (window.PageGMMacPLUSet == undefined)
                    $.getScript('SystemSetup/GMMacPLUSet.js',
                        function () {
                            PageGMMacPLUSet($(".workarea"));
                        }
                    );
                else {
                    PageGMMacPLUSet($(".workarea"));
                }
            }

            else if (pg == "GMMacPLUSetT") {
                if (window.Pagetest == undefined)
                    $.getScript('test.js',
                        function () {
                            Pagetest($(".workarea"));
                        }
                    );
                else {
                    Pagetest($(".workarea"));
                }
            }

            else if (pg == "Inv") {
                if (window.PageInv == undefined)
                    $.getScript('Inv.js',
                        function () {
                            PageInv($(".workarea"));
                        }
                    );
                else {
                    PageInv($(".workarea"));
                }
            }

            else if (pg == "VMN01") {
                if (window.PageVMN01 == undefined)
                    $.getScript('SystemSetup/VMN01.js',
                        function () {
                            PageVMN01($(".workarea"));
                        }
                    );
                else {
                    PageVMN01($(".workarea"));
                }
            }

            else if (pg == "VMN29") {
                if (window.PageVMN29 == undefined)
                    $.getScript('VMN29.js',
                        function () {
                            PageVMN29($(".workarea"));
                        }
                    );
                else {
                    PageVMN29($(".workarea"));
                }
            }

            else if (pg == "VXT03") {

                if (window.PageVXT03 == undefined)
                    //alert("VXT03");
                    $.getScript('VXT03.js',
                        function () {
                            PageVXT03($(".workarea"));
                        }
                    );
                else {
                    PageVXT03($(".workarea"));
                }
            }


            else if (pg == "VXT03_1") {
                //alert("VXT03_1");
                if (window.PageVXT03_1 == undefined)

                    $.getScript('VXT03_1.js',
                        function () {
                            PageVXT03_1($(".workarea"));
                        }
                    );
                else {
                    PageVXT03_1($(".workarea"));
                }
            }


            else if (pg == "VIN13_1") {
                //alert("VIN13_1");
                if (window.PageVIN13_1 == undefined)

                    $.getScript('VIN13_1.js',
                        function () {
                            PageVIN13_1($(".workarea"));
                        }
                    );
                else {
                    PageVIN13_1($(".workarea"));
                }
            }


            else if (pg == "VIN13_2") {
                //alert("VIN13_2");
                if (window.PageVIN13_2 == undefined)

                    $.getScript('Inv/VIN13_2.js',
                        function () {
                            PageVIN13_2($(".workarea"));
                        }
                    );
                else {
                    PageVIN13_2($(".workarea"));
                }
            }


            else if (pg == "VIN14_2") {
                //alert("VIN14_2");
                if (window.PageVIN14_2 == undefined)

                    $.getScript('Inv/VIN14_2.js',
                        function () {
                            PageVIN14_2($(".workarea"));
                        }
                    );
                else {
                    PageVIN14_2($(".workarea"));
                }
            }


            else if (pg == "VIN14_3") {
                //alert("VIN14_3");
                if (window.PageVIN14_3 == undefined)

                    $.getScript('Inv/VIN14_3.js',
                        function () {
                            PageVIN14_3($(".workarea"));
                        }
                    );
                else {
                    PageVIN14_3($(".workarea"));
                }
            }


            else if (pg == "VIN14_4") {
                //alert("VIN14_4");
                if (window.PageVIN14_4 == undefined)

                    $.getScript('Inv/VIN14_4.js',
                        function () {
                            PageVIN14_4($(".workarea"));
                        }
                    );
                else {
                    PageVIN14_4($(".workarea"));
                }
            }


            else if (pg == "VIN14_5") {
                //alert("VIN14_5");
                if (window.PageVIN14_5 == undefined)

                    $.getScript('Inv/VIN14_5.js',
                        function () {
                            PageVIN14_5($(".workarea"));
                        }
                    );
                else {
                    PageVIN14_5($(".workarea"));
                }
            }


            else if (pg == "VIN47") {
                //alert("VIN47");
                if (window.PageVIN47 == undefined)

                    $.getScript('Inv/VIN47.js',
                        function () {
                            PageVIN47($(".workarea"));
                        }
                    );
                else {
                    PageVIN47($(".workarea"));
                }
            }


            else if (pg == "VIV10") {
                //alert("VIV10");
                if (window.PageVIV10 == undefined)

                    $.getScript('SystemSetup/VIV10.js',
                        function () {
                            PageVIV10($(".workarea"));
                        }
                    );
                else {
                    PageVIV10($(".workarea"));
                }
            }


            else if (pg == "MMMachineSet") {
                if (window.PageMMMachineSet == undefined)
                    $.getScript('SystemSetup/MMMachineSet.js',
                        function () {
                            PageMMMachineSet($(".workarea"));
                        }
                    );
                else {
                    PageMMMachineSet($(".workarea"));
                }
            }
            else if (pg == "GMInvPLUSet") {
                if (window.PageGMInvPLUSet == undefined)
                    $.getScript('SystemSetup/GMInvPLUSet.js',
                        function () {
                            PageGMInvPLUSet($(".workarea"));
                        }
                    );
                else {
                    PageGMInvPLUSet($(".workarea"));
                }
            }
            else if (pg == "VSA04P") {
                if (window.PageParameter == undefined)
                    $.getScript('AIReports/VSA04P.js',
                        function () {
                            PageVSA04P($(".workarea"));
                        }
                    );
                else {
                    PageVSA04P($(".workarea"));
                }
            }
            else if (pg == "VSA21P") {
                if (window.PageParameter == undefined)
                    $.getScript('AIReports/VSA21P.js',
                        function () {
                            PageVSA21P($(".workarea"));
                        }
                    );
                else {
                    PageVSA21P($(".workarea"));
                }
            }
            else if (pg == "VSA21_7P") {
                if (window.PageParameter == undefined)
                    $.getScript('AIReports/VSA21_7P.js',
                        function () {
                            PageVSA21_7P($(".workarea"));
                        }
                    );
                else {
                    PageVSA21_7P($(".workarea"));
                }
            }
            else if (pg == "VSA76_1P") {
                if (window.PageVSA76_1P == undefined)
                    $.getScript('AIReports/VSA76_1P.js',
                        function () {
                            PageVSA76_1P($(".workarea"));
                        }
                    );
                else {
                    PageVSA76_1P($(".workarea"));
                }
            }
            else if (pg == "VSA76P") {
                if (window.PageVSA76P == undefined)
                    $.getScript('AIReports/VSA76P.js',
                        function () {
                            PageVSA76P($(".workarea"));
                        }
                    );
                else {
                    PageVSA76P($(".workarea"));
                }
            }
            else if (pg == "VSA73P") {
                if (window.PageVSA73P == undefined)
                    $.getScript('AIReports/VSA73P.js',
                        function () {
                            PageVSA73P($(".workarea"));
                        }
                    );
                else {
                    PageVSA73P($(".workarea"));
                }
            }
            else if (pg == "VSA73_1P") {
                if (window.PageVSA73_1P == undefined)
                    $.getScript('AIReports/VSA73_1P.js',
                        function () {
                            PageVSA73_1P($(".workarea"));
                        }
                    );
                else {
                    PageVSA73_1P($(".workarea"));
                }
            }
            else if (pg == "VIN14_1P") {
                if (window.PageVIN14_1P == undefined)
                    $.getScript('VIN14_1P.js',
                        function () {
                            PageVIN14_1P($(".workarea"));
                        }
                    );
                else {
                    PageVIN14_1P($(".workarea"));
                }
            }

            else if (pg == "VMN02") {
                if (window.PageVMN02 == undefined)
                    $.getScript('SystemSetup/VMN02.js',
                        function () {
                            PageVMN02($(".workarea"));
                        }
                    );
                else {
                    PageVMN02($(".workarea"));
                }
            }
            
            else if (pg == "SDAccount") {
                if (window.PageSDAccount == undefined)
                    $.getScript('MasterFile/SDAccount.js',
                        function () {
                            PageSDAccount($("#app"), pg);
                        }
                    );
                else {
                    PageSDAccount($("#app"), pg);
                }
            }
            else if (pg == "SDProductCode") {
                if (window.PageSDProductCode == undefined)
                    $.getScript('MasterFile/SDProductCode.js',
                        function () {
                            PageSDProductCode($("#app"), pg);
                        }
                    );
                else {
                    PageSDProductCode($("#app"), pg);
                }
            }
            else if (pg == "SDUpload") {
                if (window.PageSalesData == undefined)
                    $.getScript('SalesData/SalesDataUpload.js',
                        function () {
                            PageSalesData($("#app"), pg);
                        }
                    );
                else {
                    PageSalesData($("#app"), pg);
                }
            }
            else if (pg == "GTStore") {
                if (window.PageGTStore == undefined)
                    $.getScript('MasterFile/GTStore.js',
                        function () {
                            PageGTStore($("#app"), pg);
                        }
                    );
                else {
                    PageGTStore($("#app"), pg);
                }
            }
            else if (pg == "GTSubBrand") {
                if (window.PageGTSubBrand == undefined)
                    $.getScript('MasterFile/GTSubBrand.js',
                        function () {
                            PageGTSubBrand($("#app"), pg);
                        }
                    );
                else {
                    PageGTSubBrand($("#app"), pg);
                }
            }
            else if (pg == "SOUpload") {
                if (window.PageSOUpload == undefined)
                    $.getScript('StockOrder/SOUpload.js',
                        function () {
                            PageSOUpload($("#app"), pg);
                        }
                    );
                else {
                    PageSOUpload($("#app"), pg);
                }
            }
            else if (pg == "SOReport1") {
                if (window.PageSOReport1 == undefined)
                    $.getScript('StockOrder/SOReport1.js',
                        function () {
                            PageSOReport1($("#app"), pg);
                        }
                    );
                else {
                    PageSOReport1($("#app"), pg);
                }
            }
            else if (pg == "SOReport2") {
                if (window.PageSOReport2 == undefined)
                    $.getScript('StockOrder/SOReport2.js',
                        function () {
                            PageSOReport2($("#app"), pg);
                        }
                    );
                else {
                    PageSOReport2($("#app"), pg);
                }
            }
            else if (pg == "SDReport1") {
                if (window.PageSDReport1 == undefined)
                    $.getScript('SalesData/SDReport1.js',
                        function () {
                            PageSDReport1($("#app"), pg);
                        }
                    );
                else {
                    PageSDReport1($("#app"), pg);
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

        $SIDEBAR_MENU.find('a').on('click', function (ev) {
            var $li = $(this).parent();

            if ($li.is('.active')) {
                $li.removeClass('active active-sm');
                $('ul:first', $li).slideUp(function () {
                    setContentHeight();
                });
            } else {
                // prevent closing menu if we are on child menu
                if (!$li.parent().is('.child_menu')) {
                    openUpMenu();
                } else {
                    if ($BODY.is('nav-sm')) {
                        if (!$li.parent().is('child_menu')) {
                            openUpMenu();
                        }
                    }
                }

                $li.addClass('active');

                $('ul:first', $li).slideDown(function () {
                    setContentHeight();
                });
            }
        });

        // toggle small or large menu
        $MENU_TOGGLE.on('click', function () {
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

        // check active menu
        $SIDEBAR_MENU.find('a[href="' + CURRENT_URL + '"]').parent('li').addClass('current-page');

        $SIDEBAR_MENU.find('a').filter(function () {
            return this.href == CURRENT_URL;
        }).parent('li').addClass('current-page').parents('ul').slideDown(function () {
            setContentHeight();
        }).parent().addClass('active');

        // recompute content when resizing
        $(window).smartresize(function () {
            setContentHeight();
        });

        setContentHeight();

        // fixed sidebar
        if ($.fn.mCustomScrollbar) {
            $('.menu_fixed').mCustomScrollbar({
                autoHideScrollbar: true,
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

