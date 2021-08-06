var PageVIN13_1 = function (ParentNode) {
    let AllPages;
    let grdU;
    let EditMode;
    let SetSuspend = "";
    let SysDate ;
    let gDocNo = "";
    let CkType = "";
    let OutCkNo = "";
    let InCkNo = "";

    let AssignVar = function () {

        grdU = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVIN13_1')[0],
                class_collection: ["tdColbt icon_in_td btEdit", "tdColbt icon_in_td btDel", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdCol8", "tdColbt icon_in_td btT", "tdCol10", "tdCol11", "tdCol12"],
                //class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td btsuspend", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7",  "tdCol8"],
                fields_info: [
                    { type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o"></i>' },
                    { type: "Text", name: "WhOut" },
                    { type: "Text", name: "CkNoOut" },
                    { type: "Text", name: "WhIn" },

                    { type: "Text", name: "CkNoIn" },
                    { type: "Text", name: "ExchangeDate" },
                    { type: "Text", name: "Man_Name" },
                    { type: "Text", name: "DocDate" },
                    { type: "JQ", name: "fa-certificate", element: '<i class="fa fa-certificate"></i>' },

                    { type: "Text", name: "AppStatus" },
                    { type: "Text", name: "FinStatus" }

                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );

        let csgOption = {
            InputElementsID: "WhNoOut",
            ApiForGridData: "api/SetCommSelectGridDefaultApi",
            PostDataForApi: {
                Table: "WarehouseSV",
                Column: ["ST_ID", "ST_Sname"],
                Caption: ["店代號", "店名稱"],
                OrderColumn: "ST_ID",
                Condition: "1=1",
            },
            AfterSelectData: AfterSelWhNoOut
        }
        SetCommSelectGrid(csgOption);


        let csgOption2 = {
            InputElementsID: "WhNoIn",
            ApiForGridData: "api/SetCommSelectGridDefaultApi",
            PostDataForApi: {
                Table: "WarehouseSV",
                Column: ["ST_ID", "ST_Sname"],
                Caption: ["店代號", "店名稱"],
                OrderColumn: "ST_ID",
                Condition: "1=1",
            },
            AfterSelectData: AfterSelWhNoIn
        }
        SetCommSelectGrid(csgOption2);


    };

    let AfterSelWhNoOut = function (xml) {
        $('#WhNoOut').val(GetNodeValue(xml, "ST_ID"));
        $('#WhOutName').text(GetNodeValue(xml, "ST_Sname"));
        GetModalWhDSVCkNoOut();
    }


    let AfterSelWhNoIn = function (xml) {
        $('#WhNoIn').val(GetNodeValue(xml, "ST_ID"));
        $('#WhInName').text(GetNodeValue(xml, "ST_Sname"));
        GetModalWhDSVCkNoIn();
    }


    let GetModalWhDSVCkNoOut = function () {
        //alert("GetModalWhDSVCkNo");
        if ($('#WhNoOut').val() == "") {
            $('#CkNoOut').empty();
            return;
        }

        var pData = {
            WhNo: $('#WhNoOut').val(),
            StopDay: 'Y',
            CheckUse: 'Y'
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNoWithCond", data: pData, success: AfterGetModalWhDSVCkNoOut });
    };


    let AfterGetModalWhDSVCkNoOut = function (data) {
        //alert("AfterGetModalWhDSVCkNo");
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoWithCondOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#CkNoOut')[0], dtCK, "CKNo", "CKNo", true);
        }
    };



    let GetModalWhDSVCkNoIn = function () {
        //alert("GetModalWhDSVCkNo");
        if ($('#WhNoIn').val() == "") {
            $('#CkNoIn').empty();
            return;
        }

        var pData = {
            WhNo: $('#WhNoIn').val(),
            StopDay: 'Y',
            CheckUse: 'Y'
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNoWithCond", data: pData, success: AfterGetModalWhDSVCkNoIn });
    };


    let AfterGetModalWhDSVCkNoIn = function (data) {
        //alert("AfterGetModalWhDSVCkNo");
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoWithCondOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#CkNoIn')[0], dtCK, "CKNo", "CKNo", true);
        }
    };


    let InitModifyDeleteButton = function () {
        $('#tbVIN13_1 .fa-file-text-o').click(function () { btMod_Click(this) });
        $('#tbVIN13_1 .fa-trash-o').click(function () { btDel_Click(this) });
        $('#tbVIN13_1 .fa-certificate').click(function () { btApp_Click(this) });
    }



    let btAdd_click = function () {
        EditMode = "Add";
        //$(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VIN13_1 .modal-title').text('智販機換店新增');
        $('#modal_VIN13_1 .btn-danger').text('儲存');

        //var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#WhNoOut,#CkNoOut,#WhNoIn,#CkNoIn,#ExchangeDate').prop('readonly', false);
        $('#WhNoOut,#CkNoOut,#WhNoIn,#CkNoIn,#ExchangeDate').prop('disabled', false);

        $('#WhNoOut').val('');
        $('#WhOutName').text('');
        $('#CkNoOut').val('');

        $('#WhNoIn').val('');
        $('#WhInName').text('');
        $('#CkNoIn').val('');

        $('#ExchangeDate').val('');
        $('#ExchangeDate').closest('.col-4').show();
        $('#lblExDate').closest('.col-3').hide();
        GetSysDate();
        $('#modal_VIN13_1').modal('show');
    };



    let btMod_Click = function (bt) {
        EditMode = "Mod";
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VIN13_1 .modal-title').text('智販機換店修改');
        $('#modal_VIN13_1 .btn-danger').text('儲存');
        var node = $(grdU.ActiveRowTR()).prop('Record');

        if (GetNodeValue(node, 'AppDate') != '') {
            DyAlert("此單據已批核，無法修改!");
            return
        }

        $('#WhNoOut,#CkNoOut,#WhNoIn,#CkNoIn,#ExchangeDate').prop('readonly', false);
        $('#WhNoOut,#CkNoOut,#WhNoIn,#CkNoIn,#ExchangeDate').prop('disabled', false);
        gDocNo = GetNodeValue(node, 'DocNo');
        

        $('#WhNoOut').val(GetNodeValue(node, 'WhNoOut'));
        $('#WhOutName').text(GetNodeValue(node, 'WhOutName'));
        //$('#CkNoOut').val(GetNodeValue(node, 'CkNoOut'));

        $('#WhNoIn').val(GetNodeValue(node, 'WhNoIn'));
        $('#WhInName').text(GetNodeValue(node, 'WhInName'));
        //$('#CkNoIn').val(GetNodeValue(node, 'CkNoIn'));

        $('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
        $('#ExchangeDate').closest('.col-4').show();
        $('#lblExDate').closest('.col-3').hide();
        GetSysDate();

        OutCkNo = GetNodeValue(node, 'CkNoOut');
        InCkNo = GetNodeValue(node, 'CkNoIn');

        GetWhOutCkNo("Out");

        ////alert($('#WhNoIn').val());
        //GetWhDSVCkNo("In");
        //$('#CkNoIn').val(GetNodeValue(node, 'CkNoIn'));
        ////$('#lblSTOpenDate').text(GetNodeValue(node, 'ST_OpenDay'));


        //$('#modal_VIN13_1').modal('show');
    };


    let GetWhOutCkNo = function (UpType) {

        CkType = "Out";
        
        var pData = {
            WhNo: $('#WhNoOut').val(),
            StopDay: 'Y',
            CheckUse: 'Y'
        };

        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNoWithCond", data: pData, success: AfterGetOutCkNo });

    };


    let AfterGetOutCkNo = function (data) {
        //alert("AfterGetOutCkNo");
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoWithCondOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#CkNoOut')[0], dtCK, "CKNo", "CKNo", true);
            GetWhInCkNo("In");
        }
    };


    let GetWhInCkNo = function (UpType) {

        CkType = "In";
        //alert("GetWhInCkNo：In");
        var pData = {
            WhNo: $('#WhNoIn').val(),
            StopDay: 'Y',
            CheckUse: 'Y'
        };

        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNoWithCond", data: pData, success: AfterGetInCkNo });

    };


    let AfterGetInCkNo = function (data) {
        //alert("AfterGetInCkNo");
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoWithCondOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#CkNoIn')[0], dtCK, "CKNo", "CKNo", true);

            //alert(OutCkNo);
            //alert(InCkNo);

            $('#CkNoOut').val(OutCkNo);
            $('#CkNoIn').val(InCkNo);
            $('#modal_VIN13_1').modal('show');
        }
    };


    let btDel_Click = function (bt) {
        EditMode = "Del";
        $(bt).closest('tr').click();
        //alert(GetNodeValue(node, 'AppDate'));
 

        $('.msg-valid').hide();
        $('#modal_VIN13_1 .modal-title').text('智販機換店刪除');
        $('#modal_VIN13_1 .btn-danger').text('刪除');
        var node = $(grdU.ActiveRowTR()).prop('Record');
       if (GetNodeValue(node, 'AppDate') != '' ) {
            DyAlert("此單據已批核，不可刪除!");
            return
        }
        $('#WhNoOut,#CkNoOut,#WhNoIn,#CkNoIn,#ExchangeDate').prop('readonly', true);
        $('#WhNoOut,#CkNoOut,#WhNoIn,#CkNoIn,#ExchangeDate').prop('disabled', true);
        gDocNo = GetNodeValue(node, 'DocNo');
        //alert(gDocNo);

        $('#WhNoOut').val(GetNodeValue(node, 'WhNoOut'));
        $('#WhOutName').text(GetNodeValue(node, 'WhOutName'));
        /*$('#WhOutName').val(GetNodeValue(node, 'WhOutName'));*/
        $('#CkNoOut').val(GetNodeValue(node, 'CkNoOut'));

        $('#WhNoIn').val(GetNodeValue(node, 'WhNoIn'));
        $('#WhInName').text(GetNodeValue(node, 'WhInName'));
        $('#CkNoIn').val(GetNodeValue(node, 'CkNoIn'));

        $('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
        $('#ExchangeDate').closest('.col-4').hide();
        $('#lblExDate').text(GetNodeValue(node, 'ExchangeDate'));
        $('#lblExDate').closest('.col-3').show();

        GetSysDate();

        OutCkNo = GetNodeValue(node, 'CkNoOut');
        InCkNo = GetNodeValue(node, 'CkNoIn');

        GetWhOutCkNo("Out");

        //$('#modal_VIN13_1').modal('show');
    };



    let btApp_Click = function (bt) {

        EditMode = "App";
        //alert(EditMode);
        $(bt).closest('tr').click();

        $('.msg-valid').hide();
        $('#modal_VIN13_1 .modal-title').text('智販機換店批核');
        $('#modal_VIN13_1 .btn-danger').text('儲存');
        var node = $(grdU.ActiveRowTR()).prop('Record');
        //alert(GetNodeValue(node, 'AppDate'));
        if (GetNodeValue(node, 'AppDate') != '' ) {
            DyAlert("此單據已批核!");
            return
        }

        $('#WhNoOut,#CkNoOut,#WhNoIn,#CkNoIn,#ExchangeDate').prop('readonly', true);
        $('#WhNoOut,#CkNoOut,#WhNoIn,#CkNoIn,#ExchangeDate').prop('disabled', true);
        gDocNo = GetNodeValue(node, 'DocNo');
        //alert(gDocNo);
        $('#WhNoOut').val(GetNodeValue(node, 'WhNoOut'));
        $('#WhOutName').text(GetNodeValue(node, 'WhOutName'));
        
        $('#CkNoOut').val(GetNodeValue(node, 'CkNoOut'));

        $('#WhNoIn').val(GetNodeValue(node, 'WhNoIn'));
        $('#WhInName').text(GetNodeValue(node, 'WhInName'));

        $('#CkNoIn').val(GetNodeValue(node, 'CkNoIn'));

        $('#ExchangeDate').val(GetNodeValue(node, 'ExchangeDate'));
        $('#ExchangeDate').closest('.col-4').hide();
        $('#lblExDate').text(GetNodeValue(node, 'ExchangeDate'));
        $('#lblExDate').closest('.col-3').show();

        GetSysDate();

        OutCkNo = GetNodeValue(node, 'CkNoOut');
        InCkNo = GetNodeValue(node, 'CkNoIn');

        GetWhOutCkNo("Out");


        //$('#modal_VIN13_1').modal('show');
    };


    let SearchVIN13_1 = function () {

        var pData = {
            WhNo: $('#cbWh').val(),
            CkNo: $('#cbCK').val(),
            exDate: $('#exDate').val()
        };
        PostToWebApi({ url: "api/SystemSetup/SearchVIN13_1", data: pData, success: AfterSearchVIN13_1 });
    };

    let click_PLU = function (tr) {

    };

    let AfterSearchVIN13_1 = function (data) {
        if (ReturnMsg(data, 0) != "SearchVIN13_1OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtInv = data.getElementsByTagName('dtInv');
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
        $('#modal_VIN13_1').modal('hide');
    };

 
    let cbCK_click = function () {

        if ($('#cbWh').val() == "") {
            $('#cbCK').val() == ""
            //DyAlert("請先選擇店查詢條件!!", function () { $('#cbWh').focus() });
            DyAlert("請先選擇店查詢條件!!");
            return;
        }
    };



    let GetWhDSVCkNo = function (UpType) {

        if (UpType == "Main") {
            CkType = "Main";
            
            if ($('#cbWh').val() == "") {
                $('#cbCK').empty();
                return;
            }
            else {

            }
            
            var pData = {
                WhNo: $('#cbWh').val(),
                StopDay: 'Y',
                CheckUse: 'Y'
            };
            //var pData = {
            //    WhNo: $('#cbWh').val()
            //};
        }
        else if (UpType == "Out") {
            CkType = "Out";
            
            var pData = {
                WhNo: $('#WhNoOut').val(),
                StopDay: 'Y',
                CheckUse: 'Y'
            };
        }
        else if (UpType == "In") {
            CkType = "In";
            
            var pData = {
                WhNo: $('#WhNoIn').val(),
                StopDay: 'Y',
                CheckUse: 'Y'
            };
        }

        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNoWithCond", data: pData, success: AfterGetWhDSVCkNo });
        
    };


    let AfterGetWhDSVCkNo = function (data) {
        
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoWithCondOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            if (CkType == "Main") {
                console.log("AfterGetWhDSVCkNo：Main");
                var dtCK = data.getElementsByTagName('dtCK');
                InitSelectItem($('#cbCK')[0], dtCK, "CKNo", "CkNoName", true, "請選擇機號");
            }
            else if (CkType == "Out") {
                console.log("AfterGetWhDSVCkNo：Out");
                var dtCK = data.getElementsByTagName('dtCK');
                InitSelectItem($('#CkNoOut')[0], dtCK, "CKNo", "CkNoName", true);
            }
            else if (CkType == "In") {
                console.log("AfterGetWhDSVCkNo：In");
                var dtCK = data.getElementsByTagName('dtCK');
                InitSelectItem($('#CkNoIn')[0], dtCK, "CKNo", "CkNoName", true);
            }
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

    let AfterAddChgShop = function (data) {
        if (ReturnMsg(data, 0) != "AddChgShopOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("新增完成!");

            $('#modal_VIN13_1').modal('hide');
            var userxml = data.getElementsByTagName('dtChgShop')[0];
            grdU.AddNew(userxml);
        }
    };


    let SaveData = function () {
        //alert("EditMode:" + EditMode);
        if (EditMode == "Add") {

            var pData = {
                ChangeShopSV: [
                    {
                        WhNoOut: $('#WhNoOut').val(),
                        CkNoOut: $('#CkNoOut').val(),
                        WhNoIn: $('#WhNoIn').val(),
                        CkNoIn: $('#CkNoIn').val(),
                        ExchangeDate: $('#ExchangeDate').val()
                    }
                ]
            }
            //alert("Add.." + $('#Type_ID').val());
            PostToWebApi({ url: "api/SystemSetup/AddChgShop", data: pData, success: AfterAddChgShop });
        }
        else if (EditMode == "Mod") {
            //alert("EditMode:" + EditMode);
            var mData = {
                ChangeShopSV: [
                    {
                        DocNo: gDocNo,
                        WhNoOut: $('#WhNoOut').val(),
                        CkNoOut: $('#CkNoOut').val(),
                        WhNoIn: $('#WhNoIn').val(),
                        CkNoIn: $('#CkNoIn').val(),
                        ExchangeDate: $('#ExchangeDate').val()
                    }
                ]
            }
  
            PostToWebApi({ url: "api/SystemSetup/UpdateChgShop", data: mData, success: AfterUpdateChgShop });
        }
        else if (EditMode == "App") {

            var cData = {
                ChangeShopSV: [
                    {
                        DocNo: gDocNo
                    }
                ]
            }
            //alert("Del " + $('#Type_ID').val());
            PostToWebApi({ url: "api/SystemSetup/AppChgShop", data: cData, success: AfterUpdateChgShop });
        }
        else if (EditMode == "Del") {
            var cData = {
                ChangeShopSV: [
                    {
                        DocNo: gDocNo
                    }
                ]
            }
            PostToWebApi({ url: "api/SystemSetup/DelChgShop", data: cData, success: AfterDelChgShop });
        }

    }




    let btSave_click = function () {

        //alert("SysDate " + SysDate);
        
        if ($('#WhNoOut').val() == "" | $('#WhNoOut').val() == null | $('#CkNoOut').val() == "" | $('#CkNoOut').val() == null | $('#WhNoIn').val() == "" | $('#WhNoIn').val() == null | $('#CkNoIn').val() == "" | $('#CkNoIn').val() == null | $('#ExchangeDate').val() == "" | $('#ExchangeDate').val() == null) {
            DyAlert("所有欄位都必須輸入資料!!", function () { $('#WhNoOut').focus() });
            //DyAlert("所有欄位都必須輸入資料!!", setFocus);
            return;
        }
        if ($('#WhNoOut').val() == "" | $('#WhNoOut').val() == null) {
            DyAlert("原智販店代號欄位必須輸入資料!!", function () { $('#WhNoOut').focus() });
            return;
        }
        if ($('#CkNoOut').val() == "" | $('#CkNoOut').val() == null) {
            DyAlert("原機號欄位必須輸入資料!!", function () { $('#Type_Name').focus() });
            return;
        }
        if ($('#WhNoIn').val() == "" | $('#WhNoIn').val() == null) {
            DyAlert("新智販店代號欄位必須輸入資料!!", function () { $('#WhNoIn').focus() });
            return;
        }
        if ($('#CkNoIn').val() == "" | $('#CkNoIn').val() == null) {
            DyAlert("新機號欄位必須輸入資料!!", function () { $('#CkNoIn').focus() });
            return;
        }
        if ($('#ExchangeDate').val() == "" | $('#ExchangeDate').val() == null) {
            DyAlert("換店日期欄位必須輸入資料!!", function () { $('#ExchangeDate').focus() });
            return;
        }

        if ($('#WhNoIn').val() == $('#WhNoOut').val()) {
            DyAlert("新智販店代號與原智販店代號不可相同!!", function () { $('#WhNoIn').focus() });
            return;
        }

        //檢查換店日期必須大於系統日
       if ($('#ExchangeDate').val() < SysDate ) {
           DyAlert("換店日期必須大於等於系統日期!!", function () { $('#ExchangeDate').focus() });
            return;
        }


        //var pData = {
        //        WhNoOut: $('#WhNoOut').val(),
        //        CkNoOut: $('#CkNoOut').val(),
        //        WhNoIn: $('#WhNoIn').val(),
        //        CkNoIn: $('#CkNoIn').val(),
        //        ExchangeDate: $('#ExchangeDate').val(),
        //        ChkMod: EditMode
        //}

        //PostToWebApi({ url: "api/SystemSetup/ChkChgShopCols", data: pData, success: AfterChkChgShopCols });

        //alert("EditMode:" + EditMode);
        if (EditMode == "App") {
            DyConfirm("確定要批核這筆資料嗎?", SaveData, DummyFunction);
        }
        else {
            //alert("EditMode:" + EditMode);
            SaveData();
        }

        //alert("EditMode:" + EditMode);
        //if (EditMode == "Add") {
        //    var pData = {
        //        WhNoOut: $('#WhNoOut').val(),
        //        CkNoOut: $('#CkNoOut').val(),
        //        WhNoIn: $('#WhNoIn').val(),
        //        CkNoIn: $('#CkNoIn').val(),
        //        ExchangeDate: $('#ExchangeDate').val()
        //    }
        //    //alert("Add.." + $('#Type_ID').val());
        //    PostToWebApi({ url: "api/SystemSetup/AddChgShop", data: pData, success: AfterDelChgShop });
        //}
        //else if (EditMode == "Mod") {
        //    var mData = {
        //        ChangeShopSV: [
        //            {
        //            DocNo: gDocNo,
        //            WhNoOut: $('#WhNoOut').val(),
        //            CkNoOut: $('#CkNoOut').val(),
        //            WhNoIn: $('#WhNoIn').val(),
        //            CkNoIn: $('#CkNoIn').val(),
        //            ExchangeDate: $('#ExchangeDate').val()
        //            }
        //        ]
        //    }
        //    //alert("DocNo " + gDocNo);
        //    //alert("WhNoOut " + $('#WhNoOut').val(),);
        //    //alert("CkNoOut " + $('#CkNoOut').val(),);
        //    //alert("WhNoIn " + $('#WhNoIn').val(),);
        //    //alert("CkNoIn " + $('#CkNoIn').val(),);
        //    //alert("ExchangeDate " + $('#ExchangeDate').val(),);

        //    PostToWebApi({ url: "api/SystemSetup/UpdateChgShop", data: mData, success: AfterUpdateChgShop });
        //}
        //else if (EditMode == "App") {
        //    var cData = {
        //        ChangeShopSV: [
        //            {
        //                DocNo: gDocNo
        //            }
        //        ]
        //    }
        //    //alert("Del " + $('#Type_ID').val());
        //    PostToWebApi({ url: "api/SystemSetup/AppChgShop", data: cData, success: AfterUpdateChgShop });
        //}
        //else if (EditMode == "Del") {
        //    var cData = {
        //        ChangeShopSV: [
        //            {
        //                DocNo: gDocNo
        //            }
        //        ]
        //    }
        //    PostToWebApi({ url: "api/SystemSetup/DelChgShop", data: cData, success: AfterDelChgShop });
        //}

        //return


    };


    let AfterUpdateChgShop = function (data) {
        //alert("AfterUpdateChgShop:" + EditMode);
        if (ReturnMsg(data, 0) != "UpdateChgShopOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {

            if (EditMode == "App") {
                DyAlert("批核完成!");
            }
            else {
                DyAlert("儲存完成!");
            }

            

            $('#modal_VIN13_1').modal('hide');
            var userxml = data.getElementsByTagName('dtRes')[0];
            grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
        }
    };


    let AfterDelChgShop = function (data) {
        if (ReturnMsg(data, 0) != "DelChgShopOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_VIN13_1').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdU.DeleteRow(grdU.ActiveRowTR());
        }
    };


    //let AfterChkRackUsed = function (data) {
    //    if (ReturnMsg(data, 0) != "ChkRackUsedOK") {
    //        DyAlert(ReturnMsg(data, 1));
    //    }
    //    else {
    //        var dtRack = data.getElementsByTagName("dtRack");
    //        //alert("Rack Rows:" + dtRack.length);

    //        if (EditMode == "Mod") {
    //            //alert("Mod OldID:" + OldID);
    //            if (OldID != $('#Type_ID').val()) {
    //                if (dtRack.length > 0) {
    //                    DyAlert("貨倉代號已被引用，無法修改!!")
    //                    return;
    //                }
    //            }

    //            var pData = {
    //                Rack: [
    //                    {
    //                        OldType_ID: OldID,
    //                        Type_ID: $('#Type_ID').val(),
    //                        Type_Name: $('#Type_Name').val(),
    //                        DisplayNum: $('#DisplayNum').val()
    //                    }
    //                ]
    //            };
    //            PostToWebApi({ url: "api/SystemSetup/UpdateRack", data: pData, success: AfterUpdateRack });
    //        }
    //        else if (EditMode == "Add") {
    //            if (dtRack.length > 0) {
    //                DyAlert("貨倉代號已存在，無法新增!!")
    //                return;
    //            }
    //            var pData = {
    //                Rack: [
    //                    {
    //                        Type_ID: $('#Type_ID').val(),
    //                        Type_Name: $('#Type_Name').val(),
    //                        DisplayNum: $('#DisplayNum').val()
    //                    }
    //                ]
    //            };
    //            PostToWebApi({ url: "api/SystemSetup/AddRack", data: pData, success: AfterAddRack });
    //        }
    //        else if (EditMode == "App") {
    //            if (dtRack.length > 0) {
    //                DyAlert("貨倉代號已被引用，無法刪除!!")
    //                return;
    //            }

    //            var pData = {
    //                Rack: [
    //                    {
    //                        Type_ID: $('#Type_ID').val()
    //                    }
    //                ]
    //            };

    //            PostToWebApi({ url: "api/SystemSetup/DelRack", data: pData, success: AfterDelRack });
    //        }

    //        //DyAlert("匯入完成!");
    //        //$('#modal_VMN29').modal('hide');
    //    }
    //};







    //2021-05-18
    let afterGetInitVIN13_1 = function (data) {

        //alert("afterGetInitVIN13_1");

        AssignVar();
        $('#btQuery').click(function () { SearchVIN13_1(); });

        var dtWh = data.getElementsByTagName('dtWh');
        InitSelectItem($('#cbWh')[0], dtWh, "ST_ID", "STName", true, "請選擇店代號");

        //var dtS = data.getElementsByTagName('dtS');
        //InitSelectItem($('#cbSWh')[0], dtS, "ST_ID", "STName", true);

        $('#cbWh').change(function () { GetWhDSVCkNo("Main"); });
        $('#cbCK').click(function () { cbCK_click(); });
        SetDateField($('#exDate')[0]);
        $('#exDate').datepicker();

        $('#WhNoOut').change(function () { GetModalWhDSVCkNoOut(); });
        $('#WhNoIn').change(function () { GetModalWhDSVCkNoIn(); });

        $('#btAdd').click(function () { btAdd_click(); });
        //$('#btImportFromiXMS').click(function () { btImportFromiXMS_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        $('.forminput input').change(function () { InputValidation(this) });

        SetDateField($('#ExchangeDate')[0]);
        $('#ExchangeDate').datepicker();

        //SetPLUAutoComplete("GD_NAME");
        //SetPLUAutoComplete("GD_NO");
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
        if ($(ip).attr('id') == "WhNoOut") {
            $(ip).val($(ip).val().toUpperCase());
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length > 6)
                msg = "必須6碼內英數字";
            else {

            }
        }
        if ($(ip).attr('id') == "CkNoOut") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length > 3)
                msg = "必須3碼內英數字";
        }
        if ($(ip).attr('id') == "WhNoIn") {
            $(ip).val($(ip).val().toUpperCase());
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length > 6)
                msg = "必須6碼內英數字";
        }
        if ($(ip).attr('id') == "CkNoIn") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length > 3)
                msg = "必須3碼內英數字";
        }
        //if ($(ip).attr('id') == "ExchangeDate") {

        //    var re = /^[\d|a-zA-Z]+$/;
        //    if (!re.test(str) | str.length < 6 | str.length > 20)
        //        msg = "必須3碼內英數字";
        //}

        //if ($(ip).attr('id') == "USR_NAME_L") {
        //    if (str.length > 10)
        //        msg = "必須10字元以內";
        //}
        //if ($(ip).attr('id') == "USR_EMPNO") {
        //    if (str.length > 10)
        //        msg = "必須10字元以內";
        //}
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
            $(ip).val('');
            $(ip).nextAll('.msg-valid').text(msg);
            $(ip).nextAll('.msg-valid').show();
        }
    }

    let afterLoadPage = function () {

        PostToWebApi({ url: "api/SystemSetup/GetWh", success: afterGetInitVIN13_1 });
        $('#pgVIN13_1').show();
        
    };

    if ($('#pgVIN13_1').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "VIN13_1", ["pgVIN13_1"], afterLoadPage);
    };


}