﻿var PageMSSA102 = function (ParentNode) {

    let grdM;
    let grdM_Shop;
    let grdM_PLU;

    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbQuery')[0],
                class_collection: ["tdCol1 text-left", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 text-center", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "width:260px" },
                    { type: "TextAmt", name: "SalesAmt", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "SalesQty", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CG_Amt", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CG_Qty", style: "vertical-align: middle;" },
                    { type: "TextPercent", name: "CGPer", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CGVIP_Amt", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CGVIP_Qty", style: "vertical-align: middle;" },
                    { type: "TextPercent", name: "VIPPer", style: "vertical-align: middle;" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitDetailButton,
                sortable: "Y",
                step: "Y"
            }
        );

        grdM_Shop = new DynGrid(
            {
                table_lement: $('#tbShop1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 text-center", "tdCol7 label-align", "tdCol8 label-align", "tdCol9 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "width:260px;vertical-align: middle;" },
                    { type: "TextAmt", name: "SalesAmt", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "SalesQty", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CG_Amt", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CG_Qty", style: "vertical-align: middle;" },
                    { type: "TextPercent", name: "CGPer", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CGVIP_Amt", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CGVIP_Qty", style: "vertical-align: middle;" },
                    { type: "TextPercent", name: "VIPPer", style: "vertical-align: middle;" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y",
                step: "Y"
            }
        );

        grdM_PLU = new DynGrid(
            {
                table_lement: $('#tbPLU1')[0],
                class_collection: ["tdCol1 text-center", "tdCol2 label-align", "tdCol3 label-align", "tdCol4 label-align", "tdCol5 label-align", "tdCol6 text-center"],
                fields_info: [
                    { type: "Text", name: "ID", style: "width:260px;vertical-align: middle;" },
                    { type: "TextAmt", name: "CG_Amt", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CG_Qty", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CGVIP_Amt", style: "vertical-align: middle;" },
                    { type: "TextAmt", name: "CGVIP_Qty", style: "vertical-align: middle;" },
                    { type: "TextPercent", name: "VIPPer", style: "vertical-align: middle;" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                sortable: "Y",
                step: "Y"
            }
        );


        return;
    };

    let click_PLU = function (tr) {

    };

    let InitDetailButton = function () {

        $('#tbQuery tbody tr td').click(function () { Step1_click(this) });
    }

    let Step1_click = function (bt) {

        $('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');       
        $('#tbQuery td:contains(' + GetNodeValue(node, 'ID').replace('(', '').replace(')', '') + ')').closest('tr').css('background-color', '#DEEBF7');

        var cg = GetNodeValue(node, 'ID');
        $('#lblCG_No').html(cg.split('\n')[0]);
        $('#lblCG_Date').html(cg.split('\n')[1]);
        $('#lblCG_Name').html(cg.split('\n')[2]);
        if ($('#rdoShop').prop('checked') == true) {
            $('#lblTip1_Shop1').html('總交易&ensp;:&ensp;')
            $('#lblTip2_Shop1').html('組促期間實際總交易業績')
            $('#lblTip3_Shop1').html('組促總交易&ensp;:&ensp;')
            $('#lblTip4_Shop1').html('組促期間有達到促銷條件之每筆交易總和')
            $('#divTip_Shop1').show();
        }
        else if ($('#rdoPLU').prop('checked') == true) {
            $('#lblTip1_Shop1').html('組促商品&ensp;:&ensp;')
            $('#lblTip2_Shop1').html('組促商品在期間達成促銷條件之統計')
            $('#lblTip3_Shop1').html('會員組促商品&ensp;:&ensp;')
            $('#lblTip4_Shop1').html('組促商品在期間會員交易達成促銷條件之統計')
            $('#divTip_Shop1').hide();
        }

        $('#modal_Shop1').modal('show');
        setTimeout(function () {
            QueryShop1();
        }, 500);
    };

    let QueryShop1 = function () {
        ShowLoading();

        var Flag = "";

        if ($('#rdoShop').prop('checked') == true) {
            Flag = "S";
            $('#tbShop1').show();
            $('#tbPLU1').hide();

        }
        else if ($('#rdoPLU').prop('checked') == true) {
            Flag = "P";
            $('#tbShop1').hide();
            if ($('#tbPLU1').attr('hidden') == undefined) {
                $('#tbPLU1').show();
            }
            else {
                $('#tbPLU1').removeAttr('hidden');
                $('#tbPLU1').show();
            }
        }

        var pData = {
            CG_No: $('#lblCG_No').html(),
            Flag: Flag
        }
        PostToWebApi({ url: "api/SystemSetup/MSSA102QueryShop", data: pData, success: afterMSSA102Query_Shop });
    };

    let afterMSSA102Query_Shop = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA102QueryShopOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtShop = data.getElementsByTagName('dtShop');
            var dtSum = data.getElementsByTagName('dtSum');

            if ($('#rdoShop').prop('checked') == true) {
                grdM_Shop.BindData(dtShop);

                if (dtShop.length == 0) {
                    DyAlert("無符合資料!");
                    //$(".modal-backdrop").remove();
                    $('#tbShop1 thead td#td1_Shop1').html('');
                    $('#tbShop1 thead td#td2_Shop1').html('');
                    $('#tbShop1 thead td#td3_Shop1').html('');
                    $('#tbShop1 thead td#td4_Shop1').html('');
                    $('#tbShop1 thead td#td5_Shop1').html('');
                    $('#tbShop1 thead td#td6_Shop1').html('');
                    $('#tbShop1 thead td#td7_Shop1').html('');
                    $('#tbShop1 thead td#td8_Shop1').html('');
                    return;
                }

                $('#tbShop1 thead td#td1_Shop1').html(parseInt(GetNodeValue(dtSum[0], "SalesAmt")).toLocaleString('en-US'));
                $('#tbShop1 thead td#td2_Shop1').html(parseInt(GetNodeValue(dtSum[0], "SalesQty")).toLocaleString('en-US'));
                $('#tbShop1 thead td#td3_Shop1').html(parseInt(GetNodeValue(dtSum[0], "CG_Amt")).toLocaleString('en-US'));
                $('#tbShop1 thead td#td4_Shop1').html(parseInt(GetNodeValue(dtSum[0], "CG_Qty")).toLocaleString('en-US'));
                $('#tbShop1 thead td#td5_Shop1').html(GetNodeValue(dtSum[0], "CGPer"));
                $('#tbShop1 thead td#td6_Shop1').html(parseInt(GetNodeValue(dtSum[0], "CGVIP_Amt")).toLocaleString('en-US'));
                $('#tbShop1 thead td#td7_Shop1').html(parseInt(GetNodeValue(dtSum[0], "CGVIP_Qty")).toLocaleString('en-US'));
                $('#tbShop1 thead td#td8_Shop1').html(GetNodeValue(dtSum[0], "VIPPer"));
            }
            else if ($('#rdoPLU').prop('checked') == true) {
                grdM_PLU.BindData(dtShop);

                if (dtShop.length == 0) {
                    DyAlert("無符合資料!");
                    //$(".modal-backdrop").remove();
                    $('#tbPLU1 thead td#td1_PLU1').html('');
                    $('#tbPLU1 thead td#td2_PLU1').html('');
                    $('#tbPLU1 thead td#td3_PLU1').html('');
                    $('#tbPLU1 thead td#td4_PLU1').html('');
                    $('#tbPLU1 thead td#td5_PLU1').html('');
                    return;
                }

                $('#tbPLU1 thead td#td1_PLU1').html(parseInt(GetNodeValue(dtSum[0], "CG_Amt")).toLocaleString('en-US'));
                $('#tbPLU1 thead td#td2_PLU1').html(parseInt(GetNodeValue(dtSum[0], "CG_Qty")).toLocaleString('en-US'));
                $('#tbPLU1 thead td#td3_PLU1').html(parseInt(GetNodeValue(dtSum[0], "CGVIP_Amt")).toLocaleString('en-US'));
                $('#tbPLU1 thead td#td4_PLU1').html(parseInt(GetNodeValue(dtSum[0], "CGVIP_Qty")).toLocaleString('en-US'));
                $('#tbPLU1 thead td#td5_PLU1').html(GetNodeValue(dtSum[0], "VIPPer"));

            }
        }
    };
    let btRe_Shop1_click = function (bt) {
        $('#modal_Shop1').modal('hide');
        setTimeout(function () {
            ClearShop1();
        }, 500);
    };

    let ClearShop1 = function () {
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSSD102Clear_Step1", data: pData, success: afterMSSA102Clear_Shop1 });
    };

    let afterMSSA102Clear_Shop1 = function (data) {
        if (ReturnMsg(data, 0) != "MSSD102Clear_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');

            if ($('#rdoShop').prop('checked') == true) {
                grdM_Shop.BindData(dtE);

                //$(".modal-backdrop").remove();
                $('#tbShop1 thead td#td1_Shop1').html('');
                $('#tbShop1 thead td#td2_Shop1').html('');
                $('#tbShop1 thead td#td3_Shop1').html('');
                $('#tbShop1 thead td#td4_Shop1').html('');
                $('#tbShop1 thead td#td5_Shop1').html('');
                $('#tbShop1 thead td#td6_Shop1').html('');
                $('#tbShop1 thead td#td7_Shop1').html('');
                $('#tbShop1 thead td#td8_Shop1').html('');
            }
            else if ($('#rdoPLU').prop('checked') == true) {
                grdM_PLU.BindData(dtE);

                //$(".modal-backdrop").remove();
                $('#tbPLU1 thead td#td1_PLU1').html('');
                $('#tbPLU1 thead td#td2_PLU1').html('');
                $('#tbPLU1 thead td#td3_PLU1').html('');
                $('#tbPLU1 thead td#td4_PLU1').html('');
                $('#tbPLU1 thead td#td5_PLU1').html('');
            }
            $('#rdoShop').prop('checked', 'true');
       }
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#tbQuery thead tr th').css('background-color', '#ffb620')
        $('#btQuery').prop('disabled', true)

        //本期
        if ($('#txtOpenDate').val() == "") {
            DyAlert("促銷日期需輸入!", function () { $('#btQuery').prop('disabled', false); })
            return
        }

        ShowLoading();

        setTimeout(function () {
            var pData = {
                OpenDate: $('#txtOpenDate').val().toString().replaceAll('-', '/'),
            }
            PostToWebApi({ url: "api/SystemSetup/MSSA102Query", data: pData, success: afterMSSA102Query });
        }, 1000);
    };

    let afterMSSA102Query = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSA102QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            $('#btQuery').prop('disabled', false);
            var dtD = data.getElementsByTagName('dtD');
            grdM.BindData(dtD);
            if (dtD.length == 0) {
                DyAlert("無符合資料!");
                //$(".modal-backdrop").remove();
                return;
            }
        }
    };

    let ClearQuery = function () {
        grdM.BindData(null)
    }
    //#region FormLoad
    let GetInitMSSA102 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSA102OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
                $('#txtOpenDate').val(GetNodeValue(dtE[0], "SysDate").toString().trim().replaceAll('/', '-'));
            }
            AssignVar();
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#txtOpenDate').change(function () { ClearQuery() });
            $('#btExit_Shop').click(function () { btRe_Shop1_click(this) });
            $('#rdoShop,#rdoPLU').change(function () { Step1_click() });
            btQuery_click();
        }
    };

    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSSA102"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSA102", data: pData, success: GetInitMSSA102 });
    };
    //#endregion

    if ($('#pgMSSA102').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSA102", ["pgMSSA102Init"], afterLoadPage);
    };


}