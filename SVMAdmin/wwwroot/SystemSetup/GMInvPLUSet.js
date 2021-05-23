var PageGMInvPLUSet = function (ParentNode) {
    let dtWarehouseDSVBlank;
    let dtMachineListSpecBlank;
    let obSelPLUModal;
    let ST_ID;
    let CkNo;
    let LayerNo;

    let AssignVar = function () {
        obSelPLUModal = new SelPLUModal();
        $('#selST_ID').change(function () {
            let st_id = $('#selST_ID').val();
            InitSelectItem($('#selLayerNo')[0], dtMachineListSpecBlank, "LayerNo", "LayerNo", true, "請選擇貨艙代號");
            if (st_id == "") {
                InitSelectItem($('#selCkno')[0], dtWarehouseDSV, "SNno", "CkNo", true, "請選擇機號");
            }
            else {
                var pData = {
                    ST_ID: st_id
                };
                PostToWebApi({ url: "api/SystemSetup/GetCkNoByST_ID", data: pData, success: AfterGetCkNoByST_ID });
            }
        });

        $('#selCkno').change(function () {
            let snno = $('#selCkno').val();
            if (snno == "") {
                InitSelectItem($('#selLayerNo')[0], dtMachineListSpecBlank, "LayerNo", "LayerNo", true, "請選擇貨艙代號");
            }
            else {
                var pData = {
                    SNno: snno
                };
                PostToWebApi({ url: "api/SystemSetup/GetLayerNoBySNno", data: pData, success: AfterGetLayerNoBySNno });
            }
        });


        $('#selLayerNo').change(function () {
            let layerno = $('#selLayerNo').val();
            let snno = $('#selCkno').val();
            if (layerno == "") {
                InitSelectItem($('#selLayerNo')[0], dtMachineListSpecBlank, "LayerNo", "LayerNo", true, "請選擇貨艙代號");
            }
            else {
                var pData = {
                    SNno: snno,
                    LayerNo: layerno
                };
                PostToWebApi({ url: "api/SystemSetup/GetInvtInfo", data: pData, success: AfterGetInvtInfo });
            }
        });

        $('#btSaveNewInventorySV').click(function () {
            let records = [];
            let cbs = $('#ContainerChanSet .CubeChannel-1');
            for (let i = 0; i < cbs.length; i++) {
                var cb = cbs[i];
                if ($(cb).hasClass('CubeChannel-1_mask')) {
                    continue;
                }
                let rec = $(cb).prop('Record');
                let MachineInfo = $(cb).prop('MachineInfo');
                if (rec != null) {
                    let record = {
                        WhNO: ST_ID,
                        PLU: GetNodeValue(rec, "GD_NO"),
                        PTNum: 0,
                        SafeNum: 1,
                        In_Date: "",
                        Out_Date: "",
                        StartSalesDate: "",
                        EndSalesDate: "",
                        DisPlayNum: $(cb).find('.ChanQty').val(),
                        CkNo: CkNo,
                        Layer: LayerNo,
                        Sno: GetNodeValue(MachineInfo, "ChannelNo"),
                        EffectiveDate: ""
                    }
                    records.push(record);
                }
            }
            var pData = {
                InventorySV: records
            };
            PostToWebApi({ url: "api/SystemSetup/SaveNewInventorySV", data: pData, success: AfterSaveNewInventorySV });
        });
    };


    let AfterSaveNewInventorySV = function (data) {
        if (ReturnMsg(data, 0) != "SaveNewInventorySVOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            DyAlert("完成!");
        }
    }

    let AfterGetCkNoByST_ID = function (data) {
        let dtWarehouseDSV = data.getElementsByTagName('dtWarehouseDSV');
        InitSelectItem($('#selCkno')[0], dtWarehouseDSV, "SNno", "CkNo", true, "請選擇機號");
    }

    let AfterGetLayerNoBySNno = function (data) {
        let dtMachineListSpec = data.getElementsByTagName('dtMachineListSpec');
        InitSelectItem($('#selLayerNo')[0], dtMachineListSpec, "LayerNo", "LayerNo", true, "請選擇貨艙代號");
    }


    //尚未設定InventorySV
    let AfterGetInvtInfo = function (data) {
        if (ReturnMsg(data, 0) == "AlreadySetInventorySV") {
            GetInventorySVData();
        }
        else if (ReturnMsg(data, 0) != "GetInvtInfoOK") {
            DyAlert(ReturnMsg(data, 0));
        }
        else {
            let dtMachineListSpec = data.getElementsByTagName('dtMachineListSpec');
            var Chans = dtMachineListSpec.length;
            $('.dispThisSetting').text('');
            if (Chans == 0) {
                DyAlert("這個貨倉尚未設定！", DummyFunction);
                return;
            }
            $('#ContainerChanSet').empty();
            for (var i = 0; i < 10; i++) {
                let cube = new PluBlock();
                if (i >= Chans)
                    cube.SetDesabled(true);
                else
                    cube.SetMachineInfo(dtMachineListSpec[i]);
                cube.SetPluName("商品名稱");
                cube.SetPrice("0");
                cube.SetStock("0");
                cube.SetQuantity("0");
            }

            ST_ID = GetNodeValue(dtMachineListSpec[0], "ST_ID");
            CkNo = GetNodeValue(dtMachineListSpec[0], "CkNo");
            LayerNo = GetNodeValue(dtMachineListSpec[0], "LayerNo");

            let dispThisSetting = GetNodeValue(dtMachineListSpec[0], "ST_Sname");
            dispThisSetting += "(" + ST_ID + ")\t";
            dispThisSetting += CkNo + "機\t";
            dispThisSetting += LayerNo + "倉\t";
            dispThisSetting += GetNodeValue(dtMachineListSpec[0], "Type_Name");
            $('.dispThisSetting').text(dispThisSetting);            
        }
    }


    let GetInventorySVData = function () {
        let layerno = $('#selLayerNo').val();
        let snno = $('#selCkno').val();
        var pData = {
            SNno: snno,
            LayerNo: layerno
        };
        PostToWebApi({ url: "api/SystemSetup/GetInventorySVData", data: pData, success: AfterGetInventorySVData });

    }

    //已經設定InventorySV，顯示only
    let AfterGetInventorySVData = function (data) {
        let dtMachineListSpec = data.getElementsByTagName('dtMachineListSpec');
        var Chans = dtMachineListSpec.length;
        $('.dispThisSetting').text('');
        if (Chans == 0) {
            DyAlert("這個貨倉尚未設定！", DummyFunction);
            return;
        }
        $('#ContainerChanSet').empty();
        for (var i = 0; i < 10; i++) {
            let cube = new PluBlock();
            if (i >= Chans) {
                cube.SetDesabled(true);
                cube.SetPluName("商品名稱");
                cube.SetPrice("0");
                cube.SetStock("0");
                cube.SetQuantity("0");
            }
            else {
                let rec = dtMachineListSpec[i];
                cube.SetMachineInfo(dtMachineListSpec[i]);
                cube.SetPluName(GetNodeValue(rec, 'GD_NAME'));
                cube.SetPrice(parseInt(GetNodeValue(rec, 'GD_PRICES')));
                cube.SetStock("0");
                cube.SetQuantity(parseInt(GetNodeValue(rec, 'DisPlayNum')));
                cube.SetPicture(GetNodeValue(rec, 'Photo1'));
                cube.SetDesabled(true);
            }
        }
        ST_ID = GetNodeValue(dtMachineListSpec[0], "ST_ID");
        CkNo = GetNodeValue(dtMachineListSpec[0], "CkNo");
        LayerNo = GetNodeValue(dtMachineListSpec[0], "LayerNo");

        let dispThisSetting = GetNodeValue(dtMachineListSpec[0], "ST_Sname");
        dispThisSetting += "(" + ST_ID + ")\t";
        dispThisSetting += CkNo + "機\t";
        dispThisSetting += LayerNo + "倉\t";
        dispThisSetting += GetNodeValue(dtMachineListSpec[0], "Type_Name");
        $('.dispThisSetting').text(dispThisSetting);        
    }


    let PluBlock = function () {

        let cube = $('#CubeChannelTemp .CubeChannel-1').clone();
        let img = cube.find('.picPLU');
        let addIcon = cube.find('.IconSetPLU');
        img.prop('src', '');
        img.hide();
        addIcon.show();
        $('#ContainerChanSet').append(cube);

        addIcon.click(function () {
            obSelPLUModal.Show(AfterSelPlu);
        });

        img.click(function () {
            obSelPLUModal.Show(AfterSelPlu);
        });

        cube.find('.ChanQty').change(function () {
            $('#btSaveNewInventorySV').prop('disabled', !CheckAllBlockSet());
        });

        let AfterSelPlu = function (Record) {
            let PluName = GetNodeValue(Record, 'GD_Sname');
            if (PluName == '')
                PluName = (GetNodeValue(Record, 'GD_NAME') + '      ').substr(0, 6);
            cube.find('.PluName').text(PluName);
            cube.find('.PluPrice').text(parseInt(GetNodeValue(Record, 'GD_PRICES')));
            cube.find('.PluPrice').text(parseInt(GetNodeValue(Record, 'GD_PRICES')));
            let Photo1 = GetNodeValue(Record, 'Photo1');
            ShowPicture(Photo1);
            cube.prop('Record', Record);
            $('#btSaveNewInventorySV').prop('disabled', !CheckAllBlockSet());
        }

        let ShowPicture = function (Photo1) {
            img.show();
            addIcon.hide();
            if (Photo1 == "")
                cube.find('.picPLU').prop('src', 'images/No_Pic.jpg');
            else {
                let url = "api/GetImageResize?SGID=" + EncodeSGID(Photo1) + "&UU=" + encodeURIComponent(UU);
                url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
                url += "&MaxPix=300";
                cube.find('.picPLU').prop('src', url);
            }
        }
        
        this.SetDesabled = function (disableYN) {
            if (disableYN) {
                cube.addClass('CubeChannel-1_mask');
                cube.find('.ChanQty').prop('disabled', true);
            }
            else
                cube.removeClass('CubeChannel-1_mask');
        }

        this.SetPluName = function (PluName) {
            cube.find('.PluName').text(PluName);
        }

        this.SetPrice = function (price) {
            cube.find('.PluPrice').text(price);
        }

        this.SetStock = function (stock) {
            cube.find('.PluStock').text(stock);
        }

        this.SetQuantity = function (Quantity) {
            cube.find('.ChanQty').val(Quantity);
        }

        this.SetMachineInfo = function (record) {
            cube.prop('MachineInfo', record);
        }

        this.SetPicture = function (PicSGID) {
            ShowPicture(PicSGID);
        }

    }

    let CheckAllBlockSet = function () {
        let cbs = $('#ContainerChanSet .CubeChannel-1');
        let allset = true;
        for (let i = 0; i < cbs.length; i++) {
            var cb = cbs[i];
            if ($(cb).hasClass('CubeChannel-1_mask')) {
                continue;
            }
            if ($(cb).prop('Record') == null) {
                allset = false;
                break;
            }
            else {
                let qty = $(cb).find('.ChanQty').val();
                if (parseInt(qty) == NaN || parseInt(qty) == 0) {
                    allset = false;
                    break;
                }
            }
        }
        return allset;
    }

    let SelPLUModal = function () {
        let HandleAfterSel = null;

        $('#btSelPLU').prop('disabled', true);

        $('#btCanSelPLU').click(function ()
        {
            CloseModal();
        });

        $('#btSearchPlu').click(function () {
            SearchPlu();
        });

        $('#btSelPLU').click(function () {
            let selCube = $('#modal_GMInvPLUSet .ActivePLU');
            if (selCube.length > 0 & HandleAfterSel != null) {
                HandleAfterSel($(selCube[0]).prop('Record'));
                $('#modal_GMInvPLUSet').modal('hide');
            }
        });

        this.Show = function (Handle) {
            $('#ContainerSelPlu').empty();
            $('#btSelPLU').prop('disabled', true);
            $('#modal_GMInvPLUSet').modal('show');
            HandleAfterSel = Handle;
        };

        let CloseModal = function () {
            $('#modal_GMInvPLUSet').modal('hide');
        };

        let SearchPlu = function () {
            $('#btSelPLU').prop('disabled', true);
            var pData = {
                KeyWord: $('#txtPLUSearch').val()
            };
            PostToWebApi({ url: "api/SystemSetup/SearchPLU", data: pData, success: AfterSearchPLU });
        }

        let AfterSearchPLU = function (data) {
            if (ReturnMsg(data, 0) != "SearchPLUOK") {
                DyAlert(ReturnMsg(data, 0));
                return;
            }
            else {
                var dtPLU = data.getElementsByTagName('dtPLU');
                $('#ContainerSelPlu').empty();
                if (dtPLU.length == 0) {
                    DyAlert("無符合資料!", BlankMode);
                    return;
                }
                
                for (let i = 0; i < dtPLU.length; i++)
                    PluBlockS(dtPLU[i]);
            }
        }

        let PluBlockS = function (Record) {
            let cube = $('#CubePluTemp .CubePlu-1').clone();
            cube.prop('Record', Record);
            $('#ContainerSelPlu').append(cube);
            let PluName = GetNodeValue(Record, 'GD_Sname'); 
            if (PluName == '')
                PluName = (GetNodeValue(Record, 'GD_NAME') + '      ').substr(0, 6);
            cube.find('.PluName').text(PluName);
            cube.find('.PluGDNO').text(GetNodeValue(Record, 'GD_NO'));
            let Photo1 = GetNodeValue(Record, 'Photo1'); 
            if (Photo1 == "")
                cube.find('.picPLU').prop('src', 'images/No_Pic.jpg');
            else {
                let url = "api/GetImageResize?SGID=" + EncodeSGID(Photo1) + "&UU=" + encodeURIComponent(UU);
                url += "&Ver=" + encodeURIComponent(new Date().toLocaleTimeString());
                url += "&MaxPix=200";
                cube.find('.picPLU').prop('src', url);
            }
            cube.click(function (){
                $('#btSelPLU').prop('disabled', false);
                $('#modal_GMInvPLUSet .ActivePLU').removeClass('ActivePLU');
                $(this).addClass('ActivePLU');
            });
        }
    }

    let afterGetInitGMInvPLUSet = function (data) {
        var dtWarehouse = data.getElementsByTagName('dtWarehouse');
        dtWarehouseDSVBlank = data.getElementsByTagName('dtWarehouseDSV');
        dtMachineListSpecBlank = data.getElementsByTagName('dtMachineListSpec');
        InitSelectItem($('#selST_ID')[0], dtWarehouse, "ST_ID", "ST_Sname", true, "請選擇店代號");
        InitSelectItem($('#selCkno')[0], dtWarehouseDSVBlank, "SNno", "CkNo", true, "請選擇機號");
        InitSelectItem($('#selLayerNo')[0], dtMachineListSpecBlank, "LayerNo", "LayerNo", true, "請選擇貨艙代號");
        AssignVar();

    };

    let afterLoadPage = function () {
        PostToWebApi({ url: "api/SystemSetup/GetInitGMInvPLUSet", success: afterGetInitGMInvPLUSet });
        $('#pgGMInvPLUSet').show();
    };

    if ($('#pgGMMacPLUSet').length == 0) {
        AllPages = new LoadAllPages(ParentNode, "SystemSetup/GMInvPLUSet", ["pgGMInvPLUSet"], afterLoadPage);
    };




}