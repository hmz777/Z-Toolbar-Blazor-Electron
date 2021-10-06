var menuX;
var menuY;
var ContextMenuComponentReference;

export function InitCM(ref) {
    ContextMenuComponentReference = ref;

    document.onclick = function (e) {
        if (e.button != 1) {
            ContextMenuComponentReference.invokeMethodAsync("Hide");
        }
    }
}

export function UpdateMouseLocation(x, y) {
    menuX = x;
    menuY = y;
}