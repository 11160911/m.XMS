

String.prototype.padLeft = function (l, c) { return Array(l - this.length + 1).join(c || " ") + this }
String.prototype.padRight = function (l, c) { return this + Array(l - this.length + 1).join(c || " ") }

var DummyFunction = function () { };

var UU = null;

var PostToWebApi = function (option) {

    let AlertErr = function (XMLHttpRequest, textStatus, errorThrown) {
        alert(XMLHttpRequest.status);
    };

    if (UU != null)
        option.headers = { "Authorization": "Bearer " + UU };
    var defultoption = {
        method: "POST",
        url: option.url == null ? null : option.url,
        contentType: option.contentType == null ? "application/x-www-form-urlencoded; charset=UTF-8" : option.contentType,
        dataType: option.dataType == null ? "xml" : option.dataType,
        headers: option.headers == null ? null : option.headers,
        data: option.data == null ? null : option.data,
        success: option.success == null ? null : option.success,
        error: option.error == null ? AlertErr : option.error,
        complete: option.complete == null ? null : option.complete
    };
    $.ajax(defultoption);
}

var ReturnMsg = function (rr, iIndex) {
    var xmlRT = rr.getElementsByTagName('dtMessage')[0];
    var nodename = "Msg" + iIndex;
    var Msg0 = GetNodeValue(xmlRT, nodename);
    return Msg0;
}

var GetNodeValue = function (xmlobj, ndName, Digits) {
    var nds = $(xmlobj).children(ndName);
    if (nds.length == 0)
        return '';
    var nd = nds[0];
    if (Digits == null) {
        if (nd != null) {
            var str = $(nd).text();
            if (str == '0.0000')
                str = '0';
            return str;
        }
        else {
            return '';
        }
    }
    else {
        if (nd != null) {
            var str = $(nd).text();
            if (!isNaN(str)) {
                var nn = parseFloat(str);
                if (Digits == 0)
                    str = '' + Math.round(nn);
                if (Digits == 2)
                    str = '' + Math.round(nn * 100) / 100;
                if (Digits == 1)
                    str = '' + Math.round(nn * 10) / 10;
                if (Digits == 'N')
                    str = '' + nn;
            }
            if (str == '0.0000')
                str = '0';
            return addCommas(str);
        }
        else {
            return '';
        }
    }
}

var LoadAllPages = function (ParentNode, pgHtml, pages, afterLoadAllPage ,data) {
    var iPg = 0;
    var divN = $('<div></div>');
    $("body").append(divN);
    var LoadPage = function () {
        if (data == null) {
            data = [];
            for (var i = 0; i < pages.length; i++)
                data.push(null);
        }
        var rqdata = data[iPg];
        divN.load(pgHtml + " #" + pages[iPg], rqdata, function () { LoadDone(); });
    };

    var LoadDone = function () {
        var p = $('#' + pages[iPg]).detach();
        $(ParentNode).append(p);
        iPg++;

        if (iPg < pages.length)
            LoadPage();
        else {
            divN.remove();
            afterLoadAllPage();
        }
    };

    this.RemoveAllPage = function () {
        for (var i = 0; i < pages.length; i++)
            $('#' + pages[iPg]).remove();
    };

    LoadPage();
};

