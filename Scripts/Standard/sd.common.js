//Declare namespaces
var sd = sd || {};

sd.common = (function () {
    var pub = {};

    //Private Proerties
    var x = null;

    //Public Properties
    pub.test = "Test";

    //Public Methods
    pub.log = function (msg) {
        checkConsole();
        console.info(this.getDateString() + '\n' + msg);
    };

    pub.initialize = function () {
        $(document).ajaxStart(function () {
            $(this).trigger("document:ajaxStart");
        });

        $(document).ajaxStop(function () {
            $(this).trigger("document:ajaxStop");
        });

        $(window).resize(function () {
            $(this).trigger("window:resize");
        });

        $(window).scroll(function () {
            $(this).trigger("window:scroll");
        });

        sd.ajax.initializeAjax();
    }

    pub.getDateString = function (date) {
        if (date === undefined)
            date = new Date();
        var month = padLeadingZero(date.getMonth() + 1);
        var day = padLeadingZero(date.getDate());
        var hours = padLeadingZero(date.getHours());
        var minutes = padLeadingZero(date.getMinutes());
        return date.getFullYear() + '-' + month + '-' + day + ' ' + hours + ':' + minutes;
    };

    //Private Methods
    function checkConsole() {
        if (!window.console) {
            (function () {
                var names = ["log", "debug", "info", "warn", "error",
                    "assert", "dir", "dirxml", "group", "groupEnd", "time",
                    "timeEnd", "count", "trace", "profile", "profileEnd"],
                    i, l = names.length;

                window.console = {};

                for (i = 0; i < l; i++) {
                    window.console[names[i]] = function () { };
                }
            }());
        }
    };

    function canDebug() {
        return true;
    };

    function padLeadingZero(val) {
        if (val == null)
            return '';
        return (val < 10 ? '0' : '') + val;
    };

    return pub;
}());