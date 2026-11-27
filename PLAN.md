# GRUPO 1 - CorporaDress

## 1. Resumen

- **De qué va la aplicación:** Gestión de ropero solidario.
- **Roles de usuario y qué puede hacer cada uno:**

  **Usuario:**
  - Puede crear reservas.

  **Admin:**
  - Controla las donaciones, las reservas, las oficinas y las prendas de ropa.

- **Integrantes:** Joel, Jordi, César y Toni.

## 2. Qué tenemos hecho

Lo que funciona ahora mismo.

### CRUD BÁSICO

**`ClothingItem`**
- **Admin:**
  - Crear prendas.
  - Editar prendas.
  - Eliminar prendas.
  - Validaciones de campos obligatorios: nombre, categoría, sede, materia y tipo de talla.
- **Usuario:**
  - Listar prendas.
  - Consultar el detalle de las prendas.

**`Categories`**
- **Anónimo y usuario:**
  - Listar y consultar las categorías.
- **Admin:**
  - Crear categorías, con el nombre como campo obligatorio.
  - Editar categorías.
  - Eliminar categorías.

**`Donations`**
- **Admin:**
  - Consultar donaciones.
  - Crear donaciones.
  - Editar donaciones.
  - Utilizar los filtros del listado.

**`Location`**
- **Anónimo y usuario:**
  - Consultar las oficinas.
  - Ver el detalle de las oficinas.
- **Admin:**
  - Crear oficinas, con nombre, ciudad y dirección como campos obligatorios.

**`Reservation`**
- **Admin:**
  - Gestión básica de las reservas.
- **Usuario:**
  - Dispone de la sección de reservas, aunque todavía necesita completar algunos aspectos de visualización y gestión.

### Pendiente de los CRUD

**`ClothingItem`**
- Validar que el tipo de talla concuerde con la talla.
- Traducir los mensajes de error al español.
- Cambiar la tabla por tarjetas y añadir filtros.

**`Categories`**
- Añadir ViewBag.
- Aplicar Trim en la búsqueda por nombre.
- Quitar el filtro por cantidad.
- Mejorar la navegación entre categorías y prendas.

**`Donations`**
- Cambiar los botones para disponer de las opciones de editar y eliminar.
- Permitir volver a la donación después de crear una prenda.

**`Location`**
- Añadir ViewBag y Trim.
- Convertir la ciudad en un desplegable.
- Añadir botones de editar y eliminar.
- Cargar una imagen distinta para cada oficina.
- Añadir validaciones mediante expresiones regulares para el código postal y el número de teléfono.
- Sustituir el listado de ropa por un listado de categorías.
- Permitir acceder desde una categoría al listado de ropa filtrado por categoría y oficina.
- Mostrar las categorías en una cuadrícula.

**`Reservation`**
- Corregir la visualización de las prendas en la tabla del Index.
- Revisar los botones de ver y editar.
- Añadir filtros.
- Sustituir las opciones de editar y eliminar por cancelar reserva.
- Añadir un resumen de reservas permitidas debajo del título de RESERVAS.
- Añadir `Authorize`.
- Excluir al administrador de la lista de usuarios.
- En el formulario de creación, cargar el listado de usuarios correspondiente.
- Traducir los mensajes de error al español.
- Dejar la edición de reservas para más adelante.

## 3. Errores y cosas a medias

### Errores

- Error al arrancar debido a...

### Cosas a medias

- Completar los datos demo.
- Funcionalidad para marcar como atrasado.

## 4. Qué falta de lógica de negocio para MVP

### Funcionalidades de reservas

- **Reserva de prendas:** permitir que un usuario reserve una prenda disponible, respetando el límite de reservas activas.
- **Cancelación de una reserva:** permitir cancelar una reserva y devolver la prenda a disponible cuando corresponda.
- **Recogida de prenda:** permitir registrar que el usuario ha recogido la prenda.
- **Caducidad de reservas vencidas:** cambiar el estado de las reservas vencidas y liberar las prendas correspondientes.
- **Eliminación de reservas:** permitir al administrador eliminar reservas y liberar la prenda si la reserva continúa activa.

### Funcionalidades de interfaz relacionadas

- Añadir un botón para reservar desde el detalle de una prenda.
- Añadir botones de gestión en el detalle de las reservas.
- Añadir un botón para caducar reservas vencidas.

## 5. Qué más nos falta

- Validaciones.
- Permisos.
- Diseño.
- Datos de prueba.
- README.
- Añadir imágenes a las categorías.

## 6. Reparto por persona

### Joel

- Entidad Reservation.
- Controlador y vistas de Reservation.
- Caducidad de reservas vencidas.
- Botón para caducar reservas vencidas.

### Jordi

- Funcionalidad ClothingItem.
- Entidad Category.
- Controlador y vistas de ClothingItem.
- Reserva de prendas.

### César

- Entidad Location.
- Controlador y vistas de Category.
- Controlador y vistas de Location.
- Recogida de prendas.
- Imágenes del proyecto.

### Toni

- Regenerar la migración inicial.
- Entidad Donation.
- Controlador y vistas de Donation.
- README final y diagrama actualizado.
- Cancelación de reservas.
- Eliminación de reservas.
- Botones de gestión en el detalle de reservas.
- Botón de reserva en el detalle de prendas.

### Trabajo conjunto

- Presentación y demo.

## 7. Si sobra tiempo

- Ideas tras terminar el MVP.
- Revisar la posibilidad de editar reservas.
- Que las reservas caduquen automaticamente

## 8. Dudas, ruegos y preguntas

## 9. Correcciones del mentor

presentaar el equipo dar paso con nombre  
evitar el tema de la historia, presentar la problematica sin nombrar
crear un dato minimo flujo crear editar elimar
el orden de presentacion, de mas grande a mas pequeño primero sedes -> donaciones -> categorias -> vestuario -> reservas
Explicar mas lo de expirar  -> cambiar el nombre a "liberar caducadas" o simil
colocar los estados de colorinchis
primero admin y luego usuario, hacerlo todo de una, tener dos ventanas abiertas, una con admin y otra con usuario
cambiar el nombre del usuario y el de admin para darle mas personalidad 
añadir review a las reservas, aun que sean de mentirilla, decir que se plantea a futuro

hacer el scroll de la pagina de inicio
eneseñar el modo claro y modo oscuro