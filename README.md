# Riad Figther / Road Fighter 2.5D (Unity + C#)

Prototype de conducción arcade en **Unity 3D** inspirado en **Road Fighter**, adaptado a una cámara 2.5D con pista larga, tráfico dinámico, consumo de combustible y progresión de dificultad.

Este proyecto ya no es solo una base de scrolling vertical: actualmente incluye un bucle principal jugable con aceleración normal/turbo, tráfico y obstáculos por carriles, pickups de combustible, penalización por choques, distancia recorrida, récord persistente y reinicio rápido. El objetivo de este README es dejar claro **qué existe hoy**, **cómo se conecta en la escena** y **cómo extenderlo** con sistemas adicionales como HUD, puntaje avanzado, game over más robusto, combustible, tráfico y otras capas de gameplay.

---

## 1. Estado actual del juego

### Bucle jugable implementado

1. El coche del jugador avanza constantemente.
2. El usuario gira usando **giroscopio** en móvil o el eje `Horizontal` en editor/PC.
3. El usuario puede activar **turbo** manteniendo el dedo en pantalla o usando `Space` / `Shift`.
4. El combustible disminuye con el tiempo y se consume más rápido en turbo.
5. Se generan oleadas de tráfico, obstáculos y pickups de combustible delante del jugador.
6. Si el jugador choca, entra en una animación de trompo, pierde combustible y queda temporalmente invulnerable.
7. Mientras sobrevive, se acumulan **tiempo**, **distancia** y una **dificultad progresiva**.
8. Cuando el combustible llega a cero, se dispara el **game over** y se guardan récords en `PlayerPrefs`.

### Mecánicas que ya existen

- **Movimiento hacia adelante con velocidad interpolada**.
- **Modo Cruise / Turbo**.
- **Steering con giroscopio o fallback de teclado**.
- **Penalización por choque** con consumo de combustible.
- **Invulnerabilidad temporal tras impacto**.
- **Tráfico móvil y obstáculos estáticos**.
- **Spawn por carriles** con control de probabilidades.
- **Pickup de combustible**.
- **Contador de tiempo**.
- **Contador de distancia**.
- **Récord persistente de tiempo y distancia**.
- **Escalado automático de dificultad**.
- **Reinicio rápido** después de perder.
- **Script de editor** para montar una escena base más rápido.

---

## 2. Scripts principales del proyecto

### `Assets/Scripts/GameManager.cs`
Controla el estado general de la partida.

**Responsabilidades actuales:**
- Gestiona combustible máximo, consumo normal, consumo turbo y penalización por choque.
- Lleva el tiempo de supervivencia.
- Calcula la distancia recorrida.
- Expone `DifficultyMultiplier` para endurecer el juego con el tiempo.
- Guarda récords de tiempo y distancia usando `PlayerPrefs`.
- Actualiza la UI principal.
- Activa el panel de game over.
- Reinicia la escena actual.

### `Assets/Scripts/PlayerCarController.cs`
Controla el vehículo del jugador.

**Responsabilidades actuales:**
- Lee input de giro por giroscopio o teclado.
- Activa/desactiva turbo.
- Interpola la velocidad hacia adelante.
- Aplica rotación al coche.
- Usa `Rigidbody` para el desplazamiento físico.
- Detecta colisiones por trigger con `Obstacle` y `Traffic`.
- Lanza la rutina de choque con giro de 360°.

### `Assets/Scripts/TrafficSpawner.cs`
Genera el contenido jugable delante del jugador.

**Responsabilidades actuales:**
- Crea oleadas de elementos por carril.
- Controla frecuencia de spawn.
- Decide cuándo aparece combustible.
- Alterna entre obstáculos y tráfico.
- Ajusta la presión del juego con la dificultad.
- Asigna `TrafficMover` a tráfico dinámico.
- Añade `ForwardDestroyer` para limpiar objetos ya superados.

### `Assets/Scripts/TrafficMover.cs`
Mueve algunos vehículos de tráfico de manera autónoma.

