//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using UnityEngine;
using System.Collections;

using Mgr;
using SO;
using Logic;

[System.Serializable]
public struct PlayerParticles
{
	public ParticleSystem WalkParticles;
	public ParticleSystem DeadParticles;
	public GameObject SmokeParticles;
}

[System.Serializable]
public struct PlayerSounds
{
	public AudioClip JumpSound;
	public AudioClip DamageSound;
	public AudioClip WrongWayClip;
}

/// <summary>
/// Controlador general de un jugador.
/// </summary>
public class PlayerController : MonoBehaviour
{
	//----------------------------------------------------------
	//            Propiedades p�blicas (inspector)
	//----------------------------------------------------------
	#region Propiedades p�blicas (inspector

	[Tooltip("Modelo del jugador. Se manipula su material seg�n el color del jugador.")]
	public MeshRenderer model;

	public Animator animator;

	public PlayerParticles playerParticles;

	public PlayerSounds playerSounds;

	#endregion

	//----------------------------------------------------------
	//                     M�todos p�blicos
	//----------------------------------------------------------
	#region M�todos p�blicos

	/// <summary>
	/// realiza una configuraci�n b�sica usada en el uso de los personajes en la
	/// escena de selecci�n del personaje.
	/// </summary>
	/// <param name="color">Color a establecer</param>
	public void ConfigureColor(SO.ColorSpec color)
	{
		_color = color.colorType;
		_colorSpec = color;

		// Ponemos el material al modelo
		model.material = color.playerMaterial;

	} // ConfigureColor

	//----------------------------------------------------------

	/// <summary>
	/// Configura el jugador.
	/// </summary>
	/// <param name="levelManager">Gestor del nivel al que notificarle las
	/// cosas que nos pasen.</param>
	/// <param name="id">Identificador num�rico del jugador en la partida.
	/// Se usa por comodidad para que le resulte c�modo al LevelManager
	/// identificarnos cuando nos ocurran cosas.</param>
	/// <param name="color">Color del jugador.</param>
	/// <param name="bindings">Asignaci�n de controles que hay que usar.</param>
	public void Configure(LevelManager levelManager, int id, SO.ColorSpec color, ControlBindings bindings)
	{

		ConfigureColor(color);

		_lives = 4; // TODO: Cableado. La idea de que dependiera del n�mero de jugadores, de momento al cubo.

		_controlBindings = bindings;
		_id = id;
		
		_levelManager = levelManager;

		_map = levelManager.GetMap();
		_path = _map.GetColorInitialPath(_color);

		// Empezamos en el tile 5.
		_n = _lives - 1; // TODO: animacioncilla antes de empezar.
		transform.position = CalculateScenePosition();

		_jumpTile = -100.0f; // -inf
		_deadSimulationPosition = -100.0f; // -inf

		_lastYAxis = 0.0f;

		_audioSource = gameObject.GetComponent<AudioSource>();

	} // Configure

	//----------------------------------------------------------

	/// <summary>
	/// Devuelve el id del jugador.
	/// </summary>
	/// <returns>Id del jugador</returns>
	public int GetId()
	{
		return _id;
	} // GetId

	//----------------------------------------------------------

	public ControlBindings GetControlBindings()
	{
		return _controlBindings;
	}

	//----------------------------------------------------------

	public ColorSpec GetColorSpec()
	{
		return _colorSpec;
	}

	//----------------------------------------------------------

	/// <summary>
	/// Devuelve cierto si el jugador ha muerto.
	/// </summary>
	/// <returns>Cierto si el jugador ha muerto</returns>
	public bool IsDead()
	{
		return _lives == 0;
	}

	//----------------------------------------------------------

	/// <summary>
	/// M�todo llamado externamente cuando queremos impedir el movimiento
	/// del jugador completamente. Se usa al acabar la partida y en la
	/// selecci�n del personaje
	/// </summary>
	public void FreezeForever()
	{
		_alwaysInert = true;
	} // FreezeForever

