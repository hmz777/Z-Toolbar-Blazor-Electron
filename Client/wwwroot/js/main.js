const { ipcRenderer } = require('electron');
var AppReference;

ZHelpers = {
    InitElementContainerScrollbars: function (element) {
        OverlayScrollbars(document.querySelector(element), {
            className: "os-theme-light",
            overflowBehavior: {
                x: "hidden",
                y: "scroll"
            },
        });
    },
    AttachItemDropEvent: function (handlerRef) {
        let target = document.getElementById("main");
        target.addEventListener("drop", (e) => {
            e.preventDefault();
            e.stopPropagation();

            var NamesPaths = [];

            for (const f of e.dataTransfer.files) {
                NamesPaths.push({ Name: f.name, Path: f.path });
            }

            handlerRef.invokeMethodAsync("ItemDropHandler", JSON.stringify(NamesPaths));
        });
        document.addEventListener('dragover', (e) => {
            e.preventDefault();
            e.stopPropagation();
        });
    },
    FocusElement: function (selector) {
        document.querySelector(selector).focus();
    },
    HookBlazorToCommandChannel: function (appReference) {
        AppReference = appReference;

        ipcRenderer.on('CommandChannel', async (event, arg) => {
            _ = await AppReference.invokeMethodAsync("ToggleVisibility", arg.value);
        });
    }
}



