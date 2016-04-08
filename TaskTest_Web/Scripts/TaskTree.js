
var dialogs = {};
    
// 将对应url的视图加载并显示在对话框中
var loadAndShowDialog = function (id, url, title, dialogWidth) {

    dialogs[id] = $();
    // 加载对话框, 获取 PartialView
    $.get(url)
        .done(function (content) {
            dialogs[id] = $('<div class="modal-popup">' + content + '</div>')
                .hide() 
                .appendTo(document.body)
                .filter('div') 
                .dialog({ 
                    title: title,
                    modal: true,
                    resizable: true,
                    draggable: true,
                    width: dialogWidth
                })
                .end();
        });
};

// 依据taskId, 固定的宽度显示任务树
function getTaskTreeDialog(taskId, title, width) {

    // 对话框id
    var dialogId = "TaskTree_" + taskId;
    // 获取视图的url地址
    var url = "/Admin/GetTaskTree?id=" + taskId;
    // 
    if (!dialogs[dialogId]) {
        loadAndShowDialog(dialogId, url, title, width);
    } else {
        dialogs[dialogId].dialog('open');
    }
}

  