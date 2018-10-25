export function clone(o1) {
    var o2 = {};
    function deepClone(v) {
        if (v === o1) {
            return o2;
        } else if (Array.isArray(v)) {
            var ar = new Array(v.length);
            for (var i = 0; i < v.length; i++) {
                ar[i] = deepClone(v[i]);
            }
            return ar;
        } else if (typeof v === "object") {
            var v2 = {};
            Object.keys(v).forEach(k => {
                v2[k] = deepClone(v[k]);
            });
            return v2;
        } else {
            return v;
        }
    }
    Object.keys(o1).forEach(k => {
        o2[k] = deepClone(o1[k]);
    });
    return o2;
}