var PageMSSetLogo = function (ParentNode) {

    let grdM;
    let cs_EditMode = "";       //狀態 Q:編查 A:新增 M:修改

    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbQMSSetLogo')[0],
                class_collection: ["tdCol1", "tdCol2", "tdCol3", "tdCol4", "tdCol5"],
                fields_info: [
                    { type: "Text", name: "Companycode", style: "" },
                    { type: "Text", name: "CompanyName", style: "" },
                    { type: "Text", name: "ProgramID", style: "" },
                    { type: "Text", name: "ProgramName", style: "" },
                    { type: "Image", name: "Pic", style: "", programid: "MSSetLogo" }
                ],
                //rows_per_page: 10,
                method_clickrow: click_PLU,
                afterBind: InitModifyDeleteButton,
                sortable: "Y"
            }
        );
        return;
    };

    let click_PLU = function (tr) {
    };

    let InitModifyDeleteButton = function () {
        //$('#tbQMSSetLogo .fa-trash-o').click(function () { btMod_click(this) });
        $('#tbQMSSetLogo tbody tr td').click(function () { MSSetLogoQuery(this) });
        $('#tbQMSSetLogo thead tr th').mouseenter(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "Companycode") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead1').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "CompanyName") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead2').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ProgramID") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead3').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ProgramName") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead4').css('background-color', '#ffff00')
                }
            }
        });
        $('#tbQMSSetLogo thead tr th').mouseleave(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "Companycode") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead1').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead1').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "CompanyName") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead2').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ProgramID") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead3').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ProgramName") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead4').css('background-color', '#ffb620')
                }
            }
        });
        $('#tbQMSSetLogo thead tr th').click(function () {
            $('#tbQMSSetLogo thead tr th').css('background-color', '#ffb620')
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "Companycode") {
                $('#tbQMSSetLogo thead tr th#thead1').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "CompanyName") {
                $('#tbQMSSetLogo thead tr th#thead2').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ProgramID") {
                $('#tbQMSSetLogo thead tr th#thead3').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ProgramName") {
                $('#tbQMSSetLogo thead tr th#thead4').css('background-color', '#ffeaa7')
            }
        });
    }

    //Logo查詢
    let MSSetLogoQuery = function (bt) {
        $('#tbQMSSetLogo td').closest('tr').css('background-color', 'transparent');
        $(bt).closest('tr').click();
        $('.msg-valid').hide();
        var node = $(grdM.ActiveRowTR()).prop('Record');
        $('#tbQMSSetLogo td:contains(' + GetNodeValue(node, 'ProgramID') + ')').closest('tr').css('background-color', '#DEEBF7');
        var pData = {
            CompanyID: GetNodeValue(node, 'Companycode'),
            ProgramID: GetNodeValue(node, 'ProgramID')
        }
        PostToWebApi({ url: "api/SystemSetup/MSSetLogoQuery", data: pData, success: aftergrdMQuery });
    };

    let aftergrdMQuery = function (data) {
        if (ReturnMsg(data, 0) != "MSSetLogoQueryOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            if (dtE.length == 0) {
                DyAlert("無符合資料!");
                $(".modal-backdrop").remove();
                return;
            }
            cs_EditMode = "Q"
            ClearData_Logo()
            BindForm_Logo(data)
            FunctionEnable_Logo(cs_EditMode)
            EnableForm_Logo(true)
            $('#modal_Logo').modal('show');
        }
    };

    let MSSetLogoGetImage_Logo = function (elmImg, picCompanyID, picProgramID) {
        if (picCompanyID == "") {
            $('#' + elmImg).prop('src', "");
            return;
        }
        var url = "api/MSSetLogoGetImage_Logo?CompanyID=" + picCompanyID + "&ProgramID=" + picProgramID + "&UU=" + encodeURIComponent(UU);
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

    //Logo按鍵控制
    let FunctionEnable_Logo = function (EditMode) {
        if (EditMode == "A") {
            $('#btMod_Logo').prop('disabled', true)
            $('#btMod_Logo').css('background-color','gray')
            $('#btSave_Logo').prop('disabled', false)
            $('#btSave_Logo').css('background-color', 'red')
            $('#btCancel_Logo').prop('disabled', true)
            $('#btCancel_Logo').css('background-color', 'gray')
            $('#btExit_Logo').prop('disabled', false)
            $('#btExit_Logo').css('background-color', '#348000')
        }
        else if (EditMode == "M") {
            $('#btMod_Logo').prop('disabled', true)
            $('#btMod_Logo').css('background-color', 'gray')
            $('#btSave_Logo').prop('disabled', false)
            $('#btSave_Logo').css('background-color', 'red')
            $('#btCancel_Logo').prop('disabled', false)
            $('#btCancel_Logo').css('background-color', 'red')
            $('#btExit_Logo').prop('disabled', true)
            $('#btExit_Logo').css('background-color', 'gray')
        }
        else if (EditMode == "Q") {
            $('#btMod_Logo').prop('disabled', false)
            $('#btMod_Logo').css('background-color', 'red')
            $('#btSave_Logo').prop('disabled', true)
            $('#btSave_Logo').css('background-color', 'gray')
            $('#btCancel_Logo').prop('disabled', true)
            $('#btCancel_Logo').css('background-color', 'gray')
            $('#btExit_Logo').prop('disabled', false)
            $('#btExit_Logo').css('background-color', '#348000')
        }
    };

    //Logo畫面控制
    let EnableForm_Logo = function (mod) {
        if (mod == true) {
            $('#cboCompany_Logo').prop('disabled', mod);
            $('#cboProgramID_Logo').prop('disabled', mod);
            $('#btPic_Logo').css('pointer-events', 'none');
            window.t1.enableReadOnlyMode('t1');         //停用
        }
        else {
            if (cs_EditMode == "M") {
                $('#cboCompany_Logo').prop('disabled', true);
                $('#cboProgramID_Logo').prop('disabled', true);
            }
            else {
                $('#cboCompany_Logo').prop('disabled', mod);
                $('#cboProgramID_Logo').prop('disabled', mod);
            }
            $('#btPic_Logo').css('pointer-events', 'unset');
            window.t1.disableReadOnlyMode('t1');        //啟用
        }
    };

    //Logo清除資料
    let ClearData_Logo = function () {
        $('#cboCompany_Logo').val('');
        $('#lblCompanyName_Logo').html('');
        $('#cboProgramID_Logo').val('');
        $('#lblChineseName_Logo').html('');
        MSSetLogoGetImage_Logo("Pic_Logo", "");
        window.t1.setData('');
    };

    //Logo代入資料
    let BindForm_Logo = function (data) {
        var dtE = data.getElementsByTagName('dtE');
        $('#cboCompany_Logo').val(GetNodeValue(dtE[0], "Companycode"))
        $('#lblCompanyName_Logo').html(GetNodeValue(dtE[0], "CompanyName"))
        $('#cboProgramID_Logo').val(GetNodeValue(dtE[0], "ProgramID"))
        $('#lblChineseName_Logo').html(GetNodeValue(dtE[0], "ProgramName"))
        MSSetLogoGetImage_Logo("Pic_Logo", GetNodeValue(dtE[0], "Companycode"), GetNodeValue(dtE[0], "ProgramID"));
        window.t1.setData(GetNodeValue(dtE[0], "TE"));

    };

    //Logo新增
    let btAdd_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSSetLogoGetVMDocNo", data: pData, success: afterAddLogo });
    };

    let afterAddLogo = function (data) {
        if (ReturnMsg(data, 0) != "MSSetLogoGetVMDocNoOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#lblVMDocNo_Logo').html(GetNodeValue(dtE[0], "DocNo"))
            cs_EditMode = "A";
            ClearData_Logo();
            FunctionEnable_Logo(cs_EditMode);
            EnableForm_Logo(false)
            $('#modal_Logo').modal('show');
        }
    };

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#cboCompany').val('');
        $('#lblCompanyName').html('');
        $('#cboProgramID').val('');
        $('#lblChineseName').html('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#tbQMSSetLogo thead tr th').css('background-color', '#ffb620')
        $('#btQuery').prop('disabled', true)

        if ($('#cboCompany').val() == "") {
            DyAlert("請選擇公司代號!", function () { $('#btQuery').prop('disabled', false); })
            return;
        }
       
        ShowLoading();
        var pData = {
            CompanyID: $('#cboCompany').val(),
            ProgramID: $('#cboProgramID').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSetLogoQuery", data: pData, success: afterMSSetLogoQuery });
    };

    let afterMSSetLogoQuery = function (data) {
        CloseLoading();
        if (ReturnMsg(data, 0) != "MSSetLogoQueryOK") {
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

    let GetChineseName = function (bt) {
        if ($('#cboProgramID').val() == "") {
            $('#lblChineseName').html('');
            return;
        }
        var pData = {
            ProgramID: $('#cboProgramID').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: afterGetChineseName });
    };

    let afterGetChineseName = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#lblChineseName').html(GetNodeValue(dtE[0], "ChineseName"));
        }
    };

    let GetChineseName_Logo = function (bt) {
        if ($('#cboProgramID_Logo').val() == "") {
            $('#lblChineseName_Logo').html('');
            return;
        }
        var pData = {
            ProgramID: $('#cboProgramID_Logo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/GetInitmsDM", data: pData, success: afterGetChineseName_Logo });
    };

    let afterGetChineseName_Logo = function (data) {
        if (ReturnMsg(data, 0) != "GetInitmsDMOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#lblChineseName_Logo').html(GetNodeValue(dtE[0], "ChineseName"));
        }
    };

    let GetCompanyName = function (bt) {
        if ($('#cboCompany').val() == "") {
            $('#lblCompanyName').html('');
            return;
        }
        var pData = {
            Company: $('#cboCompany').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSetLogoGetCompany", data: pData, success: afterGetCompanyName });
    };

    let afterGetCompanyName = function (data) {
        if (ReturnMsg(data, 0) != "MSSetLogoGetCompanyOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#lblCompanyName').html(GetNodeValue(dtE[0], "ChineseName"));
        }
    };

    let GetCompanyName_Logo = function (bt) {
        if ($('#cboCompany_Logo').val() == "") {
            $('#lblCompanyName_Logo').html('');
            return;
        }
        var pData = {
            Company: $('#cboCompany_Logo').val()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSetLogoGetCompany", data: pData, success: afterGetCompanyName_Logo });
    };

    let afterGetCompanyName_Logo = function (data) {
        if (ReturnMsg(data, 0) != "MSSetLogoGetCompanyOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#lblCompanyName_Logo').html(GetNodeValue(dtE[0], "ChineseName"));
        }
    };

    //Logo離開
    let btExit_Logo_click = function (bt) {
        //Timerset();
        $('#modal_Logo').modal('hide')
        btQuery_click();
    };

    //Logo儲存
    let btSave_Logo_click = function (bt) {
        //Timerset();
        EnableForm_Logo(true)
        $('#btSave_Logo').prop('disabled', true);

        if ($('#cboCompany_Logo').val() == "") {
            DyAlert("請選擇公司代號!", function () {
                EnableForm_Logo(false);
                $('#btSave_Logo').prop('disabled', false);
            })
            return;
        }
        if ($('#cboProgramID_Logo').val() == "") {
            DyAlert("請選擇程式代號!", function () {
                EnableForm_Logo(false);
                $('#btSave_Logo').prop('disabled', false);
            })
            return;
        }
        var Pic = $('#Pic_Logo').attr('src');
        if (Pic == "") {
            DyAlert("請設定公司Logo!", function () {
                EnableForm_Logo(false);
                $('#btSave_Logo').prop('disabled', false);
            })
            return;
        }
        if (window.t1.getData() == "<p>&nbsp;</p>") {
            DyAlert("請輸入公司聲明區!", function () {
                EnableForm_Logo(false)
                $('#btSave_Logo').prop('disabled', false);
            })
            return;
        }

        var pData = {
            EditMode: cs_EditMode,
            CompanyID: $('#cboCompany_Logo').val(),
            ProgramID: $('#cboProgramID_Logo').val(),
            TE: window.t1.getData(),
            VMDocNo: $('#lblVMDocNo_Logo').html()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSetLogo_Save", data: pData, success: afterMSSetLogo_Save });
    };

    let afterMSSetLogo_Save = function (data) {
        if (ReturnMsg(data, 0) != "MSSetLogo_SaveOK") {
            DyAlert(ReturnMsg(data, 1), function () {
                EnableForm_Logo(false)
                $('#btSave_Logo').prop('disabled', false);
            });
        }
        else {
            DyAlert("存檔成功!", function () {
                cs_EditMode = "Q";
                FunctionEnable_Logo(cs_EditMode);
                EnableForm_Logo(true)
            })
        }
    };

    //Logo圖檔上傳
    let btPic_Logo_click = function (bt) {
        //Timerset();
        var Pic = $('#Pic_Logo').attr('src');
        if (Pic == "") {
            btUPLogo_click();
        }
        else {
            $('#modal_ImgUp').modal('show');
        }
    };

    let btUPLogo_click = function (bt) {
        //Timerset();
        $('#modal_ImgUp').modal('hide');
        InitFileUpload(bt);
        var UploadFileType = "P1";
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
            data.formData = {
                "UploadFileType": $('#modal-media').prop("UploadFileType"),
                "ImgSGID": $('#' + $('#modal-media').prop("FieldName")).val(),
                "DocNo": $('#lblVMDocNo_Logo').html()
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
            var DocNo = $('#lblVMDocNo_Logo').html();
            GetImage_EDM("Pic_Logo", DocNo, "P1", "N");
            $('#modal-media').prop("UploadFileType", UploadFileType);
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
            DocNo = $('#lblVMDocNo_Logo').html();
            var pData = {
                DocNo: DocNo
            }
            PostToWebApi({ url: "api/SystemSetup/MSSetLogoDelImg", data: pData, success: afterMSSetLogoDelImg });
        }, function () {
            DummyFunction();
        })
    };

    let afterMSSetLogoDelImg = function (data) {
        if (ReturnMsg(data, 0) != "MSSetLogoDelImgOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#modal_ImgUp').modal('hide');
            DyAlert("圖片刪除完成!", function () {
                GetImage_EDM("Pic_Logo", "");
            })
        }
    };

    //Logo修改
    let btMod_Logo_click = function (bt) {
        //Timerset();
        var pData = {
        }
        PostToWebApi({ url: "api/SystemSetup/MSSetLogoGetVMDocNo", data: pData, success: afterModLogo });
    };

    let afterModLogo = function (data) {
        if (ReturnMsg(data, 0) != "MSSetLogoGetVMDocNoOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            $('#lblVMDocNo_Logo').html(GetNodeValue(dtE[0], "DocNo"))
            cs_EditMode = "M"
            FunctionEnable_Logo(cs_EditMode)
            EnableForm_Logo(false)
        }
    };

    //Logo取消
    let btCancel_Logo_click = function (bt) {
        //Timerset();
        var pData = {
            CompanyID: $('#cboCompany_Logo').val(),
            ProgramID: $('#cboProgramID_Logo').val(),
            DocNo: $('#lblVMDocNo_Logo').html()
        }
        PostToWebApi({ url: "api/SystemSetup/MSSetLogoCancel", data: pData, success: afterMSSetLogoCancel });
    };

    let afterMSSetLogoCancel = function (data) {
        if (ReturnMsg(data, 0) != "MSSetLogoCancelOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            cs_EditMode = "Q"
            BindForm_Logo(data)
            FunctionEnable_Logo(cs_EditMode)
            EnableForm_Logo(true)
        }
    };
    //FormLoad
    let GetInitMSSetLogo = function (data) {
        if (ReturnMsg(data, 0) != "InitMSSetLogoOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            var dtP = data.getElementsByTagName('dtP');
            var dtC = data.getElementsByTagName('dtC');
            if (dtE.length > 0) {
                $('#lblProgramName').html(GetNodeValue(dtE[0], "ChineseName"));
            }
            InitSelectItem($('#cboCompany')[0], dtC, "Companycode", "CompanyName", true);
            InitSelectItem($('#cboCompany_Logo')[0], dtC, "Companycode", "CompanyName", true);
            InitSelectItem($('#cboProgramID')[0], dtP, "ProgramID", "ProgramName", true);
            InitSelectItem($('#cboProgramID_Logo')[0], dtP, "ProgramID", "ProgramName", true);
            AssignVar();
            $('#btAdd').click(function () { btAdd_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#cboCompany').change(function () { GetCompanyName(); });
            $('#cboCompany_Logo').change(function () { GetCompanyName_Logo(); });
            $('#cboProgramID').change(function () { GetChineseName(); });
            $('#cboProgramID_Logo').change(function () { GetChineseName_Logo(); });
            $('#btMod_Logo').click(function () { btMod_Logo_click(this) });
            $('#btSave_Logo').click(function () { btSave_Logo_click(this) });
            $('#btCancel_Logo').click(function () { btCancel_Logo_click(this) });
            $('#btExit_Logo').click(function () { btExit_Logo_click(this) });
            $('#btPic_Logo').click(function () { btPic_Logo_click(this) });
            $('#btExit_ImgUp').click(function () { btExit_ImgUp_click(this) });
            $('#btDelete_ImgUp').click(function () { btDelete_ImgUp_click(this) });
            $('#btImgUp').click(function () { btUPLogo_click(this) });

            //文字編輯器
            ClassicEditor
                .create(document.querySelector('#txtTE_Logo'), {
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
            ProgramID: "MSSetLogo"
        }
        PostToWebApi({ url: "api/SystemSetup/InitMSSetLogo", data: pData, success: GetInitMSSetLogo });
    };

    if ($('#pgMSSetLogo').length == 0) {  
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSSetLogo", ["MSSetLogobtns", "pgMSSetLogoInit", "pgMSSetLogoAdd", "pgMSSetLogoMod"], afterLoadPage);
    };

}
