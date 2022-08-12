var PageISAMQFTPREC = function (ParentNode) {
    
    let tbDetail;
    let grdM;
    let EditMode = "";
    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbISAMQFTPREC')[0],
                class_collection: ["tdColbt icon_in_td", "tdCol1", "tdCol2", "tdCol3", "tdCol4"],
                fields_info: [
                    { type: "JQ", name: "fa-search", element: '<i class="fa fa-search"></i>' },
                    { type: "Text", name: "DocTypeDesc" },
                    { type: "Text", name: "CrtDT" },
                    { type: "Text", name: "CrtUserName" },
                    { type: "Text", name: "UpdateTypeDesc" }],
                //rows_per_page: 10,
                method_clickrow: click_Machine,
                afterBind: InitModifyDeleteButton
                //sortable: "Y"
            }
        );

    };

    let click_Machine = function (tr) {

    };


    //let afterGetInitMMMachineSet = function (data) {
    //    AssignVar();
    //    tbDetail = $('#pgMMMachineSetDetail #tbMMMachineRack tbody');
    //    var dtRack = data.getElementsByTagName('dtRack');
    //    InitSelectItem($('.sel_Rack')[0], dtRack, "Type_ID", "Type_Name", true);
    //    $('#btNewMachine').click(function () { btNewMachine_click(); });
    //    $('#btNewRack').click(function () { btNewRack_click(); });
    //    $('#pgMMMachineSetDetail .valid-blank').prev('input').focus(
    //        function () { $('.valid-blank').text(''); }
    //    );

    //    $('#btQueryMachine').click(function () { SearchMachine(); });
    //    //$('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
    //    //$('#btDelete').click(function () { btDelete_click(); });
    //    $('#btSave').click(function () { btSave_click(); });
    //    $('#btCancel').click(function () { btCancel_click(); });
    //    //$('.forminput input').change(function () { InputValidation(this) });
    //    SetDateField($('#StartDay')[0]);
    //    SetDateField($('#StopDay')[0]);
    //    $('#StartDay,#StopDay').datepicker();
    //    //SetPLUAutoComplete("GD_NAME");
    //    //SetPLUAutoComplete("GD_NO");
    //};

    let InitModifyDeleteButton = function () {
        $('#tbISAMQFTPREC .fa-search').click(function () { btDetail_click(this) });
        
    }


    //let SearchMachine = function () {
    //    var pData = {
    //        KeyWord: $('#txtMachineSearch').val()
    //    };
    //    PostToWebApi({ url: "api/SystemSetup/SearchMachine", data: pData, success: AfterSearchMachine });
    //}
    //let AfterSearchMachine = function (data) {
    //    if (ReturnMsg(data, 0) != "SearchMachineOK") {
    //        DyAlert(ReturnMsg(data, 0));
    //        return;
    //    }
    //    else {
    //        var dtMachine = data.getElementsByTagName('dtMachine');
    //        grdM.BindData(dtMachine);
    //        if (dtMachine.length == 0) {
    //            DyAlert("無符合資料!", BlankMode);
    //            return;
    //        }

    //    }
    //};


   
    let btDetail_click = function (bt) {
        $(bt).closest('tr').click();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //var pData = {
        //    CompanyCode: GetNodeValue(node, 'CompanyCode'),
        //    SNno: GetNodeValue(node, 'SNno')
        //}
        //PostToWebApi({ url: "api/SystemSetup/GetMachineDetail", data: pData, success: AfterGetMachineDetail });

    };

    //let AfterGetMachineDetail = function (data) {
    //    if (ReturnMsg(data, 0) != "GetMachineDetailOK") {
    //        DyAlert(ReturnMsg(data, 0));
    //    }
    //    else {
    //        var MachineList = data.getElementsByTagName('MachineList')[0];
    //        var MachineListSpec = data.getElementsByTagName('MachineListSpec');
    //        $('#pgMMMachineSetDetail .x_title h4').text('修改智販機');
    //        tbDetail.empty();
    //        $('#SNno').prop('disabled', true);
    //        $('#pgMMMachineSetDetail .forminput input:text').val('');
    //        $('#SNno').val(GetNodeValue(MachineList, 'SNno'));
    //        $('#StartDay').val(GetNodeValue(MachineList, 'StartDay'));
    //        $('#StopDay').val(GetNodeValue(MachineList, 'StopDay'));
    //        $('#Temperature').val(GetNodeValue(MachineList, 'Temperature'));
    //        for (var i = 0; i < MachineListSpec.length; i++) {
    //            var trD = NewOneDetail(i);
    //            $(trD).find('.sel_Rack').val(GetNodeValue(MachineListSpec[i], 'ChannelType'));
    //            $(trD).find('.ChanQty').val(parseInt(GetNodeValue(MachineListSpec[i], 'ChannelNo')));
    //            tbDetail.append(trD);
    //        }
    //        $('#pgMMMachineSet').hide();
    //        $('#pgMMMachineSetDetail').show();

    //    }
    //}


    //let BlankMode = function () {

    //}

    //let btSave_click = function () {
    //    var isCheck = true;
    //    var nullchk = $('#pgMMMachineSetDetail .valid-blank').prev('input').filter(function () { return this.value == '' });
    //    if (nullchk.length > 0) {
    //        $('.valid-blank').text('請填入資料');
    //        isCheck = false;
    //    }
    //    var nullsel = $('#tbMMMachineRack .sel_Rack').filter(function () { return $(this).val() == "" });
    //    if (nullsel.length > 0) {
    //        nullsel.addClass('ErrorControl');
    //        isCheck = false;
    //    }
    //    var nullQty = $('#tbMMMachineRack .ChanQty').filter(function () { return $(this).val() == "" });
    //    if (nullQty.length > 0) {
    //        nullQty.addClass('ErrorControl');
    //        isCheck = false;
    //    }
    //    if (!isCheck) {
    //        waitingDialog.hide();
    //        DyAlert("有資料未填寫，請填妥以後再儲存");
    //        return;
    //    }
    //    ShowLoading();
    //    setTimeout(
    //        function () { SaveMachineList(); }
    //        ,100
    //    );
    //}

    //let SaveMachineList = function () {
        
    //    var MachineList = [];
    //    var MachineListSpec = [];
    //    var DataH = {
    //        EditMode: EditMode,
    //        CompanyCode: "",
    //        SNno: $('#SNno').val(),
    //        StartDay: $('#StartDay').val(),
    //        StopDay: $('#StopDay').val(),
    //        Temperature: $('#Temperature').val()
    //    }
    //    MachineList.push(DataH);
    //    var trs = $('#tbMMMachineRack tbody tr');
    //    for (var i = 0; i < trs.length; i++) {
    //        var tr = trs[i];
    //        var DataD = {
    //            CompanyCode: "",
    //            SNno: $('#SNno').val(),
    //            LayerNo: $(tr).find('.tdSerNo').text(),
    //            ChannelType: $(tr).find('.sel_Rack').val(),
    //            ChannelQty: $(tr).find('.ChanQty').val()
    //        }
    //        MachineListSpec.push(DataD);
    //    }
    //    pData = {
    //        MachineList: MachineList,
    //        MachineListSpec: MachineListSpec
    //    }
    //    PostToWebApi({ url: "api/SystemSetup/SaveMachineList", data: pData, success: AfterSaveMachineList });
    //}


    //let AfterSaveMachineList = function (data) {
    //    CloseLoading();
    //    if (ReturnMsg(data, 0) != "SaveMachineListOK") {
    //        DyAlert(ReturnMsg(data, 0));
    //    }
    //    else {
    //        DyAlert("儲存完成!");
    //        $('#pgMMMachineSet').show();
    //        $('#pgMMMachineSetDetail').hide();

    //        var dtMachine = data.getElementsByTagName('dtMachine')[0];

    //        if (EditMode == "Modify") {
    //            grdM.RefreshRocord(grdM.ActiveRowTR(), dtMachine);
    //        }
    //        else if (EditMode == "Add") {
    //            grdM.AddNew(dtMachine);
    //        }
    //    }
    //}

    //let btNewMachine_click = function () {
    //    $('#SNno').prop('disabled', false);
    //    EditMode = "Add";
    //    $('#pgMMMachineSetDetail .x_title h4').text('新增智販機');
    //    tbDetail.empty();
    //    $('#pgMMMachineSetDetail .forminput input:text').val('');
    //    for (var i = 0; i < 4; i++)
    //        tbDetail.append(NewOneDetail(i));
    //    $('#pgMMMachineSet').hide();
    //    $('#pgMMMachineSetDetail').show();
    //}

    //let btNewRack_click = function () {
    //    var rowCount = tbDetail.find('tr').length;
    //    tbDetail.append(NewOneDetail(rowCount));
    //}

    //let NewOneDetail = function (Rowindex) {
    //    var tr = $('<tr></tr>');
    //    var td1 = $('<td calss="tdLayer"></td>');
    //    var td2 = $('<td class="tdSerNo"></td>');
    //    var td3 = $('<td></td>');
    //    var td4 = $('<td></td>');
    //    tr.append(td1);
    //    tr.append(td2);
    //    tr.append(td3);
    //    tr.append(td4);
    //    td1.append($('<span class="fa fa-trash-o"></span>'));
    //    td2.text(String.fromCharCode(Rowindex + 65));
    //    td3.append($($('.sel_Rack')[0]).clone());
    //    td4.append($('<input class="ChanQty" type="text" value="" />'));
    //    tr.find('.fa-trash-o').click(function () { deleteDetail(this) });

    //    $(td3).find('.sel_Rack').click(function () { $(this).removeClass('ErrorControl') });
    //    $(td4).find('.ChanQty').click(function () { $(this).removeClass('ErrorControl') });


    //    return tr;
    //}

    //let deleteDetail = function (img) {
    //    $(img).closest('tr').remove();
    //    var trs = tbDetail.find('tr');
    //    for (var i = 0; i < trs.length; i++) {
    //        var trTmp = trs[i];
    //        $(trTmp).find('.tdSerNo').text(String.fromCharCode(i + 65));
    //    }
    //}

    let btDetail_click = function (bt) {
        Timerset(sessionStorage.getItem('isamcomp'));
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        //$('#modal_ISAM02PLUDel .modal-title').text('盤點資料單筆刪除');
        ////$('#modal_ISAM02Mod .btn-danger').text('刪除');
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //DelPLU = GetNodeValue(node, 'PLU');
        //DelPLUQty = GetNodeValue(node, 'Qty');
        var cData = {
            Shop: $('#lblShop2').html().split(' ')[0],
            //PLU: DelPLU
        }
        //PostToWebApi({ url: "api/SystemSetup/GetGDName", data: cData, success: AfterGetGDName });
    }

    let btCancel_click = function () {
        $('#pgMMMachineSet').show();
        $('#pgMMMachineSetDetail').hide();
    }

    let afterSearchISAMQFTPREC = function (data) {
        if (ReturnMsg(data, 0) != "GetISAMQFTPRECDataOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtQRec = data.getElementsByTagName('dtQRec');
            grdM.BindData(dtQRec);
        }
    }

    let btQuery_click = function () {
        EditMode = "Q";
        pData = {
            WhNo: $('#lblShop2').html().split(' ')[0],
            EditMode: EditMode,
            DateS: $('#txtCrtDateS').val(),
            DateE: $('#txtCrtDateE').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetISAMQFTPRECData", data: pData, success: afterSearchISAMQFTPREC });
    }


    //#region FormLoad
    let afterGetInitISAMQFTPREC = function (data) {
        if (ReturnMsg(data, 0) != "GetISAMQFTPRECDataOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            
            //AssignVar();
            ////
            //tbDetail = $('#pgISAMQFTPREC #tbISAMQFTPREC tbody');
            //var dtQDate = data.getElementsByTagName('dtQDate');
            //SetDateField($('#txtCrtDateS')[0]);
            //SetDateField($('#txtCrtDateE')[0]);
            //$('#txtCrtDateS').val(GetNodeValue(dtQDate[0], "DTS"));
            //$('#txtCrtDateE').val(GetNodeValue(dtQDate[0], "DTE"));

            //alert("dtQDate:"+dtQDate.length);
            //var dtQRec = data.getElementsByTagName('dtQRec');
            //grdM.BindData(dtQRec);
            //$('#btQuery').click(function () { btQuery_click(); });
            ////if (dtQRec.length == 0) {
            ////    //alert("No RowData");
            ////    DyAlert("無符合資料!");
            ////    return;
            ////}
            ////alert(GetNodeValue(dtISAMShop[0], "STName"));
            
            ////$('#lblManID1').text(GetNodeValue(dtISAMShop[0], "ManName"));
            ////SetDateField($('#txtISAMDate')[0]);
            ////$('#pgISAM01Init').removeAttr('hidden');
            ////$('#pgISAM01Init').show();
            ////$('#btSave').click(function () { btSave_click(); });
            ////$('#txtBinNo').keypress(function (e) {
            ////    if (e.which == 13) { btSave_click(); }
            ////});

            ////
            ////$('#btMod').click(function () { btMod_click(); });
            ////$('#btToFTP').click(function () { btToFTP_click(); });
            ////$('#btRtn').click(function () { btRtn_click(); afterRtnclick(); });

            ////$('#txtQty1').prop('disabled', true);
            ////$('#btQtySave1').prop('disabled', true);
            ////$('#btBCSave1').click(function () { btBCSave1_click(); });
            ////$('#btKeyin1').click(function () { btKeyin1_click(); });
            ////$('#btQtySave1').click(function () { btQtySave1_click(); });
            ////$('#txtBarcode1').keypress(function (e) {
            ////    if (e.which == 13) { btBCSave1_click(); }
            ////});


            ////$('#btDelCancel').click(function () { btDelCancel_click(); });
            ////$('#btDelSave').click(function () { btDelSave_click(); });


            ////$('#btModCancel').click(function () { btModCancel_click(); });
            ////$('#btModSave').click(function () { btModSave_click(); });
            ////$('#btBCSave3').click(function () { btBCSave3_click(); });
            ////$('#txtBarcode3').keypress(function (e) {
            ////    if (e.which == 13) { btBCSave3_click(); }
            ////});
        }
    };


    let AfterGetWhName = function (data) {
        if (ReturnMsg(data, 0) != "GetWhNameOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtWh = data.getElementsByTagName('dtWh');
            alert(GetNodeValue(dtWh[0], "ST_ID"));
            if (GetNodeValue(dtWh[0], "STName") == "") {
                DyAlert("請確認店櫃(" + GetNodeValue(dtWh[0], "WhNo") + ")是否為允許作業之店櫃!", DummyFunction);
                return;
            }
            $('#lblShop2').html(GetNodeValue(dtWh[0], "STName"));
            EditMode = "Init";
            pData = {
                WhNo: GetNodeValue(dtWh[0], "WhNo"),
                EditMode: EditMode
            }
            PostToWebApi({ url: "api/SystemSetup/GetISAMQFTPRECData", data: pData, success: afterGetInitISAMQFTPREC });
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

    if ($('#pgISAMQFTPREC').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAMQFTPREC", ["pgISAMQFTPREC"], afterLoadPage);
    };
}