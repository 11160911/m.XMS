var PageISAM01 = function (ParentNode) {

    let grdM;
    let EditMode = "";
    let DelPLU = "";
    let DelPLUQty;
    let ModPLU = "";
    let ModPLUQty;


    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbISAM01Mod')[0],
                class_collection: ["tdColbt icon_in_td btPLUDelete", "tdCol1", "tdCol2 label-align", "tdColbt icon_in_td btPLUMod"],
                fields_info: [
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o"></i>' },
                    { type: "Text", name: "GD_Name", style: "width:40%;word-wrap:break-word;word-break:break-all" },
                    { type: "TextAmt", name: "Qty1", style: "text-align:right"},
                    { type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );
        return;
    };

    let click_PLU = function (tr) {

    };

    let InitModifyDeleteButton = function () {
        $('#tbISAM01Mod .fa-file-text-o').click(function () { btPLUMod_click(this) });
        $('#tbISAM01Mod .fa-trash-o').click(function () { btPLUDelete_click(this) });
    }

//#region 編查
//#region 單筆修改
    let AfterSaveISAM01PLUMod = function (data) {
        if (ReturnMsg(data, 0) != "SaveISAM01PLUModOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("修改完成!");

            $('#modal_ISAM01PLUMod').modal('hide');
            var dtBINMod = data.getElementsByTagName('dtBINMod')[0];
            grdM.RefreshRocord(grdM.ActiveRowTR(), dtBINMod);
        }

    };

    let btModSave_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtModQty1').val() == "") {
            DyAlert("請輸入數量!");
            return;
        }
        else {

            if (isNaN($('#txtModQty1').val())) {
                DyAlert("請輸入數字!");
                return;
            }
            if ($('#txtModQty1').val().indexOf(".") > 0) {
                DyAlert("請輸入整數!");
                return;
            }

            if ($('#txtModQty1').val() <= 0) {
                DyAlert("數量需大於0!");
                return;
            }
        }
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: ModPLU,
            Qty: $('#txtModQty1').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SaveISAM01PLUMod", data: cData, success: AfterSaveISAM01PLUMod });
    };

    let btModCancel_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM01PLUMod').modal('hide');
    };

    let AfterGetModGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            $('#lblModPLU').html(ModPLU);
            $('#lblModQty1').html(ModPLUQty);
            $('#txtModQty1').val(ModPLUQty);
            if (dtP.length > 0) {
                $('#lblModPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
            }
            else {
                //DyAlert("無符合之商品資料!");
                //return;
                $('#lblModPLUName').html('');
            }
            $('#modal_ISAM01PLUMod').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUMod_click = function (bt) {
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        //alert(GetNodeValue(node, 'AppDate'));


        $('.msg-valid').hide();
        $('#modal_ISAM01PLUMod .modal-title').text('盤點資料單筆修改');
        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        ModPLU = GetNodeValue(node, 'PLU');
        ModPLUQty = GetNodeValue(node, 'Qty1');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: ModPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetModGDName });

    };
//#endregion

//#region 單筆刪除
    let AfterDelISAM01PLU = function (data) {
        if (ReturnMsg(data, 0) != "DelISAM01PLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_ISAM01PLUDel').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdM.DeleteRow(grdM.ActiveRowTR());
        }

    };

    let btDelSave_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: DelPLU
        }
        PostToWebApi({ url: "api/SystemSetup/DelISAM01PLU", data: cData, success: AfterDelISAM01PLU });
    };

    let btDelCancel_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM01PLUDel').modal('hide');
    };

    let AfterGetGDName = function (data) {
        if (ReturnMsg(data, 0) != "GetGDNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtP = data.getElementsByTagName('dtPLU');
            //alert(dtP.length);
            $('#lblPLU').html(DelPLU);
            $('#lblDelQty1').html(DelPLUQty);
            if (dtP.length > 0) {
                /*DyConfirm("確定要刪除商品" + GetNodeValue(dtP[0], 'PLU') + GetNodeValue(dtP[0], 'GD_Name') + "？", afterDelPLU(GetNodeValue(dtP[0], 'PLU')), DummyFunction);*/
                $('#lblPLUName').html(GetNodeValue(dtP[0], 'GD_Name'));
            }
            else {
                $('#lblPLUName').html('');
             }
            $('#modal_ISAM01PLUDel').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUDelete_click = function (bt) {
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_ISAM01PLUDel .modal-title').text('盤點資料單筆刪除');
        //$('#modal_ISAM01Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        DelPLU = GetNodeValue(node, 'PLU');
        DelPLUQty = GetNodeValue(node, 'Qty1');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: DelPLU
        }
        PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetGDName });

    };