### `Assets/Scripts/FuelPickup.cs`
Recarga combustible al tocar el objeto con el jugador.

### `Assets/Scripts/RoadScroller.cs`
Desplaza la textura de la carretera según la velocidad actual del coche.

### `Assets/Scripts/ForwardDestroyer.cs`
Destruye objetos que quedaron detrás del jugador para evitar acumular basura en escena.

### `Assets/Scripts/CameraFollow3D.cs`
Hace seguimiento suave del coche usando offset local y rotación interpolada.

### `Assets/Editor/SetupRoadFighter.cs`
Herramienta de editor para crear/configurar rápidamente una escena base con coche, cámara, `GameManager` y `TrafficSpawner`.

---

## 3. Estructura recomendada de escena

Crea una escena principal, por ejemplo `Main.unity`, con esta jerarquía sugerida:

```text
Main Scene
├── Directional Light
├── Main Camera
├── EventSystem                (si usas UI)
├── Canvas
│   ├── FuelSlider
│   ├── TimerText
│   ├── BestTimeText
│   ├── DistanceText           (opcional pero recomendado)
│   ├── StatusText             (opcional pero recomendado)
│   └── GameOverPanel
├── GameManager
├── TrafficSpawner
├── Player
├── Road / Track Meshes
└── Spawned Runtime Objects
```

### Configuración mínima del jugador

El `Player` debe tener:

- `Tag = Player`
- `Rigidbody`
- `Collider`
- `PlayerCarController`

### Configuración mínima del `GameManager`

Asigna desde el inspector:

- `fuelSlider`
- `timerText`
- `bestTimeText`
- `distanceText` (recomendado)
- `statusText` (recomendado)
- `gameOverPanel`

### Configuración mínima del `TrafficSpawner`

Asigna:

- `player`
- `obstaclePrefabs[]`
- `trafficPrefabs[]`
- `fuelPickupPrefab`

### Configuración mínima de la carretera

Si usas una carretera con material repetible:

- añade `RoadScroller`
- asigna `roadRenderer`
- ajusta `textureTilingSpeedMultiplier`

---

## 4. Controles actuales

### Móvil
- **Girar:** inclinando el dispositivo.
- **Turbo:** manteniendo un dedo sobre la pantalla.
- **Reintentar tras perder:** tocar la pantalla.

### Editor / PC
- **Girar:** `A / D`, flechas o eje `Horizontal`.
- **Turbo:** `Space`, `Left Shift` o `Right Shift`.
- **Reintentar tras perder:** `R`.

---

## 5. Cómo probar el proyecto rápidamente

## Opción A: montar a mano

1. Abre el proyecto en Unity.
2. Carga la escena de tu pista o crea una nueva.
3. Añade el coche del jugador con `PlayerCarController`.
4. Añade `GameManager` a un objeto vacío.
5. Añade `TrafficSpawner` a otro objeto vacío.
6. Crea la UI con slider, textos y panel de game over.
7. Asigna prefabs de tráfico, obstáculos y combustible.
8. Ejecuta la escena y ajusta valores desde el inspector.

## Opción B: usar la herramienta del editor

En Unity, ejecuta:

`Tools > Configurar Road Fighter`

Esta utilidad intenta:
- instanciar un coche base,
- añadirle `PlayerCarController`, `Rigidbody` y `Collider`,
- detectar la pista,
- ajustar anchura utilizable y carriles,
- configurar cámara,
- crear `GameManager` si no existe,
- crear `TrafficSpawner` si no existe,
- cargar algunos prefabs de tráfico por defecto.

> Importante: esta utilidad acelera la preparación de una demo, pero normalmente tendrás que revisar referencias, UI y balance manualmente.

---

## 6. Jugabilidad actual explicada sistema por sistema

## 6.1 Combustible

### Qué hace hoy
- El combustible inicia en `maxFuel`.
- Baja automáticamente cada frame.
- Consume una tasa distinta en modo normal y turbo.
- Al chocar, además del trompo, pierde una cantidad fija adicional.
- Los pickups restauran combustible.
- Si llega a cero, termina la partida.