var DynGrid = function (option) {
    
    var elm_table = option.table_lement;
    var ar_classnames = option.class_collection;
    var fields = option.fields_info;
    var clicktr = option.method_clickrow;
    var rows_per_page = option.rows_per_page;
    var fixPage = false;
    var fixPageA = null;
    var ths = $(elm_table).find('thead th');
    var activeTR = null;
    var pagination = null;
    var btExpXls = null;
    var thisXmls = null;
    var export_xlsx_api = option.export_xlsx_api;
    var iframe_for_export = null;
    var disable_select = false;
    var theads = $(elm_table).find('thead tr th');
    for (var x = 0; x < theads.length; x++) {
        $(theads[x]).prop('fdinfo', fields[x]);
        var tdclass = ar_classnames[x];
        if (tdclass != null) {
            var cls = tdclass.split(' ');
            for (var c = 0; c < cls.length; c++)
                $(theads[x]).addClass(cls[c]);
        }
    }

    if (export_xlsx_api != null) {
        btExpXls = $('<button type="button" class="btn btn-oval btn-primary">匯出xlsx</button>');
        btExpXls.css('margin-top', '5px');
        btExpXls.css('margin-left', '10px');
        btExpXls.css('max-width', '110px');
        btExpXls.css('display', 'inline-block');
        btExpXls.hide();
        iframe_for_export = $("<iframe hidden></iframe>");
        $(elm_table).closest('div').after(btExpXls);
        $(elm_table).closest('div').after(iframe_for_export);
        btExpXls.click(function () { click_btExpXls();});
    }

    if (rows_per_page != null) {
        var nav = $('<nav class="text-left"></nav>');
        pagination = $('<ul class="pagination"></ul>');
        nav.append(pagination);
        nav.css('margin-top', '5px');
        nav.css('display', 'inline-block');
        $(elm_table).closest('div').after(nav);
        var spTotal = $('<sapn class="rows_per_page_spTotal"></span>');
        spTotal.css('padding-left', '5px');
        spTotal.css('color', '#999999');
        spTotal.css('font-size', '0.9rem');
        pagination.prop('rows_per_page_spTotal', spTotal);
        nav.append(spTotal);
    }

    if (option.sortable != null) {
        if (option.sortable == "Y") {
            $(elm_table).find('thead tr th').click(
                function () { SortData(this); }
            );
        }
    }

    if (option.fixPage != null)
        fixPage = option.fixPage;


    var click_btExpXls = function () {
        var xmls = thisXmls;
        var fds = JSON.parse(JSON.stringify(fields));
        var ths = $(elm_table).find('thead tr th');
        for (var i = 0; i < ths.length; i++) {
            fds[i]["head"] = $(ths[i]).text();
            fds[i]["ishidden"] = "N";
            if ($(ths[i]).is(":hidden"))
                fds[i]["ishidden"] = "Y";
        }

        var Records = [];
        for (var r = 0; r < xmls.length; r++) {
            var record = {};
            for (var c = 0; c < fds.length; c++) {
                var fdname = fds[c].name;
                if (fdname != null) {
                    var value = GetNodeValue(xmls[r], fdname);
                    record[fdname] = value;
                }
            }
            Records.push(record);
        }

        var pData = {
            Fields: fds,
            Records: Records
        };
        PostToWebApi({ url: export_xlsx_api, data: pData, success: After_export_xlsx_api });
    }

    var After_export_xlsx_api = function (data) {
        if (ReturnMsg(data, 0) != "XlsExportOK") {
            DyAlert(ReturnMsg(data, 0));
            return;
        }
        else {
            var url = "../Admin/ExportXlsx?ID=" + ReturnMsg(data, 1);
            iframe_for_export.prop('src', url);
        }
    }

    var gbody = $(elm_table).children('tbody');

    var clicktr = function (tr) {
        if (disable_select)
            return;
        gbody.children('tr').removeClass('dygrid-activetr');
        activeTR = tr;
        $(tr).addClass('dygrid-activetr');
        if (option.method_clickrow != null)
            option.method_clickrow(tr);
    };

    var SortData = function (td) {
        if (thisXmls == null)
            return;
        if (thisXmls.length == 0)
            return;
        var fdinfo = $(td).prop('fdinfo');
        var fdType = fdinfo.type;
        var fdName = fdinfo.name;
        if (fdType != "Text" & fdType != "TextAmt")
            return;
        var srot = "asc";
        var em = $(td).find('em');
        if (em.length > 0) {
            if (em.hasClass("fa-sort-asc"))
                srot = "desc";
        }
        var tr = $(td).closest('tr');
        $(tr).find('em').remove();
        var emn = $('<em class="fa"></em>');
        emn.css('padding-right','8px');
        var xmlsort = $(thisXmls).clone();
        if (srot == "asc") {
            emn.addClass("fa-sort-asc");
            if (fdType == "Text")
                xmlsort.sort(function (a, b) { return GetNodeValue(a, fdName) > GetNodeValue(b, fdName) ? 1 : -1 });
            else if (fdType == "TextAmt")
                xmlsort.sort(function (a, b) { return parseFloat(GetNodeValue(a, fdName)) > parseFloat(GetNodeValue(b, fdName)) ? 1 : -1 });
        }
        else {
            emn.addClass("fa-sort-desc");
            if (fdType == "Text")
                xmlsort.sort(function (a, b) { return GetNodeValue(a, fdName) > GetNodeValue(b, fdName) ? -1 : 1 });
            else if (fdType == "TextAmt")
                xmlsort.sort(function (a, b) { return parseFloat(GetNodeValue(a, fdName)) > parseFloat(GetNodeValue(b, fdName)) ? -1 : 1 });
        }
        $(td).prepend(emn);
        thisBindData(xmlsort);
    };

    var MappingRecord = function (tr, xml) {
        tr.prop('Record', xml);
        for (var j = 0; j < ar_classnames.length; j++) {
            var td = $('<td></td>');
            var tdclass = ar_classnames[j];
            if (tdclass != null) {
                var cls = tdclass.split(' ');
                for (var c = 0; c < cls.length; c++)
                    td.addClass(cls[c]);
            }
            tr.append(td);
            var fdinfo = fields[j];
            var fdType = fdinfo.type;
            var fdName = fdinfo.name;
            var fdStyle = fdinfo.style;
            td.prop('FieldName', fdName);
            if (fdType == 'Blank') {
                //do nothing
            }
            if (fdType == 'Image') {
                var img = $("<img />")
                td.append(img);
                tr.prop(fdName, img);
                img.css('height', '50px');
            }
            else if (fdType == 'Text' || fdType == 'TextAmt' || fdType == 'TextTime') {
                td.text(GetNodeValue(xml, fdName));
                if (fdType == 'TextAmt') {
                    td.css('textAlign', 'right');
                    var str = GetNodeValue(xml, fdName);
                    var amt = parseFloat(str);
                    td.text(new Intl.NumberFormat().format(amt));
                }
                if (fdType == 'TextTime') {
                    td.css('textAlign', 'center');
                    var str = GetNodeValue(xml, fdName);
                    var hms = str.split(':');
                    if (hms.length == 3) {
                        str = parseInt(hms[0]) + "時";
                        if (str == "0時")
                            str = "";
                        str += parseInt(hms[1]) + "分";
                        if (str == "0分")
                            str = "";
                        str += parseInt(hms[2]) + "秒";
                        td.text(str);
                    }
                    else if (!isNaN(parseFloat(str))) {
                        var sec = parseFloat(str);
                        str = SecToHMS(sec);
                        //var s = sec % 60;
                        //var min = (sec - s) / 60;
                        //var m = min % 60;
                        //var h = (min - m) / 60;
                        //str = h + "時";
                        //if (str == "0時")
                        //    str = "";
                        //str += m + "分";
                        //if (str == "0分")
                        //    str = "";
                        //str += s + "秒";
                        td.text(str);
                    }
                }
                if (fdStyle != null) {
                    td.attr('style', fdStyle);
                }
                tr.prop(fdName, td);
            }
            else if (fdType == 'TextInput') {
                var ip = $('<input type="text" />');
                ip.prop('FieldName', fdName);
                if (fdStyle != null) {
                    ip.attr('style', fdStyle);
                }
                
                var ipclass = fdinfo.classname;
                if (ipclass != null) {
                    var cls = ipclass.split(' ');
                    for (var c = 0; c < cls.length; c++)
                        ip.addClass(cls[c]);
                }
                ip.val(GetNodeValue(xml, fdName));
                tr.prop(fdName, ip);
                td.append(ip);
            }
            else if (fdType == 'checkbox') {
                var lb = $("<label></label>");
                td.append(lb);
                var jqelm = $("<input type='checkbox' />");
                jqelm.prop('Record', xml);
                jqelm.addClass('checkbox');
                jqelm.addClass('rounded');
                tr.prop(fdName, jqelm);
                if (GetNodeValue(xml, fdName) == "Y")
                    jqelm.prop('checked', true);
                else
                    jqelm.prop('checked', false);
                if (fdStyle != null) {
                    jqelm.attr('style', fdStyle);
                }
                lb.append(jqelm);
                lb.append('<span></span>');
            }
            else if (fdType == 'JQ') {
                if (fdinfo.element != "") {
                    var jqelm = $(fdinfo.element);
                    var ipclass = fdinfo.classname;
                    if (ipclass != null) {
                        var cls = ipclass.split(' ');
                        for (var c = 0; c < cls.length; c++)
                            jqelm.addClass(cls[c]);
                    }
                    jqelm.prop('Record', xml);
                    tr.prop(fdName, jqelm);
                    td.append(jqelm);
                }
            }

            var th = ths[j];
            if ($(th).is(":hidden"))
                td.hide();
        }
        tr.click(function () { clicktr(this); });
    }

    var thisBindData = function (data) {
        var xmls = $(data).clone();
        thisXmls = xmls;
        gbody.empty();
        activeTR = null;
        var pgscount = 0;
        if (btExpXls != null & xmls.length > 0) {
            btExpXls.hide();
            btExpXls.show();
        }
        if (rows_per_page != null) {
            pagination.empty();
            if (xmls.length > rows_per_page) {
                var pgscount = Math.ceil((xmls.length) / rows_per_page);
                if (pgscount > 1) {
                    for (var i = 0; i < pgscount; i++) {
                        var li = $('<li class="page-item"></li>');
                        var aa = $('<a class="page-link" href="#"></a>');
                        var t = i + 1;
                        aa.text(t);
                        aa.prop('start', i * rows_per_page);
                        aa.prop('end', i * rows_per_page + rows_per_page);
                        li.append(aa);
                        pagination.append(li);
                        aa.click(function () { ShowPageData(this); });
                    }
                }
            }
            pagination.prop('rows_per_page_spTotal').text("共:" + xmls.length + "筆" );
        }
        if (pgscount > 1) {
            if (fixPageA == null)
                $(pagination.find('.page-link')[0]).click();
            else {
                var p = parseInt(fixPageA) - 1;
                $(pagination.find('.page-link')[p]).click();
            }
            return;
        }

        for (var i = 0; i < xmls.length; i++) {
            var tr = $('<tr></tr>');
            var xml = xmls[i];
            MappingRecord(tr, xml);
            gbody.append(tr);
        }

        if (option.afterBind != null)
            option.afterBind();
    };

    this.BindData = function (data) {
        ths.find('em').remove();
        thisBindData(data);
    };

    var ShowPageData = function (a) {
        gbody.empty();
        pagination.find('.page-item').removeClass('active');
        $(a).closest('.page-item').addClass('active');
        if (fixPage)
            fixPageA = $(a).text();
        var xmls = thisXmls;
        var start = $(a).prop('start');
        var end = $(a).prop('end');
        if (end > xmls.length)
            end = xmls.length;
        for (var i = start; i < end; i++) {
            var tr = $('<tr></tr>');
            var xml = xmls[i];
            MappingRecord(tr, xml);
            gbody.append(tr);
        }
        if (option.afterBind != null)
            option.afterBind();

    };

    this.RefreshRocord = function (trdom,xml)
    {
        var xmlOld = $(trdom).prop('Record');
        var i = $.inArray(xmlOld, thisXmls);
        if (i > -1) {
            //console.log("inArray:" + i);
            thisXmls[i] = xml;
        }
        $(trdom).empty();
        MappingRecord($(trdom), xml);
        if (option.afterBind != null)
            option.afterBind();
    }

    this.AddNew = function (xml , prepend) {
        var tr = $('<tr></tr>');
        MappingRecord(tr, xml);
        if (prepend)
            gbody.prepend(tr);
        else
            gbody.append(tr);

        thisXmls.push(xml);
        tr[0].scrollIntoView();
        tr.click();
        if (option.afterBind != null)
            option.afterBind();
        if (pagination != null)
            pagination.prop('rows_per_page_spTotal').text("共:" + thisXmls.length + "筆");
    }

    this.DeleteRow = function (tr_del) {
        if (tr_del == null)
            return null;

        var xmlOld = $(tr_del).prop('Record');
        var indexXmlOld = $.inArray(xmlOld, thisXmls);
        if (indexXmlOld > -1)
            thisXmls.splice(indexXmlOld, 1);

        var i = tr_del.rowIndex - 1; //thead has tr
        $(tr_del).remove();
        var trs = gbody.children('tr');

        if (pagination != null)
            pagination.prop('rows_per_page_spTotal').text("共:" + thisXmls.length + "筆");

        if (trs.length == 0)
            return null;
        else if (trs[i] != null) {
            $(trs[i]).click();
            return trs[i];
        }
        else {
            $(trs[trs.length - 1]).click();
            return trs[trs.length - 1];
        }
    }

    this.disableSelect = function (isdisable) {
        disable_select = isdisable;
    }

    this.ActiveRowTR = function () {
        return activeTR;
    };

}