//#endregion


    let btBCSave3_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            PLU: $('#txtBarcode3').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetBINWebMod", data: cData, success: afterGetBINWebMod });
    };

    let afterGetBINWebMod = function (data) {
        if (ReturnMsg(data, 0) != "GetBINWebModOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBin = data.getElementsByTagName('dtBin');
            grdM.BindData(dtBin);
            if (dtBin.length == 0) {
                //alert("No RowData");
                DyAlert("無符合資料!");
                return;
            }

        }

    };

    let btMod_click = function () {
        EditMode = "M";
        Timerset(sessionStorage.getItem('isamcomp'));
        var pData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html()
        }
        PostToWebApi({ url: "api/SystemSetup/GetBINWebMod", data: pData, success: afterGetBINWebMod });
        $('#pgISAM01Mod').show();
        if ($('#pgISAM01Mod').attr('hidden') == undefined) {
            $('#pgISAM01Mod').show();
        }
        else {
            $('#pgISAM01Mod').removeAttr('hidden');
        }
        if ($('#pgISAM01Add').attr('hidden') == undefined) {
            $('#pgISAM01Add').hide();
        }
        BtnSet("M");

    };
//#endregion

//#region 新增
    let btQtySave1_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtBarcode1').val() == "" && $('#lblBarcode').html()=="") {
            DyAlert("請輸入條碼!");
            $('#txtQty1').val('');
            $('#btKeyin1').prop('disabled', false);
            $('#btBCSave1').prop('disabled', false);
            $('#txtBarcode1').prop('disabled', false);
            $('#btQtySave1').prop('disabled', true);
            $('#txtQty1').prop('disabled', true);
            return;
        }
        if ($('#txtQty1').val() == "") {
            DyAlert("請輸入數量!");
            return;
        }
        else {

            if (isNaN($('#txtQty1').val())) {
                DyAlert("請輸入數字!");
                return;
            }
            if ($('#txtQty1').val().indexOf(".") > 0) {
                DyAlert("請輸入整數!");
                return;
            }

            if ($('#txtQty1').val() <= 0) {
                DyAlert("數量需大於0!");
                return;
            }
        }
        $('#btKeyin1').prop('disabled', false);
        $('#btBCSave1').prop('disabled', false);
        $('#txtBarcode1').prop('disabled', false);
        $('#btQtySave1').prop('disabled', true);
        $('#txtQty1').prop('disabled', true);
        var pData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            Barcode: $('#txtBarcode1').val() == "" ? $('#lblBarcode').html() : $('#txtBarcode1').val(),
            Qty: $('#txtQty1').val()
        };
        //alert("908");
        PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
    };

    let btKeyin1_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#btKeyin1').prop('disabled', true);
        $('#btQtySave1').prop('disabled', false);
        $('#txtQty1').prop('disabled', false);
        $('#btBCSave1').prop('disabled', true);
        $('#txtBarcode1').prop('disabled', true);
    };

    let afterSaveBINWeb = function (data) {
        if (ReturnMsg(data, 0) != "SaveBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtSQ = data.getElementsByTagName('dtSQ');
            if (dtSQ.length > 0) {
                $('#lblSQty1').html(GetNodeValue(dtSQ[0], "SQ1"));
            }
            else {
                $('#lblSQty1').html('');
            }
            var dtSBQ = data.getElementsByTagName('dtSBQ');
            if (dtSBQ.length > 0) {
                $('#lblSBQty1').html(GetNodeValue(dtSBQ[0], "SBQ1"));
            }
            else {
                $('#lblSBQty1').html('');
            }
            var dtSWQ = data.getElementsByTagName('dtSWQ');
            if (dtSWQ.length > 0) {
                $('#lblSWQty1').html(GetNodeValue(dtSWQ[0], "SWQ1"));
            }
            else {
                $('#lblSWQty1').html('');
            }
            var dtP = data.getElementsByTagName('dtPLU');
            if (dtP.length > 0) {
                $('#lblBarcode').html(GetNodeValue(dtP[0], "PLU"));
                $('#txtBarcode1').val('');
                $('#lblQty1').html($('#txtQty1').val());
                $('#lblPrice').html(GetNodeValue(dtP[0], "GD_Retail"));
                $('#lblGDName').html(GetNodeValue(dtP[0], "GD_Name"));
            }
            else {
                if ($('#txtBarcode1').val() == "") {
                }
                else {
                    $('#lblBarcode').html($('#txtBarcode1').val());
                }
                $('#txtBarcode1').val('');
                $('#lblQty1').html($('#txtQty1').val());
                $('#lblPrice').html('');
                $('#lblGDName').html('');
            }
            //alert(dtBin.length);
            //if (dtBin.length == 0) {
            //    alert("No RowData");
            //    DyAlert("無符合資料!", BlankMode);
            //    return;
            //}
        }
    };


    let btBCSave1_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtBarcode1').val() == "") {
            DyAlert("請輸入條碼!");
            return;
        }
        if ($('#txtBarcode1').val().trim.length>16) {
            DyAlert("條碼限制輸入16個字元!");
            $('#txtBarcode1').val('');
            return;
        }
        $('#txtQty1').val("1");
        //alert($('#lblShop2').html());
        var pData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html(),
            Barcode: $('#txtBarcode1').val(),
            Qty: 1
        };
        //alert("908");
        PostToWebApi({ url: "api/SystemSetup/SaveBINWeb", data: pData, success: afterSaveBINWeb });
    };

    
    let btAdd_click = function () {
        EditMode = "A";
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#btKeyin1').prop('disabled', false);
        $('#btBCSave1').prop('disabled', false);
        $('#txtBarcode1').prop('disabled', false);
        $('#btQtySave1').prop('disabled', true);
        $('#txtQty1').prop('disabled', true);
        $('#pgISAM01Add').show();
        if ($('#pgISAM01Add').attr('hidden') == undefined) {
            $('#pgISAM01Add').show();
        }
        else {
            $('#pgISAM01Add').removeAttr('hidden');
        }
        if ($('#pgISAM01Mod').attr('hidden') == undefined) {
            $('#pgISAM01Mod').hide();
        }
        BtnSet("A");
        
    };
