var PageVMN02 = function (ParentNode) {
    let AllPages;
    let grdU;
    let grdQ;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let OldFlagInv;
    let NewFlagInv;
    

    let AssignVar = function () {
        grdU = new DynGrid(
            {
                table_lement: $('#tbVMN02')[0],
                class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td", "tdColbt icon_in_td", "tdCol2", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7 label-align", "tdCol8 label-align", "tdCol9", "tdColbt icon_in_td"],
                fields_info: [
                    { type: "JQ", name: "fa-plus", element: '<i class="fa fa-plus"></i>' },
                    { type: "JQ", name: "fa-pencil", element: '<i class="fa fa-pencil"></i>' },
                    { type: "JQ", name: "fa-search", element: '<i class="fa fa-search"></i>' },
                    { type: "Text", name: "ST_ID" },
                    { type: "Text", name: "ST_Sname" },
                    { type: "Text", name: "CkNo" },
                    { type: "Text", name: "FlagInv" },
                    { type: "Text", name: "WhnoIn" },
                    { type: "TextAmt", name: "InvGetQty" },
                    { type: "TextAmt", name: "InvSaveQty" },
                    { type: "Text", name: "ST_DeliArea" },
                    { type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' }
                ],
                rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );

        grdQ = new DynGrid(
            {
                table_lement: $('#tbVMN02_Query')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5 label-align", "tdCol6 label-align", "tdCol7", "tdCol8", "tdColbt icon_in_td"],
                fields_info: [
                    { type: "Text", name: "CkNo" },
                    { type: "Text", name: "SNno" },
                    { type: "Text", name: "WhName" },
                    { type: "Text", name: "ST_Address" },
                    { type: "TextAmt", name: "InvGetQty" },
                    { type: "TextAmt", name: "InvSaveQty" },
                    { type: "Text", name: "ST_OpenDay" },
                    { type: "Text", name: "ST_StopDay" },
                    { type: "JQ", name: "fa-qrcode", element: '<i class="fa fa-qrcode"></i>' }
                ],
                rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        SetDateField($('#txtST_OpenDay_Add')[0]);
        SetDateField($('#txtST_OpenDay_Mod')[0]);
        SetDateField($('#txtST_StopDay_Mod')[0]);
    };

    let InitModifyDeleteButton = function () {
        $('#tbVMN02 .fa-plus').click(function () { btAdd_click(this) });
        $('#tbVMN02 .fa-pencil').click(function () { btModD_click(this) });
        $('#tbVMN02 .fa-search').click(function () { btQuery_click(this) });
        $('#tbVMN02 .fa-file-text-o').click(function () { btModH_click(this) });
    }

    let SearchVMN02 = function () {
        ShowLoading();
        GetVMN02()
    };

    let GetVMN02 = function () {
        var pData = {
            ShopNo: $('#cbWh').val(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetVMN02", data: pData, success: AfterGetVMN02 });
    };

    let AfterGetVMN02 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "GetVMN02OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
                var dtVMN02 = data.getElementsByTagName('dtVMN02');
                grdU.BindData(dtVMN02);
                if (dtVMN02.length == 0) {
                    //DyAlert("無符合資料!", BlankMode);
                    return;
                }
        }
    };

    let SearchVMN02_Mod = function () {
        ShowLoading();

        if ($('#cbCkNo_Mod').val() == "") {
            CloseLoading();
            DyAlert("請選擇機號!!");
            return;
        }
        else {
            GetVMN02_Mod()
        }
    };

    let GetVMN02_Mod = function () {
        var pData = {
            ShopNo: $('#lblShop_Mod').html(),
            CkNo: $('#cbCkNo_Mod').val(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetVMN02_Mod", data: pData, success: AfterGetVMN02_Mod });
    };

    let AfterGetVMN02_Mod = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "GetVMN02_ModOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtVMN02_Mod = data.getElementsByTagName('dtVMN02_Mod');
            if (dtVMN02_Mod.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
            else {
                $('#lblCkNo_Mod').html(GetNodeValue(dtVMN02_Mod[0], 'CkNo'))
                $('#lblSNno_Mod').html(GetNodeValue(dtVMN02_Mod[0], 'SNno'))
                $('#lblMCSeq_Mod').html(GetNodeValue(dtVMN02_Mod[0], 'MCSeq'))
                $('#txtAddress_Mod').val(GetNodeValue(dtVMN02_Mod[0], 'ST_Address'))
                $('#txtGetQty_Mod').val(GetNodeValue(dtVMN02_Mod[0], 'InvGetQty'))
                $('#txtSaveQty_Mod').val(GetNodeValue(dtVMN02_Mod[0], 'InvSaveQty'))
                $('#txtST_OpenDay_Mod').val(GetNodeValue(dtVMN02_Mod[0], 'ST_OpenDay'))
                $('#txtST_StopDay_Mod').val(GetNodeValue(dtVMN02_Mod[0], 'ST_StopDay'))
            }
        }
    };

    let GetCheckSN = function () {
        var pData = {
            SNno: $('#cbSN_Add').val(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetCheckSN", data: pData, success: AfterGetCheckSN });
    };

    let AfterGetCheckSN = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "GetCheckSNOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCheckSN = data.getElementsByTagName('dtCheckSN');
            var dtCheckMachine = data.getElementsByTagName('dtCheckMachine');

            if (dtCheckSN.length == 0) {

            }
            else {
                DyAlert("SN碼不可重複!!");
                return;
            }

            if (dtCheckMachine.length == 0) {
                DyAlert("SN碼不為未配置狀態或不存在於硬體維護檔!!");
                return;
            }
            AddVMN02()
        }
    };

    let GetInitMod = function () {
        var pData = {
            ShopNo: $('#lblShop_Mod').html(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetInitMod", data: pData, success: AfterGetInitMod });
    };

    let AfterGetInitMod = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "GetInitModOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtInitMod = data.getElementsByTagName('dtInitMod');
            if (dtInitMod.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
            else {
                InitSelectItem($('#cbCkNo_Mod')[0], dtInitMod, "CkNo", "CkNo", true, "請選擇機號");
            }
        }
    };

    let GetQuery = function (ST_ID) {
        var pData = {
            ShopNo: ST_ID,
        };
        PostToWebApi({ url: "api/SystemSetup/GetQuery", data: pData, success: AfterGetQuery });
    };

    let AfterGetQuery = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "GetQueryOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtQuery = data.getElementsByTagName('dtQuery');
            grdQ.BindData(dtQuery);
            if (dtQuery.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
        }
    };

    let click_PLU = function (tr) {

    };

    let DisplaySuspend = function (tr) {
        var trNode = $(tr).prop('Record');
        var sp = GetNodeValue(trNode, "GD_Flag1");
        var bts = $(tr).find('.btsuspend i');
        bts.hide();
        if (sp == "S")
            $(tr).find('.btsuspend .fa-toggle-off').show();
        else
            $(tr).find('.btsuspend .fa-toggle-on').show();
        var img = $(tr).prop('Photo1');
        var imgSGID = GetNodeValue(trNode, "Photo1");
        var url = "api/GetImage?SGID=" + EncodeSGID(imgSGID) + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        img.prop('src', url);
    };

    let BlankMode = function () {
        
    };

    let btImportFromiXMS_click = function () {
        isImport = true;
        $('#modal_test .modal-title').text('匯入商品');
        $('#GD_NO,#GD_NAME').prop('readonly', false);
        $('#GD_NO').val('');
        $('#GD_NAME').val('');
        $('#GD_Sname').val('');
        $('#Photo1').val('');
        $('#Photo2').val('');
        $('#PLUPic1,#PLUPic2').attr('src', '../images/No_Pic.jpg');
        $('#modal_test').modal('show');
    };

    let GetCheckDoc = function () {
        var node = $(grdU.ActiveRowTR()).prop('Record');
        var pData = {
            ST_ID: GetNodeValue(node, 'ST_ID'),
            SysDate: getTodayDate(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetCheckDoc", data: pData, success: AfterGetCheckDoc });
    };

    let AfterGetCheckDoc = function (data) {
        if (ReturnMsg(data, 0) != "GetCheckDocOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtTransfer = data.getElementsByTagName('dtTransfer');
            var dtUseless = data.getElementsByTagName('dtUseless');
            var dtAdjust = data.getElementsByTagName('dtAdjust');
            var dtChangePLU = data.getElementsByTagName('dtChangePLU');
            var dtChangeShop = data.getElementsByTagName('dtChangeShop');

            if (dtTransfer.length == 0) {

            }
            else {
                DyAlert("本店有調撥單據未完成/過帳，不允許修改!!");
                return;
            }
            if (dtUseless.length == 0) {

            }
            else {
                DyAlert("本店有報廢單據未完成/過帳，不允許修改!!");
                return;
            }
            if (dtAdjust.length == 0) {

            }
            else {
                DyAlert("本店有調整單據未完成/過帳，不允許修改!!");
                return;
            }
            if (dtChangePLU.length == 0) {

            }
            else {
                DyAlert("本店有換貨單據未完成/過帳，不允許修改!!");
                return;
            }
            if (dtChangeShop.length == 0) {

            }
            else {
                DyAlert("本店有換店單據未完成/過帳，不允許修改!!");
                return;
            }
            UpdateVMN02H()
        }
    };

    let UpdateVMN02H = function () {
       
        $('.msg-valid').hide();
        $('#modal_VMN02_ModH .modal-title').text('雲智販群修改');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        /*$('#ShopNo,#OpenDate,#Cash').prop('readonly', true);*/
        $('#lblShop_ModH').html(GetNodeValue(node, 'ST_ID'));
        $('#lblSName_ModH').html(GetNodeValue(node, 'ST_Sname'));
        $('#cbFlagInv_ModH').val(GetNodeValue(node, 'FlagInv'))
        $('#txtWhNoIn_ModH').val(GetNodeValue(node, 'WhnoIn'))
        $('#cbDeliArea_ModH').val(GetNodeValue(node, 'ST_DeliArea'))
        OldFlagInv = GetNodeValue(node, 'FlagInv')
        $('#modal_VMN02_ModH').modal('show');
    };

    let btModH_click = function (bt) {
        setTimeout(function () { GetCheckDoc(); }, 500)
    };

    let btSaveH_click = function (bt) {
        ShowLoading();
        if ($('#cbFlagInv_ModH').val() == "" || $('#cbFlagInv_ModH').val() == null) {
            CloseLoading();
            DyAlert("請選擇母倉!!");
            return;
        }
        NewFlagInv = $('#cbFlagInv_ModH').val()
        if (NewFlagInv == "Y") {
            if ($('#txtWhNoIn_ModH').val() == "") {
                CloseLoading();
                DyAlert("請輸入補貨店倉!!");
                return;
            }
            else {
                if ($('#txtWhNoIn_ModH').val() == $('#lblShop_ModH').html()) {
                    CloseLoading();
                    DyAlert("補貨店倉不可與本店代碼一致!!");
                    return;
                }
            }
        }
        else {
            if ($('#txtWhNoIn_ModH').val() != "") {
                if ($('#txtWhNoIn_ModH').val() == $('#lblShop_ModH').html()) {
                    CloseLoading();
                    DyAlert("補貨店倉不可與本店代碼一致!!");
                    return;
                }
            }
        }
        if ($('#cbDeliArea_ModH').val() == "") {
            CloseLoading();
            DyAlert("請選擇配送區!!");
            return;
        }

        GetCheckSaveH()
    };

    let GetCheckSaveH = function () {
        var pData = {
            ST_ID: $('#lblShop_ModH').html(),
            WhNoIn: $('#txtWhNoIn_ModH').val(),
        };
        PostToWebApi({ url: "api/SystemSetup/GetCheckSaveH", data: pData, success: AfterGetCheckSaveH });
    };

    let AfterGetCheckSaveH = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "GetCheckSaveHOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCheckSaveH = data.getElementsByTagName('dtCheckSaveH');
            var dtCheckSaveD = data.getElementsByTagName('dtCheckSaveD');

            if (dtCheckSaveH.length == 0) {
                DyAlert("補貨店倉不存在，請重新輸入!!");
                return;
            }
            else {
                
            }
            if (NewFlagInv == "N") {
                if (dtCheckSaveD.length == 0) {

                }
                else {
                    if ($('#txtWhNoIn_ModH').val() == "") {
                        CloseLoading();
                        DyAlert("請輸入補貨店倉!!");
                        return;
                    }
                }
            }
            UpdateVMN02H_1()
        }
    };

    let UpdateVMN02H_1 = function () {
        var pData = {
            WarehouseSV: [
                {
                    ST_ID: $('#lblShop_ModH').html(),
                    FlagInv: NewFlagInv,
                    WhNoIn: $('#txtWhNoIn_ModH').val(),
                    OldFlagInv: OldFlagInv,
                    ST_DeliArea: $('#cbDeliArea_ModH').val()
                }
            ]
        };
        PostToWebApi({ url: "api/SystemSetup/UpdateVMN02H_1", data: pData, success: AfterUpdateVMN02H_1 });
    };

    let AfterUpdateVMN02H_1 = function (data) {
        if (ReturnMsg(data, 0) != "UpdateVMN02H_1OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("儲存完成!");
            $('#modal_VMN02_ModH').modal('hide');
            SearchVMN02()
            //var userxml = data.getElementsByTagName('dtWarehouseSV')[0];
            //grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
        }
    };

    let btAdd_click = function (bt) {
        EditMode = "Add"

        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VMN02_Add .modal-title').text('雲智販群機號新增');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        /*$('#ShopNo,#OpenDate,#Cash').prop('readonly', true);*/
        $('#lblShop_Add').html(GetNodeValue(node, 'ST_ID'));
        $('#lblSName_Add').html(GetNodeValue(node, 'ST_Sname'));

        var CkNo_1 ;
        if (GetNodeValue(node, 'FlagInv') == "Y") {
            CkNo_1 = parseInt(GetNodeValue(node, 'CkNo'))
        }
        else {
            CkNo_1 = parseInt(GetNodeValue(node, 'CkNo')) + 1
        }
        if (CkNo_1 < 10)
            $('#lblCkNo_Add').html("0" + CkNo_1)
        else
            $('#lblCkNo_Add').html(CkNo_1)


        $('#txtGetQty_Add').val(GetNodeValue(node, 'InvGetQty'))
        $('#txtSaveQty_Add').val(GetNodeValue(node, 'InvSaveQty'))

        $('#cbSN_Add').val("")
        $('#lblMCSeq_Add').html("")
        $('#txtAddress_Add').val("")
        $('#txtST_OpenDay_Add').val("")
        $('#modal_VMN02_Add').modal('show');
    };

    let btModD_click = function (bt) {
        EditMode = "Mod"

        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VMN02_Mod .modal-title').text('雲智販群機號修改');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        /*$('#ShopNo,#OpenDate,#Cash').prop('readonly', true);*/
        $('#lblShop_Mod').html(GetNodeValue(node, 'ST_ID'));
        $('#lblSName_Mod').html(GetNodeValue(node, 'ST_Sname'));
        GetInitMod()

        $('#lblCkNo_Mod').html("")
        $('#lblSNno_Mod').html("")
        $('#lblMCSeq_Mod').html("")
        $('#txtAddress_Mod').val("")
        $('#txtGetQty_Mod').val("")
        $('#txtSaveQty_Mod').val("")
        $('#txtAddress_Mod').val("")
        $('#txtST_OpenDay_Mod').val("")
        $('#txtST_StopDay_Mod').val("")

        $('#modal_VMN02_Mod').modal('show');
    };

    let btQuery_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VMN02_Query .modal-title').text('雲智販群機號查詢');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#lblShop_Query').html(GetNodeValue(node, 'ST_ID') + GetNodeValue(node, 'ST_SName'));
        setTimeout(function () { GetQuery(GetNodeValue(node, 'ST_ID')); }, 500)
        $('#modal_VMN02_Query').modal('show');
    };

    let btCancel_Add_click = function () {
        $('#modal_VMN02_Add').modal('hide');
    };

    let btCancel_Mod_click = function () {
        $('#modal_VMN02_Mod').modal('hide');
    };

    let btCancel_Query_click = function () {
        $('#modal_VMN02_Query').modal('hide');
    };

    let btCancel_ModH_click = function () {
        $('#modal_VMN02_ModH').modal('hide');
    };

    let btSave_click = function () {
        ShowLoading();

        if (EditMode == "Add") {
            if ($('#cbSN_Add').val() == "") {
                CloseLoading();
                DyAlert("請選擇SN碼!!");
                return;
            }

            if ($('#txtGetQty_Add').val() == "") {
                CloseLoading();
                DyAlert("請輸入電子發票每次取號張數!!");
                return;
            }
            else {
                if (parseInt($('#txtGetQty_Add').val()) > 0) {

                }
                else {
                    CloseLoading();
                    DyAlert("電子發票每次取號張數需為正整數!!");
                    return;
                }
            }

            if ($('#txtSaveQty_Add').val() == "") {
                CloseLoading();
                DyAlert("請輸入電子發票安全存量!!");
                return;
            }
            else {
                if (parseInt($('#txtSaveQty_Add').val()) > 0) {

                }
                else {
                    CloseLoading();
                    DyAlert("電子發票安全存量需為正整數!!");
                    return;
                }
            }

            GetCheckSN()
            
        }

        else if (EditMode == "Mod") {
            if ($('#lblCkNo_Mod').html() == "") {
                CloseLoading();
                DyAlert("請先查詢機號!!");
                return;
            }
            if ($('#txtGetQty_Mod').val() == "") {
                CloseLoading();
                DyAlert("請輸入電子發票每次取號張數!!");
                return;
            }
            else {
                if (parseInt($('#txtGetQty_Mod').val()) > 0) {

                }
                else {
                    CloseLoading();
                    DyAlert("電子發票每次取號張數需為正整數!!");
                    return;
                }
            }
            if ($('#txtSaveQty_Mod').val() == "") {
                CloseLoading();
                DyAlert("請輸入電子發票安全存量!!");
                return;
            }
            else {
                if (parseInt($('#txtSaveQty_Mod').val()) > 0) {

                }
                else {
                    CloseLoading();
                    DyAlert("電子發票安全存量需為正整數!!");
                    return;
                }
            }
            UpdateVMN02()
        }
        
    };

    let AddVMN02 = function () {
        var pData = {
            WarehouseDSV: [
                {
                    ST_ID: $('#lblShop_Add').html(),
                    CkNo: $('#lblCkNo_Add').html(),
                    SNno: $('#cbSN_Add').val(),
                    ST_Address: $('#txtAddress_Add').val(),
                    InvGetQty: $('#txtGetQty_Add').val(),
                    InvSaveQty: $('#txtSaveQty_Add').val(),
                    ST_OpenDay: $('#txtST_OpenDay_Add').val(),
                    QrCode: $('#lblShop_Add').html() + $('#lblCkNo_Add').html() + $('#lblMCSeq_Add').html()
                }
            ]
        }
        PostToWebApi({ url: "api/SystemSetup/AddVMN02", data: pData, success: AfterAddVMN02 });
    };

    let AfterAddVMN02 = function (data) {
        if (ReturnMsg(data, 0) != "AddVMN02OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("新增完成!");
            $('#modal_VMN02_Add').modal('hide');
            SearchVMN02()
            //var userxml = data.getElementsByTagName('dtWarehouseDSV')[0];
            //grdU.AddNew(userxml);
        }
    };

    let UpdateVMN02 = function () {
        var pData = {
            WarehouseDSV: [
                {
                    ST_ID: $('#lblShop_Mod').html(),
                    CkNo: $('#lblCkNo_Mod').html(),
                    ST_Address: $('#txtAddress_Mod').val(),
                    InvGetQty: $('#txtGetQty_Mod').val(),
                    InvSaveQty: $('#txtSaveQty_Mod').val(),
                    ST_OpenDay: $('#txtST_OpenDay_Mod').val(),
                    ST_StopDay: $('#txtST_StopDay_Mod').val(),
                }
            ]
        };
        PostToWebApi({ url: "api/SystemSetup/UpdateVMN02", data: pData, success: AfterUpdateVMN02 });
    };

    let AfterUpdateVMN02 = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "UpdateVMN02OK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("儲存完成!");
            $('#modal_VMN02_Mod').modal('hide');
            var userxml = data.getElementsByTagName('dtWarehouseDSV')[0];
            grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
        }
    };

    let GetGetImage = function (elmImg, picSGID) {
        var url = "api/GetImage?SGID=" + EncodeSGID(picSGID) + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        $('#' + elmImg).prop('src', url);
    }

    let btSuspend_click = function (bt) {
        $(bt).closest('tr').click();
        var act = "停用";
        SetSuspend = "S";
        if ($(bt).hasClass('fa-toggle-off')) {
            act = "啟用";
            SetSuspend = "";
        }
        if (grdU.ActiveRowTR() == null) {
            DyAlert("未選取欲" + act +"之PLU");
            return;
        }
        DyConfirm("確定要" + act + "這筆資料嗎?", SuspendPLU, DummyFunction);
    };

    let SuspendPLU = function () {
        var tr = grdU.ActiveRowTR();
        var trNode = $(tr).prop('Record');
        var pData = {
            GD_NO: GetNodeValue(trNode, "GD_NO"),
            SetSuspend: SetSuspend
        };
        PostToWebApi({ url: "api/SystemSetup/SuspendPLU", data: pData, success: AfterSuspendPLU });
    };

    let AfterSuspendPLU = function (data) {
        if (ReturnMsg(data, 0) != "SuspendPLUOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            DyAlert("完成!");
            var userxml = data.getElementsByTagName('dtPLU')[0];
            grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
            var tr = grdU.ActiveRowTR();
            DisplaySuspend(tr);

        }
    };

    let btCancel_click = function () {
        //2021-04-27
        $('#modal_Inv').modal('hide');
    };

    //2021-05-07
    let cbCK_click = function () {

        if ($('#cbWh').val() == "") {
            $('#cbCK').val() == ""
            DyAlert("請先選擇店查詢條件!!");
            return;
        }
    };

    let GetMCSeq = function () {
        if ($('#cbSN_Add').val() == "") {
            $('#lblMCSeq_Add').empty();
            return;
        }
        else {

        }
        var pData = {
            SNno: $('#cbSN_Add').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetMCSeq", data: pData, success: AfterGetMCSeq });
    };

    let AfterGetMCSeq = function (data) {
        if (ReturnMsg(data, 0) != "GetMCSeqOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtMCSeq = data.getElementsByTagName('dtMCSeq');
            $('#lblMCSeq_Add').html(GetNodeValue(dtMCSeq[0], "MCSeq"))
        }
    };

    let afterGetInitVMN02 = function (data) {
        AssignVar();
        $('#pgVMN02 .fa-search').click(function () { SearchVMN02(); });
        $('#btCancel_Add').click(function () { btCancel_Add_click(); });
        $('#btSave_Add').click(function () { btSave_click(); });
        $('#btCancel_Mod').click(function () { btCancel_Mod_click(); });
        $('#btSave_Mod').click(function () { btSave_click(); });
        $('#modal_VMN02_Mod .fa-search').click(function () { SearchVMN02_Mod(); });
        $('#btCancel_Query').click(function () { btCancel_Query_click(); });
        $('#btCancel_ModH').click(function () { btCancel_ModH_click(); });
        $('#btSave_ModH').click(function () { btSaveH_click(); });
        $('.forminput input').change(function () { InputValidation(this) });


        var dtWarehouse = data.getElementsByTagName('dtWarehouse');
        InitSelectItem($('#cbWh')[0], dtWarehouse, "ST_ID", "ST_Sname", true, "請選擇店代號");

        var dtSN = data.getElementsByTagName('dtSN');
        InitSelectItem($('#cbSN_Add')[0], dtSN, "SNno", "SNno", true, "請選擇SN碼");
        $('#cbSN_Add').change(function () { GetMCSeq(); });

        var dtDeliArea = data.getElementsByTagName('dtDeliArea');
        InitSelectItem($('#cbDeliArea_ModH')[0], dtDeliArea, "Type_ID", "Type_Name", true, "請選擇配送區");
    };

    let InitFileUpload = function () {
        $('#fileupload').fileupload({
            dataType: 'xml',
            url: "api/FileUpload",
            dropZone: $('#dropzone'),
            headers: { "Authorization": "Bearer " + UU }
        });

        $('#fileupload').bind('fileuploadfail',
            function (e, data) {

            }
        );

        $('#fileupload').bind('fileuploadsubmit', function (e, data) {
            data.formData = {
                "UploadFileType": $('#modal-media').prop("UploadFileType"),
                "ImgSGID": $('#' + $('#modal-media').prop("FieldName")).val()
            };
        });

        $('#fileupload').bind('fileuploadalways', function (e, data) {
            AfterFileUpoad(data);
        });

    };

    let AfterFileUpoad = function (returndata) {
        var data = returndata.jqXHR.responseXML;
        if (ReturnMsg(data, 0) != "FileUploadOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            $('#modal-media').modal('hide');
            var UploadFileType = $('#modal-media').prop("UploadFileType");// "PLU+Pic1";
            if (UploadFileType == "PLU+Pic1") {
                GetGetImage("PLUPic1", ReturnMsg(data, 1));
                $('#Photo1').val(ReturnMsg(data, 1));
            }
            if (UploadFileType == "PLU+Pic2") {
                GetGetImage("PLUPic2", ReturnMsg(data, 1));
                $('#Photo2').val(ReturnMsg(data, 1));
            }
            $('#modal-media').prop("UploadFileType", UploadFileType);

        }

    };


    let InputValidation = function (ip) {
        var str = $(ip).val();
        var msg = "";
        $(ip).nextAll('.msg-valid').text(msg);
        $(ip).nextAll('.msg-valid').show();
        if (str == "")
            return;
        if ($(ip).attr('id') == "txtWhNoIn_ModH") {
            $(ip).val($(ip).val().toUpperCase());
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length > 6)
                msg = "必須6碼內英數字";
        }
        if ($(ip).attr('id') == "Type_Name") {
            if (str.length > 20)
                msg = "必須20字元以內";
        }
        if ($(ip).attr('id') == "DisplayNum") {
            var re = /^[\d]+$/;
            if (!re.test(str) | str.length > 2 | str < 0)
                msg = "必須2位內正整數";
        }
        if (msg != "") {
            //$(ip).val('');
            $(ip).nextAll('.msg-valid').text(msg);
            $(ip).nextAll('.msg-valid').show();
        }
    }

    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetInitVMN02", success: afterGetInitVMN02 });
        $('#pgVMN02').show();
    };

    if ($('#pgVMN02').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "VMN02", ["pgVMN02"], afterLoadPage);
    };

    function getTodayDate() {
        var fullDate = new Date();
        var yyyy = fullDate.getFullYear();
        var MM = (fullDate.getMonth() + 1) >= 10 ? (fullDate.getMonth() + 1) : ("0" + (fullDate.getMonth() + 1));
        var dd = fullDate.getDate() < 10 ? ("0" + fullDate.getDate()) : fullDate.getDate();
        var today = yyyy + "/" + MM + "/" + dd;
        return today;
    }
}