var DyAlert = function (txtMessage, HandlerOK, txtMessage2, txtMessage3) {
    var divN = $('<div></div>');

    $("body").append(divN);
    
    var pg = JSPath('JSUtility.js') + "../DyAlert.html";

    var LoadPage = function () {
        if ($('#DyAlert_modal').length == 0) {
            divN.load(pg + " #DyAlert_modal", function () { LoadDone(); });
        }
        else {
            $('#DyAlert_modal button').click()
        }
    };

    var LoadDone = function () {
        var ParentNode = $('body');
        var p = $('#DyAlert_modal').detach();
        ParentNode.append(p);
        
        $('#DyAlert_modal').find('p[name="pMsg"]').text(txtMessage);
        $('#DyAlert_modal').find('p[name="pMsg2"]').text(txtMessage2);
        $('#DyAlert_modal').find('p[name="pMsg3"]').text(txtMessage3);

        $('button[name="btYes"], button[name="btNo"]').hide();
        $('button[name="btOk"]').text("確定");
        setTimeout(function () {
            $('#DyAlert_modal').modal('show');
        }, 200);
        $('#DyAlert_modal button').click(function () {
            $('#DyAlert_modal').modal('hide');
            setTimeout(function () {
                $('#DyAlert_modal').remove();
                if (HandlerOK != null)
                    HandlerOK();
            }, 500);
        });
    };
    LoadPage();
}

