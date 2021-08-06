var PageVIN14_2 = function (ParentNode) {
    let AllPages;
    let grdU;
    //let EditMode;
    let SetSuspend = "";
    let gDocNo = "";
    let isEntryQty = false;

    let AssignVar = function () {
        grdU = new DynGrid(
            {
                table_lement: $('#tbVIN14_2')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4 label-align", "tdCol5 label-align", "tdCol6", "tdColbt icon_in_td"],
                //class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td btsuspend", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdCol8"],
                fields_info: [
                    //{ type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
                    //{ type: "JQ", name: "fa-toggle-off", element: '<i class="fa fa-toggle-off"></i><i class="fa fa-toggle-on"></i>' },
                    { type: "Text", name: "Channel" },
                    { type: "Text", name: "GD_Sname" },
                    { type: "Text", name: "ShowQty" },
                    { type: "TextAmt", name: "ShortQty" },
                    { type: "TextAmt", name: "Qty" },
                    { type: "Text", name: "EffectiveDate" },
                    { type: "JQ", name: "fa-tags", element: '<i class="fa fa-tags"></i>' }
                    //{ type: "JQ", name: "btn-outline-success", element: '<i class="btn btn-outline-success"></i>' }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        
    };

    let InitModifyDeleteButton = function () {
        $('#tbVIN14_2 .fa-tags').click(function () { btModify_click(this) });
        //$('#tbVIN14_2 .btn-outline-success').click(function () { btModify_click(this) });
        //$('#tbVIN14_2').find('.fa-toggle-off,.fa-toggle-on').click(function () { btSuspend_click(this) });
    }

    let SearchVIN14_2 = function () {
        gDocNo = '';
        isEntryQty = false;
        if (($('#cbWh').val() == "" | $('#cbWh').val() == null) ) {
            //CloseLoading();
            DyAlert("請選擇店查詢條件!!");
            return;
        }
        if ( ($('#cbCK').val() == "" | $('#cbCK').val() == null)) {
            //CloseLoading();
            DyAlert("請選擇機號查詢條件!!");
            return;
        }
        var pData = {
            WhNo: $('#cbWh').val(),
            CkNo: $('#cbCK').val(),
            LayerNo: $('#cbLayer').val()
        };
        PostToWebApi({ url: "api/SystemSetup/SearchVIN14_2", data: pData, success: AfterSearchVIN14_2 });
    };


    let click_PLU = function (tr) {

    };


    let AfterSearchVIN14_2 = function (data) {
        
        if (ReturnMsg(data, 0) != "SearchVIN14_2OK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtPLU = data.getElementsByTagName('dtPLU');
            grdU.BindData(dtPLU);

            var dtDocNo = data.getElementsByTagName('dtDocNo');
            gDocNo = GetNodeValue(dtDocNo[0], "DocNo");
            //alert(gDocNo);

            if (dtPLU.length == 0) {
                DyAlert("無符合資料!", BlankMode);
                return;
            }
            else {
                $('#btInv').prop('disabled', false);
            }
        }
    };

 
    let BlankMode = function () {
        
    };


    let btModify_click = function (bt) {
    
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VIN14_2 .modal-title').text('商品補貨作業');
        var node = $(grdU.ActiveRowTR()).prop('Record');
        //$('#GD_NO,#GD_NAME').prop('readonly', true);
        $('#MachineData').text(GetNodeValue(node, 'WhNo') + '店 ' + GetNodeValue(node, 'CkNo') + '機 ' + GetNodeValue(node, 'ST_SName') + GetNodeValue(node, 'CkNo') + '機 ');
        $('#Channel').text(GetNodeValue(node, 'Channel'))

        $('#GD_No').text(GetNodeValue(node, 'PLU'));
        $('#GD_Name').text(GetNodeValue(node, 'GD_SName'));
        //$('#GD_Sname').val(GetNodeValue(node, 'GD_Sname'));
        $('#SeqNo').text(GetNodeValue(node, 'SeqNo'));
        $('#SeqNo').closest('.col-2').hide();

        $('#ShowQty').text(GetNodeValue(node, 'ShowQty'));
        $('#ShortQty').text(GetNodeValue(node, 'ShortQty'));

        $('#AdjQty').val(GetNodeValue(node, 'Qty'));
        $('#ExpDate').val(GetNodeValue(node, 'EffectiveDate'));

        $('#Photo1').val(GetNodeValue(node, 'Photo1'));
        //$('#Photo2').val(GetNodeValue(node, 'Photo2'));
        $('#PLUPic1,#PLUPic2').attr('src', '../images/No_Pic.jpg');
        var Photo1 = GetNodeValue(node, 'Photo1');
        if (Photo1.length == 10)
            GetGetImage("PLUPic1", Photo1);
        else
            $('#PLUPic1').prop('src', '../images/No_Pic.jpg');

        $('#modal_VIN14_2').modal('show');
    };

    let GetGetImage = function (elmImg, picSGID) {
        var url = "api/GetImage?SGID=" + EncodeSGID(picSGID) + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        $('#' + elmImg).prop('src', url);
    }


    let btCancel_click = function () {
        $('#modal_VIN14_2').modal('hide');
    };

    let btSave_click = function () {

        if ($('#AdjQty').val() == "" | $('#AdjQty').val() == null) {
            DyAlert("補貨量欄位必須輸入資料!!", function () { $('#AdjQty').focus() });
            return;
        }
        else if ($('#AdjQty').val() == "0") {
            DyAlert("補貨量必須>0!!", function () { $('#AdjQty').focus() });
            return;
        }
        else {
            if (parseInt($('#AdjQty').val()) > parseInt($('#ShortQty').text())) {
                DyAlert("補貨量不可大於缺貨量!!", function () { $('#AdjQty').focus() });
                return;
            }
        }
        if ($('#ExpDate').val() == "" | $('#ExpDate').val() == null) {
            DyAlert("最近有效日期欄位必須輸入資料!!");
            return;
        }
        //alert(gDocNo);
        var pData = {
            TempDocumentSV: [
                {
                    DocNo: gDocNo,
                    SeqNo: $('#SeqNo').text(),
                    Qty: $('#AdjQty').val(),
                    Qty2: 0,
                    ExchangeDate: $('#ExpDate').val()
                }
            ]
        };
        PostToWebApi({ url: "api/SystemSetup/UpdateTempSV", data: pData, success: AfterUpdateTempSV });
 
    };


    let AfterUpdateTempSV = function (data) {
        if (ReturnMsg(data, 0) != "UpdateTempSVOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            //DyAlert("儲存完成!");
            $('#modal_VIN14_2').modal('hide');
            var userxml = data.getElementsByTagName('dtRes')[0];
            grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
            isEntryQty = true;
            //var tr = grdU.ActiveRowTR();
        }
    };


    let GetWhDSVCkNo = function () {

        console.log("GetWhDSVCkNo");
        $('#MachineDataM').text('');
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
        PostToWebApi({ url: "api/SystemSetup/GetWhDSVCkNoWithCond", data: pData, success: AfterGetWhDSVCkNo });
    };


    let AfterGetWhDSVCkNo = function (data) {
        //alert("AfterGetWhDSVCkNo");
        if (ReturnMsg(data, 0) != "GetWhDSVCkNoWithCondOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#cbCK')[0], dtCK, "CKNo", "CkNoName", true, "*請選擇機號");
        }
    };


    let cbCK_click = function () {

        if ($('#cbWh').val() == "") {
            $('#cbCK').val() == ""
            //DyAlert("請先選擇店查詢條件!!", function () { $('#cbWh').focus() });
            DyAlert("請先選擇店查詢條件!!");
            return;
        }
        else {
            if ($('#cbCK').val() != "") {
            }
            
        }
        //GetLayerNo();
    };


    let GetLayerNo = function () {

        //console.log("GetLayerNo");
        ////$('#MachineDataM').text('');
        //if ($('#cbWh').val() == "") {
        //    $('#cbLayer').empty();
        //    return;
        //}
        //else {

        //}

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
            InitSelectItem($('#cbLayer')[0], dtCK, "LayerNo", "LayerNo", true, "請選擇貨倉");
        }
    };



    let btEqual_click = function () {

        if ($('#ShortQty').text() != "") {
            $('#AdjQty').val($('#ShortQty').text());
            //DyAlert("請先選擇店查詢條件!!", function () { $('#cbWh').focus() });
            //DyAlert("請先選擇店查詢條件!!");
            //return;
        }
    };

    let afterGetInitVIN14_2 = function (data) {
        //alert("afterGetInitVIN14_2");
        AssignVar();
        $('#btQuery').click(function () { SearchVIN14_2(); });
        //$('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
        //$('#btDelete').click(function () { btDelete_click(); });
        //$('#btImportFromiXMS').click(function () { btImportFromiXMS_click(); });
        $('#btEqual').click(function () { btEqual_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        $('#btInv').click(function () { btInv_click(); });
        $('.forminput input').change(function () { InputValidation(this) });
        $('#cbWh').change(function () { GetWhDSVCkNo(); });
        $('#cbCK').click(function () { cbCK_click(); });
        $('#cbCK').change(function () { GetLayerNo(); });
        var dtWh = data.getElementsByTagName('dtWh');
        InitSelectItem($('#cbWh')[0], dtWh, "ST_ID", "ST_SName", true, "*請選擇店代號");

        SetDateField($('#ExpDate')[0]);
        $('#ExpDate').datepicker();
        $('#btInv').prop('disabled', true);
        //SetPLUAutoComplete("GD_NAME");
        //SetPLUAutoComplete("GD_NO");
    };


    let btInv_click = function () {

        //if ($('#cbWh').val() == "") {
        //    $('#cbCK').empty();
        //    return;
        //}
        //else {

        //}
        
        if (isEntryQty == false) {
            DyAlert("尚未對任何明細商品進行補貨!!")
            return;
        }
        var pData = {
            TempDocumentSV: [
                {
                    DocNo: gDocNo,
                    WhNo: $('#cbWh').val(),
                    CkNo: $('#cbCK').val()
                }
            ]
        };
        //var pData = {
        //    DocNo: gDocNo
        //};
        PostToWebApi({ url: "api/SystemSetup/SaveInv", data: pData, success: AfterSaveInv });
    };


    let AfterSaveInv = function (data) {
        //alert("AfterGetWhDSVCkNo");
        if (ReturnMsg(data, 0) != "SaveInvOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            DyAlert("儲存過帳完成!!");

            SearchVIN14_2();

            //$('#btInv').prop('disabled', true);
            //var pData = {
            //    DocNo: gDocNo
            //};
            //PostToWebApi({ url: "api/SystemSetup/SearchVIN14_2Saved", data: pData, success: AfterSearchVIN14_2Saved });
        }
    };


    let AfterSearchVIN14_2Saved = function (data) {
        //alert("AfterSearchVIN14_2Saved");
        if (ReturnMsg(data, 0) != "SearchVIN14_2SavedOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtPLU = data.getElementsByTagName('dtPLU');
            grdU.BindData(dtPLU);

            //var dtDocNo = data.getElementsByTagName('dtDocNo');
            //gDocNo = GetNodeValue(dtDocNo[0], "DocNo");
            ////alert(gDocNo);

            //if (dtPLU.length == 0) {
            //    DyAlert("無符合資料!", BlankMode);
            //    return;
            //}
            //else {
            //    $('#btInv').prop('disabled', false);
            //}
        }
    };


    let SetPLUAutoComplete = function (inputID, apiPath) {
        var divmenu = $("<div></div>");
        divmenu.prop('id', 'EMAC' + inputID);
        divmenu.css("display", "block");
        divmenu.css("position", "relative");
        $('#' + inputID).after(divmenu);
        if (apiPath == null)
            apiPath = "";
        $('#' + inputID).autocomplete({
            position: { my: "left top", at: "left bottom" },
            appendTo: "#" + divmenu.prop('id'),
            source: function (request, response) {
                $.ajax({
                    url: apiPath + "api/SystemSetup/GetPLUFromIXms",
                    method: "POST",
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    headers: { "Authorization": "Bearer " + UU },
                    dataType: "json",
                    data: {
                        "term": request.term
                    },
                    success: function (data) {
                        response($.map(data.list, function (item) {
                            return {
                                label: item.value + ' ' + item.label,
                                value: item.value,
                                display: item.label
                            }
                        }));
                    }
                });

            },
            select: function (event, ui) {
                $("#GD_NO").val(ui.item.value);
                $("#GD_NAME").val(ui.item.display);
                return false;
            }
        });
    }

    let UploadPicture = function (bt) {
        InitFileUpload();
        var UploadFileType = "PLU+Pic1";
        if (bt.id == "btUPPic2") {
            UploadFileType = "PLU+Pic2"
        }
        $('#modal-media').prop("UploadFileType", UploadFileType);
        $('#modal-media').modal('show');
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
        if ($(ip).attr('id') == "AdjQty") {
            var re = /^[\d]/;
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
            $(ip).nextAll('.msg-valid').text(msg);
            $(ip).nextAll('.msg-valid').show();
        }
    }

    let afterLoadPage = function () {
        //alert("afterLoadPage");
        PostToWebApi({ url: "api/SystemSetup/GetInitVIN14_2", success: afterGetInitVIN14_2 });
        $('#pgVIN14_2').show();
        
    };

    if ($('#pgVIN14_2').length == 0) {
        
        AllPages = new LoadAllPages(ParentNode, "VIN14_2", ["pgVIN14_2"], afterLoadPage);
    };


}