var PageISAMToFTP = function (ParentNode) {
    //let AllPages;
    //let grdU;
    //let EditMode;
    //let SetSuspend = "";
    //let isImport = false;
    //let AssignVar = function () {
    //    grdU = new DynGrid(
    //        {
    //            table_lement: $('#tbGMMacPLUSet')[0],
    //            class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td btsuspend", "tdCol3", "tdCol4", "tdCol5", "tdCol6 label-align", "tdCol7",  "tdCol8"],
    //            fields_info: [
    //                { type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
    //                { type: "JQ", name: "fa-toggle-off", element: '<i class="fa fa-toggle-off"></i><i class="fa fa-toggle-on"></i>' },
    //                { type: "Text", name: "GD_NO" },
    //                { type: "Text", name: "GD_NAME" },
    //                { type: "Text", name: "GD_Sname" },
    //                { type: "TextAmt", name: "GD_Retail" },
    //                { type: "Image", name: "Photo1" },
    //                { type: "Text", name: "GDStatus" }
    //            ],
    //            //rows_per_page: 10,
    //            method_clickrow: click_PLU,
    //            afterBind: InitModifyDeleteButton,
    //            sortable: "Y"
    //        }
    //    );
        
    //};

    //let InitModifyDeleteButton = function () {
    //    $('#tbGMMacPLUSet .fa-file-text-o').click(function () { btModify_click(this) });
    //    $('#tbGMMacPLUSet').find('.fa-toggle-off,.fa-toggle-on').click(function () { btSuspend_click(this) });
    //    var trs = $('#tbGMMacPLUSet tbody tr');
    //    for (var i = 0; i < trs.length; i++) {
    //        var tr = trs[i];
    //        DisplaySuspend(tr);
    //    }
    //}

    //let SearchPLU = function () {

    //    var sGDStatus = "";
    //    if ($('#cbGDFlag1').val() == "啟用") {
    //        sGDStatus = "1";
    //    }
    //    else if ($('#cbGDFlag1').val() == "停用") {
    //        sGDStatus = "2";
    //    }
    //    else if ($('#cbGDFlag1').val() == "未設定") {
    //        sGDStatus = "0";
    //    };


    //    var pData = {
    //        KeyWord: $('#txtPLUSearch').val(),
    //        GDDept: $('#cbDept').val(),
    //        GDBGNo: $('#cbBGNo').val(),
    //        GDStatus: sGDStatus
    //    };
    //    PostToWebApi({ url: "api/SystemSetup/SearchPLU", data: pData, success: AfterSearchPLU });
    //};

    //let click_PLU = function (tr) {

    //};

    //let AfterSearchPLU = function (data) {
    //    if (ReturnMsg(data, 0) != "SearchPLUOK") {
    //        DyAlert(ReturnMsg(data, 0));
    //        return;
    //    }
    //    else {
    //        var dtPLU = data.getElementsByTagName('dtPLU');
    //        grdU.BindData(dtPLU);
    //        if (dtPLU.length == 0) {
    //            DyAlert("無符合資料!", DummyFunction);
    //            return;
    //        }
            
    //    }
    //};

    //let DisplaySuspend = function (tr) {
    //    var trNode = $(tr).prop('Record');
    //    var sp = GetNodeValue(trNode, "GD_Flag1");
    //    var bts = $(tr).find('.btsuspend i');
    //    bts.hide();
    //    if (sp == "1")
    //        $(tr).find('.btsuspend .fa-toggle-on').show();
    //    else
    //        $(tr).find('.btsuspend .fa-toggle-off').show();
    //    var img = $(tr).prop('Photo1');
    //    var imgSGID = GetNodeValue(trNode, "Photo1");
    //    var url = "api/GetImage?SGID=" + EncodeSGID(imgSGID) + "&UU=" + encodeURIComponent(UU);
    //    url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
    //    img.prop('src', url);
    //};

    //let BlankMode = function () {
        
    //};

    //let btImportFromiXMS_click = function () {
    //    isImport = true;
    //    $('#modal_GMMacPLUSet .modal-title').text('匯入商品');
    //    $('#GD_NO,#GD_NAME').prop('readonly', false);
    //    $('#GD_NO').val('');
    //    $('#GD_NAME').val('');
    //    $('#GD_Sname').val('');
    //    $('#Photo1').val('');
    //    $('#Photo2').val('');
    //    $('#PLUPic1,#PLUPic2').attr('src', '../images/No_Pic.jpg');
    //    $('#modal_GMMacPLUSet').modal('show');
    //};

    //let btModify_click = function (bt) {
    //    isImport = false;
    //    $(bt).closest('tr').click();
    //    $('.msg-valid').hide();
    //    $('#modal_GMMacPLUSet .modal-title').text('商品維護');
    //    var node = $(grdU.ActiveRowTR()).prop('Record');
    //    $('#GD_NO,#GD_NAME').prop('readonly', true);
    //    $('#GD_NO').val(GetNodeValue(node, 'GD_NO'));
    //    $('#GD_NAME').val(GetNodeValue(node, 'GD_NAME'));
    //    $('#GD_Sname').val(GetNodeValue(node, 'GD_Sname'));
    //    $('#Photo1').val(GetNodeValue(node, 'Photo1'));
    //    $('#Photo2').val(GetNodeValue(node, 'Photo2'));
    //    $('#PLUPic1,#PLUPic2').attr('src', '../images/No_Pic.jpg');
    //    var Photo1 = GetNodeValue(node, 'Photo1');
    //    if (Photo1.length == 10)
    //        GetGetImage("PLUPic1", Photo1);
    //    else
    //        $('#PLUPic1').prop('src', '../images/No_Pic.jpg');

    //    //var Photo2 = GetNodeValue(node, 'Photo2');
    //    //if (Photo2.length == 10)
    //    //    GetGetImage("PLUPic2", Photo2); 
    //    //else
    //    //    $('#PLUPic2').prop('src', '../images/No_Pic.jpg');

    //    $('#modal_GMMacPLUSet').modal('show');
    //};

    //let GetGetImage = function (elmImg, picSGID) {
    //    var url = "api/GetImage?SGID=" + EncodeSGID(picSGID) + "&UU=" + encodeURIComponent(UU);
    //    url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
    //    $('#' + elmImg).prop('src', url);
    //}

    //let btSuspend_click = function (bt) {
    //    $(bt).closest('tr').click();
    //    var act = "停用";
    //    SetSuspend = "2";
    //    if ($(bt).hasClass('fa-toggle-off')) {
    //        act = "啟用";
    //        SetSuspend = "1";
    //    }
    //    if (grdU.ActiveRowTR() == null) {
    //        DyAlert("未選取欲" + act +"之PLU");
    //        return;
    //    }
    //    DyConfirm("確定要" + act + "這筆資料嗎?", SuspendPLU, DummyFunction);
    //};

    //let SuspendPLU = function () {
    //    var tr = grdU.ActiveRowTR();
    //    var trNode = $(tr).prop('Record');
    //    var pData = {
    //        GD_NO: GetNodeValue(trNode, "GD_NO"),
    //        SetSuspend: SetSuspend
    //    };
    //    PostToWebApi({ url: "api/SystemSetup/SuspendPLU", data: pData, success: AfterSuspendPLU });
    //};

    //let AfterSuspendPLU = function (data) {
    //    if (ReturnMsg(data, 0) != "SuspendPLUOK") {
    //        DyAlert(ReturnMsg(data, 0));
    //    }
    //    else {
    //        DyAlert("完成!");
    //        var userxml = data.getElementsByTagName('dtPLU')[0];
    //        grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
    //        var tr = grdU.ActiveRowTR();
    //        DisplaySuspend(tr);

    //    }
    //};

    //let btCancel_click = function () {
    //    $('#modal_GMMacPLUSet').modal('hide');
    //};

    //let btSave_click = function () {
    //    //$('.msg-valid').hide();
    //    //var errcount = 0;
    //    //var blankchk = $('.msg-valid.valid-blank').filter(function () {
    //    //    return $(this).prev().val() == "";
    //    //});
    //    //errcount = blankchk.length;
    //    //if (errcount > 0) {
    //    //    blankchk.text("必填欄位");
    //    //    blankchk.show();
    //    //}
    //    //errcount = $('.forminput .msg-valid').filter(function () {
    //    //    return $(this).text() != "";
    //    //}).length;

    //    //if (errcount > 0)
    //    //    return;

    //    if ($('#GD_Sname').val() == "" | $('#GD_Sname').val() == null) {
    //        DyAlert("商品簡稱欄位必須輸入資料!!", function () { $('#GD_Sname').focus() });
    //        return;
    //    }
    //    if ($('#Photo1').val() == "" | $('#Photo1').val() == null) {
    //        DyAlert("請選擇圖片資料!!");
    //        return;
    //    }
        
    //    var pData = {
    //        PLUSV: [
    //            {
    //                GD_NO: $('#GD_NO').val(),
    //                GD_Sname: $('#GD_Sname').val(),
    //                Photo1: $('#Photo1').val()
    //                //,
    //                //Photo2: $('#Photo2').val()
    //            }
    //        ]
    //    };
    //    if (!isImport)
    //        PostToWebApi({ url: "api/SystemSetup/UpdatePLU", data: pData, success: AfterUpdatePLU });
    //    else
    //        PostToWebApi({ url: "api/SystemSetup/ImportPLU", data: pData, success: AfterImportPLU });
        
    //};

    //let AfterImportPLU = function (data) {
    //    if (ReturnMsg(data, 0) != "ImportPLUOK") {
    //        DyAlert(ReturnMsg(data, 0));
    //    }
    //    else {
    //        DyAlert("匯入完成!");
    //        $('#modal_GMMacPLUSet').modal('hide');
    //    }
    //};

    //let AfterUpdatePLU = function (data) {
    //    if (ReturnMsg(data, 0) != "UpdatePLUOK") {
    //        DyAlert(ReturnMsg(data, 0));
    //    }
    //    else {
    //        DyAlert("儲存完成!");
    //        $('#modal_GMMacPLUSet').modal('hide');
    //        var userxml = data.getElementsByTagName('dtPLU')[0];
    //        grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
    //        var tr = grdU.ActiveRowTR();
    //        DisplaySuspend(tr);

    //    }
    //};


    //let afterGetInitGMMacPLUSet = function (data) {
    //    AssignVar();
    //    $('#btQueryPLU').click(function () { SearchPLU(); });
    //    $('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
    //    //$('#btDelete').click(function () { btDelete_click(); });
    //    $('#btImportFromiXMS').click(function () { btImportFromiXMS_click(); });
    //    $('#btSave').click(function () { btSave_click(); });
    //    $('#btCancel').click(function () { btCancel_click(); });
    //    //$('.forminput input').change(function () { InputValidation(this) });

    //    var dtDept = data.getElementsByTagName('dtDept');
    //    InitSelectItem($('#cbDept')[0], dtDept, "Type_ID", "Type_Name", true, "請選擇部門");

    //    var dtBGNo = data.getElementsByTagName('dtBGNo');
    //    InitSelectItem($('#cbBGNo')[0], dtBGNo, "Type_ID", "Type_Name", true, "請選擇大類");

    //    SetPLUAutoComplete("GD_NAME");
    //    SetPLUAutoComplete("GD_NO");
    //};


    //let SetPLUAutoComplete = function (inputID, apiPath) {
    //    var divmenu = $("<div></div>");
    //    divmenu.prop('id', 'EMAC' + inputID);
    //    divmenu.css("display", "block");
    //    divmenu.css("position", "relative");
    //    $('#' + inputID).after(divmenu);
    //    if (apiPath == null)
    //        apiPath = "";
    //    $('#' + inputID).autocomplete({
    //        position: { my: "left top", at: "left bottom" },
    //        appendTo: "#" + divmenu.prop('id'),
    //        source: function (request, response) {
    //            $.ajax({
    //                url: apiPath + "api/SystemSetup/GetPLUFromIXms",
    //                method: "POST",
    //                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
    //                headers: { "Authorization": "Bearer " + UU },
    //                dataType: "json",
    //                data: {
    //                    "term": request.term
    //                },
    //                success: function (data) {
    //                    response($.map(data.list, function (item) {
    //                        return {
    //                            label: item.value + ' ' + item.label,
    //                            value: item.value,
    //                            display: item.label
    //                        }
    //                    }));
    //                }
    //            });

    //        },
    //        select: function (event, ui) {
    //            $("#GD_NO").val(ui.item.value);
    //            $("#GD_NAME").val(ui.item.display);
    //            return false;
    //        }
    //    });
    //}

    //let UploadPicture = function (bt) {
    //    InitFileUpload();
    //    var UploadFileType = "PLU+Pic1";
    //    if (bt.id == "btUPPic2") {
    //        UploadFileType = "PLU+Pic2"
    //    }
    //    $('#modal-media').prop("UploadFileType", UploadFileType);
    //    $('#modal-media').modal('show');
    //};


    //let InitFileUpload = function () {
    //    $('#fileupload').fileupload({
    //        dataType: 'xml',
    //        url: "api/FileUpload",
    //        dropZone: $('#dropzone'),
    //        headers: { "Authorization": "Bearer " + UU }
    //    });

    //    $('#fileupload').bind('fileuploadfail',
    //        function (e, data) {

    //        }
    //    );

    //    $('#fileupload').bind('fileuploadsubmit', function (e, data) {
    //        data.formData = {
    //            "UploadFileType": $('#modal-media').prop("UploadFileType"),
    //            "ImgSGID": $('#' + $('#modal-media').prop("FieldName")).val()
    //        };
    //    });

    //    $('#fileupload').bind('fileuploadalways', function (e, data) {
    //        AfterFileUpoad(data);
    //    });

    //};

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
            Type: "T",
            Shop: $('#lblShop2').html().split(' ')[0]
        }
        PostToWebApi({ url: "api/SystemSetup/AddISAMToFTPRecWeb", data: cData, success: AfterAddISAMToFTPRecWeb });
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
            var whnamestr = GetNodeValue(dtWh[0], "STName");
            $('#lblShop2').html(whnamestr);
            $('#pgISAMToFTP').removeAttr('hidden');
            DyConfirm("是否要上傳 " + $('#lblShop2').html() + " 分區盤點資料？", CallSendToFTP, DummyFunction);





            //PostToWebApi({ url: "api/SystemSetup/GetInitISAM01", success: afterGetInitISAM01 });
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

    if ($('#pgISAMToFTP').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/ISAMToFTP", ["pgISAMToFTP"], afterLoadPage);
    };


}