var DyConfirm = function (txtMessage, HandlerYes, HandlerNo) {
    var divN = $('<div></div>');

    $("body").append(divN);

    var pg = JSPath('JSUtility.js') + "../DyAlert.html";

    var LoadPage = function () {
        divN.load(pg + " #DyAlert_modal", function () { LoadDone(); });
    };

    var LoadDone = function () {
        var ParentNode = $('.right_col');
        var p = $('#DyAlert_modal').detach();
        ParentNode.append(p);
        $('#DyAlert_modal').find('p[name="pMsg"]').text(txtMessage);
        $('button[name="btOk"]').hide();
        $('#DyAlert_modal .modal-header').css('background-color', '#f19149');
        $('button[name="btYes"]').css('background-color', '#f19149');

        $('button[name="btYes"]').text('是');
        $('button[name="btNo"]').text('否');
        
        $('#DyAlert_modal').modal('show');
        $('#DyAlert_modal button').click(function () {
            $('#DyAlert_modal').modal('hide');
            var bt = this;
            setTimeout(function () {
                $('#DyAlert_modal').remove();
                if (bt.name == "btYes" & HandlerYes != null)
                    HandlerYes();
                else if (bt.name == "btNo" & HandlerNo != null)
                    HandlerNo();
            }, 500);
        });
    };
    LoadPage();
}