### Variables principales
En `GameManager`:
- `maxFuel`
- `normalFuelConsumption`
- `turboFuelConsumption`
- `crashFuelPenalty`

En `FuelPickup`:
- `fuelAmount`

### Balance recomendado
- Si quieres un juego más arcade y permisivo:
  - baja `normalFuelConsumption`
  - baja `turboFuelConsumption`
  - sube `fuelAmount`
  - sube `fuelSpawnChance`

- Si quieres un juego más tenso:
  - sube el consumo turbo
  - reduce la aparición de pickups
  - aumenta la penalización por choque

## 6.2 Movimiento del jugador

### Qué hace hoy
- El coche siempre intenta avanzar.
- La velocidad cambia gradualmente por `Mathf.MoveTowards`.
- El jugador no cambia de carril con snap: **gira físicamente** y avanza en la dirección hacia la que apunta el coche.
- En choque, la velocidad efectiva baja con `crashSlowdownMultiplier`.

### Variables principales
En `PlayerCarController`:
- `normalSpeed`
- `turboSpeed`
- `acceleration`
- `lateralSpeed`
- `tiltSensitivity`
- `steeringSmoothing`
- `visualTiltAngle`

> Nota: actualmente `laneWidth`, `maxHorizontalOffset`, `steeringSmoothing` y `visualTiltAngle` no están explotados del todo por el controlador actual. Son buenos candidatos para una siguiente iteración.

## 6.3 Tráfico y obstáculos

### Qué hace hoy
- El spawner genera oleadas delante del jugador.
- Cada oleada usa carriles no repetidos.
- Puede colocar combustible si el jugador va con poco tanque.
- Puede generar obstáculos estáticos o tráfico móvil.
- La dificultad reduce el tiempo efectivo entre oleadas.

### Variables principales
En `TrafficSpawner`:
- `spawnEverySeconds`
- `laneWidth`
- `laneCount`
- `maxElementsPerWave`
- `minSpawnAhead`
- `maxSpawnAhead`
- `fuelSpawnChance`
- `obstacleChance`
- `trafficMovingChance`
- `minTrafficSpeed`
- `maxTrafficSpeed`

## 6.4 Puntaje, distancia y progreso

### Qué hace hoy
- Se registra tiempo de supervivencia.
- Se acumula distancia a partir de la velocidad hacia adelante.
- Se guarda el mejor tiempo y la mejor distancia.
- La dificultad aumenta según el tiempo vivo.

### Variables principales
En `GameManager`:
- `distanceScoreMultiplier`
- `bestTimeKey`
- `bestDistanceKey`

## 6.5 Game Over

### Qué hace hoy
- El game over se dispara únicamente cuando el combustible llega a cero.
- Guarda récords.
- Activa un panel de UI.
- Permite reiniciar con `R` o con toque.

> Aunque los choques no finalizan la partida por sí solos, sí empujan al jugador hacia la derrota al consumir combustible y cortar el ritmo.

---

## 7. Guía detallada para extender funciones faltantes o incompletas

A continuación se describen implementaciones recomendadas para ampliar el juego sin romper la arquitectura actual.

## 7.1 Mejorar el sistema de combustible

### Objetivo
Convertir el combustible en un sistema más legible, más justo y con más profundidad estratégica.

### Mejoras recomendadas

#### A. Separar consumo base y consumo por acciones
Ahora mismo el consumo se calcula solo por modo normal/turbo. Puedes mejorarlo así:

- consumo base por tiempo,
- consumo extra por turbo,
- consumo extra por rozar paredes o salir de pista,
- consumo reducido si el jugador desacelera.

**Idea de implementación:**
- Crear un método como `GetCurrentFuelConsumption()` en `GameManager`.
- Hacer que devuelva la suma de varios factores.
- Mostrar en UI el estado del consumo actual.

#### B. Niveles visuales de combustible
Agrega retroalimentación visual:
- barra verde > 60%
- amarilla entre 30% y 60%
- roja < 30%
- parpadeo < 10%

