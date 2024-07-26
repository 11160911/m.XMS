var PageMSSetLogo = function (ParentNode) {

    let grdM;
    let cs_EditMode = "";       //狀態 Q:編查 A:新增 M:修改

    let AssignVar = function () {
        grdM = new DynGrid(
            {
                table_lement: $('#tbQMSSetLogo')[0],
                class_collection: ["tdColbt icon_in_td btMod", "tdCol1", "tdCol2", "tdCol3", "tdCol4"],
                fields_info: [
                    { type: "JQ", name: "fa-trash-o", element: '<i class="fa fa-trash-o" style="font-size:24px"></i>'},
                    { type: "Text", name: "Companycode", style: "" },
                    { type: "Text", name: "CompanyName", style: "" },
                    { type: "Text", name: "ProgramID", style: "" },
                    { type: "Text", name: "ProgramName", style: "" }
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
                //afterBind: InitModifyDeleteButton,
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
                //afterBind: InitModifyDeleteButton,
                sortable: "N"
            }
        );
        return;
    };

    let click_PLU = function (tr) {
    };

    let InitModifyDeleteButton = function () {
        //$('#tbQMSSetLogo .fa-trash-o').click(function () { btMod_click(this) });
        //$('#tbQMSSetLogo tbody tr').click(function () { MSSetLogoQuery(this) });

        $('#tbQMSSetLogo thead tr th').mouseenter(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "Companycode") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead2').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "CompanyName") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead3').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ProgramID") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead4').css('background-color', '#ffff00')
                }
            }
            else if (fdinfo.name == "ProgramName") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead5').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead5').css('background-color', '#ffff00')
                }
            }
        });
        $('#tbQMSSetLogo thead tr th').mouseleave(function () {
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "Companycode") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead2').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead2').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "CompanyName") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead3').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead3').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ProgramID") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead4').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead4').css('background-color', '#ffb620')
                }
            }
            else if (fdinfo.name == "ProgramName") {
                var rgb = $('#tbQMSSetLogo thead tr th#thead5').css('background-color')
                var hexcolor = SetRgbTo16(rgb);
                if (hexcolor != '#ffeaa7') {
                    $('#tbQMSSetLogo thead tr th#thead5').css('background-color', '#ffb620')
                }
            }
        });
        $('#tbQMSSetLogo thead tr th').click(function () {
            $('#tbQMSSetLogo thead tr th').css('background-color', '#ffb620')
            var fdinfo = $(this).prop('fdinfo');
            if (fdinfo.name == "Companycode") {
                $('#tbQMSSetLogo thead tr th#thead2').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "CompanyName") {
                $('#tbQMSSetLogo thead tr th#thead3').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ProgramID") {
                $('#tbQMSSetLogo thead tr th#thead4').css('background-color', '#ffeaa7')
            }
            else if (fdinfo.name == "ProgramName") {
                $('#tbQMSSetLogo thead tr th#thead5').css('background-color', '#ffeaa7')
            }
        });
    }

    //Logo查詢
    //let MSSetLogoQuery = function (bt) {
    //    $('#tbQMSSetLogo td').closest('tr').css('background-color', 'transparent');

    //    $(bt).closest('tr').click();
    //    $('.msg-valid').hide();
    //    var node = $(grdM.ActiveRowTR()).prop('Record');
    //    $('#tbQMSSetLogo td:contains(' + GetNodeValue(node, 'ProgramID') + ')').closest('tr').css('background-color', '#DEEBF7');
    //    var pData = {
    //        DocNo: GetNodeValue(node, 'ProgramID')
    //    }
    //    PostToWebApi({ url: "api/SystemSetup/MSSetLogoQuery", data: pData, success: afterMSSetLogoQuery });
    //};

    //let afterMSSetLogoQuery = function (data) {
    //    if (ReturnMsg(data, 0) != "MSSetLogoQueryOK") {
    //        DyAlert(ReturnMsg(data, 1));
    //    }
    //    else {
    //        var dtQ = data.getElementsByTagName('dtQ');
    //        if (dtQ.length == 0) {
    //            DyAlert("無符合資料!");
    //            $(".modal-backdrop").remove();
    //            return;
    //        }
    //        cs_EditMode = "Q"
    //        ClearData_EDM()
    //        BindForm_EDM(data)
    //        FunctionEnable_EDM(cs_EditMode)
    //        EnableForm_EDM(true)
    //        $('#modal_EDM').modal('show');
    //    }
    //};



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

    //Logo按鍵控制
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

    //Logo畫面控制
    let EnableForm_EDM = function (mod) {
        $('#txtEDMMemo_EDM').prop('disabled', mod);
        $('#txtStartDate_EDM').prop('disabled', mod);
        $('#txtEndDate_EDM').prop('disabled', mod);

        $('#btPSNO_EDM').prop('disabled', mod);
        $('#txtPSNO_EDM').prop('disabled', mod);
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

    //Logo清除資料
    let ClearData_EDM = function () {
        $('#lblDocNo_EDM').html('');
        $('#lblAppUser_EDM').html('');
        $('#lblDefUser_EDM').html('');
        $('#txtEDMMemo_EDM').val('');
        $('#lblAppDate_EDM').html('');
        $('#lblDefDate_EDM').html('');
        $('#txtStartDate_EDM').val('');
        $('#txtEndDate_EDM').val('');
        $('#txtPSNO_EDM').val('');
        $('#lblPSName_EDM').html('');
        window.t1.setData('');
        window.t2.setData('');
        GetImage_EDM("P2_EDM", "");
    };

    //Logo代入資料
    let BindForm_EDM = function (data) {
        var dtH = data.getElementsByTagName('dtH');
        $('#lblDocNo_EDM').html(GetNodeValue(dtH[0], "DocNo"));
        $('#lblAppUser_EDM').html(GetNodeValue(dtH[0], "ApproveUser"));
        $('#lblDefUser_EDM').html(GetNodeValue(dtH[0], "Defeasance"));
        $('#txtEDMMemo_EDM').val(GetNodeValue(dtH[0], "EDMMemo"));
        $('#lblAppDate_EDM').html(GetNodeValue(dtH[0], "ApproveDate"));
        $('#lblDefDate_EDM').html(GetNodeValue(dtH[0], "DefeasanceDate"));
        $('#txtStartDate_EDM').val(GetNodeValue(dtH[0], "StartDate").toString().replaceAll('/', '-'));
        $('#txtEndDate_EDM').val(GetNodeValue(dtH[0], "EndDate").toString().replaceAll('/', '-'));
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

    //Logo新增
    let btAdd_click = function (bt) {
        //Timerset();
        var pData = {
            ProgramID: "MSSetLogo"
        }
        PostToWebApi({ url: "api/SystemSetup/GetCompanyLogo", data: pData, success: afterGetCompanyLogo });
    };

    let afterGetCompanyLogo = function (data) {
        if (ReturnMsg(data, 0) != "GetCompanyLogoOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtE');
            GetImage_Logo("P1_EDM", "MSSetLogo");
            //$('#lblCompanyLogo').html(GetNodeValue(dtE[0], "Txt"))
            $('#lblVMDocNo_EDM').html(GetNodeValue(dtE[0], "DocNo"))
            cs_EditMode = "A";
            ClearData_EDM();
            FunctionEnable_EDM(cs_EditMode);
            EnableForm_EDM(false)
            $('#modal_EDM').modal('show');
        }
    };

    //清除
    let btClear_click = function (bt) {
        //Timerset();
        $('#cboProgramID').val('');
        $('#lblChineseName').html('');
    };

    //查詢
    let btQuery_click = function (bt) {
        //Timerset();
        $('#tbQMSSetLogo thead tr th').css('background-color', '#ffb620')
        $('#btQuery').prop('disabled', true)
       
        ShowLoading();
        var pData = {
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
            if (dtC.length > 0) {
                $('#lblCompany').html(GetNodeValue(dtC[0], "Company"));
            }
            InitSelectItem($('#cboProgramID')[0], dtP, "ProgramID", "ProgramID", true);
            AssignVar();
            $('#btAdd').click(function () { btAdd_click(this) });
            $('#btClear').click(function () { btClear_click(this) });
            $('#btQuery').click(function () { btQuery_click(this) });
            $('#cboProgramID').change(function () { GetChineseName(); });

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