	//----------------------------------------------------------

	/// <summary>
	/// Devuelve la velocidad actual del jugador.
	/// </summary>
	/// <returns>Velocidad</returns>
	public float GetVelocity()
	{
		return _velocity;
	} // GetVelocity

	//----------------------------------------------------------

	/// <summary>
	/// Establece la velocidad de desplazamiento (en el eje X) en
	/// la escena. Se aplicar� esta velocidad en el siguiente update.
	/// </summary>
	/// <param name="v">Velocidad</param>
	public void SetVelocity(float v)
	{
		_velocity = v / 2;
	} // SetVelocity

	#endregion

	//----------------------------------------------------------
	//                     M�todos de MonoBehaviour
	//----------------------------------------------------------
	#region M�todos de MonoBehaviour


	/// <summary>
	/// M�todo llamado por Unity en cada frame. Actualizamos la posici�n.
	/// </summary>
	void Update()
	{

		if (_controlBindings == null)
			// Si no tenemos teclas asignadas, no podemos movernos
			return;

		if (_alwaysInert)
		{
			// Hemos muerto por siempre jam�s (fin de partida).
			return;
		}

		if (_deadSimulationPosition > 0.0f)
		{
			// Estamos "muriendo". Los controles no funcionan.
			_deadSimulationPosition += _velocity * Time.deltaTime;
			if ((_deadSimulationPosition - _n) >= 1.0f)
			{
				// Hemos terminado de morir. El jugador no se ha movido
				// (se habr� yendo poco a poco hacia atr�s) y la "c�mara"
				// ha avanzado ya un tile. Dejamos de morir.
				_deadSimulationPosition = -100;
				_immunityTime = 2.0f; // TODO: decidir?
			}
			return;
		} // if estamos muriendo
        else
        {
			animator.SetBool("isFalling", false);
        }

		// Avanzamos la posici�n l�gica.
		_n += _velocity * Time.deltaTime;

		bool jumping = _n - _jumpTile <= 2.0f;

		if (!jumping)
		{
			animator.SetBool("isJumping", false);
			if (playerParticles.WalkParticles.isPaused) playerParticles.WalkParticles.Play();
			// No estamos saltando. Miramos si nos mata el color, o la
			// entrada del usuario.

			Logic.Map map = _levelManager.GetMap();
			Logic.Tile tile = map.GetTile(_path, Mathf.FloorToInt(_n)); // HACK

			if (_ignoringColorPendingTime > 0)
			{
				_ignoringColorPendingTime -= Time.deltaTime;
			}
			else
			{
				if (!tile.color.IsCompatible(_color) && (_immunityTime <= 0))
				{
					// Estamos encima de un camino que no es de nuestro color y se ha
					// pasado el periodo de gracia.
					Die();
				}
			} // if est�bamos o no en periodo de gracia con el color del camino
			if (tile.obstacle && (_immunityTime <= 0))
			{
				// El tile es de obst�culo y si estamos aqu�, no hemos saltado.
				// Muerte.
				Die();
			}
			ProcessInput();
		}

		if (_immunityTime > 0)
		{
			// Durante la inmunidad, parpadeamos.
			int aux = (int)(_immunityTime * 10);

			model.enabled = (aux % 2) == 0;
			_immunityTime -= Time.deltaTime;
		}

		transform.position = CalculateScenePosition();
	} // Update

	#endregion

	//----------------------------------------------------------
	//                     M�todos privados
	//----------------------------------------------------------
	#region M�todos privados