var ShowLoading = function () {
    $('#modal_Loading').modal('show');
}

var CloseLoading = function () {
    setTimeout(function () {
        try {
            $('#modal_Loading').modal('hide');
        }
        catch (ERR) { }
    }, 500);
    
}

var DisplayLoadingMsg = function (msg) {
    setTimeout(
        function () {
            $('#modal_Loading .l-text').text(msg);
        }, 50);
}

var DownloadSOReportByID = function (id) {
    var url = "FileDownload?ID=" + id;
    $('#iframe_for_download').prop('src', url);
}


var InitSelectItem = function (elmSelect, xml, valField, txtField, isFirstBlank, placeholder) {
    if (elmSelect == null)
        return;
    $(elmSelect).empty();
    if (placeholder != null) {
        if (placeholder.length > 0) {
            //$(elmSelect).append("<option disabled selected value=''>" + placeholder + "</option>");
            $(elmSelect).append("<option selected value=''>" + placeholder + "</option>");
            isFirstBlank = false;
        }
    }
    if (isFirstBlank) {
        $(elmSelect).append("<option value=''></option>");
    }
    for (var i = 0; i < xml.length; i++) {
        var desc = GetNodeValue(xml[i], txtField);
        var strVal = GetNodeValue(xml[i], valField);
        if (isFirstBlank & strVal == "")
            continue;
        $(elmSelect).append("<option value='" + strVal + "'>" + desc+"</option>");
    }
}

function EncodeSGID(sgid) {
    var ff = "";
    for (var j = 0; j < sgid.length; j++) {
        var c = sgid.charCodeAt(j);
        ff += c2hex(c);
    }
    return ff.toUpperCase();
}

function c2hex(c) {
    var ss = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '.'];
    var res = "";
    while (c > 0) {
        res = ss[c % 16] + res;
        c >>= 4;
    }
    res = res.padLeft(2, '0');
    return res;
}
var SetHourSelect = function (sel) {
    $(sel).empty();
    $(sel).append("<option value=''></option>");
    for (var h = 0; h < 24; h++) {
        var hh = (100 + h + "").substr(1, 2);
        $(sel).append("<option value='" + hh + "'>" + hh + ":00" + "</option>");
    }
}

var JSPath = function (jsfile) {
    var scriptElements = document.getElementsByTagName('script');
    var i, element, myfile;
    for (i = 0; element = scriptElements[i]; i++) {
        myfile = element.src;
        if (myfile.indexOf(jsfile) >= 0) {
            var myurl = myfile.substring(0, myfile.indexOf(jsfile));
        }
    }
    return myurl;
}

