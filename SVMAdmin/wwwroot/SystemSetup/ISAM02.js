var PageISAM02 = function (ParentNode) {

    let grdM;
    let EditMode = "";
    let DelPLU = "";
    let DelPLUQty;
    let ModPLU = "";
    let ModPLUQty;


    let AssignVar = function () {

        grdM = new DynGrid(
            {
                table_lement: $('#tbISAM02Mod')[0],
                class_collection: ["tdColbt icon_in_td btPLUDelete", "tdCol1", "tdCol2 label-align", "tdColbt icon_in_td btPLUMod"],
                fields_info: [
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o"></i>' },
                    { type: "Text", name: "GD_Name", style: "width:200px" },
                    { type: "TextAmt", name: "Qty" },
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
        $('#tbISAM02Mod .fa-file-text-o').click(function () { btPLUMod_click(this) });
        $('#tbISAM02Mod .fa-trash-o').click(function () { btPLUDelete_click(this) });
    }

//#region 編查
//#region 單筆修改
    let AfterSaveISAM02PLUMod = function (data) {
        if (ReturnMsg(data, 0) != "SaveISAM02PLUModOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("修改完成!");

            $('#modal_ISAM02PLUMod').modal('hide');
            var dtCollectMod = data.getElementsByTagName('dtCollectMod')[0];
            grdM.RefreshRocord(grdM.ActiveRowTR(), dtCollectMod);
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
        PostToWebApi({ url: "api/SystemSetup/SaveISAM02PLUMod", data: cData, success: AfterSaveISAM02PLUMod });
    };

    let btModCancel_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM02PLUMod').modal('hide');
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
                $('#lblModPLUName').html('');
            }
            $('#modal_ISAM02PLUMod').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUMod_click = function (bt) {
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        //alert(GetNodeValue(node, 'AppDate'));


        $('.msg-valid').hide();
        $('#modal_ISAM02PLUMod .modal-title').text('條碼蒐集資料單筆修改');
        //$('#modal_ISAM02Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        ModPLU = GetNodeValue(node, 'PLU');
        ModPLUQty = GetNodeValue(node, 'Qty');
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
    let AfterDelISAM02PLU = function (data) {
        if (ReturnMsg(data, 0) != "DelISAM02PLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_ISAM02PLUDel').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdM.DeleteRow(grdM.ActiveRowTR());
        }

    };

    let btDelSave_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            PLU: DelPLU
        }
        PostToWebApi({ url: "api/SystemSetup/DelISAM02PLU", data: cData, success: AfterDelISAM02PLU });
    };

    let btDelCancel_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#modal_ISAM02PLUDel').modal('hide');
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
            $('#modal_ISAM02PLUDel').modal('show');
        }

        $('.msg-valid').hide();
    };

    let btPLUDelete_click = function (bt) {
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_ISAM02PLUDel .modal-title').text('條碼蒐集資料單筆刪除');
        //$('#modal_ISAM02Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        DelPLU = GetNodeValue(node, 'PLU');
        DelPLUQty = GetNodeValue(node, 'Qty');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
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
        PostToWebApi({ url: "api/SystemSetup/GetCollectWebMod", data: cData, success: afterGetCollectWebMod });
    };

    let afterGetCollectWebMod = function (data) {
        if (ReturnMsg(data, 0) != "GetCollectWebModOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtCollect = data.getElementsByTagName('dtCollect');
            grdM.BindData(dtCollect);
            if (dtCollect.length == 0) {
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
            Shop: $('#lblShop2').html().split(' ')[0]
        }
        PostToWebApi({ url: "api/SystemSetup/GetCollectWebMod", data: pData, success: afterGetCollectWebMod });
        $('#pgISAM02Mod').show();
        if ($('#pgISAM02Mod').attr('hidden') == undefined) {
            $('#pgISAM02Mod').show();
        }
        else {
            $('#pgISAM02Mod').removeAttr('hidden');
        }
        if ($('#pgISAM02Add').attr('hidden') == undefined) {
            $('#pgISAM02Add').hide();
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
            Barcode: $('#txtBarcode1').val() == "" ? $('#lblBarcode').html() : $('#txtBarcode1').val(),
            Qty: $('#txtQty1').val()
        };
        //alert("908");
        PostToWebApi({ url: "api/SystemSetup/SaveCollectWeb", data: pData, success: afterSaveCollectWeb });
    };

    let btKeyin1_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#btKeyin1').prop('disabled', true);
        $('#btQtySave1').prop('disabled', false);
        $('#txtQty1').prop('disabled', false);
        $('#btBCSave1').prop('disabled', true);
        $('#txtBarcode1').prop('disabled', true);
    };

    let afterSaveCollectWeb = function (data) {
        if (ReturnMsg(data, 0) != "SaveCollectWebOK") {
            //alert("NotOK");
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
            //    DyAlert("無符合資料!", DummyFunction);
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
            Barcode: $('#txtBarcode1').val(),
            Qty: 1
        };
        //alert("908");
        PostToWebApi({ url: "api/SystemSetup/SaveCollectWeb", data: pData, success: afterSaveCollectWeb });
    };

    
    let btAdd_click = function () {
        EditMode = "A";
        Timerset(sessionStorage.getItem('isamcomp'));
        $('#btKeyin1').prop('disabled', false);
        $('#btBCSave1').prop('disabled', false);
        $('#txtBarcode1').prop('disabled', false);
        $('#btQtySave1').prop('disabled', true);
        $('#txtQty1').prop('disabled', true);
        $('#pgISAM02Add').show();
        if ($('#pgISAM02Add').attr('hidden') == undefined) {
            $('#pgISAM02Add').show();
        }
        else {
            $('#pgISAM02Add').removeAttr('hidden');
        }
        if ($('#pgISAM02Mod').attr('hidden') == undefined) {
            $('#pgISAM02Mod').hide();
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
            Type:"C",
            Shop: $('#lblShop2').html().split(' ')[0]
        }
        PostToWebApi({ url: "api/SystemSetup/AddISAMToFTPRecWeb", data: cData, success: AfterAddISAMToFTPRecWeb });
    };


    let btToFTP_click = function () {
        Timerset(sessionStorage.getItem('isamcomp'));
        DyConfirm("是否要上傳條碼蒐集資料？", CallSendToFTP, DummyFunction);
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
        if (EditMode == "A" || EditMode == "M") {
            $('#ISAM02btns').show();
            $('#pgISAM02Add').hide();
            $('#pgISAM02Mod').hide();
        }
        Timerset(sessionStorage.getItem('isamcomp'));
    };
//#endregion

//#region FormLoad
   let afterGetInitISAM02 = function (data) {
       AssignVar();
       EditMode = "Q";
       tbDetail = $('#pgISAM02Mod #tbISAM02Mod tbody');
       
       $('#lblShop2').html(GetNodeValue(data[0], "STName"));
       $('#lblSQty1').html('');
       $('#lblPrice').html('');
       $('#lblGDName').html("品名XXX");
       if ($('#ISAM02btns').attr('hidden') == undefined) {
           $('#ISAM02btns').show();
       }
       else {
           $('#ISAM02btns').removeAttr('hidden');
       }

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
            afterGetInitISAM02(dtWh);
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
    

    if ($('#pgISAM02').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAM02", ["ISAM02btns", "pgISAM02Add", "pgISAM02Mod"], afterLoadPage);
    };


}