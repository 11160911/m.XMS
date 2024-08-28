var PageMSSA108 = function (ParentNode) {

    let grdM;
    let grdTop20;
    let grdD1;
    let grdD2;
    let grdD21;
    let grdDD1;
    let grdD3;
    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "RecCount"},
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextAmt", name: "Price" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y",
                step: "Y"
            }
        );
        grdTop20 = new DynGrid(
            {
                table_lement: $('#tbTop20')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 label-align", "tdCol4 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "Text", name: "Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclickTop20,
                sortable: "Y",
                step:"Y"
            }
        );
        grdD1 = new DynGrid(
            {
                table_lement: $('#tbQueryD1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 label-align", "tdCol4 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "Text", name: "Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );
        grdD2 = new DynGrid(
            {
                table_lement: $('#tbQueryD2')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "RecCount" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" },
                    { type: "TextAmt", name: "Price" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: gridclickD2,
                sortable: "Y",
                step: "Y"
            }
        );
        grdD21 = new DynGrid(
            {
                table_lement: $('#tbQueryD2-1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 label-align", "tdCol4 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "Text", name: "Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );
        grdDD1 = new DynGrid(
            {
                table_lement: $('#tbQueryDD1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 text-center", "tdCol3 label-align", "tdCol4 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "Text", name: "Name" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "TextAmt", name: "Cash" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );
        grdD3 = new DynGrid(
            {
                table_lement: $('#tbQueryD3')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align"],
                fields_info: [
                    { type: "Text", name: "ID", style: "" },
                    { type: "TextAmt", name: "Num" },
                    { type: "TextAmt", name: "Cash" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y"
            }
        );

        return;
    };

    let click_PLU = function (tr) {

    };

    let InitModifyDeleteButton = function () {
        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });
    }

    let gridclickD2 = function () {
        $('#tbQueryD2 tbody tr td').click(function () { Step2_click(this) });
    }

    let gridclickTop20 = function () {
        $('#tbTop20 tbody tr td').click(function () { StepTop20_click(this) });
    }

    let StepTop20_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdTop20.ActiveRowTR()).prop('Record');
        $('#lblToday_D3').html($('#lblToday').html())
        $('#lblPLU_D3').html(GetNodeValue(node, 'ID') + '-' + GetNodeValue(node, 'Name'))
        $('#modalD3').modal('show');
        setTimeout(function () {
            QueryD3();
        }, 500);
    };

    let Step2_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdD2.ActiveRowTR()).prop('Record');
        $('#lblToday_DD1').html($('#lblToday_D2').html())
        $('#lblArea_DD1').html($('#lblArea_D2').html())
        $('#lblShopNo_DD1').html(GetNodeValue(node, 'ID'))
        $('#rdo20C_DD1').prop('checked', true);
        $('#modalDD1').modal('show');
        setTimeout(function () {
            QueryDD1();
        }, 500);
    };

    let Step1_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        var heads = $('#tbQuery thead tr th#thead1');
        if ($(heads).html() == "店別") {
            $('#lblToday_D1').html($('#lblToday').html())
            $('#lblShopNo_D1').html(GetNodeValue(node, 'ID'))
            $('#rdo20C_D1').prop('checked', true);
            $('#modalD1').modal('show');
            setTimeout(function () {
                QueryD1();
            }, 500);
        }
        else if ($(heads).html() == "區域") {
            $('#lblToday_D2').html($('#lblToday').html())
            $('#lblArea_D2').html(GetNodeValue(node, 'ID'))
            $('#rdoS_D2').prop('checked', true);
            $('#modalD2').modal('show');
            setTimeout(function () {
                QueryD2();
            }, 500);
        }
        
    };

    let QueryD1 = function () {
        ShowLoading();
        var Flag = "";
        var ShopNo = $('#lblShopNo_D1').html().split('-')[0];
        if ($('#rdo20C_D1').prop('checked') == true) {
            Flag = "20C";
        }
        else if ($('#rdo20N_D1').prop('checked') == true) {
            Flag = "20N";
        }

        var pData = {
            Today: $('#lblToday_D1').html(),
            ShopNo: ShopNo,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA108QueryD1", data: pData, success: afterMSSA108QueryD1 });
    };

    let afterMSSA108QueryD1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA108QueryD1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdD1.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QSumD_D1');
                for (i = 0; i < sumtdQD.childElementCount; i++) {
                    sumtdQD.children[i].innerHTML = "";
                }
                return;
            }
            var dtH = data.getElementsByTagName('dtH');
            $('#tbQueryD1 thead td#td1_D1').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
            $('#tbQueryD1 thead td#td2_D1').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
        }
    };

    let QueryD2 = function () {
        ShowLoading();
        var Flag = "";
        var Area = $('#lblArea_D2').html().split('-')[0];
        if ($('#rdoS_D2').prop('checked') == true) {
            Flag = "S";
            $('#tbQueryD2').show();
            $('#tbQueryD2-1').hide();
        }
        else if ($('#rdo20C_D2').prop('checked') == true) {
            Flag = "20C";
            $('#tbQueryD2').hide();
            if ($('#tbQueryD2-1').attr('hidden') == undefined) {
                $('#tbQueryD2-1').show();
            }
            else {
                $('#tbQueryD2-1').removeAttr('hidden');
                $('#tbQueryD2-1').show();
            }
        }
        else if ($('#rdo20N_D2').prop('checked') == true) {
            Flag = "20N";
            $('#tbQueryD2').hide();
            if ($('#tbQueryD2-1').attr('hidden') == undefined) {
                $('#tbQueryD2-1').show();
            }
            else {
                $('#tbQueryD2-1').removeAttr('hidden');
                $('#tbQueryD2-1').show();
            }
        }

        var pData = {
            Today: $('#lblToday_D2').html(),
            Area: Area,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA108QueryD2", data: pData, success: afterMSSA108QueryD2 });
    };

    let afterMSSA108QueryD2 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA108QueryD2OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if ($('#rdoS_D2').prop('checked') == true) {
                grdD2.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    //$(".modal-backdrop").remove();
                    var sumtdQD = document.querySelector('.QSumD_D2');
                    for (i = 0; i < sumtdQD.childElementCount; i++) {
                        sumtdQD.children[i].innerHTML = "";
                    }
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#tbQueryD2 thead td#td1_D2').html(parseInt(GetNodeValue(dtH[0], "SumRecCount")).toLocaleString('en-US'));
                $('#tbQueryD2 thead td#td2_D2').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbQueryD2 thead td#td3_D2').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
                $('#tbQueryD2 thead td#td4_D2').html(parseInt(GetNodeValue(dtH[0], "SumPrice")).toLocaleString('en-US'));
            }
            else {
                var dtE = data.getElementsByTagName('dtE');
                grdD21.BindData(dtE);
                if (dtE.length == 0) {
                    DyAlert("無符合資料!");
                    //$(".modal-backdrop").remove();
                    var sumtdQD = document.querySelector('.QSumD_D2-1');
                    for (i = 0; i < sumtdQD.childElementCount; i++) {
                        sumtdQD.children[i].innerHTML = "";
                    }
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#tbQueryD2-1 thead td#td1_D2-1').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbQueryD2-1 thead td#td2_D2-1').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }

        }
    };

    let QueryDD1 = function () {
        ShowLoading();
        var Flag = "";
        var Area = $('#lblArea_DD1').html().split('-')[0];
        var ShopNo = $('#lblShopNo_DD1').html().split('-')[0];
        if ($('#rdo20C_DD1').prop('checked') == true) {
            Flag = "20C";
        }
        else if ($('#rdo20N_DD1').prop('checked') == true) {
            Flag = "20N";
        }

        var pData = {
            Today: $('#lblToday_DD1').html(),
            Area: Area,
            ShopNo: ShopNo,
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA108QueryDD1", data: pData, success: afterMSSA108QueryDD1 });
    };

    let afterMSSA108QueryDD1 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA108QueryDD1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdDD1.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QSumD_DD1');
                for (i = 0; i < sumtdQD.childElementCount; i++) {
                    sumtdQD.children[i].innerHTML = "";
                }
                return;
            }
            var dtH = data.getElementsByTagName('dtH');
            $('#tbQueryDD1 thead td#td1_DD1').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
            $('#tbQueryDD1 thead td#td2_DD1').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
        }
    };

    let QueryD3 = function () {
        ShowLoading();
        var PLU = $('#lblPLU_D3').html().split('-')[0];
        var pData = {
            Today: $('#lblToday_D3').html(),
            PLU: PLU
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA108QueryD3", data: pData, success: afterMSSA108QueryD3 });
    };

    let afterMSSA108QueryD3 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA108QueryD3OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdD3.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                var sumtdQD = document.querySelector('.QSumD_D3');
                for (i = 0; i < sumtdQD.childElementCount; i++) {
                    sumtdQD.children[i].innerHTML = "";
                }
                return;
            }
            var dtH = data.getElementsByTagName('dtH');
            $('#tbQueryD3 thead td#td1_D3').html(parseInt(GetNodeValue(dtH[0], "SumNum")).toLocaleString('en-US'));
            $('#tbQueryD3 thead td#td2_D3').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
        }
    };

    let ChkLogOut_1 = function (AfterChkLogOut_1) {
        var LoginDT = sessionStorage.getItem('LoginDT');
        var cData = {
            LoginDT: LoginDT
        }
        PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut_1 });
    };

    let btRe_D1_click = function (bt) {
        $('#modalD1').modal('hide');
    };

    let btRe_D2_click = function (bt) {
        $('#modalD2').modal('hide');
    };

    let btRe_DD1_click = function (bt) {
        $('#modalDD1').modal('hide');
    };

    let btRe_D3_click = function (bt) {
        $('#modalD3').modal('hide');
    };
