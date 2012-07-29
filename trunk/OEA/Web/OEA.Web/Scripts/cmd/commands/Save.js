﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：201201
 * 说明：
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 201201
 * 
*******************************************************/

Ext.define('Oea.cmd.Save', {
    extend: 'Oea.cmd.Command',
    config: {
        meta: { text: "保存", group: "edit" }
    },
    execute: function (listView) {
        listView.save(function (res) {
            if (res.success) {
//                if (listView.getIsTree()) {
//                    setTimeout(function () { listView.expandSelection(); }, 200);
//                }
                //                Ext.Msg.alert('提示', '保存成功！');
            }
            else {
                Ext.Msg.alert('保存失败', res.msg);
            }
        });
    }
});