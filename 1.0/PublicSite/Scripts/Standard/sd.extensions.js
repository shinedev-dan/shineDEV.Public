(function ($) {
    $.fn.windowCenter = function () {
        this.css("position", "absolute");
        this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
                                                $(window).scrollTop()) + "px");
        this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
                                                $(window).scrollLeft()) + "px");
        return this;
    }

    $.fn.windowCenterTop = function (offset) {
        this.css("position", "absolute");
        this.css("top", Math.max($(window).scrollTop() + offset) + "px");
        this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
                                                $(window).scrollLeft()) + "px");
        return this;
    }

    $.fn.documentTopLeft = function () {
        this.css("position", "absolute");
        this.css("top", "0px");
        this.css("left", "0px");
        return this;
    }
    $.fn.windowTopLeft = function () {
        this.css("position", "absolute");
        this.css("top", $(window).scrollTop() + "px");
        this.css("left", $(window).scrollLeft() + "px");
        return this;
    }

    $.fn.documentHeight = function (val) {
        var height = $(document).height();
        if (val !== undefined)
            height = height * val;
        sd.common.log("height: " + height);

        this.height(height);
        return this;
    }
    $.fn.documentWidth = function (val) {
        var width = $(document).width();
        if (val !== undefined)
            width = width * val;
        sd.common.log("width: " + width);

        this.width(width);
        return this;
    }
    $.fn.windowHeight = function (val) {
        var height = $(window).height();
        if (val !== undefined)
            height = height * val;
        sd.common.log("inner height: " + height);

        this.height(height);
        return this;
    }
    $.fn.windowWidth = function (val) {
        var width = $(window).width();
        if (val !== undefined)
            width = width * val;
        sd.common.log("inner width: " + width);

        this.width(width);
        return this;
    }
})(jQuery);