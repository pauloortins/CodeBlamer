var RepoListView = Class.extend({
    init: function() {
        this._createBinds();
        this._initElements();
    },
    
    _createBinds: function() {
        this._tblProjects = $("#tblProjects");
        this._btnRequest = $("#btnRequest");
        this._frmAddNewRepository = $("#frmAddRepository");
    },
    
    _initElements: function () {
        var that = this;

        that._tblProjects.dataTable();        

        that._frmAddNewRepository.submit(function (e) {
            var postData = $(this).serializeArray();
            var formUrl = $(this).attr("action");
            $.ajax(
            {
                url: formUrl,
                type: "POST",
                data: postData,
                success: function (data, textStatus, jqXHR) {
                    //data: return data from server
                    alert('ok');
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //if fails
                    alert('error');
                }
            });
            e.preventDefault(); //STOP default action
        });

        that._btnRequest.click(function () {
            that._frmAddNewRepository.submit();
        });
    }
});