//#endregion

    let BtnSet = function (edit) {
        switch (edit) {
            case "A":
                $('#btAdd').prop('disabled', false);
                $('#btMod').prop('disabled', true);
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                $('#txtBarcode1').val('');
                $('#txtQty1').val('');
                $('#lblBarcode').html('');
                $('#lblQty1').html('');
                $('#lblSQty1').html('');
                $('#lblSBQty1').html('');
                $('#lblSWQty1').html('');
                $('#lblPrice').html('');
                $('#lblGDName').html('');
                break;
            case "Q":
                $('#btAdd').prop('disabled', false);
                $('#btMod').prop('disabled', false);
                $('#btToFTP').prop('disabled', false); //true-btn不能使用
                break;
            case "M":
                $('#btAdd').prop('disabled', true);
                $('#btMod').prop('disabled', false);
                $('#btToFTP').prop('disabled', true); //true-btn不能使用
                $('#txtBarcode3').val('');
                break;
        }
    };

//#region 上傳
    let AfterAddISAMToFTPRecWeb = function (data) {
        if (ReturnMsg(data, 0) != "AddISAMToFTPRecWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBin = data.getElementsByTagName('dtRec');
            if (dtBin.length == 0) {
                //alert("No RowData");
                DyAlert("待上傳記錄新增失敗，請重新上傳!");
                return;
            }
            else {
                DyAlert("待上傳記錄新增完成!");
                return;
            }
        }
    }

    let CallSendToFTP = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        var cData = {
            Type:"T",
            Shop: $('#lblShop2').html().split(' ')[0],
            ISAMDate: $('#lblDate2').html(),
            BinNo: $('#lblBINNo2').html()
        }
        PostToWebApi({ url: "api/SystemSetup/AddISAMToFTPRecWeb", data: cData, success: AfterAddISAMToFTPRecWeb });
    };


    let btToFTP_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        DyConfirm("是否要上傳" + $('#lblBINNo2').text() + "分區盤點資料？", CallSendToFTP, DummyFunction);
    };