	/// <summary>
	/// M�todo que procesa la entrada.
	/// </summary>
	private void ProcessInput()
	{

		if (_velocity == 0.0f)
			// Si estamos parados, no podemos movernos.
			return;

		float currentYAxis = Input.GetAxis(_controlBindings.yAxis);

		if (_lastYAxis != currentYAxis && /*Input.GetButtonDown(_controlBindings.yAxis) && */ currentYAxis < 0) {
			// Se ha pulsado ir hacia abajo.
			if (_map.CanGoPrevPath(_path, Mathf.FloorToInt(_n))) {
				// Podemos cambiar de camino
				ChangeToPath(_path - 1);
			}
			else
			{
				// No podemos cambiar de camino.
				// TODO: sonido?
				PlaySound(playerSounds.WrongWayClip);
			}
		} // if se ha pulsado ir hacia abajo
		else if (_lastYAxis != currentYAxis && /*Input.GetButtonDown(_controlBindings.yAxis) && */currentYAxis > 0)
		{
			if (_map.CanGoNextPath(_path, Mathf.FloorToInt(_n))) {
				// Podemos cambiar de camino
				ChangeToPath(_path + 1);
			}
			else
			{
				// No podemos cambiar de camino.
				// TODO: sonido?
				PlaySound(playerSounds.WrongWayClip);
			}
		} // if se ha pulsado ir hacia arriba
		else if (Input.GetButtonDown(_controlBindings.actionButton))
		{
			// Se ha pulsado saltar.
			_jumpTile = _n;
			animator.SetBool("isJumping", true);
			playerParticles.WalkParticles.Pause();
		}

		_lastYAxis = currentYAxis;

	} // ProcessInput

    private void OnTriggerEnter(Collider other)
    {
		Destroy(other.gameObject);
		if (_lives < 7)
		{
			_n++;
			++_lives;
		}
	}

	//----------------------------------------------------------

	public void WinAnimation()
    {
		animator.SetBool("isWinning", true);
    }

    //----------------------------------------------------------

    /// <summary>
    /// Hace un salto del camino actual al indicado como par�metro.
    /// </summary>
    /// <param name="newPath"></param>
    private void ChangeToPath(int newPath)
	{
		StartCoroutine(CreateSmokeWithJump());

		int diff = newPath - _path;
		_path = newPath;
		_ignoringColorPendingTime = 0.2f; // TODO: 

		// TODO: efectillos o algo.

		PlaySound(playerSounds.JumpSound);
	} // ChangeToPath

	IEnumerator CreateSmokeWithJump()
    {
		GameObject smoke = Instantiate<GameObject>(
			playerParticles.SmokeParticles, 
			CalculateScenePosition(), 
			Quaternion.identity);

		yield return new WaitForSeconds(2);

		Destroy(smoke);
	}

	//----------------------------------------------------------

	/// <summary>
	/// M�todo llamado cuando el jugador muere.
	/// </summary>
	private void Die()
	{
		--_lives;
		_deadSimulationPosition = _n;
		_levelManager.PlayerDead(this);
		playerParticles.DeadParticles.Play();
		PlaySound(playerSounds.DamageSound);

		animator.SetBool("isFalling", true);

		if (_lives == 0)
		{
			_levelManager.PlayerKilled(this);
		}
	} // Die

	//----------------------------------------------------------

	/// <summary>
	/// Devuelve el vector3 donde debe colocarse el jugador en la escena
	/// dado su camino y posici�n en �l.
	/// </summary>
	/// <returns></returns>
	private Vector3 CalculateScenePosition()
	{
		float y;
		float distJump = _n - _jumpTile;
		if (distJump <= 2.0f)
		{
			distJump -= 1.0f;
			// Par�bola
			y = (1 - distJump * distJump);
		}
		else
			y = 0;
		return new Vector3(_n * 2 - 1.0f /*+ _id*0.2f*/, y, 4 * _path);
	}

	private void PlaySound(AudioClip audio)
    {
		_audioSource.clip = audio;
		_audioSource.Play();
    }

	#endregion

	//----------------------------------------------------------
	//                     Atributos privados
	//----------------------------------------------------------
	#region Atributos privados

	/// <summary>
	/// Especificaci�n del color.
	/// </summary>
	private SO.ColorSpec _colorSpec;

	/// <summary>
	/// Gestor del nivel al que le notificamos las cosas que nos pasen.
	/// </summary>
	private LevelManager _levelManager;

