var highlightMatch = (function () {
    var my = {};
    my.createItemTpl = function (displayField, cmbID) {
        return Ext.create('Ext.XTemplate',
                        '{[this.highlightMatch(values.' + displayField + ')]}', {
                            highlightMatch: function (input) {
                                var searchQuery = Ext.getCmp(cmbID).getValue();
                                var searchQueryRegex = new RegExp("(" + searchQuery + ")", "i"); // case-insensitive
                                var highlightedMatch = '<span class="searchMatch">$1</span>';
                                return input.replace(searchQueryRegex, highlightedMatch);
                            }
                        })
    }

    return my;
}());