var SetDateField = function (ip) {
    $(ip).datepicker({
        dateFormat: "yy/mm/dd",
        monthNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
        dayNamesMin: ["日","一", "二", "三", "四", "五", "六"]
    });
    $(ip).change(function () { CheckDate(this); });
    $(ip).prop('isDateField', true);

    var CheckDate = function (ip) {
        var ipval = $(ip).val();
        if (ipval.length == 8 & ipval.indexOf("/") < 0)
            ipval = ipval.substr(0, 4) + "/" + ipval.substr(4, 2) + "/" + ipval.substr(6, 2)
        try {
            var dd = $.datepicker.parseDate('yy/mm/dd', ipval)
            var xx = $.datepicker.formatDate('yy/mm/dd', dd);
            $(ip).val(xx);
        }
        catch (dError) {
            $(ip).val('');
        }
    }
}


var createHiddenInputElement = function(id) {
    var hidInput;
    hidInput = document.createElement('input');
    $(hidInput).attr('type', 'hieedn');
    $(hidInput).attr('id', id);
    $(hidInput).css('display', 'none');
    if ($('#' + id).length == 0)
        document.body.appendChild(hidInput);
    return hidInput;
}

var CalculateTime = function (sTime , eTime) {
    var ds = new Date(sTime);
    var de = new Date(eTime);
    var sec = de.getTime() - ds.getTime();
    sec = Math.round(sec / 1000);
    var TTime = {
        Second: sec,
        HMD: SecToHMS(sec)
    }
    return TTime;
}

var SecToHMS = function (sec) {
    var s = sec % 60;
    if (s < 10) { s = "0" + s }
    var min = (sec - s) / 60;
    //var m = min % 60;
    if (min < 10) { min = "0" + min }

    //var h = (min - m) / 60;

    //str = h + "時";
    //if (str == "0時")
    //    str = "";
    //str += m + "分";
    //if (str == "0分")
    //    str = "";
    //str += s + "秒";

    //str = h + ":";
    //if (str == "0:")
    //    str = "";
    str = min + ":";
    if (str == "00:")
        str = "";
    str += s;


    return str;
}


var xmlFilter = function (xml, field, value) {
    if (xml.length == 0)
        return [];
    else {
        var tbname = $(xml[0]).prop("tagName");
        return $(xml[0]).parent().find(tbname).find(field).filter(function () {
            return $(this).text() === value;
        }).parent();
    }
};



var waitingDialog = waitingDialog || (function ($) {
    'use strict';

    // Creating modal dialog's DOM
    var $dialog = $(
        '<div class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true" style="padding-top:15%; overflow-y:visible;">' +
        '<div class="modal-dialog modal-m">' +
        '<div class="modal-content">' +
        '<div class="modal-header"><h3 style="margin:0;"></h3></div>' +
        '<div class="modal-body">' +
        '<div class="progress progress-striped active" style="margin-bottom:0;"><div class="progress-bar" style="width: 100%"></div></div>' +
        '</div>' +
        '</div></div></div>');

    return {
        /**
         * Opens our dialog
         * @param message Custom message
         * @param options Custom options:
         * 				  options.dialogSize - bootstrap postfix for dialog size, e.g. "sm", "m";
         * 				  options.progressType - bootstrap postfix for progress bar type, e.g. "success", "warning".
         */
        show: function (message, options) {
            // Assigning defaults
            if (typeof options === 'undefined') {
                options = {};
            }
            if (typeof message === 'undefined') {
                message = 'Loading';
            }
            var settings = $.extend({
                dialogSize: 'm',
                progressType: '',
                onHide: null // This callback runs after the dialog was hidden
            }, options);

            // Configuring dialog
            $dialog.find('.modal-dialog').attr('class', 'modal-dialog').addClass('modal-' + settings.dialogSize);
            $dialog.find('.progress-bar').attr('class', 'progress-bar');
            if (settings.progressType) {
                $dialog.find('.progress-bar').addClass('progress-bar-' + settings.progressType);
            }
            $dialog.find('h3').text(message);
            // Adding callbacks
            if (typeof settings.onHide === 'function') {
                $dialog.off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
                    settings.onHide.call($dialog);
                });
            }
            // Opening dialog
            $dialog.modal();
        },
        /**
         * Closes dialog
         */
        hide: function () {
            $dialog.modal('hide');
        }
    };

})(jQuery);

