# Pruebas y evidencias

1) Registro de cliente (POST `/api/clientes`)
- Tipo: multipart/form-data
- Campos:
  - CI
  - Nombres
  - Direccion
  - Telefono
  - FotoCasa1 (archivo jpg/png)
  - FotoCasa2
  - FotoCasa3
- Resultado esperado: 201 Created y objeto con CI y Nombres.

2) Subida ZIP (POST `/api/archivos/upload-zip?ci={ci}`)
- Campo form-data: `zipFile` (archivo .zip)
- Resultado esperado: 200 OK con lista de archivos extraídos y `UrlArchivo`.

3) Consulta logs (GET `/api/logs`)
- Resultado esperado: 200 OK con logs almacenados en tabla `LogApis`.

Incluye capturas de Postman/Swagger para cada prueba.
