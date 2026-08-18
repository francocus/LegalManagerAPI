# Minuta de Relevamiento

**Proyecto:** Legal Manager (Sistema de Gestión para Estudio Jurídico)
**Fecha:** 18 de Agosto de 2026
**Equipo de Desarrollo:** Agustín Angelini, Franco Cuscianna, Thiago Cuscianna

## 1. Temática de la Aplicación

El proyecto consiste en el desarrollo de un sistema informático orientado a la administración y gestión operativa de un estudio jurídico. El objetivo principal de la aplicación es digitalizar y centralizar la información de los casos legales (expedientes) y optimizar la agenda de los profesionales mediante un sistema de turnos interactivo. La plataforma servirá como un nexo de comunicación estructurado entre los abogados del estudio y sus respectivos clientes, garantizando la seguridad en el acceso a la información según el rol de cada usuario.

## 2. Roles de Usuario Identificados

El sistema contará con autenticación (basada en JWT) y control de acceso estructurado en tres roles principales:

- **Administrador del Sistema (Sysadmin):** Encargado de la configuración general y la administración del personal y usuarios.
- **Abogado:** Profesional del estudio encargado de llevar adelante los casos y atender las consultas.
- **Cliente:** Usuario final que requiere los servicios legales del estudio y necesita dar seguimiento a su situación.

## 3. Funcionalidades Principales (Alcance del Sistema)

A continuación, se detallan las funcionalidades con las que debe cumplir el sistema, categorizadas por módulo o actor:

### A. Módulo de Gestión de Usuarios (Sysadmin)

- **Administración (CRUD):** El administrador deberá poder crear, leer, actualizar y dar de baja cuentas de usuario mediante baja lógica (desactivación), sin eliminarlas de forma permanente.
- **Asignación de Roles:** Capacidad de designar a los usuarios registrados como Administradores, Abogados o Clientes, restringiendo sus vistas y permisos en el sistema.

### B. Módulo de Gestión de Expedientes (Casos Legales)

- **Seguimiento de Casos:** El sistema debe permitir el registro (Alta, Baja y Modificación) de los expedientes legales.
- **Vinculación:** Cada expediente debe estar obligatoriamente vinculado al Abogado que lo gestiona y al Cliente al que le pertenece.
- **Portal del Cliente:** Los clientes deberán tener una vista exclusiva donde puedan consultar el estado actual y seguimiento de sus casos activos.
- **Panel del Abogado:** Los abogados deberán poder listar, revisar y actualizar el estado de los múltiples expedientes que tengan asignados.

### C. Módulo de Agenda y Turnos

- **Calendario Interactivo:** La aplicación deberá incluir una interfaz de calendario dinámica para la gestión visual de las citas.
- **Solicitud de Turnos:** El sistema debe permitir a los clientes solicitar turnos con los abogados por diversos motivos.
- **Gestión de Agenda:** Los abogados deberán poder visualizar su calendario de turnos asignados, gestionar su disponibilidad y hacer seguimiento de sus próximas citas.

### D. Funcionalidades Transversales (UX/UI y Seguridad)

- **Control de Acceso:** Login seguro para todos los usuarios.
- **Accesibilidad Visual:** Inclusión de un interruptor para alternar la interfaz entre "Modo Claro" y "Modo Oscuro" (Light/Dark Theme), mejorando la experiencia de uso.
- **Baja Lógica:** Todas las entidades del sistema (Usuarios, Expedientes, Turnos) utilizan borrado lógico. Ninguna acción de "eliminar" borra el registro de forma permanente; únicamente lo marca como inactivo, preservando el historial completo.

## 4. Anexo: Diagrama de Clases Conceptual

Como respaldo al relevamiento, se presenta el modelo conceptual de dominio sin componentes de bases de datos ni métodos, reflejando las entidades y relaciones detectadas en las funcionalidades requeridas.

![Diagrama de clases conceptual](./diagrama-clases.png)
