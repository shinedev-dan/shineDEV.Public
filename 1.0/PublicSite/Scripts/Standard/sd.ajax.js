sd.ajax = (function () {
	var pub = {};

	pub.ajaxOverlay = null;
	pub.ajaxImg = null;
	pub.ajaxContainer = null;

	pub.VAL_ISAJAXACTIVE = false;
	pub.VAL_ISPOSTBACK = false;
	pub.KEY_FILEINPUTIDENTIFIER = "fileinputidentifier";

	pub.showAjaxLoadingMsg = function () {
		sd.ajax.initializeAjaxElements();
		ajaxOverlay.show();
		ajaxContainer.show();
		sd.ajax.resizeAjaxLoadingMsg();
	}

	pub.hideAjaxLoadingMsg = function () {
		ajaxOverlay.hide();
		ajaxContainer.hide();
	}

	pub.clearAjaxLoadingMsg = function () {
		$("body").find(".sd-global-ajax-loading-overlay").remove();
		$("body").find(".sd-site-ajax-loading-container").remove();
		$("body").find(".sd-site-ajax-loading-img").remove();
	}

	pub.resizeAjaxLoadingMsg = function () {
		ajaxContainer.windowCenter();
	}

	pub.createAjaxLoadingMsgText = function (text) {
		ajaxContainer.remove("span");
		var span = $("<span>" + text + "</span>");
		ajaxContainer.append(span);
	}

	pub.initializeAjaxElements = function () {
		pub.clearAjaxLoadingMsg();

		ajaxOverlay = $("<div class='sd-site-ajax-loading-overlay'></div>");
		ajaxImg = $("<i class='fa fa-circle-o-notch fa-pulse fa-4x'></i>");
		ajaxContainer = $("<div class='sd-site-ajax-loading-container'></div>");

		ajaxOverlay.hide();
		$("body").append(ajaxOverlay);

		ajaxContainer.append(ajaxImg).hide();
		$("body").append(ajaxContainer);
	}

	pub.initializeAjax = function () {

		$(document).on("document:ajaxStart", function () {
			if (!sd.ajax.VAL_ISPOSTBACK)
				sd.ajax.VAL_ISPOSTBACK = true;
			sd.ajax.VAL_ISAJAXACTIVE = true;
			sd.ajax.showAjaxLoadingMsg();
			//sd.common.log("Ajax start");
		});

		$(document).on("document:ajaxStop", function () {
			sd.ajax.VAL_ISAJAXACTIVE = false;
			sd.ajax.hideAjaxLoadingMsg();
			//sd.common.log("Ajax stop");
		});

		$(window).on("window:resize", function () {
			if (sd.ajax.VAL_ISAJAXACTIVE)
				sd.ajax.resizeAjaxLoadingMsg();
			//sd.common.log("Window resize");
		});
		$(window).on("window:scroll", function () {
			if (sd.ajax.VAL_ISAJAXACTIVE)
				sd.ajax.resizeAjaxLoadingMsg();
			//sd.common.log("Window scroll");
		});
	}
	
	return pub;
}());