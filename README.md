
# Project Aftermath — Actividad 4.1

## Implementación de físicas, colisiones y movimiento básico en Unity 2D

**Materia:** Interactividad y Videojuegos 2D  
**Universidad:** CESUN Universidad  
**Motor:** Unity 6.3 LTS  
**Proyecto:** Project Aftermath

## Descripción

Práctica de implementación de un personaje 2D con físicas, detección de suelo, movimiento horizontal, cambio de orientación y salto.

## Funcionalidades desarrolladas

- Movimiento horizontal mediante teclado.
- Orientación visual según la dirección del personaje.
- Salto mediante Rigidbody2D.
- Aplicación de gravedad.
- Colisiones con el suelo.
- Detección de suelo para evitar el doble salto.
- Integración de componentes de animación.

## Archivos principales

- `Assets/Scripts/PlayerMovement.cs` — lógica del personaje.
- `Assets/Scenes/Actividad4_1.unity` — escena de la práctica.
- `Assets/GameAssets/Player/` — archivos propios de animación.
- `Packages/` — dependencias de Unity.
- `ProjectSettings/` — configuración del proyecto.

## Recursos externos

El personaje utiliza Dragon Warrior (Free), de Kin Ng, disponible en Unity Asset Store:

https://assetstore.unity.com/packages/2d/characters/dragon-warrior-free-93896

Los gráficos originales no están incluidos en este repositorio. Para visualizar correctamente la escena es necesario disponer de esos recursos y conservar sus referencias originales de Unity.

## Validaciones realizadas

Se comprobaron las siguientes funciones:

1. Aplicación de gravedad.
2. Colisión con el suelo.
3. Desplazamiento hacia la izquierda.
4. Desplazamiento hacia la derecha.
5. Cambio de orientación.
6. Salto sin permitir un segundo salto en el aire.

## Autor

Miguel Ángel Miranda Jaramillo  
Ingeniería en Desarrollo de Software  
CESUN Universidad
