var PageVIN13_2 = function (ParentNode) {
    let AllPages;
    let grdU;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let SysDate ;
    let gDocNo = "";
    let ModalCkNo = "";

    let AssignVar = function () {

        console.log("AssignVar");

        grdU = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVIN13_2')[0],
                class_collection: ["tdColbt icon_in_td btEdit", "tdColbt icon_in_td btDel", "tdColbt icon_in_td btDef", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdCol8", "tdCol9", "tdCol10", "tdCol11", "tdCol12", "tdCol13", "tdColbt icon_in_td btT", "tdCol15", "tdCol16"],
                //class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td btsuspend", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7",  "tdCol8"],
                fields_info: [
                    { type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o"></i>' },
                    { type: "JQ", name: "fa-lock", element: '<i class="fa fa-lock"></i>' },
                    { type: "Text", name: "WhName" },
                    { type: "Text", name: "CkNo" },

                    { type: "Text", name: "LayerSno" },
                    { type: "Text", name: "OldName" },
                    { type: "Text", name: "NewName" },
                    { type: "Text", name: "Num" },
                    { type: "Text", name: "DisplayNum" },

                    { type: "Text", name: "ExchangeDate" },
                    { type: "Text", name: "Man_Name" },
                    { type: "Text", name: "DocDate" },
                    { type: "JQ", name: "fa-certificate", element: '<i class="fa fa-certificate"></i>' },
                    { type: "Text", name: "AppStatus" },

                    { type: "Text", name: "FinStatus" }

                ],
                rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                //sortable: "Y"
            }
        );


        let csgOption = {
            InputElementsID: "WhNo",
            ApiForGridData: "api/SetCommSelectGridDefaultApi",
            PostDataForApi: {
                Table: "WarehouseSV",
                Column: ["ST_ID", "ST_Sname"],
                Caption: ["店代號", "店名稱"],
                OrderColumn: "ST_ID",
                Condition: "1=1",
            },
            AfterSelectData: AfterSelWhNo
        }
        SetCommSelectGrid(csgOption);


        let csgOption2 = {
            InputElementsID: "NewPLU",
            ApiForGridData: "api/SetCommSelectGridDefaultApi",
            PostDataForApi: {
                Table: "PLUSV",
                Column: ["GD_No", "GD_Sname"],
                Caption: ["品號", "品名"],
                OrderColumn: "GD_No",
                Condition: "1=1",
            },
            AfterSelectData: AfterSelPLU
        }
        SetCommSelectGrid(csgOption2);

    };

    let AfterSelWhNo = function (xml) {
        $('#WhNo').val(GetNodeValue(xml, "ST_ID"));
        $('#WhName').text(GetNodeValue(xml, "ST_Sname"));
        GetModalWhDSVCkNo();
    }


    let AfterSelPLU = function (xml) {
        $('#NewPLU').val(GetNodeValue(xml, "GD_No"));
        $('#NewPLUName').text(GetNodeValue(xml, "GD_Sname"));
    }


    let InitModifyDeleteButton = function () {
        $('#tbVIN13_2 .fa-file-text-o').click(function () { btMod_Click(this) });
        $('#tbVIN13_2 .fa-trash-o').click(function () { btDel_Click(this) });
        $('#tbVIN13_2 .fa-lock').click(function () { btDef_Click(this) });
        $('#tbVIN13_2 .fa-certificate').click(function () { btApp_Click(this) });
     }

 

    let btAdd_click = function () {
        EditMode = "Add";
        //alert(EditMode);
        //$(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VIN13_2 .modal-title').text('智販機換貨新增');
        //var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#WhNo,#CkNo,#NewPLU,#Num,#DisplayNum,#ExchangeDate').prop('readonly', false);
        //gDocNo = GetNodeValue(node, 'DocNo');

        $('#WhNo').val('');
        $('#WhNo').closest('.col-3').show();
        $('#lblWhNo').text('');
        $('#lblWhNo').closest('.col-3').hide();
        $('#WhName').text('');

        $('#CkNo').closest('.col-2').show();
        $('#CkNo').val('');
        $('#lblCkNo').text('');
        $('#lblCkNo').closest('.col-3').hide();

        $('#Layer').val('');
        $('#Layer').closest('.col-2').show();
        $('#lblLayer').text('');
        $('#lblLayer').closest('.col-3').hide();

        $('#Sno').val('');
        $('#Sno').closest('.col-2').show();
        $('#lblSno').text('');
        $('#lblSno').closest('.col-3').hide();

        //$('#OldPLU').closest('.col-5').show();
        $('#OldPLU').text('');
        $('#lblOldPLU').text('');
        $('#lblOldPLU').closest('.col-3').hide();

        $('#NewPLU').closest('.col-4').show();
        $('#NewPLU').val('');
        $('#lblNewPLU').closest('.col-2').hide();
        $('#lblNewPLU').text('');
        $('#NewPLUName').text('');

        $('#Num').closest('.col-3').show();
        $('#Num').val('');
        $('#lblNum').closest('.col-2').hide();
        $('#lblNum').text('');

        $('#DisplayNum').closest('.col-3').show();
        $('#DisplayNum').val('');
        $('#lblDisplayNum').closest('.col-2').hide();
        $('#lblDisplayNum').text('');

        $('#ExchangeDate').val('');
        $('#ExchangeDate').closest('.col-4').show();
        $('#lblExDate').closest('.col-3').hide();
        GetSysDate();

        //ModalCkNo = GetNodeValue(node, 'CkNo');

        //GetWhOutCkNo("Out");

        $('#modal_VIN13_2').modal('show');
    };



    let btMod_Click = function (bt) {
        EditMode = "Mod";
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VIN13_2 .modal-title').text('智販機換貨修改');
        var node = $(grdU.ActiveRowTR()).prop('Record');

        if (GetNodeValue(node, 'FinishDate') != '') {
            DyAlert("此單據已完成，無法修改!");
            return
        }
        if (GetNodeValue(node, 'DefeasanceDate') != '') {
            DyAlert("此單據已作廢，無法修改!");
            return
        }
        if (GetNodeValue(node, 'AppDate') != '') {
            DyAlert("此單據已批核，無法修改!");
            return
        }

        $('#WhNo,#CkNo,#NewPLU,#Num,#DisplayNum,#ExchangeDate').prop('readonly', false);
        gDocNo = GetNodeValue(node, 'DocNo');

        $('#WhNo').val(GetNodeValue(node, 'WhNo'));
        $('#WhNo').closest('.col-3').hide();
        $('#lblWhNo').text(GetNodeValue(node, 'WhNo'));
        $('#lblWhNo').closest('.col-3').show();
        $('#WhName').text(GetNodeValue(node, 'WhName'));

        $('#CkNo').closest('.col-2').hide();
        $('#CkNo').val(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').text(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').closest('.col-3').show();

        $('#Layer').closest('.col-2').hide();
        $('#lblLayer').text(GetNodeValue(node, 'Layer'));
        $('#lblLayer').closest('.col-3').show();

        $('#Sno').closest('.col-2').hide();
        $('#lblSno').text(GetNodeValue(node, 'Sno'));
        $('#lblSno').closest('.col-3').show();

        $('#OldPLU').text(GetNodeValue(node, 'PLUOld') + ' ' + GetNodeValue(node, 'OldName'));
        $('#lblOldPLU').text(GetNodeValue(node, 'PLUOld'));
        $('#lblOldPLU').closest('.col-3').hide();

        $('#NewPLU').closest('.col-4').show();
        $('#NewPLU').val(GetNodeValue(node, 'PLUNew'));
        $('#lblNewPLU').closest('.col-2').hide();
        $('#lblNewPLU').text(GetNodeValue(node, 'PLUNew'));
        $('#NewPLUName').text(GetNodeValue(node, 'NewName'));

        $('#Num').closest('.col-3').show();
        $('#Num').val(GetNodeValue(node, 'Num'));
        $('#lblNum').closest('.col-2').hide();
        $('#lblNum').text(GetNodeValue(node, 'Num'));

        $('#DisplayNum').closest('.col-3').show();
        $('#DisplayNum').val(GetNodeValue(node, 'DisplayNum'));
        $('#lblDisplayNum').closest('.col-2').hide();
        $('#lblDisplayNum').text(GetNodeValue(node, 'DisplayNum'));

        $('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
        $('#ExchangeDate').closest('.col-4').show();
        $('#lblExDate').text(GetNodeValue(node, 'ExchangeDate'));
        $('#lblExDate').closest('.col-3').hide();
        GetSysDate();

        //ModalCkNo = GetNodeValue(node, 'CkNo');

        //GetWhOutCkNo("Out");

        $('#modal_VIN13_2').modal('show');
    };

 

    let btDel_Click = function (bt) {
        EditMode = "Del";
        $(bt).closest('tr').click();
        //alert(GetNodeValue(node, 'AppDate'));

        $('.msg-valid').hide();
        $('#modal_VIN13_2 .modal-title').text('智販機換貨刪除');
        var node = $(grdU.ActiveRowTR()).prop('Record');

        if (GetNodeValue(node, 'FinishDate') != '') {
            DyAlert("此單據已完成，不可刪除!");
            return
        }
        if (GetNodeValue(node, 'DefeasanceDate') != '') {
            DyAlert("此單據已作廢，不可刪除!");
            return
        }
        if (GetNodeValue(node, 'AppDate') != '' ) {
            DyAlert("此單據已批核，不可刪除!");
            return
        }


        $('#WhNo,#CkNo,#NewPLU,#Num,#DisplayNum,#ExchangeDate').prop('readonly', false);
        gDocNo = GetNodeValue(node, 'DocNo');

        $('#WhNo').val(GetNodeValue(node, 'WhNo'));
        $('#WhNo').closest('.col-3').hide();
        $('#lblWhNo').text(GetNodeValue(node, 'WhNo'));
        $('#lblWhNo').closest('.col-3').show();
        $('#WhName').text(GetNodeValue(node, 'WhName'));

        $('#CkNo').closest('.col-2').hide();
        $('#CkNo').val(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').text(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').closest('.col-3').show();

        $('#Layer').closest('.col-2').hide();
        $('#lblLayer').text(GetNodeValue(node, 'Layer'));
        $('#lblLayer').closest('.col-3').show();

        $('#Sno').closest('.col-2').hide();
        $('#lblSno').text(GetNodeValue(node, 'Sno'));
        $('#lblSno').closest('.col-3').show();

        $('#OldPLU').text(GetNodeValue(node, 'PLUOld') + ' ' + GetNodeValue(node, 'OldName'));
        $('#lblOldPLU').text(GetNodeValue(node, 'PLUOld'));
        $('#lblOldPLU').closest('.col-3').hide();

        $('#NewPLU').closest('.col-4').hide();
        $('#NewPLU').val(GetNodeValue(node, 'PLUNew'));
        $('#lblNewPLU').closest('.col-2').show();
        $('#lblNewPLU').text(GetNodeValue(node, 'PLUNew'));
        $('#NewPLUName').text(GetNodeValue(node, 'NewName'));

        $('#Num').closest('.col-3').hide();
        $('#Num').val(GetNodeValue(node, 'Num'));
        $('#lblNum').closest('.col-2').show();
        $('#lblNum').text(GetNodeValue(node, 'Num'));

        $('#DisplayNum').closest('.col-3').hide();
        $('#DisplayNum').val(GetNodeValue(node, 'DisplayNum'));
        $('#lblDisplayNum').closest('.col-2').show();
        $('#lblDisplayNum').text(GetNodeValue(node, 'DisplayNum'));

        $('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
        $('#ExchangeDate').closest('.col-4').hide();
        $('#lblExDate').text(GetNodeValue(node, 'ExchangeDate'));
        $('#lblExDate').closest('.col-3').show();
        //GetSysDate();

        $('#modal_VIN13_2').modal('show');
    };


    let btDef_Click = function (bt) {
        EditMode = "Def";
        $(bt).closest('tr').click();
        //alert(GetNodeValue(node, 'AppDate'));

        $('.msg-valid').hide();
        $('#modal_VIN13_2 .modal-title').text('智販機換貨作廢');
        var node = $(grdU.ActiveRowTR()).prop('Record');
        if (GetNodeValue(node, 'AppDate') == '') {
            DyAlert("此單據未批核，不可作廢!");
            return
        }
        if (GetNodeValue(node, 'DefeasanceDate') != '') {
            DyAlert("此單據已作廢!");
            return
        }
        if (GetNodeValue(node, 'FinishDate') != '') {
            DyAlert("此單據已完成，不可作廢!");
            return
        }
        $('#WhNo,#CkNo,#NewPLU,#Num,#DisplayNum,#ExchangeDate').prop('readonly', false);
        gDocNo = GetNodeValue(node, 'DocNo');

        $('#WhNo').val(GetNodeValue(node, 'WhNo'));
        $('#WhNo').closest('.col-3').hide();
        $('#lblWhNo').text(GetNodeValue(node, 'WhNo'));
        $('#lblWhNo').closest('.col-3').show();
        $('#WhName').text(GetNodeValue(node, 'WhName'));

        $('#CkNo').closest('.col-2').hide();
        $('#CkNo').val(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').text(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').closest('.col-3').show();

        $('#Layer').closest('.col-2').hide();
        $('#lblLayer').text(GetNodeValue(node, 'Layer'));
        $('#lblLayer').closest('.col-3').show();

        $('#Sno').closest('.col-2').hide();
        $('#lblSno').text(GetNodeValue(node, 'Sno'));
        $('#lblSno').closest('.col-3').show();

        $('#OldPLU').text(GetNodeValue(node, 'PLUOld') + ' ' + GetNodeValue(node, 'OldName'));
        $('#lblOldPLU').text(GetNodeValue(node, 'PLUOld'));
        $('#lblOldPLU').closest('.col-3').hide();

        $('#NewPLU').closest('.col-4').hide();
        $('#NewPLU').val(GetNodeValue(node, 'PLUNew'));
        $('#lblNewPLU').closest('.col-2').show();
        $('#lblNewPLU').text(GetNodeValue(node, 'PLUNew'));
        $('#NewPLUName').text(GetNodeValue(node, 'NewName'));

        $('#Num').closest('.col-3').hide();
        $('#Num').val(GetNodeValue(node, 'Num'));
        $('#lblNum').closest('.col-2').show();
        $('#lblNum').text(GetNodeValue(node, 'Num'));

        $('#DisplayNum').closest('.col-3').hide();
        $('#DisplayNum').val(GetNodeValue(node, 'DisplayNum'));
        $('#lblDisplayNum').closest('.col-2').show();
        $('#lblDisplayNum').text(GetNodeValue(node, 'DisplayNum'));

        $('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
        $('#ExchangeDate').closest('.col-4').hide();
        $('#lblExDate').text(GetNodeValue(node, 'ExchangeDate'));
        $('#lblExDate').closest('.col-3').show();
        //GetSysDate();

        $('#modal_VIN13_2').modal('show');
    };



    let btApp_Click = function (bt) {
        
        EditMode = "App";
        //alert(EditMode);
        $(bt).closest('tr').click();

        $('.msg-valid').hide();
        $('#modal_VIN13_2 .modal-title').text('智販機換貨批核');
        var node = $(grdU.ActiveRowTR()).prop('Record');
        //alert(GetNodeValue(node, 'AppDate'));
        if (GetNodeValue(node, 'AppDate') != '' ) {
            DyAlert("此單據已批核!");
            return
        }

        $('#WhNo,#CkNo,#NewPLU,#Num,#DisplayNum,#ExchangeDate').prop('readonly', false);
        gDocNo = GetNodeValue(node, 'DocNo');

        $('#WhNo').val(GetNodeValue(node, 'WhNo'));
        $('#WhNo').closest('.col-3').hide();
        $('#lblWhNo').text(GetNodeValue(node, 'WhNo'));
        $('#lblWhNo').closest('.col-3').show();
        $('#WhName').text(GetNodeValue(node, 'WhName'));

        $('#CkNo').closest('.col-2').hide();
        $('#CkNo').val(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').text(GetNodeValue(node, 'CkNo'));
        $('#lblCkNo').closest('.col-3').show();

        $('#Layer').closest('.col-2').hide();
        $('#lblLayer').text(GetNodeValue(node, 'Layer'));
        $('#lblLayer').closest('.col-3').show();

        $('#Sno').closest('.col-2').hide();
        $('#lblSno').text(GetNodeValue(node, 'Sno'));
        $('#lblSno').closest('.col-3').show();

        $('#OldPLU').text(GetNodeValue(node, 'PLUOld') + ' ' + GetNodeValue(node, 'OldName'));
        $('#lblOldPLU').text(GetNodeValue(node, 'PLUOld'));
        $('#lblOldPLU').closest('.col-3').hide();

        $('#NewPLU').closest('.col-4').hide();
        $('#NewPLU').val(GetNodeValue(node, 'PLUNew'));
        $('#lblNewPLU').closest('.col-2').show();
        $('#lblNewPLU').text(GetNodeValue(node, 'PLUNew'));
        $('#NewPLUName').text(GetNodeValue(node, 'NewName'));

        $('#Num').closest('.col-3').hide();
        $('#Num').val(GetNodeValue(node, 'Num'));
        $('#lblNum').closest('.col-2').show();
        $('#lblNum').text(GetNodeValue(node, 'Num'));

        $('#DisplayNum').closest('.col-3').hide();
        $('#DisplayNum').val(GetNodeValue(node, 'DisplayNum'));
        $('#lblDisplayNum').closest('.col-2').show();
        $('#lblDisplayNum').text(GetNodeValue(node, 'DisplayNum'));

        $('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
        $('#ExchangeDate').closest('.col-4').hide();
        $('#lblExDate').text(GetNodeValue(node, 'ExchangeDate'));
        $('#lblExDate').closest('.col-3').show();
        //GetSysDate();

        $('#modal_VIN13_2').modal('show');
    };


    let SearchVIN13_2 = function () {

        console.log("SearchVIN13_2");

        var pData = {
            WhNo: $('#cbWh').val(),
            CkNo: $('#cbCK').val(),
            Layer: $('#cbLayer').val(),
            exDate: $('#exDate').val()
        };
        PostToWebApi({ url: "api/SystemSetup/SearchVIN13_2", data: pData, success: AfterSearchVIN13_2 });
    };

    let click_PLU = function (tr) {

    };

    let AfterSearchVIN13_2 = function (data) {
        if (ReturnMsg(data, 0) != "SearchVIN13_2OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtInv = data.getElementsByTagName('dtInv');
            //alert(dtInv.length);
            grdU.BindData(dtInv);
            if (dtInv.length == 0) {
                DyAlert("無符合資料!", BlankMode);
                return;
            }
        }
    };


    let BlankMode = function () {
        
    };


    let btCancel_click = function () {
        $('#modal_VIN13_2').modal('hide');
    };

 
    //2021-05-07
    let cbCK_click = function () {

        if ($('#cbWh').val() == "") {
            $('#cbCK').val() == ""
            DyAlert("請先選擇店查詢條件!!", function () { $('#cbWh').focus() });
            //DyAlert("請先選擇店查詢條件!!");
            return;
        }
    };


    let cbLayer_click = function () {

        if ($('#cbCK').val() == "") {
            $('#cbLayer').val() == ""
            DyAlert("請先選擇機號查詢條件!!", function () { $('#cbCK').focus() });
            //DyAlert("請先選擇店查詢條件!!");
            return;
        }
    };


    let GetLayerNo = function () {

        if ($('#cbCK').val() == "") {
            $('#cbLayer').empty();
            return;
        }
        else {

        }

        var pData = {
            WhNo: $('#cbWh').val(),
            CkNo: $('#cbCK').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhCkLayer", data: pData, success: AfterGetLayerNo });
    };


    let AfterGetLayerNo = function (data) {
        //alert("AfterGetLayerNo");
        if (ReturnMsg(data, 0) != "GetWhCkLayerOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#cbLayer')[0], dtCK, "LayerNo", "LayerNo", true);
        }
    };


    let GetWhDSVCkNo = function () {

        console.log("GetWhDSVCkNo");

        if ($('#cbWh').val() == "") {
            $('#cbCK').empty();
            return;
        }
        else {

        }

        var pData = {
            WhNo: $('#cbWh').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNo", data: pData, success: AfterGetWhDSVCkNo });
    };


    let AfterGetWhDSVCkNo = function (data) {
        //alert("AfterGetWhDSVCkNo");
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#cbCK')[0], dtCK, "CKNo", "CKNo", true);
        }
    };


    let GetModalWhDSVCkNo = function () {
        //alert("GetModalWhDSVCkNo");
        if ($('#WhNo').val() == "") {
            $('#CkNo').empty();
            return;
        }

        var pData = {
            WhNo: $('#WhNo').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNo", data: pData, success: AfterGetModalWhDSVCkNo });
    };


    let AfterGetModalWhDSVCkNo = function (data) {
        //alert("AfterGetModalWhDSVCkNo");
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#CkNo')[0], dtCK, "CKNo", "CKNo", true);
        }
    };


    let GetModalLayer = function () {
        //alert("GetModalWhDSVCkNo");
        if ($('#CkNo').val() == "") {
            $('#Layer').empty();
            return;
        }

        var pData = {
            WhNo: $('#WhNo').val(),
            CkNo: $('#CkNo').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhCkLayer", data: pData, success: AfterGetModalLayer });
    };


    let AfterGetModalLayer = function (data) {
        
        if (ReturnMsg(data, 0) != "GetWhCkLayerOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            //alert("AfterGetModalLayer");
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#Layer')[0], dtCK, "LayerNo", "LayerNo", true);
        }
    };



    let GetModalSno = function () {

        if ($('#Layer').val() == "") {
            $('#Sno').empty();
            return;
        }
        else {

        }

        var pData = {
            WhNo: $('#WhNo').val(),
            CkNo: $('#CkNo').val(),
            LayerNo: $('#Layer').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhCkLayerSno", data: pData, success: AfterGetModalSno });
    };


    let AfterGetModalSno = function (data) {
        //alert("AfterGetModalSno");
        if (ReturnMsg(data, 0) != "GetWhCkLayerSnoOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#Sno')[0], dtCK, "ChannelNo", "ChannelNo", true);
        }
    };


    let GetModalPLU = function () {

        var pData = {
            WhNo: $('#WhNo').val(),
            CkNo: $('#CkNo').val(),
            LayerNo: $('#Layer').val(),
            Sno: $('#Sno').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhCkLayerSnoPLU", data: pData, success: AfterGetModalPLU });
    };


    let AfterGetModalPLU = function (data) {
        //alert("AfterGetModalPLU");
        if (ReturnMsg(data, 0) != "GetWhCkLayerSnoPLUOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            //alert(GetNodeValue(dtCK[0], "PLU"));
            //alert(GetNodeValue(dtCK[0], "GD_SName"));
            $('#OldPLU').text(GetNodeValue(dtCK[0], "PLU") + ' ' + GetNodeValue(dtCK[0], "GD_SName"));
            $('#lblOldPLU').text(GetNodeValue(dtCK[0], "PLU"));
            //OldPLU = GetNodeValue(dtSysDate[0], "SysDate");
            //InitSelectItem($('#Sno')[0], dtCK, "ChannelNo", "ChannelNo", true);
        }
    };


    let GetSysDate = function () {

        var cData = {
            DocNo: $('#DocNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetSysDate", data: cData, success: AfterGetSysDate });
   
        //return;
    };


    let AfterGetSysDate = function (data) {
        //alert("AfterGetSysDate");
        if (ReturnMsg(data, 0) != "GetSysDateOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtSysDate = data.getElementsByTagName('dtSysDate');
            
            if (dtSysDate.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                //return;
            }
            else {
                SysDate = GetNodeValue(dtSysDate[0], "SysDate");
            }

        }
    };



    let SaveData = function () {
        //alert("EditMode:" + EditMode);
        if (EditMode == "Add") {
            var pData = {
                ChangePLUSV: [
                    {
                       WhNo: $('#WhNo').val(),
                        CkNo: $('#CkNo').val(),
                        Layer: $('#Layer').val(),
                        Sno: $('#Sno').val(),
                        OldPLU: $('#lblOldPLU').text(),
                        NewPLU: $('#NewPLU').val(),
                        Num: $('#Num').val(),
                        DisplayNum: $('#DisplayNum').val(),
                        ExchangeDate: $('#ExchangeDate').val()
                    }
                ]
 
            }
            //alert("Add.." + $('#Type_ID').val());
            PostToWebApi({ url: "api/SystemSetup/AddChgPLU", data: pData, success: AfterAddChgPLU });
        }
        else if (EditMode == "Mod") {
            //alert($('#NewPLU').val());
            var mData = {
                ChangePLUSV: [
                    {
                        DocNo: gDocNo,
                        PLUNew: $('#NewPLU').val(),
                        Num: $('#Num').val(),
                        DisplayNum: $('#DisplayNum').val(),
                        ExchangeDate: $('#ExchangeDate').val()
                    }
                ]
            }
  
            PostToWebApi({ url: "api/SystemSetup/UpdateChgPLU", data: mData, success: AfterUpdateChgPLU });
        }
        else if (EditMode == "App") {

            var cData = {
                ChangePLUSV: [
                    {
                        DocNo: gDocNo
                    }
                ]
            }
            //alert("Del " + $('#Type_ID').val());
            PostToWebApi({ url: "api/SystemSetup/AppChgPLU", data: cData, success: AfterUpdateChgPLU });
        }
        else if (EditMode == "Del") {
            var cData = {
                ChangePLUSV: [
                    {
                        DocNo: gDocNo
                    }
                ]
            }
            PostToWebApi({ url: "api/SystemSetup/DelChgPLU", data: cData, success: AfterDelChgPLU });
        }
        else if (EditMode == "Def") {
            var cData = {
                ChangePLUSV: [
                    {
                        DocNo: gDocNo
                    }
                ]
            }
            PostToWebApi({ url: "api/SystemSetup/DefChgPLU", data: cData, success: AfterUpdateChgPLU });
        }


    }




    let btSave_click = function () {

        //alert("SysDate " + SysDate);
        
        //if ($('#WhNo').val() == "" | $('#WhNo').val() == null | $('#CkNo').val() == "" | $('#CkNo').val() == null | $('#WhNoIn').val() == "" | $('#WhNoIn').val() == null | $('#CkNoIn').val() == "" | $('#CkNoIn').val() == null | $('#ExchangeDate').val() == "" | $('#ExchangeDate').val() == null) {
        //    DyAlert("所有欄位都必須輸入資料!!", function () { $('#WhNo').focus() });
        //    //DyAlert("所有欄位都必須輸入資料!!", setFocus);
        //    return;
        //}
        if ($('#WhNo').val() == "" | $('#WhNo').val() == null) {
            DyAlert("智販店代號欄位必須輸入資料!!", function () { $('#WhNo').focus() });
            return;
        }

        if (EditMode == "Add") {
            if ($('#CkNo').val() == "" | $('#CkNo').val() == null) {
                DyAlert("機號欄位必須輸入資料!!", function () { $('#CkNo').focus() });
                return;
            }
            if ($('#Layer').val() == "" | $('#Layer').val() == null) {
                DyAlert("貨倉欄位必須輸入資料!!", function () { $('#Layer').focus() });
                return;
            }
            if ($('#Sno').val() == "" | $('#Sno').val() == null) {
                DyAlert("貨道欄位必須輸入資料!!", function () { $('#Sno').focus() });
                return;
            }
        }


 
        if ($('#NewPLU').val() == "" | $('#NewPLU').val() == null) {
            DyAlert("新品號欄位必須輸入資料!!", function () { $('#NewPLU').focus() });
            return;
        }
        if ($('#Num').val() == "" | $('#Num').val() == null) {
            DyAlert("新補貨量欄位必須輸入資料!!", function () { $('#Num').focus() });
            return;
        }
        if ($('#DisplayNum').val() == "" | $('#DisplayNum').val() == null) {
            DyAlert("新滿倉量欄位必須輸入資料!!", function () { $('#DisplayNum').focus() });
            return;
        }
        if ($('#ExchangeDate').val() == "" | $('#ExchangeDate').val() == null) {
            DyAlert("換貨日期欄位必須輸入資料!!", function () { $('#ExchangeDate').focus() });
            return;
        }

        //檢查新舊品號是否相同
        if ($('#NewPLU').val() == $('#lblOldPLU').text()) {
            DyAlert("新舊品號相同!!", function () { $('#NewPLU').focus() });
            return;
        }

        //檢查換貨日期必須大於系統日
       if ($('#ExchangeDate').val() < SysDate ) {
           DyAlert("換貨日期必須大於等於系統日期!!", function () { $('#ExchangeDate').focus() });
            return;
        }



        //alert("EditMode:" + EditMode);
        if (EditMode == "App") {
            DyConfirm("確定要批核這筆資料嗎?", SaveData, DummyFunction);
        }
        else if (EditMode == "Def") {
            DyConfirm("確定要作廢這筆資料嗎?", SaveData, DummyFunction);
        }
        else if (EditMode == "Del") {
            DyConfirm("確定要刪除這筆資料嗎?", SaveData, DummyFunction);
        }
        else {
            //alert("EditMode:" + EditMode);
            SaveData();
        }

    };


    let AfterAddChgPLU = function (data) {
        if (ReturnMsg(data, 0) != "AddChgPLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("新增完成!");

            $('#modal_VIN13_2').modal('hide');
            var userxml = data.getElementsByTagName('dtChgPLU')[0];
            grdU.AddNew(userxml);
        }
    };


    let AfterUpdateChgPLU = function (data) {
        //alert("AfterUpdateChgPLU:" + EditMode);
        if (ReturnMsg(data, 0) != "UpdateChgPLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            if (EditMode == "App") {
                DyAlert("批核完成!");
            }
            else if (EditMode == "Def") {
                DyAlert("作廢完成!");
            }
            else {
                DyAlert("儲存完成!");
            }

            $('#modal_VIN13_2').modal('hide');
            var userxml = data.getElementsByTagName('dtRes')[0];
            grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
        }
    };


    let AfterDelChgPLU = function (data) {
        if (ReturnMsg(data, 0) != "DelChgPLUOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_VIN13_2').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdU.DeleteRow(grdU.ActiveRowTR());
        }
    };


 

    //2021-05-18
    let afterGetInitVIN13_2 = function (data) {

        //alert("afterGetInitVIN13_2");
        AssignVar();
        $('#btQuery').click(function () { SearchVIN13_2(); });

        var dtWh = data.getElementsByTagName('dtWh');
        InitSelectItem($('#cbWh')[0], dtWh, "ST_ID", "STName", true);

        $('#cbWh').change(function () { GetWhDSVCkNo(); });
        $('#cbCK').click(function () { cbCK_click(); });
        $('#cbCK').change(function () { GetLayerNo(); });
        $('#cbLayer').click(function () { cbLayer_click(); });
        //$('#cbLayer').change(function () { GetLayerNo(); });

        $('#WhNo').change(function () { GetModalWhDSVCkNo(); });
        $('#CkNo').click(function () { CkNo_click(); });        
        $('#CkNo').change(function () { GetModalLayer(); });
        $('#Layer').click(function () { Layer_click(); }); 
        $('#Layer').change(function () { GetModalSno(); });
        $('#Sno').click(function () { Sno_click(); }); 
        $('#Sno').change(function () { GetModalPLU(); });

        SetDateField($('#exDate')[0]);
        $('#exDate').datepicker();

        $('#btAdd').click(function () { btAdd_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        $('.forminput input').change(function () { InputValidation(this) });

        SetDateField($('#ExchangeDate')[0]);
        $('#ExchangeDate').datepicker();

        //SetPLUAutoComplete("GD_NAME");
        //SetPLUAutoComplete("GD_NO");
    };


    let CkNo_click = function () {

        if ($('#WhNo').val() == "") {
            $('#CkNo').val() == ""
            DyAlert("請先選擇智販店資料!!", function () { $('#WhNo').focus() });
            //DyAlert("請先選擇智販店資料!!");
            return;
        }
    };


    let Layer_click = function () {

        if ($('#CkNo').val() == "") {
            $('#Layer').val() == ""
            DyAlert("請先選擇店查詢條件!!", function () { $('#CkNo').focus() });
            //DyAlert("請先選擇機號資料!!");
            return;
        }
    };


    let Sno_click = function () {

        if ($('#Layer').val() == "") {
            $('#Sno').val() == ""
            DyAlert("請先選擇貨倉資料!!", function () { $('#Layer').focus() });
            //DyAlert("請先選擇貨倉資料!!");
            return;
        }
    };


    let InputValidation = function (ip) {
        var str = $(ip).val();
        var msg = "";
        //$('.forminput .msg-valid').text('');
        //$('.forminput .msg-valid').hide();
        $(ip).nextAll('.msg-valid').text(msg);
        $(ip).nextAll('.msg-valid').show();
        if (str == "")
            return;
        if ($(ip).attr('id') == "WhNo") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length < 5 | str.length > 10)
                msg = "必須5~10碼英數字";
        }
        if ($(ip).attr('id') == "Num") {
            var re = /^[\d]+$/;
            if (!re.test(str) | str.length > 2 )
                msg = "必須為個位或十位數字";
        }
        if ($(ip).attr('id') == "DisplayNum") {
            var re = /^[\d]+$/;
            if (!re.test(str) | str.length > 2)
                msg = "必須為個位或十位數字";
        }
        if ($(ip).attr('id') == "USR_PWD") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length < 6 | str.length > 20)
                msg = "必須6~20碼英數字";
        }
        if ($(ip).attr('id') == "USR_NAME_L") {
            if (str.length > 10)
                msg = "必須10字元以內";
        }
        if ($(ip).attr('id') == "USR_EMPNO") {
            if (str.length > 10)
                msg = "必須10字元以內";
        }
        if ($(ip).attr('id') == "USR_MAIL") {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            if (!re.test(str))
                msg = "e-mail格式錯誤!";
        }
        if ($(ip).attr('id') == "USR_MOBILE") {
            var re = /^09\d{8}$/;
            if (!re.test(str))
                msg = "手機格式錯誤!";
        }
        if ($(ip).attr('id') == "USR_NOTE") {
            if (str.length > 50)
                msg = "必須50字元以內";
        }
        if (msg != "") {
            //$(ip).val('');
            $(ip).nextAll('.msg-valid').text(msg);
            $(ip).nextAll('.msg-valid').show();
        }
    }

    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetWh", success: afterGetInitVIN13_2 });
        $('#pgVIN13_2').show();
    };

    if ($('#pgVIN13_2').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "VIN13_2", ["pgVIN13_2"], afterLoadPage);
    };


}