**Implementación sugerida:**
- Cambiar el `fill` del `Slider` desde `UpdateFuelUI()`.
- Disparar una animación o `CanvasGroup` cuando quede poco combustible.

#### C. Pickups con valores distintos
Crea varias clases de pickup:
- pequeño (+10)
- mediano (+20)
- grande (+35)
- especial que llene todo pero aparezca muy rara vez.

**Implementación sugerida:**
- Duplicar prefab y modificar `fuelAmount`.
- O crear una enum `FuelPickupType` para variar material/color/cantidad.

#### D. Consumo por terreno
Si la pista tiene nieve, grava o césped:
- detecta el terreno en el que está el coche,
- aumenta consumo y reduce velocidad fuera de carretera.

**Implementación sugerida:**
- usar `OnTriggerStay` con zonas marcadas como `OffRoad`, o
- raycast hacia abajo para detectar material/capa.

---

## 7.2 Mejorar el tráfico

### Objetivo
Hacer que el tráfico se sienta menos aleatorio y más “jugable”.

### Mejoras recomendadas

#### A. Perfiles de tráfico
Define perfiles distintos:
- coche lento,
- coche rápido,
- camión ancho,
- autobús resistente visualmente,
- tráfico zigzagueante ocasional.

**Implementación sugerida:**
- crear un script `TrafficProfile` o `ScriptableObject`,
- definir velocidad, ancho, peso visual, probabilidad y comportamiento.

#### B. Spawn contextual
Ahora el sistema usa probabilidades generales. Puedes añadir reglas como:
- no spawnear tráfico justo encima de un pickup,
- dejar siempre al menos un carril razonablemente libre en dificultades bajas,
- usar patrones reconocibles a dificultad alta.

**Patrones recomendados:**
- muro parcial (2 carriles ocupados),
- carril libre central,
- pickup cebado con peligro lateral,
- tráfico rápido detrás de obstáculo.

#### C. Tráfico reactivo
Haz que algunos vehículos:
- cambien ligeramente de carril,
- aceleren si el jugador está cerca,
- frenen si tienen otro coche delante,
- usen luces o feedback de amenaza.

**Implementación sugerida:**
- ampliar `TrafficMover` con estados simples (`Cruise`, `Overtake`, `Avoid`, `Escape`).
- usar raycasts frontales cortos para evitar superposición entre NPCs.

#### D. Pools de objetos
Si el juego va a móvil, evita `Instantiate/Destroy` excesivo.

**Implementación sugerida:**
- crear un `PoolManager` por tipo de prefab,
- pedir objetos al pool en `TrafficSpawner`,
- devolverlos en `ForwardDestroyer` en vez de destruirlos.

---

## 7.3 Mejorar el puntaje

### Objetivo
Pasar de “tiempo y distancia” a un sistema de score más arcade.

### Propuesta de score compuesto
Puedes definir una fórmula como:

```text
score = distancia
      + tiempo * 10
      + adelantamientos * 25
      + pickupsTomados * 15
      + bonusTurbo
      - choques * 40
```

### Variables nuevas recomendadas
En `GameManager`:
- `CurrentScore`
- `NearMissCount`
- `OvertakeCount`
- `CrashCount`
- `FuelCollectedCount`
- `BestScore`

### Cómo implementarlo

#### A. Score por distancia
Ya tienes base con `RegisterTravel()`. Solo separa:
- `distanceTravelled` para HUD físico,
- `score` para lógica de ranking.

#### B. Bonus por turbo sostenido
Da puntos si el jugador se arriesga:
- sumar score mientras el turbo esté activo,
- multiplicar si además tiene combustible crítico.

#### C. Bonus por adelantamientos
Cuando el jugador deje atrás un coche de tráfico a poca distancia lateral, suma bonus.

**Implementación sugerida:**
- añade un trigger al tráfico o usa una comparación de producto punto,
- registra si el objeto pasó de “delante” a “detrás” sin choque.

#### D. Near miss
Otorga puntos por esquivar un coche muy cerca sin tocarlo.

