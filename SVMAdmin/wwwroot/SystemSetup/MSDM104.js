var PageMSDM104 = function (ParentNode) {

    let grdM;
    let grdLookUp_ActivityCode;
    let grdLookUp_ShopNo;
    let grdLookUp_ShopNo_EDM;
    let grdLookUp_PSNO_EDM
    let cs_EditMode = "";       //狀態 Q:編查 A:新增 M:修改
    let chkShopNo = "";

    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbQMSDM104')[0],
                class_collection: ["tdColbt icon_in_td btDelete", "tdCol1", "tdCol2", "tdCol3 text-center", "tdCol4", "tdCol5", "tdCol6 label-align", "tdCol7 text-center", "tdCol8 text-center"],
                fields_info: [
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o" style="font-size:24px"></i>'},
                    { type: "Text", name: "DocNO", style: "" },
                    { type: "Text", name: "EDMMemo", style: "" },
                    { type: "Text", name: "EDDate", style: "" },
                    { type: "Text", name: "PS_Name", style: "" },
                    { type: "Text", name: "ActivityCode", style: "" },
                    { type: "TextAmt", name: "Cnt", style: "" },
                    { type: "Text", name: "ApproveDate", style: "" },
                    { type: "Text", name: "DefeasanceDate", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        grdLookUp_ActivityCode = new DynGrid(
            {
                table_lement: $('#tbDataLookup_ActivityCode')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3", "tdCol4", "tdCol5"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ActivityCode", style: "" },
                    { type: "Text", name: "PS_Name", style: "" },
                    { type: "Text", name: "StartDate", style: "" },
                    { type: "Text", name: "EndDate", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );
        grdLookUp_ShopNo = new DynGrid(
            {
                table_lement: $('#tbDataLookup_ShopNo')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ST_ID", style: "" },
                    { type: "Text", name: "ST_SName", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );
        grdLookUp_ShopNo_EDM = new DynGrid(
            {
                table_lement: $('#tbDataLookup_ShopNo_EDM')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3"],
                fields_info: [
                    { type: "checkbox", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "ST_ID", style: "" },
                    { type: "Text", name: "ST_SName", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );
        grdLookUp_PSNO_EDM = new DynGrid(
            {
                table_lement: $('#tbDataLookup_PSNO_EDM')[0],
                class_collection: ["tdCol1 text-center", "tdCol2", "tdCol3", "tdCol4", "tdCol5", "tdCol6"],
                fields_info: [
                    { type: "radio", name: "chkset", style: "width:16px;height:16px" },
                    { type: "Text", name: "PS_NO", style: "" },
                    { type: "Text", name: "PS_Name", style: "" },
                    { type: "Text", name: "ActivityCode", style: "" },
                    { type: "Text", name: "StartDate", style: "" },
                    { type: "Text", name: "EndDate", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );
        return;
    };

    let click_PLU = function (tr) {
    };

    let InitModifyDeleteButton = function () {
        $('#tbQMSDM104 .fa-trash-o').click(function () { btDelete_click(this) });
        $('#tbQMSDM104 tbody tr .tdCol1').click(function () { MSDM104Query_EDM_click(this) });
    }

    let btDelete_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        var pData = {
            DocNo: GetNodeValue(node, 'DocNo')
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104ChkDelete", data: pData, success: afterMSDM104ChkDelete });
    };

    let afterMSDM104ChkDelete = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104ChkDeleteOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("此DM單據無資料，無法刪除!");
                $(".modal-backdrop").remove();
                return;
            }
            if (GetNodeValue(dtE[0], "ApproveDate") != "") {
                DyAlert("此DM單據已批核，無法刪除!");
            }
            else {
                DyConfirm("請確認是否刪除DM單據(" + GetNodeValue(dtE[0], "DocNo") + ")？", function () {
                    var pData = {
                        DocNo: GetNodeValue(dtE[0], "DocNo")
                    }
                    PostToWebApi({ url: "api/SystemSetup/MSDM104Delete", data: pData, success: afterMSDM104Delete });
                }, function () { DummyFunction();})
            }
        }
    };

    let afterMSDM104Delete = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104DeleteOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!", function () {
                btQuery_click();
            })
        }
    };


    let DMMod_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');

        if (GetNodeValue(node, 'Type') == "A") {
            GetGetImage("Logo_A", "");
            GetGetImage("Subject_A", "");
            GetGetImage("PLUPic1_A", "");
            GetGetImage("PLUPic2_A", "");
            GetGetImage("PLUPic3_A", "");
            GetGetImage("PLUPic4_A", "");
            GetGetImage("PLUPic5_A", "");
            GetGetImage("PLUPic6_A", "");
            $('#Barcode1_A').val('');
            $('#QRCode1_A').val('');
            window.editor.setData('');

            $('#lblDocNo_DMAdd_A').html(GetNodeValue(node, 'DocNo'))
            $('#modal_DMAdd_A').modal('show');
            setTimeout(function () {
                Print_DMMod_A();
            }, 500);
        }
    };

    let Print_DMMod_A = function () {
        var pData = {
            DocNo: $('#lblDocNo_DMAdd_A').html()
        }
        PostToWebApi({ url: "api/SystemSetup/Print_DMMod_A", data: pData, success: afterPrint_DMMod_A });
    };

    let afterPrint_DMMod_A = function (data) {
        if (ReturnMsg(data, 0) != "Print_DMMod_AOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#modal_DMAdd_A').modal('hide');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            for (var i = 0; i < dtE.length; i++) {
                if (GetNodeValue(dtE[i], "DataType") == "P1") {
                    GetGetImage("Logo_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P2") {
                    GetGetImage("Subject_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P3") {
                    GetGetImage("PLUPic1_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P4") {
                    GetGetImage("PLUPic2_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P5") {
                    GetGetImage("PLUPic3_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P6") {
                    GetGetImage("PLUPic4_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P7") {
                    GetGetImage("PLUPic5_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P8") {
                    GetGetImage("PLUPic6_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "B1") {
                    $('#Barcode1_A').val(GetNodeValue(dtE[i], "Memo"))
                }
                else if (GetNodeValue(dtE[i], "DataType") == "Q1") {
                    $('#QRCode1_A').val(GetNodeValue(dtE[i], "Memo"))
                }
                else if (GetNodeValue(dtE[i], "DataType") == "T1") {
                    window.editor.setData(GetNodeValue(dtE[i], "Memo"));
                }
            }
        }
    };

    let btP2_EDM_click = function (bt) {
        //Timerset();
        var P2 = $('#P2_EDM').attr('src');
        if (P2 == "") {
            btUPEDM_click();
        }
        else {
            $('#modal_ImgUp').modal('show');
        }
    };

    let btExit_ImgUp_click = function (bt) {
        //Timerset();
        $('#modal_ImgUp').modal('hide');
    };

    let btDelete_ImgUp_click = function (bt) {
        //Timerset();
        DyConfirm("請確認是否刪除此圖片？", function () {
            var DocNo = "";
            if (cs_EditMode == "A") {
                DocNo = $('#lblVMDocNo_EDM').html();
            }
            else if (cs_EditMode == "M") {
                DocNo = $('#lblDocNo_EDM').html();
            }
            var pData = {
                DocNo: DocNo
            }
            PostToWebApi({ url: "api/SystemSetup/MSDM104DelImg", data: pData, success: afterMSDM104DelImg });
        },function () {
            DummyFunction();
        })
    };

    let afterMSDM104DelImg = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104DelImgOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#modal_ImgUp').modal('hide');
            DyAlert("圖片刪除完成!", function () {
                GetImage_EDM("P2_EDM", "");
            })
        }
    };

    let btUPEDM_click = function (bt) {
        //Timerset();
        $('#modal_ImgUp').modal('hide');

        InitFileUpload(bt);
        var UploadFileType = "P2";
        $('#modal-media').prop("UploadFileType", UploadFileType);
        $('#fileURL').val('')
        $('#modal-media').modal('show');
    };

    let InitFileUpload = function (bt) {
        $('#fileupload').fileupload({
            dataType: 'xml',
            url: "api/FileUpload_EDM",
            dropZone: $('#dropzone'),
            headers: { "Authorization": "Bearer " + UU }
        });

        $('#fileupload').bind('fileuploadfail',
            function (e, data) {
            }
        );

        $('#fileupload').bind('fileuploadsubmit', function (e, data) {
            var DocNo = ""
            if (cs_EditMode == "A") {
                DocNo = $('#lblVMDocNo_EDM').html();
            }
            else {
                DocNo = $('#lblDocNo_EDM').html();
            }
            data.formData = {
                "UploadFileType": $('#modal-media').prop("UploadFileType"),
                "ImgSGID": $('#' + $('#modal-media').prop("FieldName")).val(),
                "DocNo": DocNo
            };
        });

        $('#fileupload').bind('fileuploadalways', function (e, data) {
            AfterFileUpoad(data, bt);
        });

    };

    let AfterFileUpoad = function (returndata, bt) {
        var data = returndata.jqXHR.responseXML;
        if (ReturnMsg(data, 0) != "FileUpload_EDMOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            $('#modal-media').modal('hide');
            var UploadFileType = $('#modal-media').prop("UploadFileType");
            var DocNo = "";
            if (UploadFileType == "P2") {
                if (cs_EditMode == "A") {
                    DocNo = $('#lblVMDocNo_EDM').html();
                }
                else if (cs_EditMode == "M") {
                    DocNo = $('#lblDocNo_EDM').html();
                }
                GetImage_EDM("P2_EDM", DocNo, "P2", "N");
            }
            $('#modal-media').prop("UploadFileType", UploadFileType);
        }

    };

    let GetImage_Logo = function (elmImg, picProgramID) {
        if (picProgramID == "") {
            $('#' + elmImg).prop('src', "");
            return;
        }
        var url = "api/GetImage_Logo?ProgramID=" + picProgramID + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        $('#' + elmImg).prop('src', url);
    }

    let GetImage_EDM = function (elmImg, picDocNo, picDataType, picFlag) {
        if (picDocNo == "") {
            $('#' + elmImg).prop('src', "");
            return;
        }
        var url = "api/GetImage_EDM?DocNo=" + picDocNo + "&DataType=" + picDataType + "&Flag=" + picFlag + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        $('#' + elmImg).prop('src', url);
    }

    let GetImage_QRCode = function (elmImg, picQRCode) {
        if (picQRCode == "") {
            $('#' + elmImg).prop('src', "");
            return;
        }
        var url = "api/GetImage_QRCode?QRCode=" + picQRCode + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        $('#' + elmImg).prop('src', url);
    }

    let GetImage_Barcode = function (elmImg, picBarcode) {
        if (picBarcode == "") {
            $('#' + elmImg).prop('src', "");
            return;
        }
        var url = "api/GetImage_Barcode?Barcode=" + picBarcode + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        $('#' + elmImg).prop('src', url);
    }

    let GetImage_QRCodeandBarcode = function (elmImg, picCode) {
        if (picCode == "") {
            $('#' + elmImg).prop('src', "");
            return;
        }
        var url = "api/GetImage_QRCodeandBarcode?Code=" + picCode + "&UU=" + encodeURIComponent(UU);
        url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
        $('#' + elmImg).prop('src', url);
    }

    let Print_DM_A = function () {
        var pData = {
            DocNo: $('#lblDocNo_DMAdd_A').html()
        }
        PostToWebApi({ url: "api/SystemSetup/Print_DM_A", data: pData, success: afterPrint_DM_A });
    };

    let afterPrint_DM_A = function (data) {
        if (ReturnMsg(data, 0) != "Print_DM_AOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#modal_DM_A').modal('hide');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("請設定DM資料!", function () {
                    $('#modal_DM_A').modal('hide');
                });
                $(".modal-backdrop").remove();
                return;
            }
            for (var i = 0; i < dtE.length; i++) {
                if (GetNodeValue(dtE[i], "DataType") == "P1") {
                    GetGetImage("Logo_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P2") {
                    GetGetImage("Subject_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P3") {
                    GetGetImage("PLUPic1_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P4") {
                    GetGetImage("PLUPic2_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P5") {
                    GetGetImage("PLUPic3_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P6") {
                    GetGetImage("PLUPic4_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P7") {
                    GetGetImage("PLUPic5_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P8") {
                    GetGetImage("PLUPic6_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "B1") {
                    GetGetImage("Barcode1_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "Q1") {
                    GetGetImage("QRCode1_DM_A", GetNodeValue(dtE[i], "SGID"));
                }
                else if (GetNodeValue(dtE[i], "DataType") == "T1") {
                    var p = document.createElement('p')
                    p.innerHTML = GetNodeValue(dtE[i], "Memo")
                    var element = document.getElementById("editor_DM_A");
                    element.appendChild(p);
                }
            }
        }
    };

    //EDM按鍵控制
    let FunctionEnable_EDM = function (EditMode) {
        if (EditMode == "A") {
            $('#btMod_EDM').prop('disabled', true)
            $('#btMod_EDM').css('background-color','gray')
            $('#btSave_EDM').prop('disabled', false)
            $('#btSave_EDM').css('background-color', 'red')
            $('#btCancel_EDM').prop('disabled', true)
            $('#btCancel_EDM').css('background-color', 'gray')
            $('#btShow_EDM').prop('disabled', true)
            $('#btShow_EDM').css('background-color', 'gray')
            $('#btApp_EDM').prop('disabled', true)
            $('#btApp_EDM').css('background-color', 'gray')
            $('#btDef_EDM').prop('disabled', true)
            $('#btDef_EDM').css('background-color', 'gray')
            $('#btExit_EDM').prop('disabled', false)
            $('#btExit_EDM').css('background-color', '#6ed117')
        }
        else if (EditMode == "M") {
            $('#btMod_EDM').prop('disabled', true)
            $('#btMod_EDM').css('background-color', 'gray')
            $('#btSave_EDM').prop('disabled', false)
            $('#btSave_EDM').css('background-color', 'red')
            $('#btCancel_EDM').prop('disabled', false)
            $('#btCancel_EDM').css('background-color', 'red')
            $('#btShow_EDM').prop('disabled', true)
            $('#btShow_EDM').css('background-color', 'gray')
            $('#btApp_EDM').prop('disabled', true)
            $('#btApp_EDM').css('background-color', 'gray')
            $('#btDef_EDM').prop('disabled', true)
            $('#btDef_EDM').css('background-color', 'gray')
            $('#btExit_EDM').prop('disabled', true)
            $('#btExit_EDM').css('background-color', 'gray')
        }
        else if (EditMode == "Q") {
            //未批核 未作廢
            if ($('#lblAppDate_EDM').html() == "" && $('#lblDefDate_EDM').html() == "") {
                $('#btMod_EDM').prop('disabled', false)
                $('#btMod_EDM').css('background-color', 'red')
                $('#btSave_EDM').prop('disabled', true)
                $('#btSave_EDM').css('background-color', 'gray')
                $('#btCancel_EDM').prop('disabled', true)
                $('#btCancel_EDM').css('background-color', 'gray')
                $('#btShow_EDM').prop('disabled', false)
                $('#btShow_EDM').css('background-color', 'red')
                $('#btApp_EDM').prop('disabled', false)
                $('#btApp_EDM').css('background-color', '#3d94f6')
                $('#btDef_EDM').prop('disabled', true)
                $('#btDef_EDM').css('background-color', 'gray')
                $('#btExit_EDM').prop('disabled', false)
                $('#btExit_EDM').css('background-color', '#6ed117')
            }
            //已批核 未作廢
            else if ($('#lblAppDate_EDM').html() != "" && $('#lblDefDate_EDM').html() == "") {
                $('#btMod_EDM').prop('disabled', true)
                $('#btMod_EDM').css('background-color', 'gray')
                $('#btSave_EDM').prop('disabled', true)
                $('#btSave_EDM').css('background-color', 'gray')
                $('#btCancel_EDM').prop('disabled', true)
                $('#btCancel_EDM').css('background-color', 'gray')
                $('#btShow_EDM').prop('disabled', false)
                $('#btShow_EDM').css('background-color', 'red')
                $('#btApp_EDM').prop('disabled', true)
                $('#btApp_EDM').css('background-color', 'gray')
                $('#btDef_EDM').prop('disabled', false)
                $('#btDef_EDM').css('background-color', 'red')
                $('#btExit_EDM').prop('disabled', false)
                $('#btExit_EDM').css('background-color', '#6ed117')
            }
            //已批核 已作廢
            else if ($('#lblAppDate_EDM').html() != "" && $('#lblDefDate_EDM').html() != "") {
                $('#btMod_EDM').prop('disabled', true)
                $('#btMod_EDM').css('background-color', 'gray')
                $('#btSave_EDM').prop('disabled', true)
                $('#btSave_EDM').css('background-color', 'gray')
                $('#btCancel_EDM').prop('disabled', true)
                $('#btCancel_EDM').css('background-color', 'gray')
                $('#btShow_EDM').prop('disabled', false)
                $('#btShow_EDM').css('background-color', 'red')
                $('#btApp_EDM').prop('disabled', true)
                $('#btApp_EDM').css('background-color', 'gray')
                $('#btDef_EDM').prop('disabled', true)
                $('#btDef_EDM').css('background-color', 'gray')
                $('#btExit_EDM').prop('disabled', false)
                $('#btExit_EDM').css('background-color', '#6ed117')
            }
        }
    };

    //EDM畫面控制
    let EnableForm_EDM = function (mod) {
        $('#txtEDMMemo_EDM').prop('disabled', mod);
        $('#txtStartDate_EDM').prop('disabled', mod);
        $('#txtEndDate_EDM').prop('disabled', mod);
        $('#chkAllShopNo_EDM').prop('disabled', mod);
        if (mod == false) {
            if ($('#chkAllShopNo_EDM').prop('checked') == true) {
                $('#btShopNo_EDM').prop('disabled', true);
            }
            else {
                $('#btShopNo_EDM').prop('disabled', mod);
            }
        }
        else {
            $('#btShopNo_EDM').prop('disabled', mod);
        }
        $('#btPSNO_EDM').prop('disabled', mod);
        if (mod == true) {
            window.t1.enableReadOnlyMode('t1');         //停用
            window.t2.enableReadOnlyMode('t2');         //停用
            $('#btP2_EDM').css('pointer-events', 'none');
        }
        else {
            window.t1.disableReadOnlyMode('t1');        //啟用
            window.t2.disableReadOnlyMode('t2');        //啟用
            $('#btP2_EDM').css('pointer-events', 'unset');
        }
    };

    //EDM清除資料
    let ClearData_EDM = function () {
        $('#lblDocNo_EDM').html('');
        $('#lblAppUser_EDM').html('');
        $('#lblDefUser_EDM').html('');
        $('#txtEDMMemo_EDM').val('');
        $('#lblAppDate_EDM').html('');
        $('#lblDefDate_EDM').html('');
        $('#txtStartDate_EDM').val('');
        $('#txtEndDate_EDM').val('');
        $('#chkAllShopNo_EDM').prop('checked', true);
        chkShopNo = "";
        $('#lblShopNoCnt_EDM').html('');
        $('#txtPSNO_EDM').val('');
        $('#lblPSName_EDM').html('');
        window.t1.setData('');
        window.t2.setData('');
        GetImage_EDM("P2_EDM", "");
    };

    //EDM代入資料
    let BindForm_EDM = function (data) {
        var dtH = data.getElementsByTagName('dtH');
        var dtShop = data.getElementsByTagName('dtShop');

        $('#lblDocNo_EDM').html(GetNodeValue(dtH[0], "DocNo"));
        $('#lblAppUser_EDM').html(GetNodeValue(dtH[0], "ApproveUser"));
        $('#lblDefUser_EDM').html(GetNodeValue(dtH[0], "Defeasance"));
        $('#txtEDMMemo_EDM').val(GetNodeValue(dtH[0], "EDMMemo"));
        $('#lblAppDate_EDM').html(GetNodeValue(dtH[0], "ApproveDate"));
        $('#lblDefDate_EDM').html(GetNodeValue(dtH[0], "DefeasanceDate"));
        $('#txtStartDate_EDM').val(GetNodeValue(dtH[0], "StartDate").toString().replaceAll('/', '-'));
        $('#txtEndDate_EDM').val(GetNodeValue(dtH[0], "EndDate").toString().replaceAll('/', '-'));

        if (GetNodeValue(dtH[0], "WhNoFlag") == "Y") {
            $('#chkAllShopNo_EDM').prop('checked', true);
            $('#lblShopNoCnt_EDM').html('');
            chkShopNo = "";
        }
        else {
            $('#chkAllShopNo_EDM').prop('checked', false);
            chkShopNo = "";
            for (var i = 0; i < dtShop.length; i++) {
                chkShopNo += "'" + GetNodeValue(dtShop[i], "ShopNo") + "',";
            }
            chkShopNo = chkShopNo.substr(0, chkShopNo.length - 1)
            $('#lblShopNoCnt_EDM').html(dtShop.length.toString());
        }
        $('#txtPSNO_EDM').val(GetNodeValue(dtH[0], "PS_NO"));
        $('#lblPSName_EDM').html(GetNodeValue(dtH[0], "PS_Name"));
        for (var i = 0; i < dtH.length; i++) {
            if (GetNodeValue(dtH[i], "DataType") == "P1") {
                GetImage_EDM("P1_EDM", GetNodeValue(dtH[i], "DocNo"), GetNodeValue(dtH[i], "DataType"), "Y");
                //$('#lblCompanyLogo').html(GetNodeValue(dtH[i], "TXT"))
            }
            else if (GetNodeValue(dtH[i], "DataType") == "T1") {
                window.t1.setData(GetNodeValue(dtH[i], "TXT"));
            }
            else if (GetNodeValue(dtH[i], "DataType") == "P2") {
                GetImage_EDM("P2_EDM", GetNodeValue(dtH[i], "DocNo"), GetNodeValue(dtH[i], "DataType"), "Y");
            }
            else if (GetNodeValue(dtH[i], "DataType") == "T2") {
                window.t2.setData(GetNodeValue(dtH[i], "TXT"));
            }
        }
    };

    //EDM查詢
    let MSDM104Query_EDM_click = function (bt) {
        $('#tbQMSDM104 td').closest('tr').css('background-color', 'white');

        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#tbQMSDM104 td:contains(' + GetNodeValue(node, 'DocNo') + ')').closest('tr').css('background-color', '#DEEBF7');
        var pData = {
            DocNo: GetNodeValue(node, 'DocNo')
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104Query_EDM", data: pData, success: afterMSDM104Query_EDM });
    };

    let afterMSDM104Query_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104Query_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtH = data.getElementsByTagName('dtH');
            if (dtH.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }
            cs_EditMode = "Q"
            BindForm_EDM(data)
            FunctionEnable_EDM(cs_EditMode)
            EnableForm_EDM(true)
            $('#modal_EDM').modal('show');
        }
    };

    //EDM新增
    let btAdd_click = function (bt) {
        //Timerset();
        var pData = {
            ProgramID: "MSDM104"
        }
        PostToWebApi({ url: "api/SystemSetup/GetCompanyLogo", data: pData, success: afterGetCompanyLogo });
    };

    let afterGetCompanyLogo = function (data) {
        if (ReturnMsg(data, 0) != "GetCompanyLogoOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            GetImage_Logo("P1_EDM", "MSDM104");
            //$('#lblCompanyLogo').html(GetNodeValue(dtE[0], "Txt"))
            $('#lblVMDocNo_EDM').html(GetNodeValue(dtE[0], "DocNo"))
            cs_EditMode = "A";
            ClearData_EDM();
            FunctionEnable_EDM(cs_EditMode);
            EnableForm_EDM(false)
            $('#modal_EDM').modal('show');
        }
    };

    //EDM修改
    let btMod_EDM_click = function (bt) {
        //Timerset();
        cs_EditMode = "M"
        FunctionEnable_EDM(cs_EditMode)
        EnableForm_EDM(false)
    };

    //EDM取消
    let btCancel_EDM_click = function (bt) {
        //Timerset();
        var pData = {
            DocNo: $('#lblDocNo_EDM').html()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104Cancel_EDM", data: pData, success: afterMSDM104Cancel_EDM });
    };

    let afterMSDM104Cancel_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104Cancel_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            cs_EditMode = "Q"
            BindForm_EDM(data)
            FunctionEnable_EDM(cs_EditMode)
            EnableForm_EDM(true)
        }
    };

    //EDM儲存
    let btSave_EDM_click = function (bt) {
        //Timerset();
        EnableForm_EDM(true)
        $('#btSave_EDM').prop('disabled', true)

        if ($('#txtEDMMemo_EDM').val() == "") {
            DyAlert("請輸入DM主旨!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false)
            })
            return;
        }

        if ($('#txtStartDate_EDM').val() == "" | $('#txtEndDate_EDM').val() == "") {
            DyAlert("入會期間兩欄皆需輸入!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false)
            })
            return;
        }
        else {
            if ($('#txtStartDate_EDM').val() > $('#txtEndDate_EDM').val()) {
                DyAlert("入會開始日不可大於結束日!", function () {
                    EnableForm_EDM(false)
                    $('#btSave_EDM').prop('disabled', false)
                })
                return;
            }
        }

        if ($('#chkAllShopNo_EDM').prop('checked') == false) {
            if (chkShopNo == "") {
                DyAlert("請選擇入會店別!", function () {
                    EnableForm_EDM(false)
                    $('#btSave_EDM').prop('disabled', false)
                })
                return;
            }
        }
        if ($('#txtPSNO_EDM').val() == "") {
            DyAlert("請選擇小計折價單號!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false)
            })
            return;
        }
        var P2 = $('#P2_EDM').attr('src');
        if (P2 == "") {
            DyAlert("請設定活動圖片!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false)
            })
            return;
        }
        if (window.t2.getData() == "<p>&nbsp;</p>") {
            DyAlert("請輸入優惠券內容!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false)
            })
            return;
        }

        var WhNoFlag = ""
        if ($('#chkAllShopNo_EDM').prop('checked') == true) {
            WhNoFlag = "Y";
        }
        else {
            WhNoFlag = "N";
        }

        var T1 = ""
        if (window.t1.getData() != "<p>&nbsp;</p>") {
            T1 = window.t1.getData();
        }

        var DocNo = ""
        if (cs_EditMode == "A") {
            DocNo = ""
        }
        else if (cs_EditMode == "M") {
            DocNo = $('#lblDocNo_EDM').html();
        }

        var VMDocNo = ""
        if (cs_EditMode == "A") {
            VMDocNo = $('#lblVMDocNo_EDM').html();
        }
        else if (cs_EditMode == "M") {
            VMDocNo = "";
        }

        var pData = {
            EditMode: cs_EditMode,
            EDMMemo: $('#txtEDMMemo_EDM').val(),
            StartDate: $('#txtStartDate_EDM').val().toString().replaceAll('-', '/'),
            EndDate: $('#txtEndDate_EDM').val().toString().replaceAll('-', '/'),
            WhNoFlag: WhNoFlag,
            chkShopNo: chkShopNo,
            PS_NO: $('#txtPSNO_EDM').val(),
            T1: T1,
            T2: window.t2.getData(),
            DocNo: DocNo,
            VMDocNo: VMDocNo
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_Save_EDM", data: pData, success: afterMSDM104_Save_EDM });
    };

    let afterMSDM104_Save_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_Save_EDMOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false)
            });
        }
        else {
            var dtS = data.getElementsByTagName('dtS');
            DyAlert("存檔成功!", function () {
                if (cs_EditMode == "A") { $('#lblDocNo_EDM').html(GetNodeValue(dtS[0], "DocNo")) }
                cs_EditMode = "Q";
                FunctionEnable_EDM(cs_EditMode);
                EnableForm_EDM(true)
            })
        }
    };

    //EDM批核
    let btApp_EDM_click = function (bt) {
        //Timerset();
        DyConfirm("確定批核此DM？", function () {
            var pData = {
                DocNo: $('#lblDocNo_EDM').html()
            }
            PostToWebApi({ url: "api/SystemSetup/MSDM104Query_EDM", data: pData, success: afterMSDM104_ChkApprove_EDM });
        }, function () { DummyFunction() })
    };

    let afterMSDM104_ChkApprove_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104Query_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtH = data.getElementsByTagName('dtH');
            if (GetNodeValue(dtH[0], "ApproveDate") != "") {
                DyAlert("此DM已批核，請重新確認!", function () {
                    cs_EditMode = "Q"
                    BindForm_EDM(data)
                    FunctionEnable_EDM(cs_EditMode)
                    EnableForm_EDM(true)
                })
            }
            else {
                var pData = {
                    DocNo: $('#lblDocNo_EDM').html()
                }
                PostToWebApi({ url: "api/SystemSetup/MSDM104_Approve_EDM", data: pData, success: afterMSDM104_Approve_EDM });
            }
        }
    };

    let afterMSDM104_Approve_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_Approve_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtH = data.getElementsByTagName('dtH');
            DyAlert("批核完成!", function () {
                $('#lblAppUser_EDM').html(GetNodeValue(dtH[0], "ApproveUser"))
                $('#lblAppDate_EDM').html(GetNodeValue(dtH[0], "ApproveDate"))
                cs_EditMode = "Q"
                FunctionEnable_EDM(cs_EditMode)
                EnableForm_EDM(true)
            })
        }
    };

    //EDM作廢
    let btDef_EDM_click = function (bt) {
        //Timerset();
        DyConfirm("確定作廢此DM？", function () {
            var pData = {
                DocNo: $('#lblDocNo_EDM').html()
            }
            PostToWebApi({ url: "api/SystemSetup/MSDM104Query_EDM", data: pData, success: afterMSDM104_ChkDefeasance_EDM });
        }, function () { DummyFunction() })
    };

    let afterMSDM104_ChkDefeasance_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104Query_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtH = data.getElementsByTagName('dtH');
            if (GetNodeValue(dtH[0], "ApproveDate") != "") {
                if (GetNodeValue(dtH[0], "DefeasanceDate") != "") {
                    DyAlert("此DM已作廢，請重新確認!", function () {
                        cs_EditMode = "Q"
                        BindForm_EDM(data)
                        FunctionEnable_EDM(cs_EditMode)
                        EnableForm_EDM(true)
                    })
                }
                else {
                    var pData = {
                        DocNo: $('#lblDocNo_EDM').html()
                    }
                    PostToWebApi({ url: "api/SystemSetup/MSDM104_Defeasance_EDM", data: pData, success: afterMSDM104_Defeasance_EDM });
                }
            }
            else {
                DyAlert("此DM未批核，無法作廢!", function () {
                    cs_EditMode = "Q"
                    BindForm_EDM(data)
                    FunctionEnable_EDM(cs_EditMode)
                    EnableForm_EDM(true)
                })
            }
        }
    };

    let afterMSDM104_Defeasance_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_Defeasance_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtH = data.getElementsByTagName('dtH');
            DyAlert("作廢完成!", function () {
                $('#lblDefUser_EDM').html(GetNodeValue(dtH[0], "Defeasance"))
                $('#lblDefDate_EDM').html(GetNodeValue(dtH[0], "DefeasanceDate"))
                cs_EditMode = "Q"
                FunctionEnable_EDM(cs_EditMode)
                EnableForm_EDM(true)
            })
        }
    };

    //EDM預覽
    let btShow_EDM_click = function (bt) {
        //Timerset();
        $('#lbl_Purpose_ShowEDM').html('');
        GetImage_EDM("P1_ShowEDM", "");
        GetImage_EDM("P2_ShowEDM", "");
        GetImage_QRCodeandBarcode("Code_ShowEDM", "");
        
        var T1 = document.getElementById("T1_ShowEDM");
        T1.replaceChildren();
        var T2 = document.getElementById("T2_ShowEDM");
        T2.replaceChildren();

        $('#modal_ShowEDM').modal('show');
        setTimeout(function () {
            MSDM104ShowEDM();
        }, 500);
    };

    let MSDM104ShowEDM = function () {
        var pData = {
            DocNo: $('#lblDocNo_EDM').html()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104ShowEDM", data: pData, success: afterMSDM104ShowEDM });
    };

    let afterMSDM104ShowEDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104ShowEDMOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                $('#modal_ShowEDM').modal('hide');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("請設定DM資料!", function () {
                    $('#modal_ShowEDM').modal('hide');
                });
                $(".modal-backdrop").remove();
                return;
            }

            $('#lbl_Purpose_ShowEDM').html(GetNodeValue(dtE[0], "EDMMemo"));
            GetImage_QRCodeandBarcode("Code_ShowEDM", "123ABC");

            for (var i = 0; i < dtE.length; i++) {
                if (GetNodeValue(dtE[i], "DataType") == "P1") {
                    GetImage_EDM("P1_ShowEDM", GetNodeValue(dtE[i], "DocNo"), GetNodeValue(dtE[i], "DataType"), "Y");
                }
                else if (GetNodeValue(dtE[i], "DataType") == "T1") {
                    var p = document.createElement('p')
                    p.innerHTML = GetNodeValue(dtE[i], "TXT")
                    var T1 = document.getElementById("T1_ShowEDM");
                    T1.appendChild(p);
                }
                else if (GetNodeValue(dtE[i], "DataType") == "P2") {
                    GetImage_EDM("P2_ShowEDM", GetNodeValue(dtE[i], "DocNo"), GetNodeValue(dtE[i], "DataType"), "Y");
                }
                else if (GetNodeValue(dtE[i], "DataType") == "T2") {
                    var p = document.createElement('p')
                    p.innerHTML = GetNodeValue(dtE[i], "TXT")
                    var T2 = document.getElementById("T2_ShowEDM");
                    T2.appendChild(p);
                }
            }


        }
    };

    //EDM預覽-離開
    let btExit_ShowEDM_click = function (bt) {
        //Timerset();
        $('#modal_ShowEDM').modal('hide');
    };


    //EDM離開
    let btExit_EDM_click = function (bt) {
        //Timerset();
        $('#modal_EDM').modal('hide')
        btQuery_click();
    };

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#txtDocNo').val('');
        $('#txtActivityCode').val('');
        $('#chkNoApp').prop('checked',true);
        $('#chkApp').prop('checked', true);
        $('#txtEDMMemo').val('');
        $('#txtShopNo').val('');
        $('#chkNoDef').prop('checked', true);
        $('#chkDef').prop('checked', false);
        $('#txtEDDate').val('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#btQuery').prop('disabled', true)
        var App = "";
        var Def = "";

        if ($('#chkNoApp').prop('checked') == true && $('#chkApp').prop('checked') == false) {
            App = "NoApp"
        }
        else if ($('#chkNoApp').prop('checked') == false && $('#chkApp').prop('checked') == true) {
            App = "App"
        }
        else if ($('#chkNoApp').prop('checked') == false && $('#chkApp').prop('checked') == false) {
            DyAlert("批核識別條件至少選擇一個項目!", function () { $('#btQuery').prop('disabled', false); })
            return
        }

        if ($('#chkNoDef').prop('checked') == true && $('#chkDef').prop('checked') == false) {
            Def = "NoDef"
        }
        else if ($('#chkNoDef').prop('checked') == false && $('#chkDef').prop('checked') == true) {
            Def = "Def"
        }
        else if ($('#chkNoDef').prop('checked') == false && $('#chkDef').prop('checked') == false) {
            DyAlert("作廢識別條件至少選擇一個項目!", function () { $('#btQuery').prop('disabled', false); })
            return
        }

        ShowLoading();
        var pData = {
            DocNo: $('#txtDocNo').val(),
            EDMMemo: $('#txtEDMMemo').val(),
            ActivityCode: $('#txtActivityCode').val(),
            ShopNo: $('#txtShopNo').val().split(' ')[0],
            App: App,
            Def: Def,
            EDDate: $('#txtEDDate').val().toString().replaceAll('-', '/')
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104Query", data: pData, success: afterMSDM104Query });
    };

    let afterMSDM104Query = function (data) {
        CloseLoading();
       
        if (ReturnMsg(data, 0) != "MSDM104QueryOK") {
            DyAlert(ReturnMsg(data, 1), function () { $('#btQuery').prop('disabled', false); });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            grdM.BindData(dtE);
            if (dtE.length == 0) {
                DyAlert("無符合資料!", function () { $('#btQuery').prop('disabled', false); });
                $(".modal-backdrop").remove();
                return;
            }
            $('#btQuery').prop('disabled', false);
        }
    };

    //活動代號[...]
    let btActivityCode_click = function (bt) {
        //Timerset();
        var pData = {
            ActivityCode: $('#txtActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_LookUpActivityCode", data: pData, success: afterMSDM104_LookUpActivityCode });
    };

    let afterMSDM104_LookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_LookUpActivityCodeOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            //if (dtE.length == 0) {
            //    DyAlert("無符合資料!");
            //    $(".modal-backdrop").remove();
            //    return;
            //}
            $('#txtQLookup_ActivityCode').val($('#txtActivityCode').val());
            $('#modal_Lookup_ActivityCode').modal('show');
            setTimeout(function () {
                grdLookUp_ActivityCode.BindData(dtE);
                $('#tbDataLookup_ActivityCode tbody tr .tdCol2').filter(function () { return $(this).text() == $('#txtActivityCode').val(); }).closest('tr').find('.tdCol1 input:radio').prop('checked', true);
            }, 500);
        }
    };

    let btQLookup_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btQLookup_ActivityCode').prop('disabled', true)
        var pData = {
            ActivityCode: $('#txtQLookup_ActivityCode').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_LookUpActivityCode", data: pData, success: afterMSDM104_QLookUpActivityCode });
    };

    let afterMSDM104_QLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_LookUpActivityCodeOK") {
            $('#modal_Lookup_ActivityCode').modal('hide');
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_ActivityCode').prop('disabled', false);
                $('#modal_Lookup_ActivityCode').modal('show');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                $('#modal_Lookup_ActivityCode').modal('hide');
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_ActivityCode').prop('disabled', false);
                    $('#modal_Lookup_ActivityCode').modal('show');
                });
                $(".modal-backdrop").remove();
                return;
            }
            grdLookUp_ActivityCode.BindData(dtE);
            $('#btQLookup_ActivityCode').prop('disabled', false);
        }
    };

    let btLpOK_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btLpOK_ActivityCode').prop('disabled', true)
        var obchkedtd = $('#tbDataLookup_ActivityCode input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            $('#modal_Lookup_ActivityCode').modal('hide');
            DyAlert("未選取活動代號，請重新確認!", function () {
                $('#btLpOK_ActivityCode').prop('disabled', false);
                $('#modal_Lookup_ActivityCode').modal('show');
            });
        }
        else {
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');
            $('#txtActivityCode').val(GetNodeValue(trNode, "ActivityCode"))
            $('#btLpOK_ActivityCode').prop('disabled', false);
            $('#modal_Lookup_ActivityCode').modal('hide')
        }
    };

    let btLpExit_ActivityCode_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_ActivityCode').modal('hide')
    };

    //店別[...]
    let btShopNo_click = function (bt) {
        //Timerset();
        var pData = {
            ShopNo: $('#txtShopNo').val().split(' ')[0]
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_LookUpShopNo", data: pData, success: afterMSDM104_LookUpShopNo });
    };

    let afterMSDM104_LookUpShopNo = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_LookUpShopNoOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            //if (dtE.length == 0) {
            //    DyAlert("無符合資料!");
            //    $(".modal-backdrop").remove();
            //    return;
            //}
            $('#txtQLookup_ShopNo').val($('#txtShopNo').val().split(' ')[0]);
            $('#modal_Lookup_ShopNo').modal('show');
            setTimeout(function () {
                grdLookUp_ShopNo.BindData(dtE);
                $('#tbDataLookup_ShopNo tbody tr .tdCol2').filter(function () { return $(this).text() == $('#txtShopNo').val().split(' ')[0]; }).closest('tr').find('.tdCol1 input:radio').prop('checked', true);
            }, 500);
        }
    };

    let btQLookup_ShopNo_click = function (bt) {
        //Timerset();
        $('#btQLookup_ShopNo').prop('disabled', true)
        var pData = {
            ShopNo: $('#txtQLookup_ShopNo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_LookUpShopNo", data: pData, success: afterMSDM104_QLookUpShopNo });
    };

    let afterMSDM104_QLookUpShopNo = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_LookUpShopNoOK") {
            $('#modal_Lookup_ShopNo').modal('hide');
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_ShopNo').prop('disabled', false)
                $('#modal_Lookup_ShopNo').modal('show');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                $('#modal_Lookup_ShopNo').modal('hide');
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_ShopNo').prop('disabled', false)
                    $('#modal_Lookup_ShopNo').modal('show');
                });
                $(".modal-backdrop").remove();
                return;
            }
            grdLookUp_ShopNo.BindData(dtE);
            $('#btQLookup_ShopNo').prop('disabled', false)
        }
    };

    let btLpOK_ShopNo_click = function (bt) {
        //Timerset();
        $('#btLpOK_ShopNo').prop('disabled', true)
        var obchkedtd = $('#tbDataLookup_ShopNo input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            $('#modal_Lookup_ShopNo').modal('hide');
            DyAlert("未選取店別，請重新確認!", function () {
                $('#btLpOK_ShopNo').prop('disabled', false)
                $('#modal_Lookup_ShopNo').modal('show');
            });
        }
        else {
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');
            $('#txtShopNo').val(GetNodeValue(trNode, "ST_ID") + ' ' + GetNodeValue(trNode, "ST_SName"))
            $('#btLpOK_ShopNo').prop('disabled', false)
            $('#modal_Lookup_ShopNo').modal('hide')
        }
    };

    let btLpExit_ShopNo_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_ShopNo').modal('hide')
    };


    let GetAllShop = function () {
        //Timerset();
        if ($('#chkAllShopNo_EDM').prop('checked') == true) {
            $('#btShopNo_EDM').prop('disabled', true)
            $('#lblShopNoCnt_EDM').html('')
            $('#tbDataLookup_ShopNo_EDM tbody').empty('')
            chkShopNo = "";

            $('#txtPSNO_EDM').val('');
            $('#lblPSName_EDM').html('');
            $('#tbDataLookup_PSNO_EDM tbody').empty('')
        }
        else {
            $('#btShopNo_EDM').prop('disabled', false)

            $('#txtPSNO_EDM').val('');
            $('#lblPSName_EDM').html('');
            $('#tbDataLookup_PSNO_EDM tbody').empty('')
        }
    };

    let btShopNo_EDM_click = function (bt) {
        //Timerset();
        var pData = {
            ShopNo: ""
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_LookUpShopNo_EDM", data: pData, success: afterMSDM104_ShopNo_EDM });
    };

    let afterMSDM104_ShopNo_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_LookUpShopNo_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            //$('#modal_EDM').modal('hide');
            $('#txtQLookup_ShopNo_EDM').val('');
            $('#modal_Lookup_ShopNo_EDM').modal('show');
            setTimeout(function () {
                grdLookUp_ShopNo_EDM.BindData(dtE);
                if (chkShopNo != "") {
                    var shop = chkShopNo.split(',');
                    for (var i = 0; i < shop.length; i++) {
                        //$('#tbDataLookup_ShopNo_EDM tbody tr .tdCol2:contains("' + shop[i].replaceAll("'", "") + '")').closest('tr').find('.tdCol1 input:checkbox').prop('checked', true);
                        $('#tbDataLookup_ShopNo_EDM tbody tr .tdCol2').filter(function () { return $(this).text() == shop[i].replaceAll("'", ""); }).closest('tr').find('.tdCol1 input:checkbox').prop('checked', true);
                    }
                }
            }, 500);
        }
    };

    let btQLookup_ShopNo_EDM_click = function (bt) {
        //Timerset();
        $('#btQLookup_ShopNo_EDM').prop('disabled', true);
        var pData = {
            ShopNo: $('#txtQLookup_ShopNo_EDM').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_LookUpShopNo_EDM", data: pData, success: afterMSDM104_LookUpShopNo_EDM });
    };

    let afterMSDM104_LookUpShopNo_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_LookUpShopNo_EDMOK") {
            $('#modal_Lookup_ShopNo_EDM').modal('hide');
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_ShopNo_EDM').prop('disabled', false);
                $('#modal_Lookup_ShopNo_EDM').modal('show');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                $('#modal_Lookup_ShopNo_EDM').modal('hide');
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_ShopNo_EDM').prop('disabled', false);
                    $('#modal_Lookup_ShopNo_EDM').modal('show');
                });
                $(".modal-backdrop").remove();
                return;
            }
            grdLookUp_ShopNo_EDM.BindData(dtE);
            $('#btQLookup_ShopNo_EDM').prop('disabled', false);
        }
    };

    let btLpExit_ShopNo_EDM_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_ShopNo_EDM').modal('hide');
    };

    let btLpOK_ShopNo_EDM_click = function (bt) {
        //Timerset();
        $('#btLpOK_ShopNo_EDM').prop('disabled', true);
        var obchkedtd = $('#tbDataLookup_ShopNo_EDM .checkbox:checked');
        chkedRow = obchkedtd.length.toString();   //本次已勾選的總筆數
        if (chkedRow == 0) {
            $('#modal_Lookup_ShopNo_EDM').modal('hide');
            DyAlert("請選擇店倉資料!", function () {
                $('#btLpOK_ShopNo_EDM').prop('disabled', false);
                $('#modal_Lookup_ShopNo_EDM').modal('show');
            });
            return
        } else {
            $('#lblShopNoCnt_EDM').html(chkedRow)
            chkShopNo = "";
            for (var i = 0; i < obchkedtd.length; i++) {
                var a = $(obchkedtd[i]).closest('tr');
                var trNode = $(a).prop('Record');
                chkShopNo += "'" + GetNodeValue(trNode, "ST_ID") + "',";  //已勾選的每一筆店倉
            }
            chkShopNo = chkShopNo.substr(0, chkShopNo.length - 1)
            $('#btLpOK_ShopNo_EDM').prop('disabled', false);
            $('#modal_Lookup_ShopNo_EDM').modal('hide');
        }
    };

    let btPSNO_EDM_click = function (bt) {
        //Timerset();
        var WhNoFlag = ""
        if ($('#txtStartDate_EDM').val() == "" | $('#txtEndDate_EDM').val() == "") {
            DyAlert("入會期間兩欄皆需輸入!")
            return;
        }
        else {
            if ($('#txtStartDate_EDM').val() > $('#txtEndDate_EDM').val()) {
                DyAlert("入會開始日不可大於結束日!")
                return;
            }
        }
        if ($('#chkAllShopNo_EDM').prop('checked') == true) {
            WhNoFlag = "Y"
        }
        else {
            WhNoFlag = "N"
            if (chkShopNo == "") {
                DyAlert("請先選擇入會店別!")
                return;
            }
        }
        var pData = {
            EndDate: $('#txtEndDate_EDM').val().toString().replaceAll('-', '/'),
            WhNoFlag: WhNoFlag,
            ShopNo: chkShopNo,
            PS_NO: $('#txtPSNO_EDM').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_LookUpPSNO_EDM", data: pData, success: afterMSDM104_LookUpPSNO_EDM });
    };

    let afterMSDM104_LookUpPSNO_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_LookUpPSNO_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#txtQLookup_PSNO_EDM').val($('#txtPSNO_EDM').val())
            $('#modal_Lookup_PSNO_EDM').modal('show');
            setTimeout(function () {
                grdLookUp_PSNO_EDM.BindData(dtE);
                $('#tbDataLookup_PSNO_EDM tbody tr .tdCol2').filter(function () { return $(this).text() == $('#txtPSNO_EDM').val(); }).closest('tr').find('.tdCol1 input:radio').prop('checked', true);
            }, 500);
        }
    };

    let btQLookup_PSNO_EDM_click = function (bt) {
        //Timerset();
        $('#btQLookup_PSNO_EDM').prop('disabled', true);
        var WhNoFlag = ""
        if ($('#chkAllShopNo_EDM').prop('checked') == true) {
            WhNoFlag = "Y"
        }
        else {
            WhNoFlag = "N"
        }
        var pData = {
            EndDate: $('#txtEndDate_EDM').val().toString().replaceAll('-', '/'),
            WhNoFlag: WhNoFlag,
            ShopNo: chkShopNo,
            PS_NO: $('#txtQLookup_PSNO_EDM').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104_LookUpPSNO_EDM", data: pData, success: afterMSDM104_QLookUpPSNO_EDM });
    };

    let afterMSDM104_QLookUpPSNO_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM104_LookUpPSNO_EDMOK") {
            $('#modal_Lookup_PSNO_EDM').modal('hide');
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_PSNO_EDM').prop('disabled', false);
                $('#modal_Lookup_PSNO_EDM').modal('show');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                $('#modal_Lookup_PSNO_EDM').modal('hide');
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_PSNO_EDM').prop('disabled', false);
                    $('#modal_Lookup_PSNO_EDM').modal('show');
                });
                $(".modal-backdrop").remove();
                return;
            }
            setTimeout(function () {
                grdLookUp_PSNO_EDM.BindData(dtE);
                $('#btQLookup_PSNO_EDM').prop('disabled', false);
            }, 500);
        }
    };

    let btLpOK_PSNO_EDM_click = function (bt) {
        //Timerset();
        $('#btLpOK_PSNO_EDM').prop('disabled', true);
        var obchkedtd = $('#tbDataLookup_PSNO_EDM input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            $('#modal_Lookup_PSNO_EDM').modal('hide');
            DyAlert("未選取單號，請重新確認!", function () {
                $('#btLpOK_PSNO_EDM').prop('disabled', false);
                $('#modal_Lookup_PSNO_EDM').modal('show');
            });
        }
        else {
            var a = $(obchkedtd[0]).closest('tr');
            var trNode = $(a).prop('Record');
            $('#txtPSNO_EDM').val(GetNodeValue(trNode, "PS_NO"))
            $('#lblPSName_EDM').html(GetNodeValue(trNode, "PS_Name") + '&nbsp &nbsp' + GetNodeValue(trNode, "StartDate") + ' ~ ' + GetNodeValue(trNode, "EndDate"))
            $('#btLpOK_PSNO_EDM').prop('disabled', false);
            $('#modal_Lookup_PSNO_EDM').modal('hide')
        }
    };

    let btLpExit_PSNO_EDM_click = function (bt) {
        //Timerset();
        $('#modal_Lookup_PSNO_EDM').modal('hide');
    };

    
    //FormLoad
    let GetInitMSDM104 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }

            AssignVar();
            $('#btAdd').click(function () { btAdd_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btQuery').click(function () { btQuery_click(this) });

            $('#btActivityCode').click(function () { btActivityCode_click(this) });
            $('#btQLookup_ActivityCode').click(function () { btQLookup_ActivityCode_click(this) });
            $('#btLpOK_ActivityCode').click(function () { btLpOK_ActivityCode_click(this) });
            $('#btLpExit_ActivityCode').click(function () { btLpExit_ActivityCode_click(this) });
            $('#btShopNo').click(function () { btShopNo_click(this) });
            $('#btQLookup_ShopNo').click(function () { btQLookup_ShopNo_click(this) });
            $('#btLpOK_ShopNo').click(function () { btLpOK_ShopNo_click(this) });
            $('#btLpExit_ShopNo').click(function () { btLpExit_ShopNo_click(this) });


            $('#btMod_EDM').click(function () { btMod_EDM_click(this) });
            $('#btSave_EDM').click(function () { btSave_EDM_click(this) });
            $('#btCancel_EDM').click(function () { btCancel_EDM_click(this) });
            $('#btApp_EDM').click(function () { btApp_EDM_click(this) });
            $('#btDef_EDM').click(function () { btDef_EDM_click(this) });
            $('#btShow_EDM').click(function () { btShow_EDM_click(this) });
            $('#btExit_EDM').click(function () { btExit_EDM_click(this) });
            $('#chkAllShopNo_EDM').change(function () { GetAllShop(); });
            $('#btShopNo_EDM').click(function () { btShopNo_EDM_click(this) });
            $('#btQLookup_ShopNo_EDM').click(function () { btQLookup_ShopNo_EDM_click(this) });
            $('#btLpExit_ShopNo_EDM').click(function () { btLpExit_ShopNo_EDM_click(this) });
            $('#btLpOK_ShopNo_EDM').click(function () { btLpOK_ShopNo_EDM_click(this) });
            $('#btPSNO_EDM').click(function () { btPSNO_EDM_click(this) });
            $('#btQLookup_PSNO_EDM').click(function () { btQLookup_PSNO_EDM_click(this) });
            $('#btLpOK_PSNO_EDM').click(function () { btLpOK_PSNO_EDM_click(this) });
            $('#btLpExit_PSNO_EDM').click(function () { btLpExit_PSNO_EDM_click(this) });
            $('#btP2_EDM').click(function () { btP2_EDM_click(this) });

            $('#btExit_ShowEDM').click(function () { btExit_ShowEDM_click(this) });

            $('#btExit_ImgUp').click(function () { btExit_ImgUp_click(this) });
            $('#btDelete_ImgUp').click(function () { btDelete_ImgUp_click(this) });
            $('#btImgUp').click(function () { btUPEDM_click(this) });

            //文字編輯器
            ClassicEditor
                .create(document.querySelector('#txtT1_EDM'), {
                    toolbar: {
                        items: [
                            'Undo',                     //上一步
                            'Redo',                     //下一步
                            'bold',                     //粗體
                            'Italic',                   //斜體
                            'Underline',                //底線
                            'Strikethrough',            //刪除線
                            'fontColor',                //文字顏色
                            'fontSize',                 //文字大小
                            'FontBackgroundColor',      //文字背景顏色
                            'fontFamily',                //文字字型
                            'Indent',                    //增加縮排
                            'Outdent',             //減少縮排
                            'alignment'             //置左、置中、置右
                        ]
                    },
                    placeholder: '請在這裡填寫活動內容!',   //文字編輯器顯示的預設文字
                    removePlugins: ['Title'],           //移除文字編輯器的標題
                    fontSize: {
                        options: [10, 12, 14, 'default', 18, 20, 22,26,28,30,32,34],    //設定文字大小的格式
                        supportAllValues: true                                          //支援其他地方複製的文字大小至文字編輯器
                    },
                    fontFamily: {
                        options: [
                            'default',
                            'Arial, Helvetica, sans-serif',
                            'Courier New, Courier, monospace',
                            'Georgia, serif',
                            'Lucida Sans Unicode, Lucida Grande, sans-serif',
                            'Tahoma, Geneva, sans-serif',
                            'Times New Roman, Times, serif',
                            'Trebuchet MS, Helvetica, sans-serif',
                            'Verdana, Geneva, sans-serif'                               //設定文字字型的格式
                        ],
                        supportAllValues: true                                          //支援其他地方複製的文字字型至文字編輯器
                    },
                    alignment: {
                        options: ['left', 'center', 'right'],
                        supportAllValues: true
                    }
                })
                .then(t1 => {
                    window.t1 = t1;                             //使用window.t1.getData()取得文字編輯html內容
                    //t1.editing.view.document.on('change:isSelecting', (evt, name, value) => {
                    //   alert("ss")
                    //});
                })
                .catch(handleSampleError);

            ClassicEditor
                .create(document.querySelector('#txtT2_EDM'), {
                    toolbar: {
                        items: [
                            'Undo',                     //上一步
                            'Redo',                     //下一步
                            'bold',                     //粗體
                            'Italic',                   //斜體
                            'Underline',                //底線
                            'Strikethrough',            //刪除線
                            'fontColor',                //文字顏色
                            'fontSize',                 //文字大小
                            'FontBackgroundColor',      //文字背景顏色
                            'fontFamily',                //文字字型
                            'Indent',                    //增加縮排
                            'Outdent',                   //減少縮排
                            'alignment'             //置左、置中、置右
                        ]
                    },
                    placeholder: '請在這裡填寫優惠券內容!',   //文字編輯器顯示的預設文字
                    removePlugins: ['Title'],           //移除文字編輯器的標題
                    fontSize: {
                        options: [10, 12, 14, 'default', 18, 20, 22, 26, 28, 30, 32, 34],    //設定文字大小的格式
                        supportAllValues: true                                          //支援其他地方複製的文字大小至文字編輯器
                    },
                    fontFamily: {
                        options: [
                            'default',
                            'Arial, Helvetica, sans-serif',
                            'Courier New, Courier, monospace',
                            'Georgia, serif',
                            'Lucida Sans Unicode, Lucida Grande, sans-serif',
                            'Tahoma, Geneva, sans-serif',
                            'Times New Roman, Times, serif',
                            'Trebuchet MS, Helvetica, sans-serif',
                            'Verdana, Geneva, sans-serif'                               //設定文字字型的格式
                        ],
                        supportAllValues: true                                          //支援其他地方複製的文字字型至文字編輯器
                    },
                    resize: 50,
                    alignment: {
                        options: ['left', 'center', 'right'],
                        supportAllValues: true
                    }
                })
                .then(t2 => {
                    window.t2 = t2;                             //使用window.t2.getData()取得文字編輯html內容
                })
                .catch(handleSampleError);

            //window.t1.enableReadOnlyMode('t1');          停用
            //window.t1.disableReadOnlyMode('t1');         啟用
                    }
    };

    let handleSampleError = function (error) {
        const issueUrl = 'https://github.com/ckeditor/ckeditor5/issues';

        const message = [
            'Oops, something went wrong!',
            `Please, report the following error on ${issueUrl} with the build id "z9q36oeareu6-5mceor8expe3" and the error stack trace:`
        ].join('\n');

        console.error(message);
        console.error(error);
    }


    let afterLoadPage = function () {
        var pData = {
            ProgramID: "MSDM104"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSDM104 });
    };

    if ($('#pgMSDM104').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSDM104", ["MSDM104btns", "pgMSDM104Init", "pgMSDM104Add", "pgMSDM104Mod"], afterLoadPage);
    };


}