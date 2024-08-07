var PageMSSA102 = function (ParentNode) {

    let grdM;
    let grdLookUp_ShopNo;

    let EditMode = "";
    let DelPLU = "";
    let DelPLUQty;
    let ModPLU = "";
    let ModPLUQty;
    let chkShopNo = "";

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
                afterBind: InitModifyDeleteButton,
                sortable: "Y",
                step:"Y"
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
                afterBind: InitShopHead,
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
    let InitShopHead = function () {

    }

    let Step1_click = function (bt) {
        //var heads = $('#tbQuery thead tr th#th0').html();
        //if (heads.toString().indexOf("時段") >= 0) {
        //    return;
        //}

        $('#tbQuery td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#tbQuery td:contains(' + GetNodeValue(node, 'ID') + ')').closest('tr').css('background-color', '#DEEBF7');
        var cg = GetNodeValue(node, 'ID');
        
        $('#lblCG_No').html(cg.split('\n')[0]);
        $('#lblCG_Date').html(cg.split('\n')[1]);
        $('#lblCG_Name').html(cg.split('\n')[2]);
        //alert($('#lblShop1').html());

        $('#modal_Shop1').modal('show');
        setTimeout(function () {
            QueryShop1();
        }, 500);
    };

    let QueryShop1 = function () {
        ShowLoading();
        var pData = {
            CG_No: $('#lblCG_No').html() 
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
            grdM_Shop.BindData(dtShop);

            if (dtShop.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                $('#tbShop1 thead td#td1').html('');
                $('#tbShop1 thead td#td2').html('');
                $('#tbShop1 thead td#td3').html('');
                $('#tbShop1 thead td#td4').html('');
                $('#tbShop1 thead td#td5').html('');
                $('#tbShop1 thead td#td6').html('');
                $('#tbShop1 thead td#td7').html('');
                $('#tbShop1 thead td#td8').html('');
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
        PostToWebApi({ url: "api/SystemSetup/MSSD105Clear_Step1", data: pData, success: afterMSSA102Clear_Shop1 });
    };

    let afterMSSA102Clear_Shop1 = function (data) {
        if (ReturnMsg(data, 0) != "MSSD105Clear_Step1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdShop1.BindData(dtE);

            $(".modal-backdrop").remove();
            $('#tbShop1 thead td#td1_Shop1').html('');
            $('#tbShop1 thead td#td2_Shop1').html('');
            $('#tbShop1 thead td#td3_Shop1').html('');
            $('#tbShop1 thead td#td4_Shop1').html('');
            $('#tbShop1 thead td#td5_Shop1').html('');
            $('#tbShop1 thead td#td6_Shop1').html('');
            $('#tbShop1 thead td#td7_Shop1').html('');
            $('#tbShop1 thead td#td8_Shop1').html('');

        }
    };

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        var pData = {
            ProgramID: "MSSA102"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitMSSA102", data: pData, success: MSSA102Clear });
    };

    let MSSA102Clear = function (data) {
        if (ReturnMsg(data, 0) != "GetInitMSSA102OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#txtOpenDate').val(GetNodeValue(dtE[0], "SysDate1").toString().trim().replaceAll('/', '-'));
            }
            //$('#lblShopNoCnt').html('');
            //$('#lblShopNoName').html('');
            //chkShopNo = "";
            //$('#rdoS').prop('checked', 'true');
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
                $(".modal-backdrop").remove();
                return;
            }

      
        }
    };


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
            $('#btClear').click(function () { btClear_click(this) });

            $('#btExit_Shop').click(function () { btRe_Shop1_click(this) });
           //$('#rdoS,#rdoD').change(function () { ClearQuery() });
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