
var PageISAM02 = function (ParentNode) {
    let AllPages;
    let grdU;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let AssignVar = function () {

        console.log("AssignVar");

        grdU = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbInv')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7", "tdCol8 label-align", "tdCol9 label-align", "tdCol10"],
                //class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td btsuspend", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7",  "tdCol8"],
                fields_info: [
                    //{ type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
                    //{ type: "JQ", name: "fa-toggle-off", element: '<i class="fa fa-toggle-off"></i><i class="fa fa-toggle-on"></i>' },
                    { type: "Text", name: "WhNo" },
                    { type: "Text", name: "CkNo" },
                    { type: "Text", name: "Layer" },
                    { type: "Text", name: "SNO" },
                    { type: "Text", name: "PLU" },
                    { type: "Text", name: "GD_SNAME" },
                    { type: "Text", name: "EffectiveDate" },
                    { type: "TextAmt", name: "PtNum" },
                    { type: "TextAmt", name: "DisplayNum" },
                    { type: "Text", name: "Share" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                //afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        
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

    let btAdd_click = function () {
        $('#pgISAM03').show();
        $('#pgISAM03_Add').show();
        $('#pgISAM03_Query').hide();

        $('#btQuery').prop('disabled', true)
        $('#btUpLoad').prop('disabled', true)

        $('#txtQty_Add').prop('disabled', true)
        $('#btHand_Q_Add').prop('disabled', true)
        $('#btYes_Q_Add').prop('disabled', true)

        $('#txtBarcode_Add').val("")
        $('#txtQty_Add').val("")
        $('#lblQty_Add').html("")
        $('#lblCost_Add').html("")
        $('#lblGDName_Add').html("")


    };

    //編查
    let btQuery_click = function () {
        $('#pgISAM03').show();
        $('#pgISAM03_Add').hide();
        $('#pgISAM03_Query').show();

        $('#btAdd').prop('disabled', true)
        $('#btUpLoad').prop('disabled', true)

    };

    //上傳
    let btUpLoad_click = function () {
        $('#pgISAM03').show();
        $('#pgISAM03_Add').hide();
        $('#pgISAM03_Query').show();

        $('#btAdd').prop('disabled', true)
        $('#btQuery').prop('disabled', true)

    };

    //返回
    let btExit_click = function () {
        $('#pgISAM02').show();
        $('#pgISAM02_Add').hide();
        $('#pgISAM02_Query').hide();

        $('#btAdd').prop('disabled', false)
        $('#btQuery').prop('disabled', false)
        $('#btUpLoad').prop('disabled', false)

    };

    //條碼-確認(新增)
    let btYes_B_Add_click = function () {

        if ($('#txtBarcode_Add').val() == "") {
            DyAlert("請輸入條碼!");
            return;
        }
        $('#btHand_Q_Add').prop('disabled', false)
        //$('#btYes_Q_Add').prop('disabled', false)

        var pData = {
            PLU: $('#txtBarcode_Add').val(),
            WhNo: $('#lblShop').html()
        };

        PostToWebApi({ url: "api/SystemSetup/GetISAM03Collect_Add", data: pData, success: AfterGetISAM03Collect_Add });
    }

    let AfterGetISAM03Collect_Add = function (data) {
        
        //檢查條碼蒐集是否存在，若存在則數量+1，不存在則新增
        if (ReturnMsg(data, 0) != "GetISAM03Collect_AddOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            
            var dtCollectOK = data.getElementsByTagName('dtCollectOK');
            var dtCollectSumOK = data.getElementsByTagName('dtCollectSumOK');

            if (dtCollectOK.length == 0) {
                DyAlert("條碼蒐集存檔失敗，請重新確認!!");
                return;
            }
            else {
                
                //$('#txtQty_Add').val(GetNodeValue(dtCollectOK[0], 'Qty'))
                $('#lblQty_Add').html(GetNodeValue(dtCollectSumOK[0], 'SumQty'))

                $('#lblGDName_Add').html(GetNodeValue(dtCollectOK[0], 'GD_Name'))
                if (GetNodeValue(dtCollectOK[0], 'GD_Name') == "") {
                    $('#lblCost_Add').html("")
                }
                else {
                    $('#lblCost_Add').html(GetNodeValue(dtCollectOK[0], 'GD_Retail'))
                }

            }

        }
    };

    //數量-手動(新增)
    let btHand_Q_Add_click = function () {

        $('#txtQty_Add').prop('disabled', false)
        $('#btYes_Q_Add').prop('disabled', false)
    }

    //數量-確認(新增)
    let btYes_Q_Add_click = function () {
        if ($('#txtBarcode_Add').val() == "") {
            DyAlert("請輸入條碼!");
            return;
        }

        if ($('#txtQty_Add').val() == "") {
            DyAlert("請輸入數量!");
            return;
        }
        else {
           
            if (isNaN($('#txtQty_Add').val())) {
                DyAlert("請輸入數字!");
                return;
            }
            if ($('#txtQty_Add').val().indexOf(".") > 0) {
                DyAlert("請輸入整數!");
                return;
            }
          
            if ($('#txtQty_Add').val() <= 0) {
                DyAlert("數量需大於0!");
                return;
            }
        }
        
        var pData = {
            PLU: $('#txtBarcode_Add').val(),
            WhNo: $('#lblShop').html(),
            Qty: $('#txtQty_Add').val()
        };

        PostToWebApi({ url: "api/SystemSetup/GetISAM03CollectMod_Add", data: pData, success: AfterGetISAM03CollectMod_Add });

    }
   
    let AfterGetISAM02CollectMod_Add = function (data) {

        //開始修改條碼蒐集數量
        if (ReturnMsg(data, 0) != "GetISAM03CollectMod_AddOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {

            var dtCollect = data.getElementsByTagName('dtCollect');
            var dtCollectOK = data.getElementsByTagName('dtCollectOK');

            if (dtCollect.length == 0) {
                DyAlert("條碼蒐集不存在，請重新確認!!");
                return;
            }
            else {
                $('#lblQty_Add').html(GetNodeValue(dtCollectOK[0], 'SumQty'))
                $('#lblGDName_Add').html(GetNodeValue(dtCollect[0], 'GD_Name'))
                if (GetNodeValue(dtCollect[0], 'GD_Name') == "") {
                    $('#lblCost_Add').html("")
                }
                else {
                    $('#lblCost_Add').html(GetNodeValue(dtCollect[0], 'GD_Retail'))
                }

            }

        }
    };




    let click_PLU = function (tr) {

    };

    //let AfterSearchInv = function (data) {
    //    CloseLoading();
    //    if (ReturnMsg(data, 0) != "SearchInvOK") {
    //        DyAlert(ReturnMsg(data, 0));
    //        return;
    //    }
    //    else {
    //        var dtInv = data.getElementsByTagName('dtInv');
    //        if (ReturnMsg(data, 1) == "") {
    //            grdU.BindData(dtInv);
    //        }
    //        else //Excel
    //        {
    //            var url = "api/FileDownload?ID=" + EncodeSGID(ReturnMsg(data, 1));
    //            url += "&CID=" + EncodeSGID(ReturnMsg(data, 2));
    //            url += "&UID=" + EncodeSGID(ReturnMsg(data, 3));
    //            $('#iframe_for_download').prop('src', url);
    //        }
    //        //var dtInv = data.getElementsByTagName('dtInv');
    //        //grdU.BindData(dtInv);
    //        if (dtInv.length == 0) {
    //            DyAlert("無符合資料!", BlankMode);
    //            return;
    //        }
    //    }
    //    CloseLoading();
    //};

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

        console.log("GetWhCkNo");

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
            InitSelectItem($('#cbCK')[0], dtCK, "CKNo", "CKNoName", true, "*請選擇機號");
        }
    };
    
    let afterGetInitISAM03 = function (data) {

        AssignVar();
        //選擇畫面
        $('#btAdd').click(function () { btAdd_click(); });
        $('#btQuery').click(function () { btQuery_click(); });
        $('#btUpLoad').click(function () { btUpLoad_click(); });
        $('#btExit').click(function () { btExit_click(); });

        //新增畫面
        $('#btYes_B_Add').click(function () { btYes_B_Add_click(); });
        $('#btHand_Q_Add').click(function () { btHand_Q_Add_click(); });
        $('#btYes_Q_Add').click(function () { btYes_Q_Add_click(); });



        var dtISAM03Wh = data.getElementsByTagName('dtISAM02Wh');
        if (GetNodeValue(dtISAM03Wh[0], 'WhNo') == "") {
            alert("請先至店號設定進行作業店櫃設定!")
            $('#pgISAM02').hide();
            $('#pgISAM02_Add').hide();
            $('#pgISAM02_Query').hide();
            return;
        }
        else if (GetNodeValue(dtISAM03Wh[0], 'ST_SName') == "") {
            alert("請確認店櫃(" + GetNodeValue(dtISAM03Wh[0], 'WhNo') + ")是否為允許作業之店櫃!")
            $('#pgISAM02').hide();
            $('#pgISAM02_Add').hide();
            $('#pgISAM02_Query').hide();
            return;
        }
        $('#lblShop').html(GetNodeValue(dtISAM03Wh[0], 'WhNo'));
        $('#lblShopName').html(GetNodeValue(dtISAM03Wh[0], 'ST_SName'));
        
        //InitSelectItem($('#cbWh')[0], dtInvWh, "WhNo", "WhName", true, "*請選擇店代號");       

        //$('#cbWh').change(function () { GetWhCkNo(); });
        //$('#cbCK').change(function () { cbCK_click(); });
        //$('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
        //$('#btDelete').click(function () { btDelete_click(); });
        //$('#btImportFromiXMS').click(function () { btImportFromiXMS_click(); });
        //$('#btSave').click(function () { btSave_click(); });
        //$('#btCancel').click(function () { btCancel_click(); });
        //$('.forminput input').change(function () { InputValidation(this) });

        $('#pgInv #btExpoInv').click(function () { SearchInv(true); });

        
        

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

        console.log("afterLoadPage");

        PostToWebApi({ url: "api/SystemSetup/GetInitISAM03", success: afterGetInitISAM03 });

        $('#pgISAM03').show();
        $('#pgISAM03_Add').hide();
        $('#pgISAM03_Query').hide();

        //$('#pgSysUsersEdit').hide();
    };

    if ($('#pgISAM03').length == 0) {
        //2021-04-29 Debug用，按F12後，在主控台內會顯示aaaaaa
        console.log("aaaaaa");

        AllPages = new LoadAllPages(ParentNode, "ISAM03", ["pgISAM03","pgISAM03_Add","pgISAM03_Query"], afterLoadPage);
    };


}