var PageVMN29 = function (ParentNode) {
    let AllPages;
    let grdU;
    let EditMode;
    let SetSuspend = "";
    let isImport = false;
    let isDelete = false;
    //let actMode = "";
    let OldID = "";
    let AssignVar = function () {
        grdU = new DynGrid(
            {
                //2021-04-27
                table_lement: $('#tbVMN29')[0],
                class_collection: ["tdColbt icon_in_td", "tdColbt icon_in_td btsuspend", "tdCol3", "tdCol4", "tdCol5", "tdCol6", "tdCol7",  "tdCol8"],
                fields_info: [
                    { type: "JQ", name: "fa-file-text-o", element: '<i class="fa fa-file-text-o"></i>' },
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o"></i>' },
                    { type: "Text", name: "Type_ID" },
                    { type: "Text", name: "Type_NAME" },
                    { type: "Text", name: "DisplayNum" },
                    { type: "Text", name: "ModUser" },
                    { type: "Text", name: "ModDate" },
                    { type: "Text", name: "ModTime" }
                ],
                rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        
    };

    let InitModifyDeleteButton = function () {
        //2021-04-27
        $('#tbVMN29 .fa-file-text-o').click(function () { btModify_click(this) });
        $('#tbVMN29 .fa-trash-o').click(function () { btDelete_click(this) });
        //var trs = $('#tbVMN29 tbody tr');
        //for (var i = 0; i < trs.length; i++) {
        //    var tr = trs[i];
        //    DisplaySuspend(tr);
        //}
    }

    let GetRack = function () {
        
        var pData = {
            KeyWord: ""
        };
        PostToWebApi({ url: "api/SystemSetup/GetRack", data: pData, success: AfterGetRack });
    };

    let click_PLU = function (tr) {

    };

    let AfterGetRack = function (data) {

        if (ReturnMsg(data, 0) != "GetRackOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {

            var dtRack = data.getElementsByTagName('dtRack');
            grdU.BindData(dtRack);

            if (dtRack.length == 0) {
                //DyAlert("無符合資料!", BlankMode);
                return;
            }
            
        }
    };


    let BlankMode = function () {
        
    };

 
    let btModify_click = function (bt) {
        //isImport = false;
        //isDelete = false;
        EditMode = "Mod"

        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VMN29 .modal-title').text('修改貨倉類型');
        $('#modal_VMN29 .btn-danger').text('儲存');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#Type_ID,#Type_Name,#DisplayNum').prop('readonly', false);
        $('#Type_ID').val(GetNodeValue(node, 'Type_ID'));
        OldID = GetNodeValue(node, 'Type_ID');
        //alert(OldID)
        $('#Type_Name').val(GetNodeValue(node, 'Type_Name'));
        $('#DisplayNum').val(GetNodeValue(node, 'DisplayNum'));

        $('#modal_VMN29').modal('show');
    };

    let btDelete_click = function (bt) {
        //isImport = false;
        //isDelete = true;
        EditMode = "Del"
        //alert(EditMode);
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VMN29 .modal-title').text('刪除貨倉類型');
        $('#modal_VMN29 .btn-danger').text('刪除');

        var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#Type_ID,#Type_Name,#DisplayNum').prop('readonly', true);
        $('#Type_ID').val(GetNodeValue(node, 'Type_ID'));
        $('#Type_Name').val(GetNodeValue(node, 'Type_Name'));
        $('#DisplayNum').val(GetNodeValue(node, 'DisplayNum'));
 
        $('#modal_VMN29').modal('show');
    };

    let btAdd_click = function (bt) {
        //isImport = false;
        //isDelete = false;
        EditMode = "Add"
        //alert(EditMode);
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        $('#modal_VMN29 .modal-title').text('新增貨倉類型');
        $('#modal_VMN29 .btn-danger').text('儲存');
        //var node = $(grdU.ActiveRowTR()).prop('Record');
        $('#Type_ID,#Type_Name,#DisplayNum').prop('readonly', false);
        $('#Type_ID').val("");
        $('#Type_Name').val("");
        $('#DisplayNum').val("");
        //$('#Type_ID').val(GetNodeValue(node, 'Type_ID'));
        //OldID = GetNodeValue(node, 'Type_ID');
        ////alert(OldID)
        //$('#Type_Name').val(GetNodeValue(node, 'Type_Name'));
        //$('#DisplayNum').val(GetNodeValue(node, 'DisplayNum'));

        $('#modal_VMN29').modal('show');
    };

    let btCancel_click = function () {
        //2021-04-27
        $('#modal_VMN29').modal('hide');
    };

    let setFocus = function () {
        $('#Type_ID').focus();
    }

    let btSave_click = function () {
        //$('.msg-valid').hide();
        //var errcount = 0;
        //var blankchk = $('.msg-valid.valid-blank').filter(function () {
        //    return $(this).prev().val() == "";
        //});
        //errcount = blankchk.length;
        //if (errcount > 0) {
        //    blankchk.text("必填欄位");
        //    blankchk.show();
        //}
        //errcount = $('.forminput .msg-valid').filter(function () {
        //    return $(this).text() != "";
        //}).length;

        //if (errcount > 0)
        //    return;
    /*alert("btSave_click " + EditMode);*/

        //alert("btSave_click");

        if ($('#Type_ID').val() == "" | $('#Type_ID').val() == null | $('#Type_Name').val() == "" | $('#Type_Name').val() == null | $('#DisplayNum').val() == "" | $('#DisplayNum').val() == null) {
           DyAlert("所有欄位都必須輸入資料!!", function () { $('#Type_ID').focus() });
           //DyAlert("所有欄位都必須輸入資料!!", setFocus);
            return;
        }
        if ($('#Type_ID').val() == "" | $('#Type_ID').val() == null )
            {
            DyAlert("貨倉代號欄位必須輸入資料!!", function () { $('#Type_ID').focus() } );
            return;
        }
        if ($('#Type_Name').val() == "" | $('#Type_Name').val() == null) {
            DyAlert("貨倉名稱欄位必須輸入資料!!", function () { $('#Type_Name').focus() });
            return;
        }
        if ($('#DisplayNum').val() == "" | $('#DisplayNum').val() == null) {
            DyAlert("建議滿倉量欄位必須輸入資料!!", function () { $('#DisplayNum').focus() });
            return;
        }
        //alert("EditMode:" + EditMode);
        if (EditMode == "Add") {
           var pData = {
               Type_ID: $('#Type_ID').val()
            }

            PostToWebApi({ url: "api/SystemSetup/ChkRackExist", data: pData, success: AfterChkRackUsed });
        }
        else if (EditMode == "Mod") {
            var pData = {
                Type_ID: OldID
            }
 
            PostToWebApi({ url: "api/SystemSetup/ChkRackUsed", data: pData, success: AfterChkRackUsed });
        }
        else  {
            var cData = {
                Type_ID: $('#Type_ID').val()
            }

            PostToWebApi({ url: "api/SystemSetup/ChkRackUsed", data: cData, success: AfterChkRackUsed });
        }

        return

        //var pData = {
        //    Rack: [
        //        {
        //            Type_ID: $('#Type_ID').val(),
        //            Type_Name: $('#Type_Name').val(),
        //            DisplayNum: $('#DisplayNum').val()
        //        }
        //    ]
        //};
        //if (!isImport)
        //    PostToWebApi({ url: "api/SystemSetup/UpdateRack", data: pData, success: AfterUpdateRack });
        //else
        //    PostToWebApi({ url: "api/SystemSetup/ImportPLU", data: pData, success: AfterImportPLU });
        
    };

    let AfterChkRackUsed = function (data) {
        if (ReturnMsg(data, 0) != "ChkRackUsedOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtRack = data.getElementsByTagName("dtRack");
            //alert("Rack Rows:" + dtRack.length);

            if (EditMode == "Mod") {
                //alert("Mod OldID:" + OldID);
                if (OldID != $('#Type_ID').val()) {
                    if (dtRack.length > 0) {
                        DyAlert("貨倉代號已被引用，無法修改!!")
                        return;
                    }
                }

  
                var pData = {
                    Rack: [
                        {
                            OldType_ID: OldID,
                            Type_ID: $('#Type_ID').val(),
                            Type_Name: $('#Type_Name').val(),
                            DisplayNum: $('#DisplayNum').val()
                        }
                    ]
                };
                PostToWebApi({ url: "api/SystemSetup/UpdateRack", data: pData, success: AfterUpdateRack });
            }
            else if (EditMode == "Add") {
                if (dtRack.length > 0) {
                    DyAlert("貨倉代號已存在，無法新增!!")
                    return;
                }
                var pData = {
                    Rack: [
                        {
                            Type_ID: $('#Type_ID').val(),
                            Type_Name: $('#Type_Name').val(),
                            DisplayNum: $('#DisplayNum').val()
                        }
                    ]
                };
                PostToWebApi({ url: "api/SystemSetup/AddRack", data: pData, success: AfterAddRack });
            }
            else if (EditMode == "Del") {
                if (dtRack.length > 0) {
                    DyAlert("貨倉代號已被引用，無法刪除!!")
                    return;
                }

                var pData = {
                    Rack: [
                        {
                            Type_ID: $('#Type_ID').val()
                        }
                    ]
                };
                
                PostToWebApi({ url: "api/SystemSetup/DelRack", data: pData, success: AfterDelRack });
            }
                
            //DyAlert("匯入完成!");
            //$('#modal_VMN29').modal('hide');
        }
    };


    let AfterUpdateRack = function (data) {
        if (ReturnMsg(data, 0) != "UpdateRackOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("儲存完成!");
            
            $('#modal_VMN29').modal('hide');
            var userxml = data.getElementsByTagName('dtRack')[0];
            grdU.RefreshRocord(grdU.ActiveRowTR(), userxml);
        }
    };

    let AfterDelRack = function (data) {
        if (ReturnMsg(data, 0) != "DelRackOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!");

            $('#modal_VMN29').modal('hide');
            //var userxml = data.getElementsByTagName('dtRack')[0];
            grdU.DeleteRow(grdU.ActiveRowTR());
        }
    };

    let AfterAddRack = function (data) {
        if (ReturnMsg(data, 0) != "AddRackOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("新增完成!");

            $('#modal_VMN29').modal('hide');
            var userxml = data.getElementsByTagName('dtRack')[0];
            grdU.AddNew( userxml);
        }
    };

    //2021-04-27
    let afterGetInitVMN29 = function (data) {

        AssignVar();
        //$('#btAddRack').click(function () { AddRack(); });
        GetRack();
        //$('#btUPPic1,#btUPPic2').click(function () { UploadPicture(this); });
        //$('#btDelete').click(function () { btDelete_click(); });
        //$('#btImportFromiXMS').click(function () { btImportFromiXMS_click(); });
        $('#btSave').click(function () { btSave_click(); });
        $('#btCancel').click(function () { btCancel_click(); });
        $('#btAddRack').click(function () { btAdd_click(); });
        $('.forminput input').change(function () { InputValidation(this) });

        //SetPLUAutoComplete("GD_NAME");
        //SetPLUAutoComplete("GD_NO");
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



    let InputValidation = function (ip) {
        var str = $(ip).val();
        var msg = "";
        //alert("InputValidation");
        //$('.forminput .msg-valid').text('');
        //$('.forminput .msg-valid').hide();
        $(ip).nextAll('.msg-valid').text(msg);
        $(ip).nextAll('.msg-valid').show();
        if (str == "")
            return;
        if ($(ip).attr('id') == "Type_ID") {
            $(ip).val($(ip).val().toUpperCase());
            var re = /^[\d|a-zA-Z]+$/;
            if (!re.test(str) | str.length > 2 )
                msg = "必須2碼內英數字";
        }
        //if ($(ip).attr('id') == "USR_PWD") {
        //    var re = /^[\d|a-zA-Z]+$/;
        //    if (!re.test(str) | str.length < 6 | str.length > 20)
        //        msg = "必須6~20碼英數字";
        //}
        if ($(ip).attr('id') == "Type_Name") {
            if (str.length > 20)
                msg = "必須20字元以內";
        }
        if ($(ip).attr('id') == "DisplayNum") {
            var re = /^[\d]+$/;
            if (!re.test(str) | str.length > 2 | str < 0 )
                msg = "必須2位內正整數";
        }
        //if ($(ip).attr('id') == "USR_MAIL") {
        //    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        //    if (!re.test(str))
        //        msg = "e-mail格式錯誤!";
        //}
        //if ($(ip).attr('id') == "USR_MOBILE") {
        //    var re = /^09\d{8}$/;
        //    if (!re.test(str))
        //        msg = "手機格式錯誤!";
        //}
        //if ($(ip).attr('id') == "USR_NOTE") {
        //    if (str.length > 50)
        //        msg = "必須50字元以內";
        //}
        if (msg != "") {
            //$(ip).val('');
            $(ip).nextAll('.msg-valid').text(msg);
            $(ip).nextAll('.msg-valid').show();
        }
    }

    let afterLoadPage = function () {
        //2021-04-27
        //alert("afterLoadPage");
        PostToWebApi({ url: "api/SystemSetup/GetInitVMN29", success: afterGetInitVMN29 });
        $('#pgVMN29').show();
    };

    if ($('#pgVMN29').length == 0) {
        //2021-04-29 Debug用，按F12後，在主控台內會顯示aaaaaaVMN29
        //console.log("aaaaaaVMN29");
        AllPages = new LoadAllPages(ParentNode, "VMN29", ["pgVMN29"], afterLoadPage);
    };


}