**Implementación sugerida:**
- esfera o trigger de proximidad en el jugador,
- comprobar duración corta y ausencia de colisión.

#### E. Multiplicador de combo
Haz que jugar limpio suba un multiplicador:
- combo sube con overtakes, turbo limpio o near misses,
- combo baja al chocar.

---

## 7.4 Mejorar el game over

### Objetivo
Transformar el final de partida en una pantalla de resultados clara y reutilizable.

### Qué le falta hoy
Actualmente el game over:
- muestra panel,
- guarda récords,
- reinicia.

Pero todavía no:
- distingue motivo de derrota,
- muestra estadísticas detalladas,
- congela audio/FX de manera controlada,
- ofrece flujo de menú o reintento avanzado.

### Mejoras recomendadas

#### A. Motivos de derrota
Crea una enum:
- `OutOfFuel`
- `CrashTotal`
- `TimeLimit` (si luego agregas modos)
- `FellOffTrack`

**Implementación sugerida:**
- cambiar `TriggerGameOver()` por `TriggerGameOver(GameOverReason reason)`.
- mostrar mensaje contextual en UI.

#### B. Resumen de partida
Muestra en el panel:
- tiempo total,
- distancia,
- score,
- choques,
- combustible recogido,
- mejor marca superada o no.

#### C. Estados de pausa/postpartida
Añade:
- `isPaused`
- `isRunActive`
- `isRunFinished`

Esto ayuda mucho cuando luego agregues:
- countdown inicial,
- pausa real,
- cinemática corta,
- transición a menú.

#### D. Reinicio más limpio
Ahora el reinicio recarga la escena. Funciona, pero si crece el proyecto podrías:
- separar escena de gameplay y escena de menú,
- usar un `RunSessionManager`,
- restaurar pools y HUD sin recargar todo.

---

## 7.5 UI / HUD recomendados

### HUD actual recomendado
- Barra de combustible.
- Tiempo.
- Récord.
- Distancia.
- Estado (`Cruise` / `Turbo` + dificultad).

### HUD futuro recomendado
- Score total.
- Multiplicador combo.
- Indicador de combustible crítico.
- Icono de turbo activo.
- Indicador de mini objetivo o misión.
- Mensajes flotantes: `+Fuel`, `Near Miss`, `Combo x2`, `Turbo Bonus`.

### Implementación sugerida
Crear un `HUDController` dedicado para no sobrecargar `GameManager` con demasiada UI.

**Responsabilidad ideal de `HUDController`:**
- escuchar cambios del juego,
- actualizar textos y barras,
- lanzar animaciones,
- gestionar paneles de pausa y game over.

---

## 7.6 Dificultad progresiva avanzada

### Qué existe hoy
`DifficultyMultiplier = 1f + Mathf.Clamp(survivalTime / 45f, 0f, 1.5f)`

Eso significa que la dificultad crece con el tiempo hasta cierto límite.

### Cómo mejorarla

#### A. Curvas en lugar de fórmula fija
Usa `AnimationCurve` para controlar:
- frecuencia de spawn,
- velocidad del tráfico,
- probabilidad de obstáculos,
- rareza de combustible.

#### B. Capas de dificultad
Por ejemplo:
- **0–30 s:** aprendizaje,
- **30–60 s:** tráfico medio,
- **60–90 s:** menos combustible,
- **90+ s:** oleadas densas y tráfico rápido.

#### C. Director de eventos
Añade un `DifficultyDirector` que active eventos temporales:
- lluvia de tráfico,
- zona escasa de combustible,
- tramo de camiones,
- tramo rápido con bonus de score.

---

## 7.7 Sonido y feedback

### Recomendado añadir
- sonido de motor normal,
- capa extra para turbo,
- impacto al chocar,
- pickup de combustible,
- alerta de combustible bajo,
- sonido de game over.

### Feedback visual recomendado
- sacudida de cámara en choque,
- destello o blink durante invulnerabilidad,
- partículas al recoger combustible,
- trails o blur ligero en turbo.

