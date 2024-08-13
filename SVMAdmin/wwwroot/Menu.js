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
    var LogOut = false;
    let grdE;
    let grdF;
    let grdJ;
    var CURRENT_URL = window.location.href.split('#')[0];

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
            var bodyHeight = $BODY.outerHeight(),
                footerHeight = $BODY.hasClass('footer_fixed') ? -10 : $FOOTER.height(),
                leftColHeight = $LEFT_COL.eq(1).height() + $SIDEBAR_FOOTER.height(),
                contentHeight = bodyHeight < leftColHeight ? leftColHeight : bodyHeight;
            contentHeight -= $NAV_MENU.height() + footerHeight;
            $RIGHT_COL.css('min-height', contentHeight);
            //$("a[href='Login']").attr("href", "Login");
            $('#logout').click(function () {
                var pData = {
                    PASSWORD: sessionStorage.getItem('UPWD')
                };
                PostToWebApi({ url: "api/LogOut", data: pData, success: afterLogOut });
            });
            $('.right_col').mousedown(function (e) {
                Sidebar_Close();
                ChkDevice();
                //Timerset();   //不可使用，否則DM的文字編輯器反白時焦點會有異常
            })
            var dtEmployeeSV = data.getElementsByTagName('dtEmployee');
            $('#navbarDropdown').text(GetNodeValue(dtEmployeeSV[0], 'ChineseName') + '-' + GetNodeValue(dtEmployeeSV[0], 'UName'));
            dtFun = data.getElementsByTagName('dtAllFunction');
            SetMenu();
            AssignVar();
            SetHome(data);
            $('#btReJ').click(function () {
                $('#modalJ').modal('hide');
            })
            $('#lblFileName').click(function () {
                download($('#lblAtt').html(), $('#lblFileName').html())
            })
            $('#btA4').click(function () {
                ProgramStart("MSSA108");
            })
            $('#btC3').click(function () {
                ProgramStart("MSSA101");
            })
            $('#btD3').click(function () {
                ProgramStart("MSSA105");
            })
            $('#imgHome').click(function () {
                window.location.reload();
            })
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

    let ProgramStart = function (Program) {
        var gobackhref = CURRENT_URL + "#" + Program;
        window.location.href = gobackhref;

        if (Program == "MSSA108") {
            if (window.PageMSSA108 == undefined)
                $.getScript('SystemSetup/' + Program + '.js',
                    function () {
                        PageMSSA108($(".workarea"));
                    }
                );
            else {
                PageMSSA108($(".workarea"));
            }
        }
        else if (Program == "MSSA101") {
            if (window.PageMSSA101 == undefined)
                $.getScript('SystemSetup/' + Program + '.js',
                    function () {
                        PageMSSA101($(".workarea"));
                    }
                );
            else {
                PageMSSA101($(".workarea"));
            }
        }
        else if (Program == "MSSA105") {
            if (window.PageMSSA105 == undefined)
                $.getScript('SystemSetup/' + Program + '.js',
                    function () {
                        PageMSSA105($(".workarea"));
                    }
                );
            else {
                PageMSSA105($(".workarea"));
            }
        }
        $('#Menu').remove();
    }

    let afterLogOut = function (data) {
        if (ReturnMsg(data, 0) != "LogOutOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            window.location.href = "Login";
        }
    };

    let AssignVar = function () {
        grdE = new DynGrid(
            {
                table_lement: $('#tbE')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3 label-align"],
                fields_info: [
                    { type: "Text", name: "E1", style: "width:15%" },
                    { type: "Text", name: "E2", style: "width:55%" },
                    { type: "TextAmt", name: "E3", style: "width:30%;color:blue" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU
            }
        );

        grdF = new DynGrid(
            {
                table_lement: $('#tbF')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 label-align"],
                fields_info: [
                    { type: "Text", name: "F1", style: "" },
                    { type: "Text", name: "F2" },
                    { type: "TextAmt", name: "F3", style: "color:blue" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU
            }
        );

        grdJ = new DynGrid(
            {
                table_lement: $('#tbJ')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 text-center"],
                fields_info: [
                    { type: "Text", name: "J1", style: "" },
                    { type: "Text", name: "J2" },
                    { type: "Text", name: "J3", style: "text-decoration: underline;cursor:pointer" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclickJ,
                step: "Y"
            }
        );

        return;
    };

    let gridclickJ = function () {
        $('#tbJ tbody tr .tdCol3').click(function () { DownLoadJ(this) });
        $('#tbJ tbody tr .tdCol1,#tbJ tbody tr .tdCol2').click(function () { StepJ(this) });
    }

    let StepJ = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdJ.ActiveRowTR()).prop('Record');
        $('#lblMONo').html(GetNodeValue(node, 'J5'))
        $('#lblAnnounceDate').html(GetNodeValue(node, 'J1'))
        $('#lblMONo2').html(GetNodeValue(node, 'J6'))
        $('#lblAnnounceUser').html(GetNodeValue(node, 'J7'))
        $('#lblTitle').html(GetNodeValue(node, 'J8'))
        $('#lblContent').html(GetNodeValue(node, 'J9'))
        $('#lblFileName').html(GetNodeValue(node, 'J3'))
        $('#lblAtt').html(GetNodeValue(node, 'J4'))
        $('#modalJ').modal('show');
    }

    let DownLoadJ = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdJ.ActiveRowTR()).prop('Record');
        download(GetNodeValue(node, 'J4'), GetNodeValue(node, 'J3'))
   
    }

    let click_PLU = function (tr) {

    };

    let SetHome = function (data) {
        var dtEmployeeSV = data.getElementsByTagName('dtEmployee');
        var dtA = data.getElementsByTagName('dtA');
        var dtB = data.getElementsByTagName('dtB');
        var dtC = data.getElementsByTagName('dtC');
        var dtD = data.getElementsByTagName('dtD');
        var dtE = data.getElementsByTagName('dtE');
        var dtF = data.getElementsByTagName('dtF');
        var dtJ = data.getElementsByTagName('dtJ');
        $('#lblSysDate').html(GetNodeValue(dtEmployeeSV[0], 'SysDate'))
        //今日
        $('#lblA1').html(GetNodeValue(dtA[0], 'A1'))
        $('#lblA2').html(parseFloat(GetNodeValue(dtA[0], 'A2')).toLocaleString('en-US'))
        $('#lblA3').html(parseFloat(GetNodeValue(dtA[0], 'A3')).toLocaleString('en-US'))
        //昨日
        $('#lblB1').html(GetNodeValue(dtB[0], 'B1'))
        $('#lblB2').html(parseFloat(GetNodeValue(dtB[0], 'B2')).toLocaleString('en-US'))
        $('#lblB3').html(parseFloat(GetNodeValue(dtB[0], 'B3')).toLocaleString('en-US'))
        //本月
        $('#lblC1').html(GetNodeValue(dtC[0], 'C1'))
        $('#lblC2').html(parseFloat(GetNodeValue(dtC[0], 'C2')).toLocaleString('en-US'))
        //本年度
        $('#lblD1').html(GetNodeValue(dtD[0], 'D1'))
        $('#lblD2').html(parseFloat(GetNodeValue(dtD[0], 'D2')).toLocaleString('en-US'))
        //本月商品銷售排行前10名
        grdE.BindData(dtE);
        //本月店別銷售排行前10名
        grdF.BindData(dtF);
        //本月各區業績比較圖(圓餅圖)
        SetPie(data);
        //本年度各月業績比較圖(長條圖)
        SetBar(data);
        //日客單價曲線圖(折線圖)
        Setline(data);
        //公佈欄
        grdJ.BindData(dtJ);
    }

    const base64toArrayBuffer = function (base64) {
        var binary_string = window.atob(base64);
        var len = binary_string.length;
        var bytes = new Uint8Array(len);
        for (var i = 0; i < len; i++) {
            bytes[i] = binary_string.charCodeAt(i);
        }
        return bytes.buffer;
    }
    //下載檔案
    const download = function (data,FileName) {
        const arrayBuffer = base64toArrayBuffer(data)
        const url = window.URL.createObjectURL(new Blob([arrayBuffer]))
        const link = document.createElement('a')
        link.style.display = 'none'
        link.href = url
        link.download = FileName
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        URL.revokeObjectURL(url);
    }

    //繪製折線圖
    let Setline = function (data) {
        var dtI = data.getElementsByTagName('dtI');
        var name = [];
        var value1 = [];
        var value2 = [];
        var value3 = [];
        for (var i = 0; i < dtI.length; i++) {
            name.push(GetNodeValue(dtI[i], 'name'));
            value1.push(GetNodeValue(dtI[i], 'value1'));
            value2.push(GetNodeValue(dtI[i], 'value2'));
            value3.push(GetNodeValue(dtI[i], 'value3'));
        }
        // 基于准备好的dom，初始化echarts实例
        var myChart = echarts.init(document.getElementById('divI'));
        // 指定图表的配置项和数据
        var option = {
            title: {
                text: ''
            },
            tooltip: {
                trigger: 'axis'
            },
            color: ['blue', 'green', 'red'],
            legend: {
                data: ['會員客單價', '非會員客單價', '總客單價'],
                textStyle: {
                    fontSize: 16
                }
            },
            xAxis: {
                data: name,
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: 'black',
                        fontSize: 14
                    }   
                }
            },
            yAxis: {
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: 'black',
                        fontSize: 14
                    }
                }
            },
            //grid: {
            //    top: 10,
            //    x: 45,
            //    x2: 30,
            //    y2: 80
            //},
            series: [
                {
                    name: '會員客單價',
                    type: 'line',
                    data: value1
                },
                {
                    name: '非會員客單價',
                    type: 'line',
                    data: value2
                },
                {
                    name: '總客單價',
                    type: 'line',
                    data: value3
                }
            ],
        };
        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
    }
    //繪製圓餅圖
    let SetPie = function (data) {
        var dtG = data.getElementsByTagName('dtG');
        var Records = [];
        for (var r = 0; r < dtG.length; r++) {
            var record = {};
            for (var c = 0; c < 2; c++) {
                var fdname = "";
                if (c == 0) {
                    fdname = "value";
                }
                else if (c == 1) {
                    fdname = "name";
                }
                if (fdname != null) {
                    var value = GetNodeValue(dtG[r], fdname);
                    record[fdname] = value;
                }
            }
            Records.push(record);
        }
        // 基于准备好的dom，初始化echarts实例
        var myChart = echarts.init(document.getElementById('divG'));
        option = {
            title: {
                text: '',
                subtext: '',
                left: 'center'
            },
            tooltip: {
                trigger: 'item'
            },
            legend: {
                orient: 'horizontal',
                bottom: '0%'
            },
            series:
            {
                name: '',
                type: 'pie',
                radius: ['0%','90%'],
                center: ['50%','45%'],
                label: {
                    show: true,
                    position: "inside",
                    formatter: '{d}%',
                    fontSize: 12,
                    color: "white"
                },
                minAngle: 1,
                data: Records,
                emphasis: {
                    itemStyle: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        };
        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
    }
    //繪製長條圖
    let SetBar = function (data) {
        var dtH = data.getElementsByTagName('dtH');
        var name = [];
        var value1 = [];
        var value2 = [];
        for (var i = 0; i < dtH.length; i++) {
            name.push(GetNodeValue(dtH[i], 'name'));
            value1.push(GetNodeValue(dtH[i], 'value1'));
            value2.push(GetNodeValue(dtH[i], 'value2'));
        }

        // 基于准备好的dom，初始化echarts实例
        var myChart = echarts.init(document.getElementById('divH'));
        // 指定图表的配置项和数据
        var option = {
            title: {
                text: ''
            },
            tooltip: {
                trigger: 'axis'
            },
            color: ['blue', 'red'],
            legend: {
                data: ['去年', '今年'],
                top: 20,
                textStyle: {
                    fontSize: 14
                }
            },
            xAxis: {
                data: name,
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: 'black',
                        fontSize: 14
                    },
                    interval: 0,
                    formatter: function (params) {
                        let newParamsName = '';
                        const paramsNameNumber = params.length;
                        const provideNumber = 2; //決定一行顯示幾個字
                        const rowNumber = Math.ceil(paramsNameNumber / provideNumber);
                        if (paramsNameNumber > provideNumber) {
                            for (let p = 0; p < rowNumber; p++) {
                                const start = p * provideNumber;
                                const end = start + provideNumber;
                                const tempstr = p === rowNumber - 1 ? params.substring(start, paramsNameNumber) : params.substring(start, end) + '\n';
                                newParamsName += tempstr;
                            }
                        }
                        else {
                            newParamsName = params;
                        }
                        return newParamsName;
                    }
                }
            },
            yAxis: {
                axisLabel: {
                    show: true,
                    textStyle: {
                        color: 'black',
                        fontSize: 14
                    }
                }
            },
            grid: {
                left: "25%",
                top: "35%"
            },
            series: [
                {
                    name: '去年',
                    type: 'bar',
                    data: value1
                },
                {
                    name: '今年',
                    type: 'bar',
                    data: value2
                }
            ],
        };
        // 使用刚指定的配置项和数据显示图表。
        myChart.setOption(option);
    }

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

    let menuitem;

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
            if (icat != strCat) {  //
                strCat = icat;
                var strLi = '<li><a><i class="fa ' + GetNodeValue(dtFun[i], 'icon') + '"></i> ' + GetNodeValue(dtFun[i], 'CategoryC') + ' <span class="fa fa-chevron-down"></span></a>';
                strLi += '<ul class="nav child_menu">';
                strLi += "</ul></li>";
                //var strLi = '<li><a><i class="fa ' + GetNodeValue(dtFun[i], 'icon') + '"></i> ' + GetNodeValue(dtFun[i], 'CategoryC') + ' </a>';
                //strLi += "</li>";
                licat = $(strLi);
                //var apg = licat.find('a');

                //apg.prop('Page', GetNodeValue(dtFun[i], "Page"));
                //apg.prop('ItemCode', GetNodeValue(dtFun[i], "ItemCode"));
                //apg.prop('Description', GetNodeValue(dtFun[i], "Description"));
                //apg.prop('SECU_PERMIT', GetNodeValue(dtFun[i], "SECU_PERMIT"));
                //apg.prop('href', '#' + GetNodeValue(dtFun[i], "Page"));

                menu.append(licat);
            }
            strLi = '<li><a href="#">' + GetNodeValue(dtFun[i], 'Description') + '</a></li>';
            var liFunc = $(strLi);
            licat.find('.child_menu').append(liFunc);
            var apg = liFunc.find('a');
            apg.prop('Page', GetNodeValue(dtFun[i], "Page"));
            apg.prop('ItemCode', GetNodeValue(dtFun[i], "ItemCode"));
            apg.prop('Description', GetNodeValue(dtFun[i], "Description"));
            apg.prop('SECU_PERMIT', GetNodeValue(dtFun[i], "SECU_PERMIT"));
            apg.prop('href', '#' + GetNodeValue(dtFun[i], "Page"));
            apg.click(function () { click_menu(this); });
        }
    };

    var click_menu = function (menuitem1) {
        Sidebar_Close();
        menuitem = menuitem1;
        var pData = {
            ProgramID: $(menuitem).prop('ItemCode')
        };
        PostToWebApi({ url: "api/ClickMenu", data: pData, success: afterClickMenu });
    }

    let afterClickMenu = function (data) {
        if (ReturnMsg(data, 0) != "ClickMenuOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#FunctionDesc').text($(menuitem).prop('Description'));
            Timerset();
            ChkDevice();
            //$MENU_TOGGLE.click();
            OpenPage(menuitem);
        }
    };

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
            if (pg == "MSSA101") {
                if (window.PageMSSA101 == undefined)
                    $.getScript('SystemSetup/MSSA101.js',
                        function () {
                            PageMSSA101($(".workarea"));
                        }
                    );
                else {
                    PageMSSA101($(".workarea"));
                }
            }
            else if (pg == "MSDM101") {
                if (window.PageMSDM101 == undefined) {
                    $.getScript('SystemSetup/MSDM101.js',
                        function () {
                            PageMSDM101($(".workarea"));
                        }
                    );
                }
                    
                else {
                    PageMSDM101($(".workarea"));
                }
            }
            else if (pg == "MSPV102") {
                if (window.PageMSPV102 == undefined) {
                    $.getScript('SystemSetup/MSPV102.js',
                        function () {
                            PageMSPV102($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSPV102($(".workarea"));
                }
            }
            else if (pg == "MSDM104") {
                if (window.PageMSDM104 == undefined) {
                    $.getScript('SystemSetup/MSDM104.js',
                        function () {
                            PageMSDM104($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSDM104($(".workarea"));
                }
            }
             else if (pg == "MSDM106") {
                if (window.PageMSDM106 == undefined) {
                    $.getScript('SystemSetup/MSDM106.js',
                        function () {
                            PageMSDM106($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSDM106($(".workarea"));
                }
            }
           else if (pg == "MSPV101") {
                if (window.PageMSPV101 == undefined) {
                    $.getScript('SystemSetup/MSPV101.js',
                        function () {
                            PageMSPV101($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSPV101($(".workarea"));
                }
            }
            else if (pg == "MSSD101") {
                if (window.PageMSSD101 == undefined) {
                    $.getScript('SystemSetup/MSSD101.js',
                        function () {
                            PageMSSD101($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSD101($(".workarea"));
                }
            }
             else if (pg == "MSSD102") {
                if (window.PageMSSD102 == undefined) {
                    $.getScript('SystemSetup/MSSD102.js',
                        function () {
                            PageMSSD102($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSD102($(".workarea"));
                }
            }
           else if (pg == "MSSD105") {
                if (window.PageMSSD105 == undefined) {
                    $.getScript('SystemSetup/MSSD105.js',
                        function () {
                            PageMSSD105($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSD105($(".workarea"));
                }
            }
            else if (pg == "MSSD106") {
                if (window.PageMSSD106 == undefined) {
                    $.getScript('SystemSetup/MSSD106.js',
                        function () {
                            PageMSSD106($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSD106($(".workarea"));
                }
            }
            else if (pg == "MSDM107") {
                if (window.PageMSDM107 == undefined) {
                    $.getScript('SystemSetup/MSDM107.js',
                        function () {
                            PageMSDM107($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSDM107($(".workarea"));
                }
            }
            else if (pg == "MSVP101") {
                if (window.PageMSVP101 == undefined) {
                    $.getScript('SystemSetup/MSVP101.js',
                        function () {
                            PageMSVP101($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSVP101($(".workarea"));
                }
            }
            else if (pg == "MSVP102") {
                if (window.PageMSVP102 == undefined) {
                    $.getScript('SystemSetup/MSVP102.js',
                        function () {
                            PageMSVP102($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSVP102($(".workarea"));
                }
            }
            else if (pg == "MSSD104") {
                if (window.PageMSSD104 == undefined) {
                    $.getScript('SystemSetup/MSSD104.js',
                        function () {
                            PageMSSD104($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSD104($(".workarea"));
                }
            }
            else if (pg == "MSSA102") {
                if (window.PageMSSA102 == undefined) {
                  $.getScript('SystemSetup/MSSA102.js',
                        function () {
                            PageMSSA102($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSA102($(".workarea"));
                }
            }
            else if (pg == "MSSA103") {
                if (window.PageMSSA103 == undefined) {
                    $.getScript('SystemSetup/MSSA103.js',
                        function () {
                            PageMSSA103($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSA103($(".workarea"));
                }
            }
            else if (pg == "MSSA105") {
                if (window.PageMSSA105 == undefined) {
                    $.getScript('SystemSetup/MSSA105.js',
                        function () {
                            PageMSSA105($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSA105($(".workarea"));
                }
            }
            else if (pg == "MSSA108") {
                if (window.PageMSSA108 == undefined) {
                    $.getScript('SystemSetup/MSSA108.js',
                        function () {
                            PageMSSA108($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSA108($(".workarea"));
                }
            }
            else if (pg == "MSSA107") {
                if (window.PageMSSA107 == undefined) {
                    $.getScript('SystemSetup/MSSA107.js',
                        function () {
                            PageMSSA107($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSA107($(".workarea"));
                }
            }
            else if (pg == "MSSA106") {
                if (window.PageMSSA106 == undefined) {
                    $.getScript('SystemSetup/MSSA106.js',
                        function () {
                            PageMSSA106($(".workarea"));
                        }
                    );
                }

                else {
                    PageMSSA106($(".workarea"));
                }
            }
            else if (pg == "MSSD103") {
                if (window.PageMSSD103 == undefined) {
                    $.getScript('SystemSetup/MSSD103.js',
                        function () {
                            PageMSSD103($(".workarea"));
                        }
                    );
                }
                else {
                    PageMSSD103($(".workarea"));
                }
            }
            else if (pg == "MSSETLOGO") {
                if (window.PageMSSETLOGO == undefined) {
                    $.getScript('SystemSetup/MSSETLOGO.js',
                        function () {
                            PageMSSETLOGO($(".workarea"));
                        }
                    );
                }
                else {
                    PageMSSETLOGO($(".workarea"));
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
         
            contentHeight -= $NAV_MENU.height() + footerHeight;
            $RIGHT_COL.css('min-height', contentHeight);
        };

        var openUpMenu = function () {
            $SIDEBAR_MENU.find('li').removeClass('active active-sm');
            $SIDEBAR_MENU.find('li ul').slideUp();
        }

        $SIDEBAR_MENU.find('a').on('click', function () {
            //ChkLogOut(sessionStorage.getItem('isamcomp'))
            //click_menu(this);
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

        // check active menu
        $SIDEBAR_MENU.find('a[href="' + CURRENT_URL + '"]').parent('li').addClass('current-page');

        $SIDEBAR_MENU.find('a').filter(function () {
            return this.href == CURRENT_URL;
        }).parent('li').addClass('current-page').parents('ul').slideDown(function () {
            setContentHeight();
        }).parent().addClass('active');

        Timerset();
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

