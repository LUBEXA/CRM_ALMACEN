// El menú lateral siempre arranca contraído. El usuario puede expandirlo de forma
// temporal, pero al elegir una opción o al recargar/navegar vuelve a contraerse.
(function () {
    function contraer() {
        const cb = document.getElementById('sidebar-collapse');
        if (cb) cb.checked = true;
    }

    function cerrarCajon() {
        const cb = document.getElementById('navmenu-toggle');
        if (cb) cb.checked = false;
    }

    // Al elegir una opción del menú, volver a contraer y cerrar el cajón móvil.
    document.addEventListener('click', function (e) {
        if (e.target && e.target.closest && e.target.closest('.sidebar .nav-link')) {
            contraer();
            cerrarCajon();
        }
    });

    // Forzar contraído al cargar y después de cada navegación de Blazor.
    document.addEventListener('DOMContentLoaded', contraer);
    document.addEventListener('enhancedload', contraer);
    contraer();
})();
