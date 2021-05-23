var PageMMMachineSet = function (ParentNode) {

    let tbDetail;
    let grdM;
    let EditMode = "";
    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbMMMachineSet')[0],
                class_collection: ["tdColbt icon_in_td", "tdCol1", "tdCol2", "tdCol3", "tdCol4 text-right", "tdCol5 text-right", "tdCol6 text-right"],
                fields_info: [
                    { type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
                    { type: "Text", name: "SNno" },
                    { type: "Text", name: "StartDay" },
                    { type: "Text", name: "StopDay" },
                    { type: "TextAmt", name: "Lyaers" },
                    { type: "TextAmt", name: "Channels" },
                    { type: "TextAmt", name: "Temperature" }],
                rows_per_page: 10,
                method_clickrow: click_Machine,
                afterBind: InitModifyDeleteButton,
                //sortable: "Y"
            }
        );

    };

    let click_Machine = function (tr) {

    };


    let afterGetInitMMMachineSet = function (data) {
        AssignVar();
        tbDetail = $('#pgMMMachineSetDetail #tbMMMachineRack tbody');
        var dtRack = data.getElementsByTagName('dtRack');
        InitSelectItem($('.sel_Rack')[0], dtRack, "Type_ID", "Type_Name", true);
        $('#btNewMachine').click(function () { btNewMachine_click(); });
        $('#btNewRack').click(function () { btNewRack_click(); });
        $('#pgMMMachineSetDetail .valid-blank').prev('input').focus(
            function () { $('.valid-blank').text(''); }
        );

        $('#btQueryMachine').click(function () { SearchMachine(); });
        //$('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
        //$('#btDelete').click(function () { btDelete_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        //$('.forminput input').change(function () { InputValidation(this) });
        SetDateField($('#StartDay')[0]);
        SetDateField($('#StopDay')[0]);
        $('#StartDay,#StopDay').datepicker();
        //SetPLUAutoComplete("GD_NAME");
        //SetPLUAutoComplete("GD_NO");
    };

    let InitModifyDeleteButton = function () {
        $('#tbMMMachineSet .fa-file-text-o').click(function () { btModify_click(this) });
        
    }


    let SearchMachine = function () {
        var pData = {
            KeyWord: $('#txtMachineSearch').val()
        };
        PostToWebApi({ url: "api/SystemSetup/SearchMachine", data: pData, success: AfterSearchMachine });
    }
    let AfterSearchMachine = function (data) {
        if (ReturnMsg(data, 0) != "SearchMachineOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtMachine = data.getElementsByTagName('dtMachine');
            grdM.BindData(dtMachine);
            if (dtMachine.length == 0) {
                DyAlert("無符合資料!", BlankMode);
                return;
            }

        }
    };


   
    let btModify_click = function (bt) {
        $(bt).closest('tr').click();
        EditMode = "Modify";
        var node = $(grdM.ActiveRowTR()).prop('Record');
        var pData = {
            CompanyCode: GetNodeValue(node, 'CompanyCode'),
            SNno: GetNodeValue(node, 'SNno')
        }
        PostToWebApi({ url: "api/SystemSetup/GetMachineDetail", data: pData, success: AfterGetMachineDetail });

    };

    let AfterGetMachineDetail = function (data) {
        if (ReturnMsg(data, 0) != "GetMachineDetailOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            var MachineList = data.getElementsByTagName('MachineList')[0];
            var MachineListSpec = data.getElementsByTagName('MachineListSpec');
            $('#pgMMMachineSetDetail .x_title h4').text('修改智販機');
            tbDetail.empty();
            $('#SNno').prop('disabled', true);
            $('#pgMMMachineSetDetail .forminput input:text').val('');
            $('#SNno').val(GetNodeValue(MachineList, 'SNno'));
            $('#StartDay').val(GetNodeValue(MachineList, 'StartDay'));
            $('#StopDay').val(GetNodeValue(MachineList, 'StopDay'));
            $('#Temperature').val(GetNodeValue(MachineList, 'Temperature'));
            for (var i = 0; i < MachineListSpec.length; i++) {
                var trD = NewOneDetail(i);
                $(trD).find('.sel_Rack').val(GetNodeValue(MachineListSpec[i], 'ChannelType'));
                $(trD).find('.ChanQty').val(parseInt(GetNodeValue(MachineListSpec[i], 'ChannelNo')));
                tbDetail.append(trD);
            }
            $('#pgMMMachineSet').hide();
            $('#pgMMMachineSetDetail').show();

        }
    }


    let BlankMode = function () {

    }

    let btSave_click = function () {
        var isCheck = true;
        var nullchk = $('#pgMMMachineSetDetail .valid-blank').prev('input').filter(function () { return this.value == '' });
        if (nullchk.length > 0) {
            $('.valid-blank').text('請填入資料');
            isCheck = false;
        }
        var nullsel = $('#tbMMMachineRack .sel_Rack').filter(function () { return $(this).val() == "" });
        if (nullsel.length > 0) {
            nullsel.addClass('ErrorControl');
            isCheck = false;
        }
        var nullQty = $('#tbMMMachineRack .ChanQty').filter(function () { return $(this).val() == "" });
        if (nullQty.length > 0) {
            nullQty.addClass('ErrorControl');
            isCheck = false;
        }
        if (!isCheck) {
            waitingDialog.hide();
            DyAlert("有資料未填寫，請填妥以後再儲存");
            return;
        }
        waitingDialog.show("儲存中.....");
        setTimeout(
            function () { SaveMachineList(); }
            ,100
        );
    }

    let SaveMachineList = function () {
        
        var MachineList = [];
        var MachineListSpec = [];
        var DataH = {
            EditMode: EditMode,
            CompanyCode: "",
            SNno: $('#SNno').val(),
            StartDay: $('#StartDay').val(),
            StopDay: $('#StopDay').val(),
            Temperature: $('#Temperature').val()
        }
        MachineList.push(DataH);
        var trs = $('#tbMMMachineRack tbody tr');
        for (var i = 0; i < trs.length; i++) {
            var tr = trs[i];
            var DataD = {
                CompanyCode: "",
                SNno: $('#SNno').val(),
                LayerNo: $(tr).find('.tdSerNo').text(),
                ChannelType: $(tr).find('.sel_Rack').val(),
                ChannelQty: $(tr).find('.ChanQty').val()
            }
            MachineListSpec.push(DataD);
        }
        pData = {
            MachineList: MachineList,
            MachineListSpec: MachineListSpec
        }
        PostToWebApi({ url: "api/SystemSetup/SaveMachineList", data: pData, success: AfterSaveMachineList });
    }


    let AfterSaveMachineList = function (data) {
        waitingDialog.hide();
        if (ReturnMsg(data, 0) != "SaveMachineListOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            DyAlert("儲存完成!");
            $('#pgMMMachineSet').show();
            $('#pgMMMachineSetDetail').hide();

            var dtMachine = data.getElementsByTagName('dtMachine')[0];

            if (EditMode == "Modify") {
                grdM.RefreshRocord(grdM.ActiveRowTR(), dtMachine);
            }
            else if (EditMode == "Add") {
                grdM.AddNew(dtMachine);
            }
        }
    }

    let btNewMachine_click = function () {
        $('#SNno').prop('disabled', false);
        EditMode = "Add";
        $('#pgMMMachineSetDetail .x_title h4').text('新增智販機');
        tbDetail.empty();
        $('#pgMMMachineSetDetail .forminput input:text').val('');
        for (var i = 0; i < 4; i++)
            tbDetail.append(NewOneDetail(i));
        $('#pgMMMachineSet').hide();
        $('#pgMMMachineSetDetail').show();
    }

    let btNewRack_click = function () {
        var rowCount = tbDetail.find('tr').length;
        tbDetail.append(NewOneDetail(rowCount));
    }

    let NewOneDetail = function (Rowindex) {
        var tr = $('<tr></tr>');
        var td1 = $('<td calss="tdLayer"></td>');
        var td2 = $('<td class="tdSerNo"></td>');
        var td3 = $('<td></td>');
        var td4 = $('<td></td>');
        tr.append(td1);
        tr.append(td2);
        tr.append(td3);
        tr.append(td4);
        td1.append($('<span class="fa fa-trash-o"></span>'));
        td2.text(String.fromCharCode(Rowindex + 65));
        td3.append($($('.sel_Rack')[0]).clone());
        td4.append($('<input class="ChanQty" type="text" value="" />'));
        tr.find('.fa-trash-o').click(function () { deleteDetail(this) });

        $(td3).find('.sel_Rack').click(function () { $(this).removeClass('ErrorControl') });
        $(td4).find('.ChanQty').click(function () { $(this).removeClass('ErrorControl') });


        return tr;
    }

    let deleteDetail = function (img) {
        $(img).closest('tr').remove();
        var trs = tbDetail.find('tr');
        for (var i = 0; i < trs.length; i++) {
            var trTmp = trs[i];
            $(trTmp).find('.tdSerNo').text(String.fromCharCode(i + 65));
        }
    }

    let btCancel_click = function () {
        $('#pgMMMachineSet').show();
        $('#pgMMMachineSetDetail').hide();
    }

    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetInitMMMachineSet", success: afterGetInitMMMachineSet });
        $('#pgMMMachineSet').show();
        $('#pgMMMachineSetDetail').hide();
    };

    if ($('#pgGMMacPLUSet').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MMMachineSet", ["pgMMMachineSetDetail","pgMMMachineSet"], afterLoadPage);
    };
}