var SetCommSelectGrid = function (option) {

    let icon = $('<span class="fa fa-list-alt form-control-feedback right" aria-hidden="true"></span>');
    icon.css('cursor', 'pointer');

    let InputElement = $('#' + option.InputElementsID);
    if (!InputElement.hasClass('InputForCommSelectGrid'))
        InputElement.addClass('InputForCommSelectGrid');
    InputElement.after(icon);

    icon.click(
        function () {
            var grdP;
            var divN = $('<div></div>');
            divN.hide();
            $("body").append(divN);
            divN.load("CommSelectGridModal.html #CommSelectGridModal", null, function () { afterLoadCommSelectGridModal(this); });

            let afterLoadCommSelectGridModal = function (TmpDiv) {
                var p = $('#CommSelectGridModal').detach();
                $("body").append(p);
                let tbGrd = $('#CommSelectGridModal .modal-body table')[0];
                grdP = new DynGrid(
                    {
                        table_lement: tbGrd,
                        class_collection: ["tdchk", "tdAuto", "tdAuto"],
                        fields_info: [
                            { type: "checkbox", name: "Blank" },
                            { type: "Text", name: option.PostDataForApi.Column[0] },
                            { type: "Text", name: option.PostDataForApi.Column[1] }
                        ]
                    }
                );
                $(tbGrd).find('tbody').empty();
                $(TmpDiv).remove();
                $('#CommSelectGridModal .SearchCond').val(option.SearchCond);
                $("#CommSelectGridModal .btCancel").click(function () {
                    $('#CommSelectGridModal').modal("hide");
                    setTimeout(function () {
                        $('#CommSelectGridModal').remove();
                    }, 200);
                });

                $('#CommSelectGridModal .btConfirm').click(function () {
                    let chked = $('#CommSelectGridModal .modal-body table tbody input:checked');
                    if (chked.length == 0) {
                        DyAlert("沒有選擇項目", DummyFunction);
                    }
                    else {
                        let xml = $(chked[0]).closest('tr').prop('Record');
                        option.AfterSelectData(xml);
                        $('#CommSelectGridModal').modal("hide");
                        setTimeout(function () {
                            $('#CommSelectGridModal').remove();
                        }, 200)
                    }
                });

                $($('#CommSelectGridModal .modal-body table thead th')[1]).text(option.PostDataForApi.Caption[0]);
                $($('#CommSelectGridModal .modal-body table thead th')[2]).text(option.PostDataForApi.Caption[1]);
                $('#CommSelectGridModal .btConfirm').prop('disabled', true);

                let AfterGetDataForCSG = function (data) {
                    setTimeout(function () {
                        let dtCommSelResult = data.getElementsByTagName('dtCommSelResult');
                        grdP.BindData(dtCommSelResult);
                        $('#CommSelectGridModal .modal-body table tbody input:checkbox').click(
                            function () { ControlCheck(this); }
                        );
                    }, 200);
                }

                let ControlCheck = function (chk) {
                    $('#CommSelectGridModal .btConfirm').prop('disabled', true);

                    let allChks = $('#CommSelectGridModal .modal-body table tbody input:checkbox');
                    if ($(chk).prop('checked')) {
                        $('#CommSelectGridModal .btConfirm').prop('disabled', false);
                        allChks.prop('checked', false);
                        $(chk).prop('checked', true);
                    }
                    else {
                        $('#CommSelectGridModal .btConfirm').prop('disabled', true);
                    }
                }

                $('#CommSelectGridModal .SearchCond').val(InputElement.val());
                $('#CommSelectGridModal').modal("show");
                PostToWebApi({ url: option.ApiForGridData, data: option.PostDataForApi, success: AfterGetDataForCSG });


            }

        }
    );


}


var DateDiff = function (interval, objDate1, objDate2) {
    var dtBeg = new Date(objDate1);
    var dtEnd = new Date(objDate2);
    if (isNaN(dtBeg)) return undefined;
    if (isNaN(dtEnd)) return undefined;
    switch (interval) {
        case "s": return parseInt((dtEnd - dtBeg) / 1000);      //秒
        case "n": return parseInt((dtEnd - dtBeg) / 60000);     //分
        case "h": return parseInt((dtEnd - dtBeg) / 3600000);   //時
        case "d": return parseInt((dtEnd - dtBeg) / 86400000);  //日
        case "w": return parseInt((dtEnd - dtBeg) / (86400000 * 7));    //週
        case "m": return (dtEnd.getMonth() + 1) + ((dtEnd.getFullYear() - this.getFullYear()) * 12) - (this.getMonth() + 1);    //月
        case "y": return dtEnd.getFullYear() - this.getFullYear();  //年
    }
}