---

## 7.8 Modos de juego que encajan con la base actual

La arquitectura actual permite añadir con relativa facilidad:

### Modo Supervivencia
El actual. Aguantar lo máximo posible.

### Modo Distancia Objetivo
Ganas al llegar a una distancia concreta.

### Modo Contrarreloj
Empiezas con poco tiempo y ganas segundos por pickups o metas intermedias.

### Modo Sin Choques
Pierdes al primer impacto o al tercer impacto.

### Modo Combustible Extremo
El combustible manda sobre todo el diseño; menos pickups y más gestión.

---

## 8. Plan técnico recomendado para la siguiente iteración

Si quieres evolucionar el proyecto sin reescribirlo todo, este es un buen orden:

### Fase 1: claridad de sistemas
1. Separar HUD de `GameManager`.
2. Añadir score real además de distancia/tiempo.
3. Añadir motivo de game over.
4. Añadir resumen de partida.

### Fase 2: tráfico y contenido
5. Añadir pools de tráfico/obstáculos.
6. Crear patrones de spawn.
7. Crear perfiles de vehículos.
8. Añadir near miss y overtakes.

### Fase 3: profundidad arcade
9. Añadir combo/multiplicador.
10. Añadir audio/FX.
11. Añadir fuera de pista con penalizaciones.
12. Añadir modos de juego.

### Fase 4: producción móvil
13. Optimizar draw calls y pooling.
14. Ajustar colliders y capas.
15. Revisar UI responsive.
16. Ajustar input y sensibilidad por dispositivo.

---

## 9. Consejos de implementación por sistema

### Para mantener el código limpio
- Deja `GameManager` como orquestador de estado, no como “script que hace todo”.
- Mueve lógica visual a `HUDController`.
- Mueve fórmulas de balance a `ScriptableObject` si crece el número de parámetros.
- Evita meter lógica compleja de tráfico dentro del spawner: usa scripts por prefab o perfiles.

### Para evitar bugs comunes
- Asegúrate de que el jugador tenga el tag `Player`.
- Verifica que tráfico y obstáculos tengan `Collider` y tags correctos.
- Si los triggers no funcionan, revisa `isTrigger`, capas y el `Rigidbody` del jugador.
- Si el giro por móvil va invertido, ajusta el signo en `GetSteeringInput()`.
- Si el coche derrapa o rebota, congela más constraints o revisa masa/fricción.

### Para balancear mejor
- Primero fija la velocidad base.
- Después ajusta el ancho de carril.
- Luego frecuencia de spawn.
- Al final ajusta consumo de combustible.

Ese orden evita que un cambio tape el problema real.

---

## 10. Limitaciones actuales conocidas

- El game over solo depende del combustible.
- No hay sistema de score compuesto todavía.
- No existe pooling de objetos.
- No hay audio ni VFX dedicados.
- El tráfico usa movimiento simple; no tiene IA real.
- El controlador del coche conserva variables listas para futuras mejoras pero no todas se usan plenamente.
- La UI depende directamente del `GameManager`.

---

## 11. Requisitos y herramientas

- **Unity** (versión no especificada en el repo; recomendable abrirlo con una versión LTS moderna compatible con los assets).
- **C#** para scripts de gameplay.
- Proyecto pensado para pruebas en **móvil**, pero con fallback funcional en editor.

---

## 12. Resumen ejecutivo

Este repositorio ya contiene una base jugable bastante completa para un **arcade de conducción infinita**:

- conducción continua,
- turbo,
- combustible,
- tráfico,
- obstáculos,
- pickups,
- choques con penalización,
- dificultad progresiva,
- distancia,
- récords,
- game over.

La siguiente evolución natural no es rehacer el proyecto, sino **profundizar sus sistemas**: score, HUD, patrones de tráfico, pooling, feedback audiovisual, razones de derrota, modos de juego y balance fino para móvil.

Si mantienes esa dirección, este prototipo puede crecer muy bien hacia un vertical slice sólido o incluso una versión publicable en móvil.