//#region FormLoad
    let MSSA108Query = function () {
        ShowLoading();
        //區域
        if ($('#rdoA').prop('checked') == true) {
            Flag = "A";
            $('#tbQuery').show();
            $('#tbTop20').hide();
        }
        //店別
        else if ($('#rdoS').prop('checked') == true) {
            Flag = "S";
            $('#tbQuery').show();
            $('#tbTop20').hide();
        }
        //前20名商品(金額)
        else if ($('#rdo20C').prop('checked') == true) {
            Flag = "20C";
            $('#tbQuery').hide();
            if ($('#tbTop20').attr('hidden') == undefined) {
                $('#tbTop20').show();
            }
            else {
                $('#tbTop20').removeAttr('hidden');
                $('#tbTop20').show();
            }
        }
        //前20名商品(數量)
        else if ($('#rdo20N').prop('checked') == true) {
            Flag = "20N";
            $('#tbQuery').hide();
            if ($('#tbTop20').attr('hidden') == undefined) {
                $('#tbTop20').show();
            }
            else {
                $('#tbTop20').removeAttr('hidden');
                $('#tbTop20').show();
            }
        }

        var pData = {
            ProgramID: "MSSA108",
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA108Query", data: pData, success: afterMSSA108Query });
    };

    let afterMSSA108Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA108QueryOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtD = data.getElementsByTagName('dtD');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
                $('#lblToday').html(GetNodeValue(dtE[0], "SysDate"));
            }
            if ($('#rdoA').prop('checked') || $('#rdoS').prop('checked')) {
                grdM.BindData(dtD);
                var heads = $('#tbQuery thead tr th#thead1');
                if ($('#rdoA').prop('checked')) {
                    $(heads).html('區域');
                }
                else if ($('#rdoS').prop('checked')) {
                    $(heads).html('店別');
                }
                if (dtD.length == 0) {
                    DyAlert("無符合資料!");
                    //$(".modal-backdrop").remove();
                    $('#tbQuery thead td#td1').html('');
                    $('#tbQuery thead td#td2').html('');
                    $('#tbQuery thead td#td3').html('');
                    $('#tbQuery thead td#td4').html('');
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#tbQuery thead td#td1').html(parseInt(GetNodeValue(dtH[0], "SumRecCount")).toLocaleString('en-US'));
                $('#tbQuery thead td#td2').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbQuery thead td#td3').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
                $('#tbQuery thead td#td4').html(parseInt(GetNodeValue(dtH[0], "SumPrice")).toLocaleString('en-US'));
            }
            else {
                grdTop20.BindData(dtD);
                if (dtD.length == 0) {
                    DyAlert("無符合資料!");
                    //$(".modal-backdrop").remove();
                    $('#tbTop20 thead td#tdTop20_1').html('');
                    $('#tbTop20 thead td#tdTop20_2').html('');
                    return;
                }
                var dtH = data.getElementsByTagName('dtH');
                $('#tbTop20 thead td#tdTop20_1').html(parseInt(GetNodeValue(dtH[0], "SumQty")).toLocaleString('en-US'));
                $('#tbTop20 thead td#tdTop20_2').html(parseInt(GetNodeValue(dtH[0], "SumCash")).toLocaleString('en-US'));
            }
        }
    };
    
    let afterLoadPage = function () {
        AssignVar();

        $('#rdoA,#rdoS,#rdo20C,#rdo20N').change(function () { MSSA108Query() });
        MSSA108Query();

        $('#btRe_D1').click(function () { btRe_D1_click(this) })
        $('#rdo20C_D1,#rdo20N_D1').change(function () { QueryD1() })

        $('#btRe_D2').click(function () { btRe_D2_click(this) })
        $('#rdoS_D2,#rdo20C_D2,#rdo20N_D2').change(function () { QueryD2() })

        $('#btRe_DD1').click(function () { btRe_DD1_click(this) })
        $('#rdo20C_DD1,#rdo20N_DD1').change(function () { QueryDD1() })

        $('#btRe_D3').click(function () { btRe_D3_click(this) })
    };
//#endregion
    

    if ($('#pgMSSA108').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA108", ["MSSA108btns", "pgMSSA108Init", "pgMSSA108Add", "pgMSSA108Mod"], afterLoadPage);
    };


}