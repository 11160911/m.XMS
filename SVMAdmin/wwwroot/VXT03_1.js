var PageVXT03_1 = function (ParentNode) {
    let AllPages;
    let grdU;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let sSNNo = "";


    let AssignVar = function () {

        console.log("AssignVar");

        grdU = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVXT03_1')[0],
                class_collection: ["tdColbt icon_in_td", "tdCol2", "tdColbt icon_in_td btsuspend", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdColbt icon_in_td btT", "tdColbt icon_in_td btNet"],
                //class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td btsuspend", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7",  "tdCol8"],
                fields_info: [
                    //{ type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
                    //{ type: "JQ", name: "fa-toggle-off", element: '<i class="fa fa-toggle-off"></i><i class="fa fa-toggle-on"></i>' },
                    { type: "JQ", name: "fa-search", element: '<i class="fa fa-file-text-o"></i>' },
                    { type: "Text", name: "SNNo" },
                    { type: "JQ", name: "fa-check-circle", element: '<i class="fa fa-check-circle"></i><i class="fa fa-exclamation-circle"></i><i class="fa fa-wrench"></i><i class="fa fa-power-off"></i>' },
                    { type: "Text", name: "ST_ID" },
                    { type: "Text", name: "CkNo" },
                    { type: "Text", name: "VMName" },
                    { type: "Text", name: "SWhName" },
                    { type: "JQ", name: "fa-check-circle", element: '<i class="fa fa-check-circle"></i><i class="fa fa-exclamation-circle"></i>' },
                    { type: "JQ", name: "fa-check-circle", element: '<i class="fa fa-check-circle"></i><i class="fa fa-exclamation-circle"></i>' }
                    //{ type: "Text", name: "FlagNet" }
                ],
                rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                //sortable: "Y"
            }
        );
        
    };


    let InitModifyDeleteButton = function () {
        $('#tbVXT03_1 .fa-file-text-o').click(function () { btDisplay_click(this) });
        //$('#tbGMMacPLUSet').find('.fa-toggle-off,.fa-toggle-on').click(function () { btSuspend_click(this) });
        var trs = $('#tbVXT03_1 tbody tr');
        for (var i = 0; i < trs.length; i++) {
            var tr = trs[i];
            DisplayStatus(tr);
        }
    }

    let DisplayStatus = function (tr) {
        var trNode = $(tr).prop('Record');

        //機器狀態
        var sp = GetNodeValue(trNode, "FlagUse");
        //alert(sp);
        var bts = $(tr).find('.btsuspend i');
        //var bts = $(tr).find('i');
        bts.hide();
        if (sp == "U")
            $(tr).find('.btsuspend .fa-check-circle').show();
        else if (sp == "S")
            $(tr).find('.btsuspend .fa-exclamation-circle').show();
        else if (sp == "M")
            $(tr).find('.btsuspend .fa-wrench').show();
        else
            $(tr).find('.btsuspend .fa-power-off').show();

        //網路狀態
        sp = GetNodeValue(trNode, "FlagNet");
        //alert("FlagNet:" + sp);
        bts = $(tr).find('.btNet i');
        //var bts = $(tr).find('i');
        bts.hide();
        if (sp == "Y")
            $(tr).find('.btNet .fa-check-circle').show();
        else
            $(tr).find('.btNet .fa-exclamation-circle').show();

        //溫度狀態
        sp = GetNodeValue(trNode, "FlagT");
        //alert("FlagT:" + sp);
        bts = $(tr).find('.btT i');
        //var bts = $(tr).find('i');
        bts.hide();
        if (sp == "Y")
            $(tr).find('.btT .fa-check-circle').show();
        else
            $(tr).find('.btT .fa-exclamation-circle').show();

        //var img = $(tr).prop('Photo1');
        //var imgSGID = GetNodeValue(trNode, "Photo1");
        //var url = "api/GetImage?SGID=" + EncodeSGID(imgSGID) + "&UU=" + encodeURIComponent(UU);
        //url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        //img.prop('src', url);
    };


    let btDisplay_click = function (bt) {
        isImport = false;
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VXT03_1 .modal-title').text('智販機監控查詢');
        var node = $(grdU.ActiveRowTR()).prop('Record');
        //$('#ST_ID,#GD_NAME').prop('readonly', true);
        $('#ST_ID').val(GetNodeValue(node, 'ST_ID'));
        $('#CkNo').val(GetNodeValue(node, 'CkNo'));
        $('#WhName').val(GetNodeValue(node, 'VMName'));
        $('#SWhName').val(GetNodeValue(node, 'SWhName'));
        sSNNo = GetNodeValue(node, 'SNNo');
        //alert(sSNNo);
        //alert(GetNodeValue(node, 'FlagT'));

        if (GetNodeValue(node, 'FlagUse') == "N") {
            if (GetNodeValue(node, 'FlagT') == "N") {
                $('#cbFlagT').val("異常");
                $('#temperature').val("異常" + GetNodeValue(node, 'temperature'));
            }
            else {
                $('#cbFlagT').val("正常");
                $('#temperature').val(GetNodeValue(node, 'temperature'));
            }
        }
        else {
            if (GetNodeValue(node, 'FlagT') == "N") {
                $('#cbFlagT').val("異常");
                $('#temperature').val(GetNodeValue(node, 'temperature'));
            }
            else {
                $('#cbFlagT').val("正常");
                $('#temperature').val(GetNodeValue(node, 'temperature'));
            }
        }

 
        //if (GetNodeValue(node, 'FlagT') == "N") {
        //    $('#temperature').val("異常" + GetNodeValue(node, 'temperature'));
        //}
        //else {
        //    $('#temperature').val(GetNodeValue(node, 'temperature'));
        //}
        //alert(GetNodeValue(node, 'FlagUse'));
        
        if (GetNodeValue(node, 'FlagUse') == "U") {
            $('#VMStatus').val("啟用");
        }
        else if (GetNodeValue(node, 'FlagUse') == "S") {
            $('#VMStatus').val("停用");
        }
        else if (GetNodeValue(node, 'FlagUse') == "M") {
            $('#VMStatus').val("維護中");
        }
        else {
            $('#VMStatus').val("未配置");
        }
        //alert(GetNodeValue(node, 'FlagNet'));
        if (GetNodeValue(node, 'FlagNet') == "N") {
            $('#NetStatus').val("異常");
        }
        else {
            $('#NetStatus').val("正常");
        }

        $('#modal_VXT03_1').modal('show');
    };



    let SearchVXT03 = function () {

        console.log("SearchVXT03_1");
     
        //if ($('#txtInvSearch').val() == "") 
        //{            
        //    if ($('#cbWh').val() == "" & ($('#cbCK').val() == "" | $('#cbCK').val() == null)) {
        //        DyAlert("請選擇店及機號查詢條件!!");
               
        //        return;
        //    }
        //}
        //else
        //{

        //}

        var VMStatus = "";
        if ($('#cbVMStatus').val()=="維護中")
            VMStatus = "M";
        else if ($('#cbVMStatus').val() == "啟用")
            VMStatus = "U";
        else if ($('#cbVMStatus').val() == "停用")
            VMStatus = "S";
        else if ($('#cbVMStatus').val() == "未配置")
            VMStatus = "N";

        var NetStatus = "";
        if ($('#cbNetStatus').val() == "正常")
            NetStatus = "Y";
        else if ($('#cbNetStatus').val() == "斷線")
            NetStatus = "N";

        var pData = {
            WhNo: $('#cbWh').val(),
            CkNo: $('#cbCK').val(),
            SWhNo: $('#cbSWh').val(),
            VMStatus: VMStatus,
            NetStatus: NetStatus
        };
        PostToWebApi({ url: "api/SystemSetup/SearchVXT03", data: pData, success: AfterSearchVXT03_1 });
    };

    let click_PLU = function (tr) {

    };

    let AfterSearchVXT03_1 = function (data) {
        if (ReturnMsg(data, 0) != "SearchVXT03OK") {
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


    let btSave_click = function () {

        //alert("btSave_click");
        //if ($('#Type_ID').val() == "" | $('#Type_ID').val() == null | $('#Type_Name').val() == "" | $('#Type_Name').val() == null | $('#DisplayNum').val() == "" | $('#DisplayNum').val() == null) {
        //    DyAlert("所有欄位都必須輸入資料!!", function () { $('#Type_ID').focus() });
        //    //DyAlert("所有欄位都必須輸入資料!!", setFocus);
        //    return;
        //}
        //if ($('#Type_ID').val() == "" | $('#Type_ID').val() == null) {
        //    DyAlert("貨倉代號欄位必須輸入資料!!", function () { $('#Type_ID').focus() });
        //    return;
        //}

        var FlagT = "";
        var FlagNet = "";

        //alert("EditMode:" + EditMode);
        var pData = {
            SNNo: sSNNo,
            FlagT: $('#FlagT').val(),
            FlagNet: $('#FlagNet').val()
        }
        PostToWebApi({ url: "api/SystemSetup/UpdateMachineList", data: pData, success: AfterUpdateMachineList });

        return

    };


    let btCancel_click = function () {
        //2021-04-27
        $('#modal_VXT03_1').modal('hide');
    };

 
    //2021-05-07
    let cbCK_click = function () {

        if ($('#cbWh').val() == "") {
            $('#cbCK').val() == ""
            //DyAlert("請先選擇店查詢條件!!", function () { $('#cbWh').focus() });
            DyAlert("請先選擇店查詢條件!!");
            return;
        }
    };

    let cbSWh_click = function () {
        //alert("cbS_click");
        //if ($('#cbWh').val() == "" | $('#cbCK').val() == "") {
        //    $('#cbLayer').val() == ""
        //    //DyAlert("請先選擇店查詢條件!!", function () { $('#cbWh').focus() });
        //    DyAlert("請先選擇店及機查詢條件!!");
        //    return;
        //}
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


    //2021-05-18
    let afterGetInitVXT03_1 = function (data) {

        //alert("afterGetInitVXT03_1");

        AssignVar();
        $('#btQueryVXT03_1').click(function () { SearchVXT03(); });

        var dtWh = data.getElementsByTagName('dtWh');
        InitSelectItem($('#cbWh')[0], dtWh, "ST_ID", "STName", true);

        var dtS = data.getElementsByTagName('dtS');
        InitSelectItem($('#cbSWh')[0], dtS, "ST_ID", "STName", true);

        $('#cbWh').change(function () { GetWhDSVCkNo(); });
        $('#cbCK').click(function () { cbCK_click(); });
        $('#cbSWh').click(function () { cbSWh_click(); });

        //$('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
        //$('#btDelete').click(function () { btDelete_click(); });
        //$('#btImportFromiXMS').click(function () { btImportFromiXMS_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        //$('.forminput input').change(function () { InputValidation(this) });
        
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
        if ($(ip).attr('id') == "USR_CODE") {
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length < 5 | str.length > 10)
                msg = "必須5~10碼英數字";
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

        //alert("afterLoadPage");
        //2021-04-27
        PostToWebApi({ url: "api/SystemSetup/GetInitVXT03", success: afterGetInitVXT03_1 });
        //alert("PostToWebApi  api/SystemSetup/GetInitVXT03_1");

        $('#pgVXT03_1').show();
        //$('#pgSysUsersEdit').hide();
    };

    if ($('#pgVXT03_1').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "VXT03_1", ["pgVXT03_1"], afterLoadPage);
    };


}