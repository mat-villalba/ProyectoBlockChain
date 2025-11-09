🚀 Aventura Interactiva con Blockchain 🔗

📝 Descripción del Proyecto

Aventura Interactiva con Blockchain es un juego web cooperativo donde la narrativa avanza mediante el consenso democrático de los jugadores. Cada "Capítulo" del juego presenta un dilema, y el grupo debe votar para elegir el camino a seguir, todo dentro de un límite de tiempo.

🌟 La característica estrella de este proyecto es la implementación de una simulación de blockchain privada en C#. Esta blockchain actúa como un registro inmutable y transparente de cada decisión tomada por el grupo, garantizando que la historia de cada partida sea verificable y a prueba de manipulaciones. Una vez que se toma una decisión en un capítulo y se sella en un bloque, ¡no hay vuelta atrás!

El objetivo es navegar a través de los diversos capítulos y alcanzar uno de los múltiples finales posibles. Toda la trayectoria y las decisiones cruciales quedan selladas permanentemente en la cadena de bloques, ofreciendo un historial auditable y fascinante de cada aventura.

✨ Concepto Clave: Blockchain como Testigo Inmutable

En este proyecto, la blockchain no es para criptomonedas, sino para la integridad de la narrativa. Cada bloque sella información vital:

Capítulo: El punto de la historia donde se tomó la decisión.

Opciones: Las alternativas presentadas.

Opción Ganadora: El resultado final de la votación.

Recuento de Votos: La distribución de las preferencias.

Timestamp: El momento exacto en que se selló la decisión.

Esto permite una visualización completamente transparente y auditable del historial de cada partida jugada, celebrando la colaboración del grupo y la integridad de su viaje narrativo.

🛠️ Tecnologías Utilizadas

Backend: ASP.NET Core MVC (🎉 .NET 8 o superior)

Lenguaje: C# (¡El poder del código gestionado!)

Base de Datos: PostgreSQL (🐘 Alojamientos en la nube para trabajo colaborativo)

Acceso a Datos: Entity Framework Core (Enfoque Database First para una integración fluida con la DB existente)

Frontend: Razor Views, HTML, CSS (con 💅 Bootstrap), JavaScript (para interactividad)

Comunicación "Tiempo Real": Mecanismos de polling (consultas periódicas a la API)

Blockchain Core: Implementación 📝 personalizada en C# (hashing SHA-256, encadenamiento de bloques).

🚀 Configuración y Puesta en Marcha

Sigue estos pasos para poner el proyecto en marcha en tu máquina local. ¡Es más sencillo de lo que parece!

📋 Requisitos Previos

SDK de .NET: Asegúrate de tener instalado el SDK de .NET correspondiente (ej. .NET 8).

IDE o Editor: Visual Studio 2022 o Visual Studio Code (¡tu elección!).

Git: Para clonar el repositorio y mantenerte actualizado.

⚙️ Pasos para Iniciar

Clonar el Repositorio: Abre tu terminal y descarga el proyecto:

git clone [URL-DEL-REPOSITORIO] cd [NOMBRE-DEL-REPOSITORIO]

Restaurar Paquetes NuGet: Abre la solución en tu IDE (Visual Studio) o una terminal en la carpeta raíz del proyecto y ejecuta:

dotnet restore

(Nota: Visual Studio suele restaurar paquetes automáticamente al compilar, pero este comando asegura que todo esté en orden).

Base de Datos (Ya Configurada):

La base de datos PostgreSQL está hospedada en la nube (Render), por lo que no necesitas instalar ni configurar una base de datos localmente.

Las clases C# que mapean la base de datos (JuegoDbContext.cs y las entidades) ya han sido generadas y subidas al repositorio (en las carpetas Data/ y Entidades/ de JuegoCaminos_Servicios).

La cadena de conexión a la base de datos de la nube se encuentra actualmente incrustada en el archivo Data/JuegoDbContext.cs (método OnConfiguring). (Mejora Futura: En un proyecto real, por seguridad, esta cadena de conexión debería moverse al archivo appsettings.json y/o a variables de entorno).

Ejecutar la Aplicación:

Desde Visual Studio: Simplemente presiona F5 o el botón ▶️ "Run".

Desde la Terminal: Navega a la carpeta del proyecto JuegoCaminos_WebApp y ejecuta:

dotnet run
