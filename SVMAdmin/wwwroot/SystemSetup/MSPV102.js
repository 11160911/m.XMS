var PageMSPV102 = function (ParentNode) {
    let setHEtype;
    let dtEL;
    let rdochecked;
    let rdoEchecked;
    let Hbtnflag;
    let chkgroupid;
    let oldHChkedgid;
    let dtG1;
    let dtG2;
    let ReSetTree = function (setType, data) {
        //alert(setType);
        switch (setType) {
            case "E":
                var dtG = data.getElementsByTagName('dtGroupID');
                //alert(dtG.length);
                $("#DVTreeView_EGroupID").jstree("destroy");
                SetTreeField("#DVTreeView_EGroupID", dtG);
                $("#DVTreeView_EGroupID").jstree(
                    {
                        "core": {
                            check_callback: true
                        }
                    });
                break;
            case "G":
                dtG2 = data.getElementsByTagName('dtGroupID');
                //alert(dtG.length);
                $("#DVTreeView_DGroupID").jstree("destroy");
                SetTreeField("#DVTreeView_DGroupID", dtG2);
                $("#DVTreeView_DGroupID").jstree(
                    {
                        "core": {
                            check_callback: true
                        }
                    });
                break;
            case "H":
                var dtS = data.getElementsByTagName('dtSystemID');
                var dtP = data.getElementsByTagName('dtProgramID');
                var dtB = data.getElementsByTagName('dtProgramButton');
                $("#DVTreeView_SystemID").jstree("destroy");
                SetTreeField("#DVTreeView_SystemID", dtS, dtP,dtB);

                dtG1 = data.getElementsByTagName('dtGroupID');
                var dtE = data.getElementsByTagName('dtEmp');
                $("#DVTreeView_GroupID").jstree("destroy");
                SetTreeField("#DVTreeView_GroupID", dtG1, dtE);

                $("#DVTreeView_SystemID").jstree(
                    {
                        "checkbox": {
                            "keep_selected_style": false,//是否默認選中
                            "cascade": "down+undetermined",
                            "three_state": false //父子級別級聯選擇
                        },
                        "plugins": ["checkbox"],
                        "core": {
                            check_callback: true
                        }
                    });
                //$root.jstree(true).close_all();
                //$root.jstree(true).open_all();
                $("#DVTreeView_GroupID").jstree(
                    {
                        "core": {
                            check_callback: true
                        }
                    });

			default:
                break;
        }
    };

    //#region 群組編輯
    let AfterAddGroup = function (data) {
        switch (rdochecked) {
            case "A":
                if (ReturnMsg(data, 0) != "AddGroupOK") {
                    DyAlert(ReturnMsg(data, 1));
                }
                else {
                    dtG2 = data.getElementsByTagName('dtGroupID');
                    DyAlert("群組新增完成!");
                    var dtCG = data.getElementsByTagName('dtCopyGroup');
                    InitSelectItem($('#selGroupID')[0], dtCG, "GroupID", "GroupName", true);
                    $('#DlblGtitle').show();
                }
                break;
            case "M":
                if (ReturnMsg(data, 0) != "EditGroupOK") {
                    DyAlert(ReturnMsg(data, 1));
                }
                else {
                    dtG2 = data.getElementsByTagName('dtGroupID');
                    DyAlert("群組修改完成!");
                    $('#DlblGtitle').show();
                }
                break;
            case "D":
                if (ReturnMsg(data, 0) != "EditGroupOK") {
                    DyAlert(ReturnMsg(data, 1));
                }
                else {
                    dtG2 = data.getElementsByTagName('dtGroupID');
                    DyAlert("群組刪除完成!");
                    //alert(dtG.length);
                    if (dtG2.length == 0) {
                        $('#DlblGtitle').hide();
                    }
                    else {
                        $('#DlblGtitle').show();
                    }
                }
                break;
            default:
                break;
        }
        
        ReSetTree("G", data);
        SetG_lblshown(data);
    };
    
    let SaveGroupData = function () {
        //alert($('#selGroupID').val());
        if (rdochecked == "A") {
            var pData = {
                GroupIDWeb: [
                    {
                        GroupID: $('#txtGroupID').val().trim(),
                        GroupName: $('#txtGroupName').val().trim(),
                        Status: "3",
                        CopyGroup: $('#selGroupID').val()
                    }
                ]

            }
            //alert("Add.." + $('#Type_ID').val());
            PostToWebApi({ url: "api/SystemSetup/AddGroup", data: pData, success: AfterAddGroup });
        }
        else if (rdochecked == "M") {
            //alert($('#NewPLU').val());
            var mData = {
                GroupIDWeb: [
                    {
                        GroupID: $('#txtGroupID').val().trim(),
                        GroupName: $('#txtGroupName').val().trim(),
                        LV: "3",
                        EditType: rdochecked
                    }
                ]

            }
            //alert("Add.." + $('#Type_ID').val());
            PostToWebApi({ url: "api/SystemSetup/EditGroup", data: mData, success: AfterAddGroup });
        }
        else if (rdochecked == "D") {
            var dData = {
                GroupIDWeb: [
                    {
                        GroupID: $('#txtGroupID').val().trim(),
                        GroupName: $('#txtGroupName').val().trim(),
                        LV: "3",
                        EditType: rdochecked
                    }
                ]

            }
            //alert("Add.." + $('#Type_ID').val());
            PostToWebApi({ url: "api/SystemSetup/EditGroup", data: dData, success: AfterAddGroup });
        }
    }

    let afterChkGroup = function (data) {
        //alert(ReturnMsg(data, 0));
        if (ReturnMsg(data, 0) != "ChkGroupExistOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            let lb_Continue = false;
            if (rdochecked == "A") {
                var dtG = data.getElementsByTagName('dtGroupID');
                if (dtG.length > 0) {
                    DyAlert("群組代號已存在，無法新增!!")
                    return;
                }
                else { lb_Continue = true; }
            }
            else {
                //var dtE = data.getElementsByTagName('dtGroupEmp');
                //if (dtE.length > 0) {
                //    DyAlert("尚有人員隸屬指定群組，故不允許刪除!!")
                //    return;
                //}
                //else {
                    if (rdochecked == "D") {
                        DyConfirm("確定要刪除這筆資料嗎?", SaveGroupData, DummyFunction);
                    };

                //}
            }
            if (lb_Continue) { SaveGroupData(); }
        }
    };

    let btG_Yes_click = function () {
        //alert(rdochecked);
        if (rdochecked == "A") {
            if ($('#txtGroupID').val() == "" | $('#txtGroupID').val() == null) {
                DyAlert("群組代碼欄位必須輸入資料!!", function () { $('#txtGroupID').focus() });
                return;
            }
        }
        if (rdochecked != "D") {
            if ($('#txtGroupName').val() == "" | $('#txtGroupName').val() == null) {
                DyAlert("群組名稱欄位必須輸入資料!!", function () { $('#txtGroupName').focus() });
                return;
            }
        }

        //let lv = $('#selLevel').val();
        //alert(lv);
        switch (rdochecked) {
            case "A":
                var pData = {
                    LV: "3",
                    GroupID: $('#txtGroupID').val(),
                    EditType: rdochecked
                };
                PostToWebApi({ url: "api/SystemSetup/ChkGroupExist", data: pData, success: afterChkGroup });
                break;
            case "M":
                SaveGroupData();
                break;
            case "D":
                var pData = {
                    LV: "3",
                    GroupID: $('#txtGroupID').val(),
                    EditType: rdochecked
                };
                PostToWebApi({ url: "api/SystemSetup/ChkGroupExist", data: pData, success: afterChkGroup });
                break;
            default:
                break;
        }
    }

    let btG_Cancel_click = function () {
        ReLoad();
        $('#modal_msPV102_AddG').modal('hide');
    }

    let SetG_lblshown = function (data) {
        rdochecked = $('input[name="Group"]:checked').val();    //群組rdo
        $('input[name="DGroup"]').val("");  //群組input(代碼名稱,cbo)
        if (rdochecked != "A") {
            $('#aa').hide();
        }
        else {
            $('#aa').show();
        }
        if (rdochecked != "D") {
            $('#btG_Yes').html('儲存');
        }
        else {
            $('#btG_Yes').html('刪除');
        }
        //alert(data.length);
        //alert(rdochecked);
        if (data.length > 0) {
            $("#DVTreeView_DGroupID").jstree('deselect_all', true);
            switch (rdochecked) {
                case "A":
                    $('#txtGroupID,#txtGroupName').prop('readonly', false);
                    $('#btG_Yes').prop('disabled', false);
                    break;
                case "M":
                    $('#txtGroupID').prop('readonly', true);
                    $('#txtGroupName').prop('readonly', false);
                    $('#btG_Yes').prop('disabled', true);
                    break;
                case "D":
                    $('#txtGroupID,#txtGroupName').prop('readonly', true);
                    $('#btG_Yes').prop('disabled', true);
                    break;
                default:
            }
        }
        else {
            switch (rdochecked) {
                case "A":
                    $('#txtGroupID,#txtGroupName').prop('readonly', false);
                    $('#btG_Yes').prop('disabled', false);
                    break;
                case "M":
                    $('#txtGroupID,#txtGroupName').prop('readonly', true);
                    $('#btG_Yes').prop('disabled', true);
                    break;
                case "D":
                    $('#txtGroupID,#txtGroupName').prop('readonly', true);
                    $('#btG_Yes').prop('disabled', true);
                    break;
                default:
                    break;
            }
        }
        $("#DVTreeView_DGroupID").bind("changed.jstree",
            function (e, data) {
                let chkedid = data.node.id;
                let chkedname = data.node.text;
                //alert(rdochecked);
                //alert(JSON.stringify(chkedid).substring(4,JSON.stringify(chkedid).length-1));
                ////alert("Checked: " + data.node.id);
                //alert("text: " + data.node.text);
                if (rdochecked != "A") {
                    $('#txtGroupID').val(chkedid.substring(3,chkedid.length));
                    $('#txtGroupName').val(chkedname);
                }
                else {
                    $('#txtGroupID').val("");
                    $('#txtGroupName').val("");
                }
                $('#btG_Yes').prop('disabled', false);    
            });    
        
    }

    let GetVPV01_AddG = function (data) {
        if (ReturnMsg(data, 0) != "GetGroupEmpOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('.msg-valid').hide();
            $('#modal_msPV102_AddG .modal-title').text('編輯群組');

            dtG2 = data.getElementsByTagName('dtGroupID');
            var dtCG = data.getElementsByTagName('dtCopyGroup');
            InitSelectItem($('#selGroupID')[0], dtCG, "GroupID", "GroupName", true);
            if (dtG2.length == 0) {
                $('#DlblGtitle').hide();
            }
            else {
                $('#DlblGtitle').show();
            }
            $('#G_Add').click(function () { SetG_lblshown(dtG2); });
            $('#G_Mod').click(function () { SetG_lblshown(dtG2); });
            $('#G_Del').click(function () { SetG_lblshown(dtG2); });

            ReSetTree("G", data);
            $('input[name="Group"]')[0].checked = true;
            SetG_lblshown(dtG2);

            $('#modal_msPV102_AddG').modal('show');
        };
    };

    let btAddG_Click = function () {
        //let lv = $('#selLevel').val();
        Hbtnflag = "G";
        var pData = {
            LV: "3",
            Hbtn: Hbtnflag,
            EmpLoadType: "X"
        };
        PostToWebApi({ url: "api/SystemSetup/GetGroupEmp", data: pData, success: GetVPV01_AddG });
    };

    //#endregion
    
    //#region 人員編輯
    let afterSetEGroupID = function (data) {
        if (ReturnMsg(data, 0) != "GetGroupEmpOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtE = data.getElementsByTagName('dtEmp');
            InitCheckBoxItem($('#chkEmp')[0], dtE, "UID", "UName","ECheck");
        }
    }

    let afterEditGroupEmp = function (data) {
        //alert(ReturnMsg(data, 0));
        if (ReturnMsg(data, 0) != "EditGroupEmpOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            if (rdoEchecked == "A") {
                DyAlert("群組人員新增完成!");
            }
            else {
                DyAlert("群組人員刪除完成!");
            }
            //let lv = $('#selLevel').val();
            var pData = {
                LV: "3",
                Hbtn: "E",
                EmpLoadType: rdoEchecked,
                ManGroup: $('#txtEGroupID').val()
            };
            PostToWebApi({ url: "api/SystemSetup/GetGroupEmp", data: pData, success: afterSetEGroupID });
        }
    }

    let GetCheckedValue = function (checkBoxName) {
        var valuelist = "";
        valuelist += $('input[name="' + checkBoxName + '"]:checked').map(function () { return $(this).val(); }).get().join(',');
        return valuelist;
    }

    let btGE_Yes_click = function () {
        if ($('#txtEGroupID').val() == "" | $('#txtEGroupID').val() == null) {
            DyAlert("請選擇群組");
            return;
        }
        let checkedemp = GetCheckedValue("ECheck");
        if (checkedemp == "") { 
            if ($('input[name=ECheck]').length == 0) {
                if (rdoEchecked == "A") { DyAlert("請勾選人員"); return; }  //請先新增人員
                if (rdoEchecked == "D") { DyAlert("請先設定群組人員"); return; }
            }
            else {
                DyAlert("請勾選人員");
                return;
            }
        }
        //alert(checkedemp);
        //let lv = $('#selLevel').val();
        //alert(lv + ";" + $('#txtEGroupID').val() + ";" + checkedemp + ";" + rdoEchecked + ";");
        var pData = {
            Account: [{
                LV: "3",
                GroupID: $('#txtEGroupID').val(),
                EmpStr: checkedemp,
                EditType: rdoEchecked
            }]
        };
        PostToWebApi({ url: "api/SystemSetup/EditGroupEmp", data: pData, success: afterEditGroupEmp });
    };

    let btGE_Cancel_click = function () {
        ReLoad();
        $('#modal_msPV102_ModE').modal('hide');
    }

    let SetE_lblshown = function (data) {
        rdoEchecked = $('input[name="EGroup"]:checked').val();
        $('#txtEGroupID').val('');
        //alert(rdoEchecked);
        if (rdoEchecked != "D") {
            $('#btGE_Yes').html('儲存');
        }
        else {
            $('#btGE_Yes').html('刪除');
        }
        if (data.length > 0) {
            $("#DVTreeView_EGroupID").jstree('deselect_all', true);
            $('#btGE_Yes').prop('disabled', false);
        }
        else {
            $('#btGE_Yes').prop('disabled', true);
        }

        //let lv = $('#selLevel').val();
        var pData = {
            LV: "3",
            Hbtn: Hbtnflag,
            EmpLoadType: rdoEchecked,
        };
        PostToWebApi({ url: "api/SystemSetup/GetGroupEmp", data: pData, success: afterSetEGroupID });
    };

    let GetVPV01_ModE = function (data) {
        if (ReturnMsg(data, 0) != "GetGroupEmpOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('.msg-valid').hide();
            var dtG = data.getElementsByTagName('dtGroupID');
            var dtE = data.getElementsByTagName('dtEmp');
            InitCheckBoxItem($('#chkEmp')[0], dtE, "UID", "UName", "ECheck");
            $('#GE_Add').click(function () { SetE_lblshown(dtG); });
            $('#GE_Del').click(function () { SetE_lblshown(dtG); });

            ReSetTree("E", data);
            $('input[name="EGroup"]')[0].checked = true;
            SetE_lblshown(dtG);
            $("#DVTreeView_EGroupID").bind("changed.jstree",
                function (e, data) {
                    let chkedid = data.node.id;
                    chkedid = chkedid.substring(3, chkedid.length);  //DG_MIS
                    //alert(chkedid);
                    if (chkedid == "" | chkedid == null) {
                        $('#txtEGroupID').val('');
                    }
                    else {
                        $('#txtEGroupID').val(chkedid);
                    }

                    //let chkedname = data.node.text;
                    //alert(data.instance.get_node(data.selected[0].text));
                    //alert(chkedid);
                    //alert(JSON.stringify(chkedid).substring(4,JSON.stringify(chkedid).length-1));
                    //alert("Checked: " + data.node.id);
                    //alert("text: " + data.node.text);
                    //let lv = $('#selLevel').val();
                    Hbtnflag = "E";
                    var pData = {
                        LV: "3",
                        Hbtn: Hbtnflag,
                        EmpLoadType: rdoEchecked,
                        ManGroup: chkedid
                    };
                    PostToWebApi({ url: "api/SystemSetup/GetGroupEmp", data: pData, success: afterSetEGroupID });
                });
            $('#modal_msPV102_ModE').modal('show');
        };
    }

    let btModE_Click = function () {
        $('#modal_msPV102_ModE .modal-title').text('人員維護');
        $('#txtEGroupID').val('');
        Hbtnflag = "E";
        var pData = {
            LV: "3",
            Hbtn: Hbtnflag,
            EmpLoadType: "A",
            ManGroup: $('#txtEGroupID').val()
        };
        PostToWebApi({ url: "api/SystemSetup/GetGroupEmp", data: pData, success: GetVPV01_ModE });
    }

    //#endregion

    //#region 程式權限設定
    let afterEditGroupProgram = function (data) {
        //alert(ReturnMsg(data, 0));
        if (ReturnMsg(data, 0) != "EditGroupProgramOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("群組程式權限設定完成");
            var dtGP = data.getElementsByTagName('dtGroupProgramID');
            if (dtGP.length > 0) {
                $('#DVTreeView_SystemID').jstree(true).uncheck_all();
                for (var i = 0; i < dtGP.length; i++) {
                    let defaultid = GetNodeValue(dtGP[i], "id");
                    $('#DVTreeView_SystemID').jstree('check_node', defaultid);
                }
            }
            var dtGB = data.getElementsByTagName('dtGroupButton');
            if (dtGB.length > 0) {
                for (var i = 0; i < dtGB.length; i++) {
                    if (GetNodeValue(dtGB[i], "Flag1") == "1") {
                        //let defaultid = GetNodeValue(dtGB[i], "id");
                        //alert(defaultid);
                        $('#DVTreeView_SystemID').jstree('check_node', GetNodeValue(dtGB[i], "id"));
                    } else {
                        $('#DVTreeView_SystemID').jstree('uncheck_node', GetNodeValue(dtGB[i], "id"));
                    }

                }
            }
        }
    }

    let btSave_Click = function () {
        let checkedpid = $('#DVTreeView_SystemID').jstree(true).get_checked().filter(function (obj) {
            //alert(obj.substring(0, 2).toString());
            return obj.substring(0, 2).toString() === "P_";
        });
        //alert(checkedpid.toString());
        let checkedbid = $('#DVTreeView_SystemID').jstree(true).get_checked().filter(function (obj) {
            return obj.substring(0, 2).toString() === "B_";
        });
        //alert(checkedbid.toString());
        if ($('#txtHGroupID').val() == "" | $('#txtHGroupID').val() == null) {
            DyAlert("請選擇群組");
            return;
        }
        if (checkedpid == "" | checkedpid == null) {
            DyAlert("請勾選程式名稱");
            return;
        }
        //alert($('#txtHGroupID').val().substring(2, $('#txtHGroupID').val().length));
        //let lv = $('#selLevel').val();
        var pData = {
            GPIDWeb: [{
                LV: "3",
                GroupID: $('#txtHGroupID').val().substring(2, $('#txtHGroupID').val().length),
                ProgramID: checkedpid.toString(),
                Btn: checkedbid.toString()
            }]
        };
        PostToWebApi({ url: "api/SystemSetup/EditGroupProgram", data: pData, success: afterEditGroupProgram });
    }
    //#endregion

    //#region 人員新增
    let AfterAddEmp = function (data) {
        //alert(ReturnMsg(data, 0));
        if (ReturnMsg(data, 0) != "AddEmpOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            DyAlert("人員新增完成!");
            $('#selELevel')[0].selectedIndex = 0;
            $('input[name="Emp"]').val('');
            $('#WhName').text('');
            $('#ELV').hide();
        }
    }

    let afterChkEmpExist = function (data) {
        //alert(ReturnMsg(data, 0));
        if (ReturnMsg(data, 0) != "ChkEmpExistOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            var dtchkE = data.getElementsByTagName('dtEmp');
            //alert(dtchkE.length);
            if (dtchkE.length > 0) {
                let msgstr = "";
                if (setHEtype == "ID") {
                    msgstr = "員工代碼已存在!";
                }
                else {
                    msgstr = "Email已存在!"
                }
                DyAlert(msgstr);
                return;
            }
            if (setHEtype == "ID") {
                setHEtype = "Eadd";
                ChkEmpExist(setHEtype);
                return;
            }
            var pData = {
                EmployeeSV: [
                    {
                        Man_ID: $('#txtManID').val().trim(),
                        Man_Name: $('#txtManName').val().trim(),
                        Password: $('#txtPWD').val().trim(),
                        Level: $('#selELevel').val(),
                        Man_ComeDay: $('#txtSDate').val(),
                        Man_Eaddress: $('#txtEmail').val(),
                        Man_Tel: $('#txtTel').val(),
                        WhNO: $('#selELevel').val() == "1" ? $('#txtWhNo').val() : ""
                    }
                ]

            }
            PostToWebApi({ url: "api/SystemSetup/AddEmp", data: pData, success: AfterAddEmp });
        }
    }

    let ChkEmpExist = function (setType) {
        var pData = {
            SetType: setType,
            Setstr: setType == "ID" ? $('#txtManID').val() : $('#txtEmail').val()
        };
        PostToWebApi({ url: "api/SystemSetup/ChkEmpExist", data: pData, success: afterChkEmpExist });
    }

    let InputValidation = function () {
        if ($('#txtManID').val() == "" | $('#txtManID').val() == null |
            $('#txtManName').val() == "" | $('#txtManName').val() == null |
            $('#txtPWD').val() == "" | $('#txtPWD').val() == null |
            $('#txtPWD2').val() == "" | $('#txtPWD2').val() == null |
            $('#txtSDate').val() == "" | $('#txtSDate').val() == null |
            $('#txtEmail').val() == "" | $('#txtEmail').val() == null |
            $('#txtTel').val() == "" | $('#txtTel').val() == null) {
            DyAlert('所有欄位必須輸入!');
            return false;
        }

        if ($('#selELevel').val() == "1") {
            if ($('#txtWhNo').val() == "" | $('#txtWhNo').val() == null) {
                DyAlert('所屬店櫃必須輸入!');
                return false;
            }
        }
        let str = "";
        let re = "";
        //#region 員工代碼
        str = $('#txtManID').val();
        re = /^[\d|a-zA-Z]+$/;
        if (!re.test(str) | str.length > 6) {
            DyAlert('員工代碼必須小於等於6碼英數字!');
            return false;
        }
        str = ""; re = "";
        //#endregion
        //#region 姓名
        str = $('#txtManName').val();
        if (str.length > 10) {
            DyAlert('姓名必須10字元以內!');
            return false;
        }
        str = "";
        //#endregion
        //#region 新密碼_再次確認
        str = $('#txtPWD').val();
        re = /^[\d|a-zA-Z]+$/;
        if (!re.test(str) | str.length < 6 | str.length > 20) {
            DyAlert('密碼必須6~20碼英數字!');
            return false;
        }
        else {
            let str2 = $('#txtPWD2').val();
            if (str2 != str) { DyAlert('輸入的密碼不相符!'); return false; }
        }
        str = ""; re = "";
        //#endregion
        //#region Email
        str = $('#txtEmail').val();
        re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (!re.test(str)) {
            DyAlert('Email格式錯誤!');
            return false;
        }
        str = ""; re = "";
        //#endregion
        //#region 手機號碼
        str = $('#txtTel').val();
        re = /^09\d{8}$/;
        if (!re.test(str)) {
            DyAlert('手機號碼格式錯誤!');
            return false;
        }
        str = ""; re = "";
        //#endregion
        return true;
    }

    let btE_Yes_click = function () {
        if (!InputValidation()) { return; };
        setHEtype = "ID";
        ChkEmpExist(setHEtype);
    }

    let btE_Cancel_click = function () {
        $('#modal_msPV102_AddE').modal('hide');
    }

    let AfterSelWhNoOut = function (xml) {
        $('#txtWhNo').val(GetNodeValue(xml, "ST_ID"));
        $('#WhName').text(GetNodeValue(xml, "ST_Sname"));
    }

    let afterChangeAddELevel = function () {
        var ELV = $('#selELevel').val();
        $('#txtWhNo').val('');
        $('#WhName').text('');
        if (ELV == "1") {
            $('#ELV').show();
        }
        else {
            $('#ELV').hide();
        }
    }

    let btAddE_Click = function () {
        $('#modal_msPV102_AddE .modal-title').text('人員新增');
        SetDateField($('#txtSDate')[0]);
        $('#txtSDate').val('');
        $('#selELevel').change(function () { afterChangeAddELevel(); });
        let csgOption = {
            InputElementsID: "txtWhNo",
            ApiForGridData: "api/SetCommSelectGridDefaultApi",
            PostDataForApi: {
                Table: "WarehouseSV",
                Column: ["ST_ID", "ST_Sname"],
                Caption: ["店代號", "店名稱"],
                OrderColumn: "ST_ID",
                Condition: "1=1",
            },
            AfterSelectData: AfterSelWhNoOut
        }
        SetCommSelectGrid(csgOption);
        $('#selELevel')[0].selectedIndex = 0;
        $('input[name="Emp"]').val('');
        $('#ELV').hide();
        $('#modal_msPV102_AddE').modal('show');
    }
    //#endregion

    let afterSetHGroupID = function (data) {
        if (ReturnMsg(data, 0) != "GetGroupProgramOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            $('#DVTreeView_SystemID').jstree(true).uncheck_all();
            $('#DVTreeView_SystemID').jstree(true).close_all();
            var dtGP = data.getElementsByTagName('dtGroupProgramID');
            if (dtGP.length > 0) {
                let oldsid = "";
                for (var i = 0; i < dtGP.length; i++) {
                    let defaultid = GetNodeValue(dtGP[i], "id");
                    //alert(defaultid);
                    $('#DVTreeView_SystemID').jstree('check_node', defaultid);
                    if (oldsid != GetNodeValue(dtGP[i], "parent")) {
                        $('#DVTreeView_SystemID').jstree('open_node', GetNodeValue(dtGP[i], "parent"));
                        oldsid = GetNodeValue(dtGP[i], "parent");
                    }
                }
            }
            var dtGB = data.getElementsByTagName('dtGroupButton');
            if (dtGB.length > 0) {
                for (var i = 0; i < dtGB.length; i++) {
                    if (GetNodeValue(dtGB[i], "Flag1") == "1") {
                        //let defaultid = GetNodeValue(dtGB[i], "id");
                        //alert(defaultid);
                        $('#DVTreeView_SystemID').jstree('check_node', GetNodeValue(dtGB[i], "id"));
                    } else {
                        $('#DVTreeView_SystemID').jstree('uncheck_node', GetNodeValue(dtGB[i], "id"));
                    }

                }
            }
        }

    };

    let afterChangeLevel = function (data) {
        ReSetTree("H", data);

        $("#DVTreeView_GroupID").bind("changed.jstree",
            function (e, data) {
                let chkedid = data.node.id;
                if (chkedid.substring(0, 1) === "G") {
                    chkgroupid = chkedid
                    data.instance.open_node(chkgroupid);
                }
                else {
                    chkgroupid = data.node.parent;
                }
                if (chkgroupid === "" | chkgroupid === null) {
                    $('#txtHGroupID').val('');
                }
                else {
                    //$('#txtHGroupID').val(chkgroupid.substring(2, chkgroupid.length));
                    $('#txtHGroupID').val(chkgroupid);
                }
                if (oldHChkedgid != chkgroupid) {
                    //alert("selected:"+oldHChkedgid);
                    if (oldHChkedgid != "" && oldHChkedgid != null) {
                        //alert("sd:" + oldHChkedgid);
                        data.instance.close_node(oldHChkedgid);
                    }
                    oldHChkedgid = chkgroupid;
                    //alert("ReSet:" +oldHChkedgid);
                }

                //let lv = $('#selLevel').val();
                var pData = {
                    LV: "3",
                    GroupID: $('#txtHGroupID').val().substring(2, $('#txtHGroupID').val().length)
                };
                PostToWebApi({ url: "api/SystemSetup/GetGroupProgram", data: pData, success: afterSetHGroupID });

            })

        $('#txtHGroupID').val('');
        var dtG = data.getElementsByTagName('dtGroupID');
        $('#DVTreeView_GroupID').jstree('open_node', GetNodeValue(dtG[0], "id"));
        $('#DVTreeView_GroupID').jstree('select_node', GetNodeValue(dtG[0], "id"));
        $('#txtHGroupID').val(GetNodeValue(dtG[0], "id"));
        var dtGP = data.getElementsByTagName('dtGroupProgramID');
        if (dtGP.length > 0) {
            for (var i = 0; i < dtGP.length; i++) {
                let defaultid = GetNodeValue(dtGP[i], "id");
                //alert(defaultid);
                $('#DVTreeView_SystemID').jstree('check_node', defaultid);
            }
        }
        var dtGB = data.getElementsByTagName('dtGroupButton');
        if (dtGB.length > 0) {
            for (var i = 0; i < dtGB.length; i++) {
                if (GetNodeValue(dtGB[i], "Flag1") == "1") {
                    //let defaultid = GetNodeValue(dtGB[i], "id");
                    //alert(defaultid);
                    $('#DVTreeView_SystemID').jstree('check_node', GetNodeValue(dtGB[i], "id"));
                } else {
                    $('#DVTreeView_SystemID').jstree('uncheck_node', GetNodeValue(dtGB[i], "id"));
                }

            }
        }
    };

    let ReLoad = function () {
        //let lv = $('#selLevel').val();
        var pData = {
            LV: "3",
            FormLoad: "N"
        };
        PostToWebApi({ url: "api/SystemSetup/GetTreeData", data: pData, success: afterChangeLevel });
    }

    let afterGetInitVPV01 = function (data) {
        if (ReturnMsg(data, 0) != "GetTreeDataOK") {
            DyAlert(ReturnMsg(data, 1));
        }
        else {
            
            //var dtL = data.getElementsByTagName('dtLV');
            //InitSelectItem($('#selLevel')[0], dtL, "LV", "LName", true);
            //$('#selLevel option:eq(0)').remove();

            //#region SetLoad_H
            //$('#selLevel').change(function () { ReLoad(); });
            $('#btAddG').click(function () { btAddG_Click(this) });
            $('#btModE').click(function () { btModE_Click(this) });
            //$('#btAddE').click(function () { btAddE_Click(this) });
            $('#btSave').click(function () { btSave_Click(this) });
            afterChangeLevel(data);
            //#endregion

            //#region SetLoad_G
            $('#btG_Yes').click(function () { btG_Yes_click(); });
            $('#btG_Cancel').click(function () { btG_Cancel_click(); });
            //#endregion

            //#region SetLoad_E
            $('#btGE_Yes').click(function () { btGE_Yes_click(); });
            $('#btGE_Cancel').click(function () { btGE_Cancel_click(); });
            //#endregion

            //#region SetLoad_AddE
            //dtEL = data.getElementsByTagName('dtLV');
            //InitSelectItem($('#selELevel')[0], dtEL, "LV", "LName", true);
            //$('#selELevel option:eq(0)').remove();
            //$('#btE_Yes').click(function () { btE_Yes_click(); });
            //$('#btE_Cancel').click(function () { btE_Cancel_click(); });
            //#endregion

            $('#pgmsPV102').show();
        };
        //alert("afterGetInitVPV01");
    };

    let afterLoadPage = function () {
        //alert("afterLoadPage");
        var pData = {
                LV: "3",
                FormLoad: "Y"
        };
        PostToWebApi({ url: "api/SystemSetup/GetTreeData", data: pData, success: afterGetInitVPV01 });
    };
    
    if ($('#pgmsPV102').length == 0) {
        setTimeout(function () {
            AllPages = new LoadAllPages(ParentNode, "SystemSetup/MSPV102", ["pgMSPV102"], afterLoadPage);
        }, 100);
    };


}