//#endregion

//#region 返回
    let afterRtnclick = function () {
        if (EditMode == "A" || EditMode == "M") {
            EditMode = "Q";
            BtnSet(EditMode);
        }
    };

    let btRtn_click = function () {
        //alert(EditMode);
        if (EditMode == "Q") {
            $('#ISAM01btns').hide();
            $('#pgISAM01Init').show();
            $('#pgISAM01Add').hide();
            $('#pgISAM01Mod').hide();
            //$('#pgISAM01UpFtp').hide();
        } else if (EditMode == "A" || EditMode == "M") {
            $('#ISAM01btns').show();
            $('#pgISAM01Init').hide();
            $('#pgISAM01Add').hide();
            $('#pgISAM01Mod').hide();
            //$('#pgISAM01UpFtp').hide();
        }
        Timerset(sessionStorage.getItem('isamcomp'));
    };
//#endregion

//#region 輸入盤點日期,分區
    let afterSearchBINWeb = function (data) {
        //EditMode = "Q";
        if (ReturnMsg(data, 0) != "SearchBINWebOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtBINData = data.getElementsByTagName('dtBINData');
            if (dtBINData.length > 0) {
                if (GetNodeValue(dtBINData[0], "BINman") != $('#lblManID1').html().split(' ')[0]) {
                    DyAlert("盤點人員" + GetNodeValue(dtBINData[0], "BINman") + "已建立分區" + $('#txtBinNo').val() + "之分區單!!", DummyFunction);
                    return;
                }
            }

            //AssignVar();
            //alert(GetNodeValue(dtISAMShop[0], "STName"));
            EditMode = "Q";
            $('#lblShop2').html($('#lblShop1').html());
            $('#lblBINNo2').html($('#txtBinNo').val());
            $('#lblDate2').html($('#txtISAMDate').val());
            $('#lblSWQty1title').html($('#lblShop1').html().split(' ')[0] + "門市總數：");
            $('#lblSBQty1title').html($('#txtBinNo').val() + "分區總數：");
            $('#lblSQty1').html('');
            $('#lblSBQty1').html('');
            $('#lblSWQty1').html('');
            $('#lblPrice').html('');
            $('#lblGDName').html("品名XXX");
            $('#pgISAM01Init').hide();
            if ($('#ISAM01btns').attr('hidden') == undefined) {
                $('#ISAM01btns').show();
            }
            else {
                $('#ISAM01btns').removeAttr('hidden');
            }
        }
    };

    let btSave_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        if ($('#txtISAMDate').val() == "" | $('#txtISAMDate').val() == null) {
            DyAlert("請輸入盤點日期!!",function () { $('#txtISAMDate').focus() });
            return;
        }
        if ($('#txtBinNo').val() == "" | $('#txtBinNo').val() == null) {
            DyAlert("請輸入分區代碼!!", function () { $('#txtBinNo').focus() });
            return;
        }
        else {
            if ($('#txtBinNo').val().length>10) {
                DyAlert("分區代碼不可超過10個字元!!", function () { $('#txtBinNo').focus() });
                return;
            }
        }
        var pData = {
            Shop: $('#lblShop1').html().split(' ')[0],
            ISAMDate: $('#txtISAMDate').val(),
            BinNo: $('#txtBinNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/SearchBINWeb", data: pData, success: afterSearchBINWeb });
    };
//#endregion

//#region FormLoad
   let afterGetInitISAM01 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitISAM01OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            AssignVar();
            EditMode = "Init";
            tbDetail = $('#pgISAM01Mod #tbISAM01Mod tbody');
            var dtISAMShop = data.getElementsByTagName('dtWh');
            //alert(GetNodeValue(dtISAMShop[0], "STName"));
            $('#lblShop1').text(GetNodeValue(dtISAMShop[0], "STName"));
            $('#lblManID1').text(GetNodeValue(dtISAMShop[0], "ManName"));
            SetDateField($('#txtISAMDate')[0]);
            $('#pgISAM01Init').removeAttr('hidden');
            //$('#pgISAM01Init').show();
            $('#btSave').click(function () { btSave_click(); });
            $('#txtBinNo').keypress(function (e) {
                if (e.which == 13) { btSave_click(); }
            });

            $('#btAdd').click(function () { btAdd_click(); });
            $('#btMod').click(function () { btMod_click(); });
            $('#btToFTP').click(function () { btToFTP_click(); });
            $('#btRtn').click(function () { btRtn_click(); afterRtnclick(); });

            $('#txtQty1').prop('disabled', true);
            $('#btQtySave1').prop('disabled', true);
            $('#btBCSave1').click(function () { btBCSave1_click(); });
            $('#btKeyin1').click(function () { btKeyin1_click(); });
            $('#btQtySave1').click(function () { btQtySave1_click(); });
            $('#txtBarcode1').keypress(function (e) {
                if (e.which == 13) { btBCSave1_click(); }
            });


            $('#btDelCancel').click(function () { btDelCancel_click(); });
            $('#btDelSave').click(function () { btDelSave_click(); });
            

            $('#btModCancel').click(function () { btModCancel_click(); });
            $('#btModSave').click(function () { btModSave_click(); });
            $('#btBCSave3').click(function () { btBCSave3_click(); });
            $('#txtBarcode3').keypress(function (e) {
                if (e.which == 13) { btBCSave3_click(); }
            });
        }
    };

    
    let AfterGetWhName = function (data) {
        if (ReturnMsg(data, 0) != "GetWhNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtWh = data.getElementsByTagName('dtWh');
            //alert(GetNodeValue(dtWh[0], "ST_ID"));
            if (GetNodeValue(dtWh[0], "STName") == "") {
                DyAlert("請確認店櫃(" + GetNodeValue(dtWh[0], "WhNo") + ")是否為允許作業之店櫃!", DummyFunction);
                return;
            }
            PostToWebApi({ url: "api/SystemSetup/GetInitISAM01", success: afterGetInitISAM01 });
        }
    };

    let afterGetPageInitBefore = function (data) {
        if (ReturnMsg(data, 0) != "GetPageInitBeforeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtISAMWh = data.getElementsByTagName('dtComp');
            //alert(GetNodeValue(dtISAMWh[0], "WhNo") );
            if (dtISAMWh.length == 0) {
                DyAlert("無符合資料!", DummyFunction);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") == null | GetNodeValue(dtISAMWh[0], "WhNo") == "") {
                DyAlert("請先至店號設定進行作業店櫃設定!", DummyFunction);
                return;
            }
            else if (GetNodeValue(dtISAMWh[0], "WhNo") != "") {
                PostToWebApi({ url: "api/SystemSetup/GetWhName", success: AfterGetWhName });
            }
            
        }
    };


    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetPageInitBefore", success: afterGetPageInitBefore });
    };
//#endregion
    

    if ($('#pgISAM01').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAM01", ["ISAM01btns", "pgISAM01Init", "pgISAM01Add", "pgISAM01Mod"], afterLoadPage);
    };


}