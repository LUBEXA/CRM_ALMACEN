// Descarga un archivo generado en el servidor (recibido como base64) desde el navegador.
window.descargarArchivo = (nombre, contenidoBase64, tipo) => {
    const link = document.createElement('a');
    link.href = `data:${tipo};base64,${contenidoBase64}`;
    link.download = nombre;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
