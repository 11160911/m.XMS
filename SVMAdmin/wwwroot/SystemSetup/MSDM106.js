var PageMSDM106 = function (ParentNode) {

    let grdM;
    let grdLookUp_ActivityCode;
    let grdLookUp_PSNO_EDM
    let cs_EditMode = "";       //狀態 Q:編查 A:新增 M:修改

    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbQMSDM106')[0],
                class_collection: ["tdColbt icon_in_td btDelete", "tdCol1", "tdCol2", "tdCol3 text-center", "tdCol4 text-center", "tdCol5 text-center", "tdCol6 text-center",  "tdCol8 text-center", "tdCol9 text-center", "tdCol10 text-center"],
                fields_info: [
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o" style="font-size:24px"></i>' },
                    { type: "Text", name: "DocNO", style: "" },
                    { type: "Text", name: "EDMMemo", style: "" },
                    { type: "Text", name: "BIR_Year", style: "" },
                    { type: "Text", name: "BIR_Month", style: "" },
                    { type: "Text", name: "PS_Name", style: "" },
                    { type: "Text", name: "ActivityCode", style: "" },
                    { type: "TextAmt", name: "Cnt2", style: "" },
                    { type: "Text", name: "ApproveDate", style: "" },
                    { type: "Text", name: "DefeasanceDate", style: "" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y",
                step: "Y"
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
                sortable: "N"
            }
        );
        return;
    };

    let click_PLU = function (tr) {
    };

    let InitModifyDeleteButton = function () {
        $('#tbQMSDM106 .fa-trash-o').click(function () { btDelete_click(this) });
        $('#tbQMSDM106 tbody tr .tdCol1,#tbQMSDM106 tbody tr .tdCol2,#tbQMSDM106 tbody tr .tdCol3,#tbQMSDM106 tbody tr .tdCol4,#tbQMSDM106 tbody tr .tdCol5,#tbQMSDM106 tbody tr .tdCol6,#tbQMSDM106 tbody tr .tdCol7,#tbQMSDM106 tbody tr .tdCol8,#tbQMSDM106 tbody tr .tdCol9').click(function () { MSDM106Query_EDM_click(this) });
    }

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
            PostToWebApi({ url: "api/SystemSetup/MSDM107DelImg", data: pData, success: afterMSDM107DelImg });
        }, function () {
            DummyFunction();
        })
    };

    let afterMSDM107DelImg = function (data) {
        if (ReturnMsg(data, 0) != "MSDM107DelImgOK") {
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
            $('#btMod_EDM').css('background-color', 'gray')
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
            $('#btExit_EDM').css('background-color', '#348000')
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
                $('#btExit_EDM').css('background-color', '#348000')
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
                $('#btExit_EDM').css('background-color', '#348000')
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
                $('#btExit_EDM').css('background-color', '#348000')
            }
        }
    };

    //EDM畫面控制
    let EnableForm_EDM = function (mod) {
        $('#txtEDMMemo_EDM').prop('disabled', mod);
        $('#cboBIRYear_EDM').prop('disabled', mod);
        $('#cboBIRMonth_EDM').prop('disabled', mod);
        
        $('#btPSNO_EDM').prop('disabled', mod);
        $('#txtPSNO_EDM').prop('disabled', mod);

        $('#chk0_EDM').prop('disabled', mod);
        $('#chk1_EDM').prop('disabled', mod);
        $('#chk2_EDM').prop('disabled', mod);
        $('#chk3_EDM').prop('disabled', mod);
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
        $('#cboBIRYear_EDM').val('');
        $('#cboBIRMonth_EDM').val('');
        $('#txtPSNO_EDM').val('');
        $('#lblPSName_EDM').html('');
        $('#chk0_EDM').prop('checked', true);
        $('#chk1_EDM').prop('checked', true);
        $('#chk2_EDM').prop('checked', true);
        $('#chk3_EDM').prop('checked', true);
        window.t1.setData('');
        window.t2.setData('');
        GetImage_EDM("P2_EDM", "");
    };

    //EDM代入資料
    let BindForm_EDM = function (data) {
        var dtH = data.getElementsByTagName('dtH');
        $('#lblDocNo_EDM').html(GetNodeValue(dtH[0], "DocNo"));
        $('#lblAppUser_EDM').html(GetNodeValue(dtH[0], "ApproveUser"));
        $('#lblDefUser_EDM').html(GetNodeValue(dtH[0], "Defeasance"));
        $('#txtEDMMemo_EDM').val(GetNodeValue(dtH[0], "EDMMemo"));
        $('#lblAppDate_EDM').html(GetNodeValue(dtH[0], "ApproveDate"));
        $('#lblDefDate_EDM').html(GetNodeValue(dtH[0], "DefeasanceDate"));
        $('#cboBIRYear_EDM').val(GetNodeValue(dtH[0], "BIR_Year"));
        $('#cboBIRMonth_EDM').val(GetNodeValue(dtH[0], "BIR_Month"));
        $('#txtPSNO_EDM').val(GetNodeValue(dtH[0], "PS_NO"));
        $('#lblPSName_EDM').html(GetNodeValue(dtH[0], "PS_Name"));

        if (GetNodeValue(dtH[0], "VIP_Type").indexOf("0") >= 0) {
            $('#chk0_EDM').prop('checked', true);
        }
        else {
            $('#chk0_EDM').prop('checked', false);
        }
        if (GetNodeValue(dtH[0], "VIP_Type").indexOf("1") >= 0) {
            $('#chk1_EDM').prop('checked', true);
        }
        else {
            $('#chk1_EDM').prop('checked', false);
        }
        if (GetNodeValue(dtH[0], "VIP_Type").indexOf("2") >= 0) {
            $('#chk2_EDM').prop('checked', true);
        }
        else {
            $('#chk2_EDM').prop('checked', false);
        }
        if (GetNodeValue(dtH[0], "VIP_Type").indexOf("3") >= 0) {
            $('#chk3_EDM').prop('checked', true);
        }
        else {
            $('#chk3_EDM').prop('checked', false);
        }

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
    let MSDM106Query_EDM_click = function (bt) {
        //$('#tbQMSDM106 td').closest('tr').css('background-color', 'transparent');

        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        //$('#tbQMSDM106 td:contains(' + GetNodeValue(node, 'DocNo') + ')').closest('tr').css('background-color', '#DEEBF7');
        var pData = {
            DocNo: GetNodeValue(node, 'DocNo')
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM106Query_EDM", data: pData, success: afterMSDM106Query_EDM });
    };

    let afterMSDM106Query_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106Query_EDMOK") {
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
            ClearData_EDM()
            BindForm_EDM(data)
            FunctionEnable_EDM(cs_EditMode)
            EnableForm_EDM(true)
            $('#modal_EDM').modal('show');
        }
    };

    //EDM刪除
    let btDelete_click = function (bt) {
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        var pData = {
            DocNo: GetNodeValue(node, 'DocNo')
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM104ChkDelete", data: pData, success: afterMSDM106ChkDelete });
    };

    let afterMSDM106ChkDelete = function (data) {
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
            if (GetNodeValue(dtE[0], "ApproveDate").toString() != "") {
                DyAlert("此DM單據已批核，無法刪除!");
            }
            else {
                DyConfirm("請確認是否刪除DM單據(" + GetNodeValue(dtE[0], "DocNo") + ")？", function () {
                    var pData = {
                        DocNo: GetNodeValue(dtE[0], "DocNo")
                    }
                    PostToWebApi({ url: "api/SystemSetup/MSDMDelete", data: pData, success: afterMSDM106Delete });
                }, function () { DummyFunction(); })
            }
        }
    };

    let afterMSDM106Delete = function (data) {
        if (ReturnMsg(data, 0) != "MSDMDeleteOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("刪除完成!", function () {
                btQuery_click();
            })
        }
    };
    //EDM新增
    let btAdd_click = function (bt) {
        //Timerset();
        var pData = {
            ProgramID: "MSDM106"
        }
        PostToWebApi({ url: "api/SystemSetup/GetCompanyLogo", data: pData, success: afterGetCompanyLogo });
    };

    let afterGetCompanyLogo = function (data) {
        if (ReturnMsg(data, 0) != "GetCompanyLogoOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            GetImage_Logo("P1_EDM", "MSDM106");
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
        PostToWebApi({ url: "api/SystemSetup/MSDM106Cancel_EDM", data: pData, success: afterMSDM106Cancel_EDM });
    };

    let afterMSDM106Cancel_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106Cancel_EDMOK") {
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
        $('#btSave_EDM').prop('disabled', true);

        if ($('#txtEDMMemo_EDM').val() == "") {
            DyAlert("請輸入DM主旨!", function () {
                EnableForm_EDM(false);
                $('#btSave_EDM').prop('disabled', false);
            })
            return;
        }

        if ($('#cboBIRYear_EDM').val() == "") {
            DyAlert("請選擇DM年度!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false);
            })
            return;
        }

        if ($('#chk0_EDM').prop('checked') == false && $('#chk1_EDM').prop('checked') == false && $('#chk2_EDM').prop('checked') == false && $('#chk3_EDM').prop('checked') == false) {
            DyAlert("請選擇會員卡別!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false);
            })
            return;
        }

        if ($('#cboBIRMonth_EDM').val() == "") {
            DyAlert("請選擇生日月份!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false);
            })
            return;
        }
        var BRDate = $('#cboBIRYear_EDM').val() + "/" + $('#cboBIRMonth_EDM').val() + "/" + "31";
        var BDate = new Date(BRDate);
        var getDate = new Date();
        if (BDate < getDate) {
            DyAlert("年度+月份請選擇大於等於目前年月!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false);
            })
            return;
        }

        //if ($('#txtPSNO_EDM').val() == "") {
        //    DyAlert("請選擇小計折價單號!", function () {
        //        EnableForm_EDM(false)
        //        $('#btSave_EDM').prop('disabled', false);
        //    })
        //    return;
        //}
        //var P2 = $('#P2_EDM').attr('src');
        //if (P2 == "") {
        //    DyAlert("請設定活動圖片!", function () { EnableForm_EDM(false) })
        //    return;
        //}
        //if (window.t2.getData() == "<p>&nbsp;</p>") {
        //    DyAlert("請輸入優惠券內容!", function () {
        //        EnableForm_EDM(false)
        //        $('#btSave_EDM').prop('disabled', false);
        //    })
        //    return;
        //}

        var P2 = $('#P2_EDM').attr('src');
        if (window.t1.getData() == "<p>&nbsp;</p>" && P2 == "" && window.t2.getData() == "<p>&nbsp;</p>") {
            DyAlert("活動內容、活動圖、優惠券內容，請擇一輸入!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false);
            })
            return;
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

        var VIPType = ""
        if ($('#chk0_EDM').prop('checked') == true) {
            VIPType += "0";
        }
        if ($('#chk1_EDM').prop('checked') == true) {
            VIPType += "1";
        }
        if ($('#chk2_EDM').prop('checked') == true) {
            VIPType += "2";
        }
        if ($('#chk3_EDM').prop('checked') == true) {
            VIPType += "3";
        }

        var pData = {
            EditMode: cs_EditMode,
            EDMMemo: $('#txtEDMMemo_EDM').val(),
            BIRYear: $('#cboBIRYear_EDM').val(),
            BIRMonth: $('#cboBIRMonth_EDM').val(),
            PS_NO: $('#txtPSNO_EDM').val(),
            T1: window.t1.getData(),
            T2: window.t2.getData(),
            VIPType: VIPType,
            DocNo: DocNo,
            VMDocNo: VMDocNo
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM106_Save_EDM", data: pData, success: afterMSDM106_Save_EDM });
    };

    let afterMSDM106_Save_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106_Save_EDMOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false);
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
        var BRDate = $('#cboBIRYear_EDM').val() + "/" + $('#cboBIRMonth_EDM').val() + "/" + "31";
        var BDate = new Date(BRDate);
        var getDate = new Date();
        if (BDate < getDate) {
            DyAlert("年度+月份請選擇大於等於目前年月!", function () {

            })
            return;
        }

        var pData = {
            BYear: $('#cboBIRYear_EDM').val(),
            BMonth: $('#cboBIRMonth_EDM').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM106QuerySame_EDM", data: pData, success: afterChkSame_EDM });
    };

    let afterChkSame_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106QuerySame_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtSame = data.getElementsByTagName('dtSame');
            if (dtSame.length > 0) {
                DyConfirm("有重覆設定相同年度月份的DM，是否仍要批核?", function () {
                    ChkApprove();
                }, function () { DummyFunction() })
            }
            else {
                ChkApprove();
            }
        }
    };
    let ChkApprove = function () {
        var pData = {
            DocNo: $('#lblDocNo_EDM').html()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM106Query_EDM", data: pData, success: afterChkApprove_EDM });
    };

    let afterChkApprove_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106Query_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtH = data.getElementsByTagName('dtH');

            if (GetNodeValue(dtH[0], "ApproveDate").toString() != "") {
                DyAlert("此DM已批核，請重新確認!", function () {
                    cs_EditMode = "Q"
                    BindForm_EDM(data)
                    FunctionEnable_EDM(cs_EditMode)
                    EnableForm_EDM(true)
                })
            }
            else {
                DyConfirm("確定批核此DM？", function () {
                    var pData = {
                        DocNo: $('#lblDocNo_EDM').html()
                    }
                    PostToWebApi({ url: "api/SystemSetup/MSDM106Approve_EDM", data: pData, success: afterMSDM106Approve_EDM });
                }, function () { DummyFunction() })
            }
        }
    };

    let afterMSDM106Approve_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106Approve_EDMOK") {
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
            PostToWebApi({ url: "api/SystemSetup/MSDM106ChkEDMVIP_EDM", data: pData, success: afterChkEDMVIP_EDM });
        }, function () { DummyFunction() })
    };
    let afterChkEDMVIP_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106ChkEDMVIP_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtEDMVIP = data.getElementsByTagName('dtEDMVIP');
            if (GetNodeValue(dtEDMVIP[0], "ApproveDate") != "") {
                DyAlert("此DM已完成發送，不可作廢!", function () {
                    cs_EditMode = "Q"
                    /*BindForm_EDM(data)*/
                    FunctionEnable_EDM(cs_EditMode)
                    EnableForm_EDM(true)
                })
            }
            else {
                var pData = {
                    DocNo: $('#lblDocNo_EDM').html()
                }
                PostToWebApi({ url: "api/SystemSetup/MSDM106Query_EDM", data: pData, success: afterChkDefeasance_EDM });
            }
        }
    };

    let afterChkDefeasance_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106Query_EDMOK") {
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
                    PostToWebApi({ url: "api/SystemSetup/MSDM106Defeasance_EDM", data: pData, success: afterMSDMDefeasance_EDM });
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

    let afterMSDMDefeasance_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106Defeasance_EDMOK") {
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
        MSDM106ShowEDM();
        //$('#lbl_Purpose_ShowEDM').html('');
        //GetImage_EDM("P1_ShowEDM", "");
        //GetImage_EDM("P2_ShowEDM", "");
        //GetImage_QRCodeandBarcode("Code_ShowEDM", "");

        //var T1 = document.getElementById("T1_ShowEDM");
        //T1.replaceChildren();
        //var T2 = document.getElementById("T2_ShowEDM");
        //T2.replaceChildren();

        //$('#modal_ShowEDM').modal('show');
        //setTimeout(function () {
        //    MSDM107ShowEDM();
        //}, 500);
    };

    let MSDM106ShowEDM = function () {
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/GetCompanyShowEDM", data: pData, success: afterMSDM106ShowEDM });
    };

    let afterMSDM106ShowEDM = function (data) {
        if (ReturnMsg(data, 0) != "GetCompanyShowEDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("此DM單據無資料，無法預覽!");
                $(".modal-backdrop").remove();
                return;
            }
            var hostname = location.hostname;
            //測試環境
            if (hostname.indexOf("94") >= 0 || hostname.indexOf("localhost") >= 0) {
                window.open("http://192.168.1.94/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + $('#lblDocNo_EDM').html() + "");
            }
            //正式環境
            else {
                window.open("https://www.portal.e-dynasty.com.tw/ShowEDMWEB/ShowEDMWEB?company=" + GetNodeValue(dtE[0], "CompanyID") + ";" + $('#lblDocNo_EDM').html() + "");
            }

            //$('#lbl_Purpose_ShowEDM').html(GetNodeValue(dtE[0], "EDMMemo"));
            //GetImage_QRCodeandBarcode("Code_ShowEDM", "123ABC");

            //for (var i = 0; i < dtE.length; i++) {
            //    if (GetNodeValue(dtE[i], "DataType") == "P1") {
            //        GetImage_EDM("P1_ShowEDM", GetNodeValue(dtE[i], "DocNo"), GetNodeValue(dtE[i], "DataType"), "Y");
            //    }
            //    else if (GetNodeValue(dtE[i], "DataType") == "T1") {
            //        var p = document.createElement('p')
            //        p.innerHTML = GetNodeValue(dtE[i], "TXT")
            //        var T1 = document.getElementById("T1_ShowEDM");
            //        T1.appendChild(p);
            //    }
            //    else if (GetNodeValue(dtE[i], "DataType") == "P2") {
            //        GetImage_EDM("P2_ShowEDM", GetNodeValue(dtE[i], "DocNo"), GetNodeValue(dtE[i], "DataType"), "Y");
            //    }
            //    else if (GetNodeValue(dtE[i], "DataType") == "T2") {
            //        var p = document.createElement('p')
            //        p.innerHTML = GetNodeValue(dtE[i], "TXT")
            //        var T2 = document.getElementById("T2_ShowEDM");
            //        T2.appendChild(p);
            //    }
            //}
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
        $('#txtEDMMemo').val('');
        $('#chkNoDef').prop('checked', true);
        $('#chkDef').prop('checked', false);
        $('#chkNoApp').prop('checked', true);
        $('#chkApp').prop('checked', true);
        $('#cboBIRYear').val('');
        $('#cboBIRMonth').val('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        //$('#tbQMSDM106 thead tr th').css('background-color', '#ffb620')
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
            App: App,
            Def: Def,
            EDM_Model: "",
            BirYear: $('#cboBIRYear').val(),
            BirMonth: $('#cboBIRMonth').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM106Query", data: pData, success: afterMSDMQuery });
    };

    let afterMSDMQuery = function (data) {
        CloseLoading();

        if (ReturnMsg(data, 0) != "MSDM106QueryOK") {
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
            ActivityCode: $('#txtActivityCode').val(),
            EDM_Model: ""
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM106LookUpActivityCode", data: pData, success: afterMSDMLookUpActivityCode });
    };

    let afterMSDMLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106LookUpActivityCodeOK") {
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
            ActivityCode: $('#txtQLookup_ActivityCode').val(),
            EDM_Model: ""
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM106LookUpActivityCode", data: pData, success: afterQMSDMLookUpActivityCode });
    };

    let afterQMSDMLookUpActivityCode = function (data) {
        if (ReturnMsg(data, 0) != "MSDM106LookUpActivityCodeOK") {
            //$('#modal_Lookup_ActivityCode').modal('hide');
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_ActivityCode').prop('disabled', false)
                //$('#modal_Lookup_ActivityCode').modal('show');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                //$('#modal_Lookup_ActivityCode').modal('hide');
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_ActivityCode').prop('disabled', false)
                    //$('#modal_Lookup_ActivityCode').modal('show');
                });
                //$(".modal-backdrop").remove();
                return;
            }
            grdLookUp_ActivityCode.BindData(dtE);
            $('#btQLookup_ActivityCode').prop('disabled', false)
        }
    };

    let btLpOK_ActivityCode_click = function (bt) {
        //Timerset();
        $('#btLpOK_ActivityCode').prop('disabled', true)
        var obchkedtd = $('#tbDataLookup_ActivityCode input[type="radio"]:checked');
        var chkedRow = obchkedtd.length.toString();

        if (chkedRow == 0) {
            //$('#modal_Lookup_ActivityCode').modal('hide');
            DyAlert("未選取活動代號，請重新確認!", function () {
                $('#btLpOK_ActivityCode').prop('disabled', false);
                //$('#modal_Lookup_ActivityCode').modal('show');
            });
            //$(".modal-backdrop").remove();
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

    let btLpClear_ActivityCode_click = function (bt) {
        //Timerset();
        $("#txtQLookup_ActivityCode").val('');
        $("#tbDataLookup_ActivityCode .checkbox").prop('checked', false);
    };

    let btPSNO_EDM_click = function (bt) {
        //Timerset();
        if ($('#cboBIRYear_EDM').val() == "") {
            DyAlert("請選擇DM年度!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false);
            })
            return;
        }

        if ($('#cboBIRMonth_EDM').val() == "") {
            DyAlert("請選擇生日月份!", function () {
                EnableForm_EDM(false)
                $('#btSave_EDM').prop('disabled', false);
            })
            return;
        }
        var EDate = $('#cboBIRYear_EDM').val() + "/" + $('#cboBIRMonth_EDM').val() + "/" + "31";
        var pData = {
            EndDate: EDate,
            PS_NO: $('#txtPSNO_EDM').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM107_LookUpPSNO_EDM", data: pData, success: afterMSDM107_LookUpPSNO_EDM });
    };

    let afterMSDM107_LookUpPSNO_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM107_LookUpPSNO_EDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            //if (dtE.length == 0) {
            //    DyAlert("無符合資料!");
            //    $(".modal-backdrop").remove();
            //    return;
            //}
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
        $('#btQLookup_PSNO_EDM').prop('disabled', true)
        var EDate = $('#cboBIRYear_EDM').val() + "/" + $('#cboBIRMonth_EDM').val() + "/" + "31";
        var pData = {
            EndDate: EDate,
            PS_NO: $('#txtQLookup_PSNO_EDM').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSDM107_LookUpPSNO_EDM", data: pData, success: afterQMSDM107_LookUpPSNO_EDM });
    };

    let afterQMSDM107_LookUpPSNO_EDM = function (data) {
        if (ReturnMsg(data, 0) != "MSDM107_LookUpPSNO_EDMOK") {
            //$('#modal_Lookup_PSNO_EDM').modal('hide');
            DyAlert(ReturnMsg(data, 1), function () {
                $('#btQLookup_PSNO_EDM').prop('disabled', false);
                //$('#modal_Lookup_PSNO_EDM').modal('show');
            });
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                //$('#modal_Lookup_PSNO_EDM').modal('hide');
                DyAlert("無符合資料!", function () {
                    $('#btQLookup_PSNO_EDM').prop('disabled', false);
                    //$('#modal_Lookup_PSNO_EDM').modal('show');
                });
                //$(".modal-backdrop").remove();
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
            //$('#modal_Lookup_PSNO_EDM').modal('hide');
            DyAlert("未選取單號，請重新確認!", function () {
                $('#btLpOK_PSNO_EDM').prop('disabled', false);
                //$('#modal_Lookup_PSNO_EDM').modal('show');
            });
            //$(".modal-backdrop").remove();
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

    let btLpClear_PSNO_EDM_click = function (bt) {
        //Timerset();
        $("#txtQLookup_PSNO_EDM").val('');
        $("#tbDataLookup_PSNO_EDM .checkbox").prop('checked', false);
    };

    let ChangeDate = function (bt) {
        if ($('#txtPSNO_EDM').val() != "") {
            DyConfirm("將自動清除小計折價單號，請確認是否變更!", function () { $('#txtPSNO_EDM').val(''); }, DummyFunction())
        }
    };

    let ChangePSNO = function (bt) {
        $('#lblPSName_EDM').html('')
    };

    let InitComboItem = function (cboYear, cboMonth) {
        //var o = new Option("2024", "2024");
        //cboYear.append(o);

        var y2 = new Date().getFullYear();
        for (i = 2024; i <= y2 + 1; i++) {
            cboYear.append($('<option>', { value: i, text: i }));
        }

        for (i = 1; i <= 12; i++) {
            cboMonth.append($('<option>', { value: ('0' + i).substr(-2), text: i + '月' }));
        }

    };
    //FormLoad
    let GetInitMSDM106 = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }

            InitComboItem($("#cboBIRYear"), $('#cboBIRMonth'));    //下拉選單
            InitComboItem($("#cboBIRYear_EDM"), $('#cboBIRMonth_EDM'));

            AssignVar();
            $('#btAdd').click(function () { btAdd_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btQuery').click(function () { btQuery_click(this) });

            $('#btActivityCode').click(function () { btActivityCode_click(this) });
            $('#btQLookup_ActivityCode').click(function () { btQLookup_ActivityCode_click(this) });
            $('#btLpOK_ActivityCode').click(function () { btLpOK_ActivityCode_click(this) });
            $('#btLpExit_ActivityCode').click(function () { btLpExit_ActivityCode_click(this) });
            $('#btLpClear_ActivityCode').click(function () { btLpClear_ActivityCode_click(this) });

            $('#btMod_EDM').click(function () { btMod_EDM_click(this) });
            $('#btSave_EDM').click(function () { btSave_EDM_click(this) });
            $('#btCancel_EDM').click(function () { btCancel_EDM_click(this) });
            $('#btApp_EDM').click(function () { btApp_EDM_click(this) });
            $('#btDef_EDM').click(function () { btDef_EDM_click(this) });
            $('#btShow_EDM').click(function () { btShow_EDM_click(this) });
            $('#btExit_EDM').click(function () { btExit_EDM_click(this) });
            $('#btPSNO_EDM').click(function () { btPSNO_EDM_click(this) });
            $('#btQLookup_PSNO_EDM').click(function () { btQLookup_PSNO_EDM_click(this) });
            $('#btLpOK_PSNO_EDM').click(function () { btLpOK_PSNO_EDM_click(this) });
            $('#btLpExit_PSNO_EDM').click(function () { btLpExit_PSNO_EDM_click(this) });
            $('#btLpClear_PSNO_EDM').click(function () { btLpClear_PSNO_EDM_click(this) });
            $('#btP2_EDM').click(function () { btP2_EDM_click(this) });
            $('#txtStartDate_EDM').change(function () { ChangeDate(this) });
            $('#txtEndDate_EDM').change(function () { ChangeDate(this) });
            $('#txtPSNO_EDM').change(function () { ChangePSNO(this) });

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
                            'Outdent',                   //減少縮排
                            'alignment'                 //置左、置中、置右
                        ]
                    },
                    placeholder: '請在這裡填寫活動內容!',   //文字編輯器顯示的預設文字
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
                            'alignment'                 //置左、置中、置右
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
            ProgramID: "MSDM106"
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: GetInitMSDM106 });
    };

    if ($('#pgMSDM106').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSDM106", ["MSDM106btns", "pgMSDM106Init", "pgMSDM106Add", "pgMSDM106Mod"], afterLoadPage);
    };


}