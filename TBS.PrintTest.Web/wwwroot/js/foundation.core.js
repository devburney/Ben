var FOUNDATION = FOUNDATION ? FOUNDATION : {};
FOUNDATION.Core = FOUNDATION.Core ? (function () { alert("FOUNDATION.Core already exists."); })() : {
    Version: "0.1.0",

    init: function () {


        // Initialize localization.
        FOUNDATION.Core._loc.init();

        // Initialize the error messages' module.
        FOUNDATION.Core.Alert.init();
        // Initialize the Loading popup.
        FOUNDATION.Core.Loading.init();
        // Initialize the forms.
        FOUNDATION.Core.Controls.Form.init();
        // Initialize Modal popups.
        FOUNDATION.Core.Modal.init();

        // Attach events to controls.
        $("form[data-on-load]").each(function (i, o) { FOUNDATION.Core.Controls.Form.onLoad(o); });
        wb.doc.on("change", "input[type=checkbox][data-on-change]", FOUNDATION.Core.Controls.Generic.onChange);
        wb.doc.on("change", "input[type=radio][data-on-change]", FOUNDATION.Core.Controls.Generic.onChange);
        wb.doc.on("change", "select[data-on-change]", FOUNDATION.Core.Controls.Generic.onChange);
        wb.doc.on("click", "button[data-on-click]", FOUNDATION.Core.Controls.Button.onClick);
        wb.doc.on("click", "button.foundation-ignore-form-monitor", FOUNDATION.Core.Controls.Button.ignoreFormMonitor);
        wb.doc.on("click", "a[data-on-click]", FOUNDATION.Core.Controls.Link.onClick);

        // Trigger "FOUNDATION.Core-ready" event.
        wb.doc.trigger("FOUNDATION.Core-ready");
    },
    initWbDisable: function () {
        // Set flag.
        FOUNDATION.Core.WET.IsDisabled = true;

        // Initialize localization.
        FOUNDATION.Core._loc.initWbDisable();

        // Initialize the Alert.
        FOUNDATION.Core.Alert.initWbDisable();
        // Initialize the Loading popup.
        FOUNDATION.Core.Loading.initWbDisable();

        // Attach events to controls.
        $("form[data-on-load]").each(function (i, o) { FOUNDATION.Core.Controls.Form.onLoad(o); });
        wb.doc.on("change", "input[type=checkbox][data-on-change]", FOUNDATION.Core.Controls.Generic.onChange);
        wb.doc.on("change", "input[type=radio][data-on-change]", FOUNDATION.Core.Controls.Generic.onChange);
        wb.doc.on("change", "select[data-on-change]", FOUNDATION.Core.Controls.Generic.onChange);
        wb.doc.on("click", "button[data-on-click]", FOUNDATION.Core.Controls.Button.onClick);
        wb.doc.on("click", "button.foundation-ignore-form-monitor", FOUNDATION.Core.Controls.Button.ignoreFormMonitor);
        wb.doc.on("click", "a[data-on-click]", FOUNDATION.Core.Controls.Link.onClick);

        // Trigger "FOUNDATION.Core-wbdisable" event.
        wb.doc.trigger("FOUNDATION.Core-wbdisable");
    },

    action: function (cntrl, type, data, url, httpMethod, trgt, dfd) {
        // Call a controller action to perform a task (e.g. replace the Html of an element, generate Html code from a JSON response).
        // - "json": replace the content of a target using json to build the new content
        // - "file": upload a file asynchronously
        // - "default"/"submit": force submit the form
        // - "html": replace the content of a target using html
        // - "htmlappendto": append to the content of a target using html

        if (type === "json") {

            FOUNDATION.Core.Ajax.execute(url, data, "json", httpMethod, trgt, type, dfd);

        } else if (type === "file") {

            data = new FormData($(".foundation-upload-form")[0]);
            FOUNDATION.Core.Ajax.execute(url, data, "html", httpMethod, trgt, type, dfd);

        } else if (type === "default" || type === "submit") {

            var frm = cntrl.closest("form"),
                tmp = {};
            if (!frm.length) { console.log("err: FOUNDATION.Core.action : Action failed. [No form]"); }
            if (type === "submit") { tmp = { "action": $(frm).attr("action"), "method": $(frm).attr("method") }; }
            $(frm).attr("method", httpMethod).attr("action", url);
            if (type === "submit") { $(frm).submit(); $(frm).attr("method", tmp.method).attr("action", tmp.action); }

            // Ignore the deferred objects.
            dfd.resolve("keep-locked");

        } else { // type: "html", "htmlappendto"

            FOUNDATION.Core.Ajax.execute(url, data, "html", httpMethod, trgt, type, dfd);

        }
    },
    Ajax: {
        ajaxDone: null, // function (response, status, xhr, target, url, type)
        ajaxError: null, // function (xhr, status, err, url, target, type); returns if error was handled true/false
        ensureRequestVerificationToken: function (data) {
            // Add the verification token to the submitted data (if it isn't included already).
            if (data !== null) {
                if (!data.hasOwnProperty("__RequestVerificationToken")) {
                    var token = $("input[name=__RequestVerificationToken]");
                    $.extend(data, JSON.parse("{\"{0}\": \"{1}\"}".format(token.attr("name"), token.val())));
                }
            }
        },
        execute: function (url, data, dataType, httpMethod, trgt, type, dfd) {
            // Add the verification token to the submitted data (if it isn't included already).
            FOUNDATION.Core.Ajax.ensureRequestVerificationToken(data);

            // Load the settings for $.ajax().
            var settings = {
                async: true,
                data: data,
                dataType: dataType,
                type: httpMethod,
                url: url
            };

            // For asynchronous file upload: tell jQuery not to process data or worry about content-type (ref.: https://stackoverflow.com/questions/166221/how-can-i-upload-files-asynchronously).
            if (type === "file")
                $.extend(settings, { cache: false, contentType: false, dataType: null, processData: false });

            // NOTE: GET will not work for security reasons. On the controller MVC puts JsonRequestBehavior=DenyGet by default.
            $.ajax(settings)
                .done(function (response, status, xhr) {
                    FOUNDATION.Core.Ajax.handleSuccess(response, status, xhr, trgt, url, type);
                    if (dfd) dfd.resolve();
                })
                .fail(function (xhr, status, err) {
                    FOUNDATION.Core.Ajax.handleError(xhr, status, err, url, trgt, type);
                    if (dfd) dfd.resolve("error");
                });
        },
        handleSuccess: function (response, status, xhr, trgt, url, type) {
            // If response has a RedirectUrl, redirect.
            if (!FOUNDATION.Core.Ajax.redirectRequest(response)) {
                // Otherwise, process response.
                switch (type) {
                    case "html":
                        switch (xhr.status) {
                            case 204: //No Content.
                                $(trgt).empty();
                                break;
                            default:
                                $(trgt).html(response);
                                FOUNDATION.Core.WET.init(trgt);
                        }
                        break;
                    case "htmlappendto":
                        switch (xhr.status) {
                            case 204: //No Content.
                                break;
                            default:
                                $(trgt).append(response);
                                FOUNDATION.Core.WET.init(trgt);
                        }
                        break;
                    case "file":
                    default:
                        if (FOUNDATION.Core.Ajax.ajaxDone)
                            FOUNDATION.Core.Ajax.ajaxDone(response, status, xhr, trgt, url, type);
                }
            }
        },
        handleError: function (xhr, status, err, url, trgt, type) {
            var errorHandled = false;
            if (FOUNDATION.Core.Ajax.ajaxError !== null)
                errorHandled = FOUNDATION.Core.Ajax.ajaxError(xhr, status, err, url, trgt, type);
            if (!errorHandled) {
                console.log("Error " & xhr.status & ": FOUNDATION.Core.Ajax.handleError");
                switch (xhr.status) {
                    case 404: //Not Found.
                        if (err) FOUNDATION.Core.Alert.warning(err, FOUNDATION.Core._loc.get("AlertWarningHeading"), trgt);
                        break;
                    case 400: //Bad Request.
                    case 500: //Internal Server Error.
                    case 503: //Service Unavailable - Throttling.
                    default:
                        FOUNDATION.Core.Alert.error(FOUNDATION.Core._loc.get("ActionUnexpectedError"), FOUNDATION.Core._loc.get("AlertErrorHeading"), trgt);
                }
            }
        },
        redirectRequest: function (response) {
            var result = false;
            if (response && response.RedirectUrl && response.RedirectUrl.length > 0) {
                result = true;
                window.location.href = response.RedirectUrl;
            }
            return result;
        }
    },
    Alert: {
        _Placeholder: null,
        _HeadingTemplate: '<h3><button title="{0}" class="foundation-alert-dismiss close" tabindex="-1" type="button">&times;<span class="wb-inv"> {0}</span></button>{1}</h3>',
        _Template: '<section class="alert alert-{0} fade in" role="alert">{2}{1}</div>',
        _display: function (clss, message, heading, target) {
            var alert = FOUNDATION.Core.Alert._Template.format(clss, message, heading ? FOUNDATION.Core.Alert._HeadingTemplate.format(heading) : "");
            if (target) {
                $(target).html(alert); $(target).focus();
            } else {
                FOUNDATION.Core.Alert._Placeholder.html(alert);
                FOUNDATION.Core.Alert._Placeholder.focus();
            }
        },

        init: function () {
            // Select the default location for error messages.
            if (!$("#foundation-alert").length && $("main").length) $("main").prepend($("<div id=\"foundation-alert\"></div>"));
            FOUNDATION.Core.Alert._Placeholder = $("#foundation-alert");
            // Localize heading template.
            FOUNDATION.Core.Alert._HeadingTemplate = FOUNDATION.Core.Alert._HeadingTemplate.format(FOUNDATION.Core._loc.get("Dismiss"), "{0}");
            // Bind dismiss events on .foundation-alert-dismiss controls.
            FOUNDATION.Core.Alert.bind();

            // Bind an event handler on the window's onerror event.
            window.onerror = FOUNDATION.Core.Alert.handleException;
        },
        initWbDisable: function () {
            // Select the default location for error messages.
            if (!$("#foundation-alert").length && $("main").length) $("main").prepend($("<div id=\"foundation-alert\"></div>"));
            FOUNDATION.Core.Alert._Placeholder = $("#foundation-alert");
            // Localize heading template.
            FOUNDATION.Core.Alert._HeadingTemplate = FOUNDATION.Core.Alert._HeadingTemplate.format(FOUNDATION.Core._loc.get("Dismiss"), "{0}");
            // Bind dismiss events on .foundation-alert-dismiss controls.
            FOUNDATION.Core.Alert.bind();
        },

        bind: function (trgt) { trgt = trgt || wb.doc; trgt.on("click", "button.foundation-alert-dismiss", FOUNDATION.Core.Alert.dismiss); },
        dismiss: function (e) { $(this).closest(".alert").remove(); },
        error: function (message, heading, target) { FOUNDATION.Core.Alert._display("danger", message, heading, target); },
        handleException: function (msg, src, lineNo, columnNo, error) {
            var data = {
                    message: msg,
                    source: src,
                    line: lineNo,
                    column: columnNo,
                    error: JSON.stringify(error),
                    pageUrl: wb.pageUrlParts.pathname,
                    pageQuerystring: wb.pageUrlParts.search
                },
                message = null,
                querystring = null,
                spacer = "<br />",
                string = msg.toLowerCase(),
                substring = "script error",
                url = null;

            if (string.indexOf(substring) > -1) {
                message = FOUNDATION.Core._loc.get("UnexpectedJavaScriptErrorTemplate").format(CONFIG.SHARED_MAILBOX, FOUNDATION.Core._loc.get("ErrorMessageScript"));
            } else {
                message = FOUNDATION.Core._loc.get("UnexpectedJavaScriptErrorTemplate").format(CONFIG.SHARED_MAILBOX, '{1} "{2}"{0}{3} {4} [Ln: {5}, Col: {6}]'.format(spacer, FOUNDATION.Core._loc.get("ErrorMessageColon"), msg, FOUNDATION.Core._loc.get("ErrorUrlColon"), src, lineNo, columnNo));
            }

            // Display an error message.
            FOUNDATION.Core.Alert.error(message, FOUNDATION.Core._loc.get("AlertErrorHeading"));

            // Log the error message.
            FOUNDATION.Core.Ajax.ensureRequestVerificationToken(data);
            $.ajax({
                async: true,
                data: data,
                type: "POST",
                url: "{0}/{1}/error/log".format(CONFIG.ROOT, FOUNDATION.Core._loc.get("TwoLetterLanguage"))
            })
                .done(function (response, status, xhr) {
                    console.log("FOUNDATION.Core: Exception logged.");
                })
                .fail(function (xhr, status, err) {
                    console.log("FOUNDATION.Core: Exception caught but NOT logged!");
                });

            // Hide the loading screen.
            FOUNDATION.Core.Loading.hide();

            return true;
        },
        info: function (message, heading, target) { FOUNDATION.Core.Alert._display("info", message, heading, target); },
        success: function (message, heading, target) { FOUNDATION.Core.Alert._display("success", message, heading, target); },
        warning: function (message, heading, target) { FOUNDATION.Core.Alert._display("warning", message, heading, target); }
    },
    Controls: {
        AllCheckbox: {
            init: function (trgt) {
                // Req: "All" option checkbox has the ".foundation-checkbox-all" class
                //      "All" option checkbox's "name" matches the "name" attribute of items + "-all"
                //      Item checkboxes have the ".foundation-checkbox-item" class
                // i.e.
                // <input name="checkbox-name-all" class="foundation-checkbox-all" id="checkbox-id-all" type="checkbox" checked="checked">
                // <input name="checkbox-name" class="foundation-checkbox-item" id="checkbox-id-1" type="checkbox" checked="checked" value="1">
                // <input name="checkbox-name" class="foundation-checkbox-item" id="checkbox-id-2" type="checkbox" checked="checked" value="2">
                trgt = trgt || wb.doc;

                // When changing the "All" option; update all the items of the group.
                $(".foundation-checkbox-all", trgt).each(function () {
                    FOUNDATION.Core.Controls.AllCheckbox.All.enableOnChange($(this));
                });

                // When changing an item of the group; update the "All" option.
                $(".foundation-checkbox-item", trgt).each(function () {
                    FOUNDATION.Core.Controls.AllCheckbox.Item.enableOnChange($(this));
                });
            },

            All: {
                disableOnChange: function (trgt) {
                    trgt.off("change", FOUNDATION.Core.Controls.AllCheckbox.All.onChange);
                },
                enableOnChange: function (trgt) {
                    trgt.on("change", FOUNDATION.Core.Controls.AllCheckbox.All.onChange);
                },
                onChange: function () {
                    var checked = $(this).is(":checked"),
                        name = $(this).attr("name").replace("-all", ""),
                        selector = "input[name='" + name + "']";

                    // Disable "onChange" event listener for all items of the group.
                    $(selector).each(function () {
                        FOUNDATION.Core.Controls.AllCheckbox.Item.disableOnChange($(this));
                    });

                    // Update the "checked" attribute of all items of the group.
                    $(selector).prop("checked", checked);

                    // Re-enable "onChange" event listener for all items of the group.
                    $(selector).each(function () {
                        FOUNDATION.Core.Controls.AllCheckbox.Item.enableOnChange($(this));
                    });
                }
            },
            Item: {
                disableOnChange: function (trgt) {
                    trgt.off("change", FOUNDATION.Core.Controls.AllCheckbox.Item.onChange);
                },
                enableOnChange: function (trgt) {
                    trgt.on("change", FOUNDATION.Core.Controls.AllCheckbox.Item.onChange);
                },
                onChange: function () {
                    var name = $(this).attr("name"),
                        all = $("input[name='" + name + "-all']"),
                        allChecked = true;
                    if (all !== null) {
                        // Verify that all options of the group of inputs (i.e. checkboxes) are checked.
                        $("input[name='" + name + "']").each(function () { allChecked = allChecked && $(this).is(":checked"); });

                        // Disable "onChange" event listener for the "All" option.
                        FOUNDATION.Core.Controls.AllCheckbox.All.disableOnChange(all);

                        // Update the "checked" attribute of the "All" option.
                        all.prop("checked", allChecked);

                        // Re-enable "onChange" event listener for the "All" option.
                        FOUNDATION.Core.Controls.AllCheckbox.All.enableOnChange(all);
                    }
                }
            }
        },
        Button: {
            ignoreFormMonitor: function (evt) {
                // Ignore the form monitor.
                FOUNDATION.Core.Controls.Form._IgnoreFormMonitor = true;
            },
            onClick: function (evt) {
                var cntrl = $(this),
                    actions = cntrl.data().onClick,
                    data = FOUNDATION.Core.Controls.Form.collectData(cntrl);
                if (!actions || !actions.length) { console.log("err: FOUNDATION.Core.Controls.Button.onClick : Action(s) undefined."); return; }

                var isValid = true;
                var hasValidation = cntrl.closest("div.wb-frmvld").length && !cntrl.is(".foundation-ignore-form-validation");
                if (hasValidation) {
                    var form = cntrl.closest("form");
                    if (typeof $(form).validate !== undefined)
                        isValid = $(form).valid();
                }

                if (isValid) {
                    // PreventDefault() only if there is no "submit" action.
                    // Note: The "submit" action should submit the action as expected.
                    if (!$.grep(actions, function (i, o) { return i.type === "default"; }).length) { evt.preventDefault(); }
                    // Execute actions.
                    $.each(actions, function (i, o) {
                        // Create Deferred object and pass it to the async method FOUNDATION.Core.action() - used for callback on completion (resolved/rejected).
                        FOUNDATION.Core.action(cntrl, o.type, data, o.url, o.httpMethod ? o.httpMethod : "POST", o.target, FOUNDATION.Core.Controls.Form.addDeferred());
                    });
                    // Ignore the form monitor.
                    FOUNDATION.Core.Controls.Form._IgnoreFormMonitor = true;
                    // Lock the form.
                    FOUNDATION.Core.Controls.Form.lock(cntrl);
                }
            }
        },
        Checkbox: {
            toggleVisible: function (targetObjectId) {
                var target = "#" + targetObjectId;

                // if this is not an id then try as a class...
                // GM - May 2017 
                // CL - fixed, used length instead of id...
                if ($(target).length === 0) {
                    target = '.' + targetObjectId;
                }

                if (event.target.checked) {
                    $(target).show();
                }
                else {
                    $(target).hide();
                }
            }
        },
        Form: {
            _Deferred: [],
            _IgnoreFormMonitor: false,
            _MonitoredInput: ":input:not(.foundation-ignore-form-monitor,.foundation-ignore-form-monitor *,.dataTables_filter :input,.dataTables_length :input)",

            init: function (trgt) {
                trgt = trgt || wb.doc;

                // Initialize handler for checkboxes to enable toggle of all items with a single checkbox.
                FOUNDATION.Core.Controls.AllCheckbox.init(trgt);
                // Attach char counters.
                FOUNDATION.Core.Controls.Textbox.initCharCounters(trgt);
                // Attach number formatting.
                FOUNDATION.Core.Controls.Textbox.initNumberFormatting(trgt);
                // Initialize the form monitor.
                FOUNDATION.Core.Controls.Form.initFormMonitor(trgt);
            },
            initFormMonitor: function (trgt) {
                trgt = trgt || wb.doc;

                // Reset the flag.
                FOUNDATION.Core.Controls.Form._IgnoreFormMonitor = false;

                // Register the controls' initial values (for the form monitor).
                $(FOUNDATION.Core.Controls.Form._MonitoredInput, trgt).each(function () {
                    if ($(this).is(":checkbox,:radio"))
                        $(this).data("initial-state", $(this).is(":checked"));
                    else
                        $(this).data("initial-value", $(this).val());
                });

                // Attach an "onbeforeunload" event handler.
                $(window).off("beforeunload").on("beforeunload", FOUNDATION.Core.Controls.Form.isDirty);
            },

            addDeferred: function (dfd) {
                dfd = dfd || $.Deferred();
                FOUNDATION.Core.Controls.Form._Deferred.push(dfd);
                return dfd;
            },
            collectData: function (cntrl, data) {
                // Collect the data of the container from which the cntrl's event was triggered.
                var container = cntrl.closest(".foundation-form-container"),
                    controls = null,
                    id = cntrl.data().id || cntrl.attr("name") || cntrl.attr("id");
                data = data || {};
                if (!container.length) { container = cntrl.closest("form"); }
                controls = $(":input", container);
                if (!!controls && !!controls.length) {
                    // Loop through the controls and extract the data.
                    $.each(controls, function (i, o) {
                        var id = $(o).data().id || $(o).attr("name") || $(o).attr("id"),
                            type = $(o).attr("type"),
                            value = $(o).val();
                        if (!id) return true; // Ignore if no "id"/"name".
                        switch (type) {
                            case "checkbox":
                            case "radio":
                                if (!$(o).is(":checked")) return true; // Ignore, if unchecked.
                                break;
                            case "file": // Special logic for file types
                                if ($(o)[0].files.length === 1) {
                                    id = $(o)[0].files[0].name;
                                    value = $(o)[0].files[0];
                                }
                                break;
                            default:
                                value = $(o).val();
                        }
                        if (data.hasOwnProperty(id)) {
                            if (Array.isArray(data[id])) {
                                data[id] = $.merge(data[id], [value]);
                            } else {
                                data[id] = $.merge([data[id]], [value]);
                            }
                        } else {
                            // Fix for multiple instance of a special character
                            if (typeof value === "string") { value = value.split("\r").join("\\r").split("\n").join("\\n").split("\t").join("\\t").split("\"").join("\\\""); }
                            $.extend(data, JSON.parse("{\"" + id + "\":\"" + value + "\"}"));
                        }
                    });
                } else if (id) {
                    // If no controls (or no container), simply submit the value of the cntrl.
                    data = { id: cntrl.val() };
                } else {
                    return null;
                }
                return data;
            },
            isDirty: function () {
                var isDirty = false;

                // Should we check if the form has changed? If not, reset the flag and exit.
                if (FOUNDATION.Core.Controls.Form._IgnoreFormMonitor) { FOUNDATION.Core.Controls.Form._IgnoreFormMonitor = false; return; }

                // Compare each field to their initial state/value.
                $(FOUNDATION.Core.Controls.Form._MonitoredInput).each(function () {
                    if ($(this).is(":checkbox,:radio")) {
                        if ($(this).data("initial-state") !== undefined && $(this).data("initial-state") !== $(this).is(":checked")) {
                            isDirty = true;
                            return false;
                        }
                    } else {
                        if ($(this).data("initial-value") !== undefined && $(this).data("initial-value") !== $(this).val()) {
                            isDirty = true;
                            return false;
                        }
                    }
                });

                if (isDirty === true)
                    return FOUNDATION.Core._loc.get("FormMonitorMessage");
            },
            lock: function (cntrl, trgt) {
                trgt = trgt || cntrl.closest(".foundation-form-container");
                if (!trgt.length) { trgt = cntrl.closest("form"); }

                // Create temporary hidden fields for checkboxes, radiobuttons and dropdownlists.
                $(":checkbox:checked, :radio:checked", trgt).each(function (i, o) {
                    $("<input class=\"foundation-form-locked-checkbox-radio\" type=\"hidden\" />").attr("name", $(o).attr("name")).val($(o).val()).insertAfter($(o));
                });
                // Fix for bug #226 - temp controls were being added inside the select element. Changed to add them after the parent select element will also re-fix bug #147
                $("select option:selected", trgt).each(function (i, o) {
                    $("<input class=\"foundation-form-locked-select\" type=\"hidden\" />").attr("name", $(o).closest("select").attr("name")).val($(o).val()).insertAfter($(o).parent());
                });
                // Disable all buttons, button links, checkboxes, dropdownlists, radiobuttons & textboxes in the form - if they are not already disabled/readonly.
                $("button, input, textarea", trgt).not(":checkbox,:radio,[readonly]").addClass("foundation-form-locked").prop("readonly", true);
                $(":checkbox, :radio, select", trgt).not(":disabled").addClass("foundation-form-locked").prop("disabled", true);
                $("a.btn", trgt).not(":disabled").addClass("disabled foundation-form-locked");
                cntrl.addClass("disabled foundation-form-locked").prop("readonly", true);

                // Unlock the form when the all of the functions in the _Deferred array are resolved/rejected.
                // "$.when.apply($, FOUNDATION.Core.Controls.Form._Deferred)": $.when() expects individual parameters but since here our number of deferred objects is unknown, we use .apply() to convert an array into individual parameters.
                $.when.apply($, FOUNDATION.Core.Controls.Form._Deferred)
                    .then(function (reason) {
                        // Clear the array of deferred objects.
                        FOUNDATION.Core.Controls.Form._Deferred = [];

                        // Unlock the form.
                        if (reason !== "keep-locked") {
                            FOUNDATION.Core.Controls.Form.unlock(cntrl, trgt);
                        }
                    });
            },
            onLoad: function (frm) {
                var cntrl = $(frm),
                    actions = cntrl.data().onLoad,
                    data = FOUNDATION.Core.Controls.Form.collectData(cntrl);
                if (!actions || !actions.length) { console.log("err: FOUNDATION.Core.Controls.Form.onLoad : Action(s) undefined."); return; }
                $.each(actions, function (i, o) {
                    FOUNDATION.Core.action(cntrl, o.type, data, o.url, o.httpMethod ? o.httpMethod : "POST", o.target);
                });
            },
            unlock: function (cntrl, trgt) {
                trgt = trgt || cntrl.closest(".foundation-form-container");
                if (!trgt.length) { trgt = cntrl.closest("form"); }

                // Enable all buttons, button links, checkboxes, dropdownlists, radiobuttons & textboxes in the form.
                $("button.foundation-form-locked, input.foundation-form-locked, textarea.foundation-form-locked", trgt).not(":checkbox,:radio").removeClass("foundation-form-locked").prop("readonly", false);
                $(":checkbox.foundation-form-locked, :radio.foundation-form-locked, select.foundation-form-locked", trgt).removeClass("foundation-form-locked").prop("disabled", false);
                $("a.btn.foundation-form-locked", trgt).removeClass("disabled foundation-form-locked");
                cntrl.removeClass("disabled foundation-form-locked").prop("readonly", false);
                // Remove temporary hidden fields (if any).
                $(".foundation-form-locked-checkbox-radio, .foundation-form-locked-select").remove();
            }
        },
        Generic: {
            onChange: function (evt) {
                var cntrl = $(this),
                    actions = cntrl.data().onChange,
                    data = FOUNDATION.Core.Controls.Form.collectData(cntrl);
                if (!actions || !actions.length) { console.log("err: FOUNDATION.Core.Controls.Generic.onChange : Action(s) undefined."); return; }

                $.each(actions, function (i, o) {
                    // Create Deferred object and pass it to the async method FOUNDATION.Core.action() - used for callback on completion (resolved/rejected).
                    FOUNDATION.Core.action(cntrl, o.type, data, o.url, o.httpMethod ? o.httpMethod : "POST", o.target, FOUNDATION.Core.Controls.Form.addDeferred());
                });

                // Lock the form.
                FOUNDATION.Core.Controls.Form.lock(cntrl);
            }
        },
        Link: {
            onClick: function (evt) {
                evt.preventDefault();
                var cntrl = $(this),
                    actions = cntrl.data().onClick,
                    data = FOUNDATION.Core.Controls.Form.collectData(cntrl);
                if (!actions || !actions.length) { console.log("err: FOUNDATION.Core.Controls.Link.onClick : Action(s) undefined."); return; }

                var bAllow = true;
                if (cntrl.attr("type") === "submit" && cntrl.closest("form").attr("method") === "post") {
                    var form = cntrl.closest("form");
                    if (typeof $(form).validate !== undefined) {
                        bAllow = $(form).validate();
                        bAllow = $(form).valid();
                    }
                }
                if (bAllow) {
                    $.each(actions, function (i, o) {
                        // Create Deferred object and pass it to the async method FOUNDATION.Core.action() - used for callback on completion (resolved/rejected).
                        FOUNDATION.Core.action(cntrl, o.type, data, o.url, o.httpMethod ? o.httpMethod : "POST", o.target, FOUNDATION.Core.Controls.Form.addDeferred());
                    });
                }

                // Lock the form.
                FOUNDATION.Core.Controls.Form.lock(cntrl);
            }
        },
        Textbox: {
            initCharCounters: function (trgt) {
                trgt = trgt || wb.doc;

                var slctr = "input[maxlength]:not([disabled],[readonly]),textarea[maxlength]:not([disabled],[readonly])";
                $(slctr, trgt).each(function () {                   
                    var charCounter = $(this).siblings(".foundation-char-counter").first(),
                        charRemaining = 0,
                        count = $(this).val().replace(/\n/g, "\r\n").length,
                        max = $(this).attr("maxlength");
                    charRemaining = max - count;
                    if (charCounter.length === 1 && !charCounter.text())
                        $(".foundation-char-count", charCounter)
                            .text(charRemaining > 0 ? charRemaining : 0)
                            .after(" " + FOUNDATION.Core._loc.get("CharactersRemaining"));
                });

                wb.doc.on("change", slctr, FOUNDATION.Core.Controls.Textbox.updateCharCounter);
                wb.doc.on("keydown", slctr, FOUNDATION.Core.Controls.Textbox.updateCharCounter);
                wb.doc.on("keyup", slctr, FOUNDATION.Core.Controls.Textbox.updateCharCounter);
            },
            initNumberFormatting: function (trgt) {
                trgt = trgt || wb.doc;
                var slctr = 'input[data-foundation-type="number"]';
                $(slctr, trgt).each(function () {
                    if (FOUNDATION.Core._loc.get("TwoLetterLanguage") === "fr")
                        $(this).off("keydown").on("keydown", FOUNDATION.Core.Controls.Textbox.keyDownOnNumeric); // Fix French decimal point
                    $(this).on("blur", function (event) { FOUNDATION.Core.Controls.Textbox.targetLoseFocus(event); }); // Reformat on exit
                });
            },

            keyDownOnNumeric: function (event) {
                /// <summary>KeyDownOnNumeric - This method is used to fix the decimal point typed by the user in French.</summary>
                /// <param name="event" type="Object">this is the on key down event</param>
                var $this = $(event.currentTarget),
                    code = event.keyCode ? event.keyCode : event.which;

                if (code === 190 || code === 110) {
                    $this.val($this.val() + ",");
                    event.preventDefault();
                }
            },
            reFormatNumber: function (target) {
                var result = '';
                var value = target.value;
                if (value === null) { return result; }
                if (value.length === 0) { return result; }

                result = value;
                var n = FOUNDATION.Core._loc.parseNumber(value);
                if (!isNaN(n)) {
                    var step = $(target).attr("data-foundation-step");
                    var decimalCount = 0;
                    if (step && step !== undefined && step.indexOf('.') > -1) {
                        decimalCount = step.length - step.indexOf('.') - 1;
                    }

                    result = FOUNDATION.Core._loc.formatNumber(n, decimalCount);
                }
                target.value = result;
            },
            targetLoseFocus: function (event) {
                $(event.currentTarget).each(function () {
                    if ($(this).hasClass("valid")) {
                        FOUNDATION.Core.Controls.Textbox.reFormatNumber(this);
                    }
                });
            },
            updateCharCounter: function () {                
                var charCounter = $(this).siblings(".foundation-char-counter").first(),
                    count = $(this).val().replace(/\n/g, "\r\n").length,
                    keyCode = event.keyCode ? event.keyCode : event.which,
                    keyCtrl = event.ctrlKey,
                    max = $(this).attr("maxlength"),
                    charRemaining = max - count;

                /* If text is bigger than the maxlength, take the first <maxlength> characters. */
                if (count > max)
                {                    
                    $(this).val($(this).val().replace(/\n/g, "\r\n").substring(0, max));
                    count = $(this).val().replace(/\n/g, "\r\n").length;
                    charRemaining = 0;
                }

                /* Update character count */
                if (charCounter.length === 1)
                    $(".foundation-char-count", charCounter).text(charRemaining > 0 ? charRemaining : 0);


                if (charRemaining === 1 && keyCode === 13)
                    return false;
                else if (charRemaining <= 0)
                {
                    if (keyCode === 8
                        || keyCode === 9
                        || (keyCode >= 33 && keyCode <= 40)
                        || keyCode === 46)
                        return true;
                    else if (keyCtrl && keyCode === 88) /* Ctrl-X: Cut */
                        return true;
                    else if (keyCtrl && keyCode === 89) /* Ctrl-Y: Redo */
                        return true;
                    else if (keyCtrl && keyCode === 90) /* Ctrl-Z: Undo */
                        return true;
                    else if (keyCtrl && keyCode === 65) /* Ctrl-A: Select all */
                        return true;
                    else if (keyCtrl && keyCode === 67) /* Ctrl-C: Copy */
                        return true;
                    return false;
                }
            }
        }
    },
    Cookies: {
        exists: function (cname) { var name = cname + "="; return document.cookie.indexOf(name) > -1; },
        getAllKeys: function () { var ca = document.cookie.split(';'), c; for (var i = 0; i < ca.length; i++) { c = $.trim(ca[i]); ca[i] = c.substring(0, c.indexOf("=")); } return ca; },
        getItem: function (cname) { var name = cname + "=", ca = document.cookie.split(';'), c; for (var i = 0; i < ca.length; i++) { c = $.trim(ca[i]); if (c.indexOf(name) === 0) return c.substring(name.length, c.length); } return ""; },
        setItem: function (cname, cvalue, cexpiry) { var d = new Date(), expires; d.setTime(d.getTime() + cexpiry * 24 * 60 * 60 * 1000); expires = "expires=" + d.toGMTString(); document.cookie = cname + "=" + cvalue + "; " + expires; }
    },
    LocalStorage: {
        get: function (name) { if (typeof Storage !== "undefined") { return localStorage.getItem(name); } else if (navigator.cookieEnabled) { if (FOUNDATION.Core.Cookies.exists(name)) { return FOUNDATION.Core.Cookies.getItem(name); } else { return null; } } },
        isSupported: function () { return typeof Storage !== "undefined" || navigator.cookieEnabled; },
        remove: function (name) { if (typeof Storage !== "undefined") { return localStorage.removeItem(name); } else if (navigator.cookieEnabled) { if (FOUNDATION.Core.Cookies.exists(name)) { FOUNDATION.Core.Cookies.setItem(name, "", 0); } else { return null; } } },
        removeStartsWith: function (prefix) { if (typeof Storage !== "undefined") { return Object.keys(localStorage).forEach(function (key) { if (key.startsWith(prefix)) { localStorage.removeItem(key); } }); } else if (navigator.cookieEnabled) { FOUNDATION.Core.Cookies.getAllKeys().forEach(function (key) { if (key.startsWith(prefix)) { FOUNDATION.Core.Cookies.setItem(key, "", 0); } }); } },
        set: function (name, value, expiry) { if (typeof Storage !== "undefined") { localStorage.setItem(name, value); } else if (navigator.cookieEnabled) { expiry = expiry ? expiry : 365; FOUNDATION.Core.Cookies.setItem(name, value, expiry); } }
    },
    Loading: {
        _Placeholder: null,

        init: function () {
            // Attach the FOUNDATION.Core.Loading._Placeholder to the elements on the page.
            if (!$(".foundation-loading").length && $("main").length) $("main").prepend($("<div id=\"foundation-loading-spinner\" class=\"foundation-loading\" role=\"alert\" aria-hidden=\"false\" aria-busy=\"true\" aria-live=\"polite\" >" + FOUNDATION.Core._loc.get("Loading") + " ...<br /><span class=\"fa fa-spinner fa-spin fa-3x fa-fw\"></span><span class=\"sr-only\">" + FOUNDATION.Core._loc.get("Loading") + " ...</span></div>"));
            FOUNDATION.Core.Loading._Placeholder = $(".foundation-loading");
            if (FOUNDATION.Core.Loading._Placeholder.length) {
                FOUNDATION.Core.Loading._Placeholder.hide();

                // Bind the ajaxStart & ajaxComplete events to the "Loading..." element.
                $(document).ajaxStart(function () { FOUNDATION.Core.Loading._Placeholder.show(); });
                $(document).ajaxComplete(function () { FOUNDATION.Core.Loading._Placeholder.hide(); });
            }
        },
        initWbDisable: function () {
            // Attach the FOUNDATION.Core.Loading._Placeholder to the elements on the page.
            if (!$(".foundation-loading").length && $("main").length) $("main").prepend($("<div id=\"foundation-loading-spinner\" class=\"foundation-loading\" role=\"alert\" aria-hidden=\"false\" aria-busy=\"true\" aria-live=\"polite\" >" + FOUNDATION.Core._loc.get("Loading") + " ...<br /><span class=\"fa fa-spinner fa-spin fa-3x fa-fw\"></span><span class=\"sr-only\">" + FOUNDATION.Core._loc.get("Loading") + " ...</span></div>"));
            FOUNDATION.Core.Loading._Placeholder = $(".foundation-loading");
            if (FOUNDATION.Core.Loading._Placeholder.length) {
                FOUNDATION.Core.Loading._Placeholder.hide();
            }
        },

        hide: function () {
            FOUNDATION.Core.Loading._Placeholder.hide();
        },
        show: function () {
            FOUNDATION.Core.Loading._Placeholder.show();
        }
    },
    Modal: {
        _DefaultSettings: {
            "type": "inline"
        },
        _Ignore: false,

        init: function () {
            // Bind buttons that will trigger a modal popup.
            wb.doc.on("click", "a[data-modal],button[data-modal]", FOUNDATION.Core.Modal.open);
        },

        Button: {
            Types: {
                ACTION: "action",
                ASYNC_ACTION: "asyncaction",
                CONFIRM: "confirm",
                DISMISS: "dismiss"
            },

            bind: function (cntrl, evt, sttngs) {
                // Bind buttons inside the modal popup.
                var id = sttngs.src;
                if (id) {
                    $("button[data-modal-button]", id).each(function (i, o) {
                        var slctr = id + " #" + $(o)[0].id,
                            type = $(o).data().modalButton.type;
                        if (slctr) {
                            // Deactivate any previous event that may have been bound to the button.
                            wb.doc.off("click", slctr);

                            // Bind a click event.
                            switch (type) {
                                case FOUNDATION.Core.Modal.Button.Types.ACTION:
                                    wb.doc.on("click", slctr, { "caller": cntrl, "event": evt }, FOUNDATION.Core.Modal.Button.callbackAction);
                                    break;
                                case FOUNDATION.Core.Modal.Button.Types.ASYNC_ACTION:
                                    wb.doc.on("click", slctr, { "caller": cntrl, "event": evt }, FOUNDATION.Core.Modal.Button.callbackAsyncAction);
                                    break;
                                case FOUNDATION.Core.Modal.Button.Types.CONFIRM:
                                    wb.doc.on("click", slctr, { "caller": cntrl, "event": evt }, FOUNDATION.Core.Modal.Button.callbackConfirm);
                                    break;
                                case FOUNDATION.Core.Modal.Button.Types.DISMISS:
                                default:
                                    wb.doc.on("click", slctr, FOUNDATION.Core.Modal.Button.callbackDismiss);
                            }
                        }
                    });
                }
            },
            callbackAction: function (args) {
                var action = $(this).data().onClick,
                    container = null,
                    cntrl = args.data.caller;
                container = $(this).closest("form");
                if (!container.length) { container = cntrl.closest("form"); }

                if (!action) { console.log("err: FOUNDATION.Core.Modal.Button.callbackAction : Action failed. [No action]"); }
                if (!container.length) { console.log("err: FOUNDATION.Core.Modal.Button.callbackAction : Action failed. [No form]"); }
                $(container).attr("action", action[0].url).submit();
            },
            callbackAsyncAction: function (args) { },
            callbackConfirm: function (args) {
                args.data.caller.trigger(args.data.event.type, { "confirmed": true });
            },
            callbackDismiss: function (evt) { }
        },
        close: function () {
            // Close the modal popup.
            $.magnificPopup.close();

            // Unbind "ESC" key.
            wb.doc.off("keyup", FOUNDATION.Core.Modal.handleEscapeKey);
        },
        handleEscapeKey: function (evt) {
            if (evt.keyCode === 27) // ESC.
                FOUNDATION.Core.Modal.close();
        },
        open: function (evt, args, callerOverride) {
            if (args !== undefined && !!args.confirmed || !!FOUNDATION.Core.Modal._Ignore) { FOUNDATION.Core.Modal._Ignore = false; return true; } // Proceed if the modal was already confirmed/should be ignored.
            if (evt) {
                evt.preventDefault();
                evt.stopImmediatePropagation();
            }

            // Collect settings from the caller.
            var button = callerOverride || $(this),
                sttngs = {};
            $.extend(sttngs, FOUNDATION.Core.Modal._DefaultSettings, button.data().modal, args);

            // Bind event handlers on the buttons of the modal.
            FOUNDATION.Core.Modal.Button.bind(button, evt, sttngs);

            // Bind "ESC" key to close the popup.
            wb.doc.on("keyup", FOUNDATION.Core.Modal.handleEscapeKey);

            // Open the modal popup: wb.doc.trigger("open.wb-lbx", ["item", "modal", "image", "ajaxSettings"]);
            wb.doc.trigger("open.wb-lbx", [sttngs, true]);
        }
    },
    WET: {
        init: function (trgt) {
            // Initialize WET objects.
            trgt = trgt || wb.doc;

            // WET buttons.
            $(".wb-toggle", trgt).trigger("wb-init.wb-toggle");
            // WET datatables.
            $(".wb-tables", trgt).trigger("wb-init.wb-tables");
            // WET form validation.
            $(".wb-frmvld", trgt).trigger("wb-init.wb-frmvld");
            // WET lightbox/modal popups.
            $(".wb-lbx", trgt).trigger("wb-init.wb-lbx");
            // WET summary/details.
            $("summary", trgt).trigger("wb-init.wb-details");
            // WET tabs.
            $(".wb-tabs", trgt).trigger("wb-init.wb-tabs");
            // Note: If issues are encountered with the above initialization triggers, please have a look at the following GitHub issue for a script provided by the WET group: https://github.com/wet-boew/wet-boew/issues/8234

            // Initialize form.
            FOUNDATION.Core.Controls.Form.init(trgt);

            // Trigger "FOUNDATION.Core-WET-ready" event.
            wb.doc.trigger("FOUNDATION.Core-WET-ready");
        },

        getStandardHtmlVersionUrl: function () {
            // Rebuild the query string.
            var qsParams = wb.pageUrlParts.params,
                url = "?";
            for (qsParam in qsParams) { if (qsParams.hasOwnProperty(qsParam) && qsParam !== "wbdisable") { url += qsParam + "=" + qsParams[qsParam] + "&#38;"; } }
            return url + "wbdisable=false";
        }
    },

    // Localization.
    _loc: {
        _Language: null,

        init: function () {
            // Set the current language.
            FOUNDATION.Core._loc._Language = $("html")[0].lang || "en";
        },
        initWbDisable: function () {
            // Set the current language.
            FOUNDATION.Core._loc._Language = $("html")[0].lang || "en";

            // Add entries to the dictionary for the basic HTML version.
            var standardHtmlVersionUrl = FOUNDATION.Core.WET.getStandardHtmlVersionUrl();
            FOUNDATION.Core._loc.Dictionary.PageUnavailableHeading = { en: "Unavailable", fr: "Non disponible" };
            FOUNDATION.Core._loc.Dictionary.PageUnavailableMessage = {
                en: "<p>Some features of the current page cannot be reproduced in the Basic HTML version.</p><p>Please reload the page in the fully accessible <a href=\"{0}\" class=\"alert-link\">standard version</a> to take advantage of all of its functionalities <strong>or</strong> <a href=\"mailto:{1}\" class=\"alert-link\">contact us</a> and we'll help you out.</p>".format(standardHtmlVersionUrl, CONFIG.SHARED_MAILBOX),
                fr: "<p>Certaines fonctions de la page actuelle ne peuvent &ecirc;tre reproduites en version HTML simplifi&eacute;e.</p><p>Veuillez recharger la page <a href=\"{0}\" class=\"alert-link\">en version standard</a> afin de b&eacute;n&eacute;ficier de toutes ses fonctionnalit&eacute;s <strong>ou</strong> <a href=\"mailto:{1}\" class=\"alert-link\">communiquez avec nous</a> pour obtenir de l'aide.</p>".format(standardHtmlVersionUrl, CONFIG.SHARED_MAILBOX)
            };
        },

        Dictionary: {
            ":": { en: ":", fr: "&#160;:" },
            ActionUnexpectedError: { en: "Unable to perform the requested action: Unexpected error.", fr: "Incapable de compl&eacute;ter la requ&ecirc;te: une erreur innatendue s'est produite." },
            AlertErrorHeading: { en: "Error", fr: "Erreur" },
            AlertInfoHeading: { en: "Information", fr: "Information" },
            AlertSuccessHeading: { en: "Success", fr: "Succ&egrave;s" },
            AlertWarningHeading: { en: "Warning", fr: "Avertissement" },
            CharactersRemaining: { en: "character(s) remaining", fr: "caract&egrave;re(s) restant(s)" },
            Close: { en: "Close", fr: "Fermer" },
            Copy: { en: "Copy", fr: "Copier" },
            CopyToClipboard: { en: "Copy to clipboard", fr: "Copier dans le presse-papier" },
            Dismiss: { en: "Dismiss", fr: "&Eacute;carter" },
            ErrorMessageColon: { en: "Error message:", fr: "Message d'erreur&nbsp;:" },
            ErrorMessageScript: { en: "Script error: see browser console (F12) for detail.", fr: "Erreur de script&nbsp;: Jetez un coup d'oeil &agrave; la console du fureteur (F12) pour plus de d&eacute;tails." },
            ErrorUrlColon: { en: "URL:", fr: "URL&nbsp;:" },
            ExportToCSV: { en: "Export to CSV", fr: "Exporter vers CSV" },
            ExportToExcel: { en: "Export to Excel", fr: "Exporter vers Excel" },
            FormMonitorMessage: { en: "You will lose your changes if you do not save them. What would like to do?", fr: "Vous perdrez vos changements si vous ne sauvegardez pas le formulaire. Que voulez-vous faire?" },
            Language: { en: 1, fr: 2 },
            LanguageCulture: { en: "en-CA", fr: "fr-CA" },
            Loading: { en: "Loading", fr: "Chargement" },
            OtherLanguageCulture: { en: "fr-CA", fr: "en-CA" },
            OtherThreeLetterLanguage: { en: "fra", fr: "eng" },
            OtherTwoLetterLanguage: { en: "fr", fr: "en" },
            SearchResults0: { en: "No results found", fr: "Aucun r&eacute;sultat obtenu" },
            SearchResults1: { en: "1 result found", fr: "1 r&eacute;sultat obtenu" },
            SearchResultsPlural: { en: "{0} results found", fr: "{0} r&eacute;sultats obtenus" },
            ThreeLetterLanguage: { en: "eng", fr: "fra" },
            TwoLetterLanguage: { en: "en", fr: "fr" },
            UnexpectedJavaScriptErrorTemplate: { en: '<p>An unexpected error occured. Something went wrong while executing the action you requested. What next?</p><ul><li>Refresh the page to try again;</li><li>Return to the <a href="/en">home page</a>;</li><li><a href="mailto:{0}">Contact us</a> and we\'ll help you out.</li></ul><p class="mrgn-tp-md small text-muted">{1}</p>', fr: '<p>Une erreur s\'est produite lors de l\'ex&eacute;cution de votre requ&ecirc;te. Que faire?</p><ul><li>Rafra&icirc;chir la page et essayer de nouveau;</li><li>Retourner &agrave; la <a href="/fr">page d\'accueil</a>;</li><li><a href="mailto:{0}">Communiquez avec nous</a> pour obtenir de l\'aide.</li></ul><p class="mrgn-tp-md small text-muted">{1}</p>' },
            ValidationUniqueNumber: { en: "Must be a unique number", fr: "Doit &ecirc;tre un chiffre unique" }
        },
        formatNumber: function (value, decimalCount) {
            var result = '';
            if (value !== null) {
                var numberFormat = Intl.NumberFormat(FOUNDATION.Core._loc.get("LanguageCulture"), { minimumFractionDigits: decimalCount, maximumFractionDigits: decimalCount });
                result = numberFormat.format(value);
                if (FOUNDATION.Core._loc._Language === "en") {
                    result = result.replace(/,/g, '');
                }
                result = result.replace(/\s+/g, '');
            }
            return result;
        },
        get: function (key) {
            return this.Dictionary[key][this._Language];
        },
        parseNumber: function (value) {
            value = value.replace(/\s+/g, '');
            if (FOUNDATION.Core._loc._Language === "en") {
                value = value.replace(/,/g, '');
            } else if (FOUNDATION.Core._loc._Language === "fr") {
                value = value.replace(/,/g, '.');
            }
            if (value.endsWith('.')) {
                var ix = value.length;
                value = value.substr(0, ix - 1);
            }
            return Number(value);
        }
    }
};