var SetTreeField = function (id, xmlParent, xmlChild) {
    $(id).empty();
    let root = $('<ul></ul>');
    $(id).append(root);
    for (var i = 0; i < xmlParent.length; i++) {
        let nodeA1;
        let branch1;
        var jid = GetNodeValue(xmlParent[i], "id");
        var jtxt = GetNodeValue(xmlParent[i], "text");
        //var jparent = GetNodeValue(xmlParent[i], "parent");
        nodeA1 = $('<li id="' + jid + '">' + jtxt + '</li>');
        root.append(nodeA1);
        if (xmlChild != null) {
            var li_Cnt = 0;
            for (var j = 0; j < xmlChild.length; j++) {
                let nodeB1;
                var jid2 = GetNodeValue(xmlChild[j], "id");
                var jtxt2 = GetNodeValue(xmlChild[j], "text");
                var jparent2 = GetNodeValue(xmlChild[j], "parent");
                if (jparent2 == jid) {
                    if (li_Cnt == 0) {
                        branch1 = $('<ul></ul>');
                        nodeA1.append(branch1);
                    }
                    nodeB1 = $('<li id="' + jid2 + '">' + jtxt2 + '</li>');
                    branch1.append(nodeB1);
                    li_Cnt++;
                }
            }
            li_Cnt = 0;
        }

    }

}

var InitCheckBoxItem = function (elmCheck, xml, valField, txtField, propName) {
    if (elmCheck == null)
        return;
    $(elmCheck).empty();
    if (xml == null)
        return;
    if (xml.length == 0)
        return;
    let ls_Name = "";
    for (var i = 0; i < xml.length; i++) {
        var desc = GetNodeValue(xml[i], txtField);
        var strVal = GetNodeValue(xml[i], valField);
        if (propName != null & propName != "") { ls_Name = "name='" + propName + "'"; };
        $(elmCheck).append("<samp class='col-md-3'><input type='checkbox' id='chk" + i.toString() + "' " + ls_Name + " value='" + strVal + "'><label for='chk" + i.toString() + "'>" + desc + "</label></samp>");
    }
}

var LogOutTimer;
var searchComp_1;
var Timerset = function (searchComp) {
    var LoginDT = sessionStorage.getItem('LoginDT');
    searchComp_1 = searchComp
    //if (LogOutTimer != null) {
    //    if (LogOutTimer.length > 1) {
            for (i = 0;i<=10000 ; i++) {
                clearInterval(i);
            }
    //    }
    //    //clearInterval(LogOutTimer);
    //}
    var pg = JSPath('JSUtility.js') + "../Menu.html";
    var divN = $('<div></div>');
    divN.load(pg + " #Timer", function () {
        var timer = $('#Timer');
        var ShowNumber = 600;
        var number = 600;
        timer.text(SecToHMS(number));
        
        LogOutTimer = setInterval(function () {
            number--;
            if (number == 0) {
                
                number = 0;

                DyAlert("您已超過" + ShowNumber / 60 + "分鐘未執行作業", function () {
                    var cData = {
                        LoginDT: LoginDT
                    }
                    PostToWebApi({ url: "api/js/UpdateLogOutT", data: cData, success: AfterUpdateLogOutT });
                },"系統已自動登出","請按[確定]，回到登入畫面");

            }
            if (number >= 0) {
                //timer.text(number + 0);
                timer.text(SecToHMS(number));
            }
            

        }, 1000);
    });        
    
}

let AfterUpdateLogOutT = function (data) {

    if (ReturnMsg(data, 0) != "UpdateLogOutTOK") {
        DyAlert(ReturnMsg(data, 1));
    } else {
        window.location.href = "Login" + searchComp_1;
    }
}


var searchComp_LogOut;
//檢查登出狀態
var ChkLogOut = function (searchComp) {
    searchComp_LogOut = searchComp
    var LoginDT = sessionStorage.getItem('LoginDT');
    var cData = {
        LoginDT: LoginDT
    }
    PostToWebApi({ url: "api/js/ChkLogOut", data: cData, success: AfterChkLogOut });
}

var AfterChkLogOut = function (data) {
    if (ReturnMsg(data, 0) != "ChkLogOutOK") {
        DyAlert(ReturnMsg(data, 1));
    }
    else {
        var dtLogin = data.getElementsByTagName('dtLogin');
        if (GetNodeValue(dtLogin[0], "LogOutType") != "") {
            window.location.href = "Login" + searchComp_LogOut;
        }
    }

}




