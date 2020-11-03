﻿/*
 * Globalize Culture hr-HR
 *
 * http://github.com/jquery/globalize
 *
 * Copyright Software Freedom Conservancy, Inc.
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * This file was generated by the Globalize Culture Generator
 * Translation: bugs found in this file need to be fixed in the generator
 */

(function (window, undefined) {

    var Globalize;

    if (typeof require !== "undefined" &&
        typeof exports !== "undefined" &&
        typeof module !== "undefined") {
        // Assume CommonJS
        Globalize = require("globalize");
    } else {
        // Global variable
        Globalize = window.Globalize;
    }

    Globalize.addCultureInfo("hr-HR", "default", {
        name: "hr-HR",
        englishName: "Croatian (Croatia)",
        nativeName: "hrvatski (Hrvatska)",
        language: "hr",
        numberFormat: {
            pattern: ["- n"],
            ",": ".",
            ".": ",",
            percent: {
                pattern: ["-n%", "n%"],
                ",": ".",
                ".": ","
            },
            currency: {
                pattern: ["-n $", "n $"],
                ",": ".",
                ".": ",",
                symbol: "kn"
            }
        },
        calendars: {
            standard: {
                "/": ".",
                firstDay: 1,
                days: {
                    names: ["nedjelja", "ponedjeljak", "utorak", "srijeda", "četvrtak", "petak", "subota"],
                    namesAbbr: ["ned", "pon", "uto", "sri", "čet", "pet", "sub"],
                    namesShort: ["ne", "po", "ut", "sr", "če", "pe", "su"]
                },
                months: {
                    names: ["siječanj", "veljača", "ožujak", "travanj", "svibanj", "lipanj", "srpanj", "kolovoz", "rujan", "listopad", "studeni", "prosinac", ""],
                    namesAbbr: ["sij", "vlj", "ožu", "tra", "svi", "lip", "srp", "kol", "ruj", "lis", "stu", "pro", ""]
                },
                monthsGenitive: {
                    names: ["siječnja", "veljače", "ožujka", "travnja", "svibnja", "lipnja", "srpnja", "kolovoza", "rujna", "listopada", "studenog", "prosinca", ""],
                    namesAbbr: ["sij", "vlj", "ožu", "tra", "svi", "lip", "srp", "kol", "ruj", "lis", "stu", "pro", ""]
                },
                AM: null,
                PM: null,
                patterns: {
                    d: "d.M.yyyy.",
                    D: "d. MMMM yyyy.",
                    t: "H:mm",
                    T: "H:mm:ss",
                    f: "d. MMMM yyyy. H:mm",
                    F: "d. MMMM yyyy. H:mm:ss",
                    M: "d. MMMM"
                }
            }
        }
    });

}(this));