// Implement $.invisible().
if (!$.fn.invisible) {
    $.fn.invisible = function () { return this.css('visibility', 'hidden'); };
}
// Implement $.visible().
if (!$.fn.visible) {
    $.fn.visible = function () { return this.css('visibility', 'visible'); };
}
// Implement String.format().
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments, str = this;
        if (str.match(/(%7B)(\d+)(%7D)/g)) { str = str.replace(/(%7B)(\d+)(%7D)/g, function (match, number) { return match.replace("%7B", "{").replace("%7D", "}"); }); }
        return str.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] !== undefined ? args[number] : match;
        });
    };
}
// Implement String.pad().
if (!String.prototype.pad) {
    String.prototype.pad = function (width, chr) {
        var num = this + '';
        chr = chr || '0';
        return num.length >= width ? num : new Array(width - num.length + 1).join(chr) + num;
    };
}
// Implement String.startsWith().
if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        position = position || 0;
        return this.substr(position, searchString.length) === searchString;
    };
}
// Implement String.endsWith().
if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (suffix) {
        return this.indexOf(suffix, this.length - suffix.length) !== -1;
    };
}
// Implement Array.groupBy()
if (!Array.prototype.groupBy) {
    Array.prototype.groupBy = function (keyName) {
        var items = {}, base, key;
        $.each(this, function (index, val) {
            key = val[keyName];
            if (!items[key]) {
                items[key] = [];
            }
            items[key].push(val);
        });

        return items;
    };
}

// Initialization.
$(function () { if (wb.isDisabled) FOUNDATION.Core.initWbDisable(); });
wb.doc.on("wb-ready.wb", function (evt) {
    FOUNDATION.Core.init();
});