	/// <summary>
	/// Id num�rico del jugador. Nos lo pasan en Configure().
	/// </summary>
	int _id;

	/// <summary>
	/// Color del jugador.
	/// </summary>
	Logic.EColor _color;

	/// <summary>
	/// Controles usados para este jugador
	/// </summary>
	SO.ControlBindings _controlBindings;

	/// <summary>
	/// Booleano que indica si el jugador ya no podr� moverse m�s. No
	/// es igual que _inert, que es "reversible" y se usa para las muertes.
	/// Este se activa al acabar la partida, para no poderse mover de ninguna
	/// forma (y nunca se pondr� a falso). De esa forma, aunque al activar
	/// el flag haya en proceso una trampa retardada, cuando se haga el
	/// respawn no podr� moverse tampoco.
	/// </summary>
	private bool _alwaysInert = false;

	/// <summary>
	/// Tiempo de cortes�a durante el que estaremos ignorando el color del
	/// camino. Se usa para permitir pasar por encima de caminos de
	/// otros colores si es por poco tiempo.
	/// </summary>
	float _ignoringColorPendingTime;

	/// <summary>
	/// Velocidad de desplazamiento (en el eje X) por unidad de tiempo. Se mide
	/// en desplazamiento en el mapa l�gico. En la escena se multiplica por 2
	/// (un tile l�gico son dos unidades de Unity).
	/// </summary>
	private float _velocity = 0.0f;

	/// <summary>
	/// Camino en el que estamos actualmente.
	/// </summary>
	private int _path;

	/// <summary>
	/// "Tile" en el que estamos *en el mapa*. Usamos un movimiento
	/// continuo, por lo que se guarda en un float.
	/// 
	/// NO es el mismo que en la escena de Unity, porque cada tile del
	/// mapa l�gico son dos unidades de Unity.
	/// </summary>
	private float _n;

	/// <summary>
	/// Mapa l�gico que estamos jugando. Se saca del LevelManager en el
	/// Configure.
	/// </summary>
	private Logic.Map _map;

	/// <summary>
	/// Posici�n donde est�bamos en el momento del �ltimo salto. Ser� negativo
	/// si no hay salto.
	/// </summary>
	private float _jumpTile;

	/// <summary>
	/// �ltimo valor del eje Y le�do de la entrada. Queremos que haya que soltar
	/// para volver a moverse. Con el teclado funciona bien usar GetButtonDown()
	/// para detectar que la pulsaci�n se ha realizado en el frame actual (aunque
	/// est� mapeado a un eje en la entrada) pero para los ejes del joystick (y eso
	/// incluye el mapeo del D-Pad a eje) no ocurre as�.
	/// 
	/// Nos toca guardar el valor del frame anterior y compararlo con el nuevo.
	/// </summary>
	private float _lastYAxis;

	/// <summary>
	/// N�mero de vidas que te quedan.
	/// </summary>
	private int _lives;

	/// <summary>
	/// Posici�n (en tiles) donde estar�amos si no acab�ramos de morir. Se utiliza para
	/// "echarnos hacia atr�s" cuando morimos. Si no acabamos de morir, esto estar� en
	/// un valor <= 0. Si no, el update() modificar� *esta* posici�n en lugar de la _n
	/// habitual, de modo que aqu� simulamos donde estar�amos si no hubi�ramos
	/// muerto. Esto permite saber el avance *de los dem�s*. Cuando los dem�s hayan
	/// avanzado un tile respecto al sitio donde nos hemos quedado nosotros (_n) entonces
	/// el proceso de morir habr� terminado, esto se pondr� otra vez en 0 y el
	/// avance continuar� normal.
	/// </summary>
	private float _deadSimulationPosition;

	/// <summary>
	/// Tiempo que nos queda de inmunidad. Se establece tras "terminar de morir".
	/// </summary>
	private float _immunityTime = 0.0f;

	private AudioSource _audioSource;

	#endregion

} // class PlayerController
