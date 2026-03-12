# Road Figther 2.5D (Unity + C#)

Prototipo de juego estilo **Road Fighter** para móvil, con pista infinita, control por giroscopio, consumo de combustible y récord por tiempo de supervivencia.

## Mecánicas implementadas

- **Pista vertical infinita** por scrolling de textura (`RoadScroller`).
- **Movimiento lateral por giroscopio** (`PlayerCarController`).
- **Dos velocidades**:
  - Normal
  - Turbo (manteniendo el dedo en pantalla)
- **Consumo de combustible según velocidad** (`GameManager`).
- **Pickups de combustible** (`FuelPickup`).
- **Obstáculos y tráfico aleatorio** (`TrafficSpawner`).
- **Choque con trompo 360° y recuperación** (`PlayerCarController.CrashRoutine`).
- **Game Over por combustible en cero** con guardado de mejor tiempo en `PlayerPrefs`.

## Estructura sugerida de escena

1. Crea una escena principal `Main.unity`.
2. Agrega un plano para carretera con material repetible y asígnalo a `RoadScroller`.
3. Crea `Player` con `Rigidbody`, `Collider`, tag `Player` y script `PlayerCarController`.
4. Crea `GameManager` con script `GameManager` y conecta:
   - `Slider` de combustible
   - `Text` de tiempo actual
   - `Text` de récord
   - Panel de game over
5. Crea un objeto `Spawner` con `TrafficSpawner`, asignando prefabs:
   - Obstáculos (tag `Obstacle`)
   - Tráfico (tag `Traffic`)
   - Pickup de combustible
6. Añade `ForwardDestroyer` a prefabs spawneados para limpiarlos al quedar atrás.

## Configuración móvil

- En `Project Settings > Player`, habilita sensores para giroscopio.
- Ajusta la sensibilidad con `tiltSensitivity`.
- Para pruebas en editor, se usa fallback con `Horizontal` axis.

## Ajustes rápidos de balance

- Más difícil:
  - subir `normalFuelConsumption` y `turboFuelConsumption`
  - bajar `fuelSpawnChance`
  - bajar `spawnEverySeconds`
- Más fácil:
  - subir `fuelAmount` de `FuelPickup`
  - bajar velocidad turbo

## Nota

No se incluyen assets 3D/artísticos en este commit. Solo la base de gameplay y scripts C# para integrar en un proyecto Unity.
