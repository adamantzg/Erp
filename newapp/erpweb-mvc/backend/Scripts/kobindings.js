﻿var instances_by_id = {} // needed for referencing instances during updates.
    , init_queue = $.Deferred() // jQuery deferred object used for creating TinyMCE instances synchronously
    , init_queue_next = init_queue;


function Bindings() {

    init_queue.resolve();
    ko.bindingHandlers.tinymce = {
        init: function(element, valueAccessor, allBindingsAccessor, context) {
            var init_arguments = arguments;
            var options = allBindingsAccessor().tinymceOptions || {};
            var modelValue = valueAccessor();
            var value = ko.utils.unwrapObservable(valueAccessor());
            var el = $(element);

            options.setup = function(ed) {
                ed.onChange.add(function(editor, l) { //handle edits made in the editor. Updates after an undo point is reached.
                    if (ko.isWriteableObservable(modelValue)) {
                        modelValue(l.content);
                    }
                });

                //This is required if you want the HTML Edit Source button to work correctly
                ed.onBeforeSetContent.add(function(editor, l) {
                    if (ko.isWriteableObservable(modelValue)) {
                        modelValue(l.content);
                    }
                });

                ed.onPaste.add(function(ed, evt) { // The paste event for the mouse paste fix.
                    var doc = ed.getDoc();

                    if (ko.isWriteableObservable(modelValue)) {
                        setTimeout(function() { modelValue(ed.getContent({ format: 'raw' })); }, 10);
                    }

                });

                ed.onInit.add(function(ed, evt) { // Make sure observable is updated when leaving editor.
                    var doc = ed.getDoc();
                    tinymce.dom.Event.add(doc, 'blur', function(e) {
                        if (ko.isWriteableObservable(modelValue)) {
                            modelValue(ed.getContent({ format: 'raw' }));
                        }
                    });
                });

            };

            //handle destroying an editor (based on what jQuery plugin does)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function() {
                $(element).parent().find("span.mceEditor,div.mceEditor").each(function(i, node) {
                    var tid = node.id.replace(/_parent$/, '');
                    var ed = tinyMCE.get(tid);
                    if (ed) {
                        ed.remove();
                        // remove referenced instance if possible.
                        if (instances_by_id[tid]) {
                            delete instances_by_id[tid];
                        }
                    }
                });
            });

            // TinyMCE attaches to the element by DOM id, so we need to make one for the element if it doesn't have one already.
            if (!element.id) {
                element.id = tinyMCE.DOM.uniqueId();
            }

            // create each tinyMCE instance synchronously. This addresses an issue when working with foreach bindings
            init_queue_next = init_queue_next.pipe(function() {
                var defer = $.Deferred();
                var init_options = $.extend({}, options, {
                    mode: 'none',
                    init_instance_callback: function(instance) {
                        instances_by_id[element.id] = instance;
                        ko.bindingHandlers.tinymce.update.apply(undefined, init_arguments);
                        defer.resolve(element.id);
                        if (options.hasOwnProperty("init_instance_callback")) {
                            options.init_instance_callback(instance);
                        }
                    }
                });
                setTimeout(function() {
                    tinyMCE.init(init_options);
                    setTimeout(function() {
                        tinyMCE.execCommand("mceAddControl", true, element.id);
                    }, 0);
                }, 0);
                return defer.promise();
            });
            el.val(value);
        },
        update: function(element, valueAccessor, allBindingsAccessor, context) {
            var el = $(element);
            var value = ko.utils.unwrapObservable(valueAccessor());
            var id = el.attr('id');

            //handle programmatic updates to the observable
            // also makes sure it doesn't update it if it's the same.
            // otherwise, it will reload the instance, causing the cursor to jump.
            if (id !== undefined && id !== '' && instances_by_id.hasOwnProperty(id)) {
                var content = instances_by_id[id].getContent({ format: 'raw' });
                if (content !== value) {
                    el.val(value);
                }
            }
        }
    };
    
    ko.bindingHandlers.attrIf = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            var h = ko.utils.unwrapObservable(valueAccessor());
            var show = ko.utils.unwrapObservable(h._if);
            if (show) {
                ko.bindingHandlers.attr.update(element, valueAccessor, allBindingsAccessor);
            } else {
                for (var k in h) {
                    if (h.hasOwnProperty(k) && k.indexOf("_") !== 0) {
                        $(element).removeAttr(k);
                    }
                }
            }
        }
    };

    ko.bindingHandlers.safeChecked = {
        update: function (element, valueAccessor) {
            var v = ko.unwrap(valueAccessor());
            $(element).prop('checked', v == $(element).val());
        },
        init: function(element, valueAccessor) {
            $(element).click(function () {
                var value = valueAccessor();
                value($(this).val());
            });
        }
    };
}