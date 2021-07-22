var PageVSA73P = function (ParentNode) {
    let AllPages;
    let grdU;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let AssignVar = function () {
        grdU = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVSA73P')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3 label-align", "tdCol4 label-align"],
                fields_info: [
                    { type: "Text", name: "SeqNo" },
                    { type: "Text", name: "part" },
                    { type: "TextAmt", name: "Num" },
                    { type: "TextAmt", name: "Cash" }
                ],
                rows_per_page: 10,
                method_clickrow: click_PLU,
                //afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        SetDateField($('#txtOpenDateS')[0]);
        SetDateField($('#txtOpenDateE')[0]);
    };

    let InitModifyDeleteButton = function () {
        //2021-04-27
        $('#tbtest .fa-file-text-o').click(function () { btModify_click(this) });
        $('#tbtest').find('.fa-toggle-off,.fa-toggle-on').click(function () { btSuspend_click(this) });
        var trs = $('#tbtest tbody tr');
        for (var i = 0; i < trs.length; i++) {
            var tr = trs[i];
            DisplaySuspend(tr);
        }
    }

    let SearchVSA73P = function () {
        ShowLoading();
        if ($('#txtOpenDateS').val() == "") {
            CloseLoading();
            $('#txtOpenDateS').val() == ""
            DyAlert("請輸入銷售日期!!", function () { $('#txtOpenDateS').focus() });
            return;
        }
        else if ($('#txtOpenDateE').val() == "") {
            CloseLoading();
            $('#txtOpenDateE').val() == ""
            DyAlert("請輸入銷售日期!!", function () { $('#txtOpenDateE').focus() });
            return;
        }
        else {
            if (DateDiff($('#txtOpenDateS').val(), $('#txtOpenDateE').val()) > 7) {
                CloseLoading();
                $('#txtOpenDateS').val() == ""
                DyAlert("銷售日期區間不可大於7天!!", function () { $('#txtOpenDateS').focus() });
                return;
            }
            $('#lblOpenDate').html($('#txtOpenDateS').val() + "~" + $('#txtOpenDateE').val())
        }
       
        if ($('#txtGoodsNo').val() == "") {
            $('#lblGoodsNo').html("")
            if (($('#cbWh').val() == "" || $('#cbWh').val() == null) & ($('#cbCK').val() == "" || $('#cbCK').val() == null)) {
                CloseLoading();
                $('#lblShopNo').html("")
                $('#lblCkNo').html("")
                DyAlert("請選擇店及機號查詢條件!!");
                return;
            }
            else {
                if ($('#cbWh').val() == "" || $('#cbWh').val() == null)
                    $('#lblShopNo').html("")
                else
                    $('#lblShopNo').html($('#cbWh').val() + "店")

                if ($('#cbCK').val() == "" || $('#cbCK').val() == null)
                    $('#lblCkNo').html("")
                else
                    $('#lblCkNo').html($('#cbCK').val() + "機")
            }
        }
        else {
            $('#lblGoodsNo').html($('#txtGoodsNo').val())
            if ($('#cbWh').val() == "" || $('#cbWh').val() == null)
                $('#lblShopNo').html("")
            else
                $('#lblShopNo').html($('#cbWh').val() + "店")

            if ($('#cbCK').val() == "" || $('#cbCK').val() == null)
                $('#lblCkNo').html("")
            else
                $('#lblCkNo').html($('#cbCK').val() + "機")
        }
        GetVSA73P()
    };

    let GetVSA73P = function () {
        var pData = {
            OpenDateS: $('#txtOpenDateS').val(),
            OpenDateE: $('#txtOpenDateE').val(),
            ShopNo: $('#cbWh').val(),
            CkNo: $('#cbCK').val(),
            GoodsNo: $('#txtGoodsNo').val(),
        };
        PostToWebApi({ url: "api/AIReports/GetVSA73P", data: pData, success: AfterGetVSA73P });
    };

    let AfterGetVSA73P = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "GetVSA73POK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
                var dtVSA73P = data.getElementsByTagName('dtVSA73P');
                grdU.BindData(dtVSA73P);
            if (dtVSA73P.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
            else {
                var Num = 0;
                var Cash = 0;
                for (var i = 0; i < dtVSA73P.length; i++) {
                    Num += parseFloat(GetNodeValue(dtVSA73P[i], 'Num'));
                    Cash += parseFloat(GetNodeValue(dtVSA73P[i], 'Cash'));
                }
                $('#lblNum').html(Num)
                $('#lblCash').html(Cash)
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

    let btModify_click = function (bt) {
        isImport = false;
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_test .modal-title').text('商品維護');
        var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#GD_NO,#GD_NAME').prop('readonly', true);
        $('#GD_NO').val(GetNodeValue(node, 'GD_NO'));
        $('#GD_NAME').val(GetNodeValue(node, 'GD_NAME'));
        $('#GD_Sname').val(GetNodeValue(node, 'GD_Sname'));
        $('#Photo1').val(GetNodeValue(node, 'Photo1'));
        $('#Photo2').val(GetNodeValue(node, 'Photo2'));
        $('#PLUPic1,#PLUPic2').attr('src', 'images/No_Pic.jpg');
        var Photo1 = GetNodeValue(node, 'Photo1');
        if (Photo1.length == 10)
            GetGetImage("PLUPic1", Photo1);
        else
            $('#PLUPic1').prop('src', '../images/No_Pic.jpg');

        var Photo2 = GetNodeValue(node, 'Photo2');
        if (Photo2.length == 10)
            GetGetImage("PLUPic2", Photo2); 
        else
            $('#PLUPic2').prop('src', '../images/No_Pic.jpg');
        //2021-04-27
        $('#modal_test').modal('show');
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

    let GetWhCkNo = function () {
        if ($('#cbWh').val() == "") {
            $('#cbCK').empty();
            return;
        }
        else {

        }

        var pData = {
            WhNo: $('#cbWh').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetWhCkNo", data: pData, success: AfterGetWhCkNo });
    };

    let AfterGetWhCkNo = function (data) {
        if (ReturnMsg(data, 0) != "GetWhCkNoOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var dtCK = data.getElementsByTagName('dtCK');
            InitSelectItem($('#cbCK')[0], dtCK, "CKNo", "CKNo", true);
        }
    };

    //2021-04-27
    let afterGetInitVSA73P = function (data) {
        SysDate = getTodayDate();
        $('#txtOpenDateS').val(SysDate);
        $('#txtOpenDateE').val(SysDate)
        $('#lblOpenDate').html(SysDate + "~" + SysDate)
        AssignVar();
        $('#pgVSA73P .fa-search').click(function () { SearchVSA73P(); });

        var dtWarehouse = data.getElementsByTagName('dtWarehouse');
        InitSelectItem($('#cbWh')[0], dtWarehouse, "ST_ID", "ST_Sname", true, "請選擇店代號");
        $('#cbWh').change(function () { GetWhCkNo(); });
        $('#cbCK').change(function () { cbCK_click(); });
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
        PostToWebApi({ url: "api/AIReports/GetInitVSA73P", success: afterGetInitVSA73P });
        $('#pgVSA73P').show();
    };

    if ($('#pgVSA73P').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "AIReports/VSA73P", ["pgVSA73P"], afterLoadPage);
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