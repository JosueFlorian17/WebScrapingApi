# WebScrapingApi - EY Technical Challenge

Este proyecto es una API REST desarrollada en ASP.NET Core que permite consultar si una entidad figura en listas de alto riesgo, específicamente en la lista de empresas sancionadas del **Banco Mundial**. El proyecto implementa autenticación por API Key, control de tasa de peticiones (rate limiting), y web scraping con HtmlAgilityPack.

---

## Objetivo

Dado el nombre de una entidad, la API realiza una búsqueda automatizada en línea (web scraping) y devuelve:

- El número de coincidencias encontradas (hits)
- Los detalles de cada coincidencia, incluyendo nombre, fecha de sanción, tipo de sanción y fundamentos

---

## Tecnologías Utilizadas

- ASP.NET Core 7.0
- HtmlAgilityPack (para scraping)
- Swagger/OpenAPI (documentación)
- Postman (colección de pruebas)
- Middleware personalizado:
  - API Key Authentication
  - Rate limiting (20 solicitudes por minuto por IP)

---

## Cómo ejecutar el proyecto

1. Clona el repositorio o descomprime el ZIP:
   git clone https://github.com/tuusuario/WebScrapingApi.git
   cd WebScrapingApi

2. Configura la API Key en appsettings.json:
```
   {
     "ApiSettings": {
       "ApiKey": "123456"
     }
   }
```
3. Ejecuta el proyecto:
   dotnet run

4. Accede a Swagger UI en tu navegador:
   https://localhost:5001/swagger

---

## Autenticación

Todas las solicitudes requieren incluir el siguiente encabezado:
```
x-api-key: 123456
```
---

## Endpoint principal
```
GET http://localhost:5001/search?entity=Fraudulent
```
Parámetro:
- entity (obligatorio): nombre de la entidad a buscar


Respuesta:
```
{
  "hits": 2,
  "results": [
    {
      "NameAndAddress": "Example Corp, USA",
      "SanctionDate": "2023-01-15",
      "SanctionType": "Debarment",
      "Grounds": "Fraudulent Practice"
    },
    {
      "NameAndAddress": "Example Corp, Panama",
      "SanctionDate": "2021-08-10",
      "SanctionType": "Conditional Debarment",
      "Grounds": "Fraudulent Practice"
    }
  ]
}
```
---

## 🔄 Rate Limiting

- Límite: 20 solicitudes por minuto por IP
- Excede el límite: se responde con 429 Too Many Requests

---

## ❌ Manejo de errores

Código | Descripción
------ | -----------
400    | Parámetro 'entity' faltante
401    | API Key no proporcionada
403    | API Key inválida
429    | Límite de solicitudes excedido
500    | Error interno del servidor

---

## 🧪 Pruebas y Postman

Se incluye una colección de Postman con pruebas:
- Petición válida
- Petición sin entity
- Petición sin API Key
- Petición con API Key inválida
- Exceso de solicitudes

Puedes importar el archivo WebScrapingApi.postman_collection.json (si lo incluyes).

---

## 🧹 Estructura del proyecto (resumida)

WebScrapingApi/  
├── Controllers/  
├── Middleware/  
├── Services/  
├── Program.cs  
├── appsettings.json  
├── WebScrapingApi.csproj  
└── README.md  

---

## 📌 Notas adicionales

- Actualmente la fuente activa es el World Bank.
- El código está modularizado para permitir integrar otras fuentes como OFAC u Offshore Leaks fácilmente.
- El scraping es sensible a cambios en la estructura del HTML de origen.

---

## 📬 Contacto

Desarrollado por **Josué Florián**  
Este proyecto fue preparado como solución al reto técnico para **EY**.