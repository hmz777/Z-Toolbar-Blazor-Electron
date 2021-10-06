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
        let target = document.getElementById("main-content");
        target.addEventListener("drop", (e) => {
            e.preventDefault();
            e.stopPropagation();

            var NamesPaths = {};

            for (const f of e.dataTransfer.files) {
                NamesPaths[f.name] = f.path;
            }

            handlerRef.invokeMethodAsync("ItemDropHandler", JSON.stringify(NamesPaths), (Object.keys(e.dataTransfer.files).length > 1) ? true : false);
        });
        document.addEventListener('dragover', (e) => {
            e.preventDefault();
            e.stopPropagation();
        });
    }
}