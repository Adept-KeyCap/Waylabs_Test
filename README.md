# Prueba Técnica - Waylabs
A continuación les presento la documentación de este pequeño proyecto, donde está explicado punto por punto los requerimientos de la prueba, el cómo se implementó y que decisiones creativas tomé al interpretar los mismos.

### 0 - Base del juego
### 1 - Objetos básicos
### 2 - Armas
### 3 - Munición
### 4 - Inventario
### 5 - Escenas
### 6 - Enemigos 
### 7 - Feedback
### 8 - UI
### 9 - Añadidos
##

## Base del juego
#### Movimiento
Decidí tomar la ruta del movimiento basádo en físicas ya que sabía que más adelante iba a implementar varios objetos con los que se comportarían con físicas y quería que estos objetos, fueran los protagonistas principales de la Demo.
Implementé el new Input System de Unity. Se puede mover con **WASD** ``` PlayerMovement.Move() ``` y **SHIFT** para correr ``` PlayerMovement.OnSprint(InputAction.CallbackContext context) ```

#### Cámara
Implementé un sistema diferente a las cámaras en primera persona convencionales, ya que quería darle cierta ambietación al estilo de [BODYCAM](https://youtu.be/bL-TWFgJpIw?si=2v-roGyF3kvR0L8v) o [Puck](https://youtu.be/GA2iLu3Heek?si=a4x7oO9h05XwOIEO), pero siendo más arcade.
Se aprovecha de algunas funcionalidades adicionales del paquete para Unity [Cinemachine](https://unity.com/features/cinemachine), creando una zona "Libre" donde el punto de mira se puede mover libremente, pero cuando el cursor llega a unos límites establecidos, empieza a girar lentamente la cámara, entre más se salga del límite, más rápido va a girar la cámara.

![GIF de movimiento de cámara.](DocResources/pt2_Camera.gif)

Para crear el punto de mira se usa el valo de la **posición del Mouse** para ubicar un **Crosshair**, este se usa para poder interactuar con los objetos por medio de un ``` Raycast ``` desde la cámara hasta la posición del mundo donde se encuentra apuntando el cursor. A lo que nos lleva a el siguiente punto.



## Objetos Básicos
#### Recoger y soltar objetos
Los objetos se recogen pulsando la **Tecla E** cuando están señalados por un delineado, antes de que pase esto, se debe apuntar con el **Crosshair** encima del objeto. Una vez tengas un objeto equipado en tu mano puedes presionar una vez a la **Tecla Q** para soltar el objeto, sin embago si en vez de darle un solo toque, la manteienes presionada, el objeto será lanzado con una fuerza que depende del peso del objeto y el tiempo que la presiones.

![GIF de Recoger objetos.](DocResources/pt4_ItemsPickUp.gif)

Hay 5 objetos de los cuales se puede interactuar, los 3 primeros son la caja de madera, el ladrillo y la pelota de baloncesto, estos 3 reaccionan de manera diferente a las físicas debido a sus diferntes propiedades como la masa, la fricción y su elastícidad.

![Interactables.](DocResources/Interactables.png)

> [!TIP]
> ¡Puedes lanzar objetos contra los enemigos! el daño dependerá del peso y la velocidad con la que se lance el objeto

## Armas
Tu **Crosshair** cambiará para poder operarlas con precisión. Tenemos 2 armas, **la pistola Semi-Automática**, solo se dispara una vez al presionarse el **Click Izquierdo**. También hay un **Rifle de asalto automático**, disparará mientras se mantenga pulsado el **Click Izquierdo**.
Si nos quedamos sin munición en el cargador actual, hay que presionar la **Tecla R** para recargarla, la pistsola tomará menos tiempo que el Rifle de Asalto.
Cuando aparezca un **Núcleo de enegría** (Objeto que ilumina rojo) Podremos tocarlo con un arma para mejorarla y así disparar municiones laser, las cuales hacen más daño e impactan instantaneamente al objetivo.

![GIF de las Armas.](DocResources/pt5_Weapons.gif)

## Inventario
Te permite almacenar objetos que hay en el entorno, puedes tener **hasta 5 de ellos en un solo espacio** _(Las armas no se pueden acumular)_, para poder cambiar los Objetos que tengas guardados en el inventario, debes subir o bajar con la **Rueda del Mouse**. 

![GIF del inventario.](DocResources/pt6_Invenotry.gif)

> [!WARNING]
> Lamentablemente no logré solucionar un error el cual al lanzar un objeto que está acumulado junto a otros, también saca al resto del inventario. Esto es debido a equivocarme al empezar a programar y haber creado tanto los objetos interactuables como el sistema del inventario de maneras muy aisladas. **Por lo que una oportunidad de mejora sería el reestructurar estos 2 sistemas de una manera más modular**.

## Escenas
#### Entrenamiento / Entretenimiento
En esta primera zona podrás probar todos los objetos disponibles en el juego, no sin antes pasar por un pequeño tutorial que te enseñará **cómo usar la nueva cámara**, haciendote pasar por un laberinto; **Cómo recoger y lanzar los objetos**, con los cuales podrás jugar un rato dentro de la escena; y finalmente el **Cómo usar las armas**, tu principal medio de defensa.

![Interactables.](DocResources/TrainingZone.png)

#### Zona de la misión
En este mapa encontraras a los enemigos esperandote, tu misión es eliminarlos a todos para limpiar el pueblo, encontrarás los 5 objetos principales por todos lados, solo una pistola que aparecerá cerca tuyo, y un rifle que tendrás que buscar, también al paso del tiempo, aparecerá un **Núcleo de energía** en alguna zona aleatoria, con el cual mejorarás tu arma para eliminar a los enemigos más fácilmente, está atento al entorno. 

![Interactables.](DocResources/MissionZone.png)

## Enemigos
#### Comportamiento básico
Empezamos por la parte del seguimiento, donde implementé el paquete de Unity [AI Navigation](https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/index.html), con el cual se puede generar fácilmente un terreno donde el **Agente** puede caminar dependiendo de ciertas condiciones o caracteristicas del **Agente** y el terreno, más conocido como **NavMesh**. 

![GIF IA NavMesh.](DocResources/pt7_NavMesh.gif)


#### Máquina de estados
El enemigo está compuesto por la famosa estructura de al máquina de estados, la cuan permite que el enemigo reaccione bajo ciertas situaciones y cambie su comportamiento acorde a esto. En este caso tenemos una bastante simple donde el enemigo tiene 4 estados. La principal función que aprovecho de este compoente es la falcilidad de cambiar las animaciones dependiendo de las situaciones.

```C#
void FixedUpdate()
{
    float targetDistance = Vector2.Distance(target.position, transform.position);
    if(!dead)
    {
        if (enemyState != EnemyState.Crawl && crawling)
        {
            UpdateState(EnemyState.Crawl);
        }
        else if (proximityRange > targetDistance && enemyState != EnemyState.Chase && !crawling)
        {
            UpdateState(EnemyState.Chase);
        }
        else if (proximityRange < targetDistance && enemyState != EnemyState.Idle && !crawling)
        {
            UpdateState(EnemyState.Idle);
        }
    }
    
}

```



#### Zonas de daño modulares
Aquí es donde me quise complicar un poco más para poder darle un toque satisfactorio al juego, además como un reto personal para aplicar de manera más aferrada el **Principio de responsabilidad única**
Básicamente esto es un sistema modular que nos permite hacerle daño en zonas específicas del cuerpo a los enemigos, donde definimos unas zonas que tienen cierta cantidad de vida, también definimos si esas zonas son dependientes de otras, lo que nos permite llegar a algo visualmente grotesco pero interesante, el cual es el desmembramiento de los enemigos.

![GIF IA NavMesh.](DocResources/pt8_DamageZones.gif)

Cada zona de daño funciona en su propio mundo, están atentas a que les hace daño y notifican a otra clase de que es lo que les está pasando en este momento, para que la clase que se encanga de manejar la vida de todo el enemigo pueda saber que hacer.

**Principal función de la zona de Daño**
```C#

public void OnHit(Vector3 hitPoint, float damage, Vector3 hitForce)
{
    if (head) // triple the amount of damage if the head gets hit
    {
        damage = damage * 3;
    }

    // decrease global health
    enemyHealth.StackDamage(damage);
    health = health - damage;

    if (health < 0 && parentOf == null)
    {
        CheckForLegs();

        enemyHealth.DropBlood(transform);
        gameObject.SetActive(false);
    }
    else if (health < 0 && parentOf != null) // if any other object depends on the position of this one, disable it too
    {
        CheckForLegs();

        enemyHealth.DropBlood(transform);
        parentOf.SetActive(false);
        gameObject.SetActive(false);
    }
}


```
