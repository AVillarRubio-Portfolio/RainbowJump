//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using UnityEngine;
using System.Collections; // IEnumerator

using SO;

namespace Mgr
{

	/// <summary>
	/// Clase global de gesti�n del juego. Es un singleton que orquesta el
	/// funcionamiento general de la aplicaci�n, sirviendo de comunicaci�n entre
	/// las escenas.
	/// </summary>
	public class GameManager : MonoBehaviour
	{
		//---------------------------------------------------------
		//               Propiedades p�blicas (inspector)
		//---------------------------------------------------------
		#region Propiedades p�blicas (inspector)

		[Header("Datos del juego (cambiar en el prefab)")]

		[Tooltip("Informaci�n sobre los colores de los jugadores")]
		public ColorSpec[] colorSpecs;

		[Header("�ndices de las escenas en el Build Settings (cambiar en el prefab)")]

		[Tooltip("�ndice de la escena del men� principal en build settings (cambiar en el prefab)")]
		public int mainMenuScene;

		[Tooltip("�ndice de la escena de cr�ditos en el build settings")]
		public int creditsScene;

		[Tooltip("�ndice de la escena del men� de selecci�n de personajes en build settings")]
		public int selectCharactersScene;

		[Tooltip("�ndice de la escena de nivel en build settings")]
		public int levelScene;

		[Header("Gestores de las escenas individuales")]

		[Tooltip("Gestor de la ejecuci�n del nivel (en la escena de juego)")]
		public LevelManager levelManager;

		// Propiedades con los datos de un jugador.
		[System.Serializable]
		public struct PlayerConfig
		{
			[Tooltip("Informaci�n del color")]
			public ColorSpec colorSpec;

			[Tooltip("Asignaci�n de controles")]
			public ControlBindings bindings;
		}

		[Tooltip("Jugadores que se lanzan. En ejecuci�n se cambian desde el men�. En desarrollo pon los que quieras")]
		public PlayerConfig[] players;

		#endregion

		//---------------------------------------------------------
		//                    Gesti�n del singleton
		//---------------------------------------------------------
		#region Gesti�n del singleton

		/// <summary>
		/// Instancia �nica de la clase (singleton).
		/// </summary>
		private static GameManager _instance;

		/// <summary>
		/// Propiedad para acceder a la �nica instancia de la clase.
		/// </summary>
		public static GameManager Instance
		{
			get
			{
				Debug.Assert(_instance != null);
				return _instance;
			}
		} // Instance

		//---------------------------------------------------------

		/// <summary>
		/// Devuelve cierto si la instancia del singleton est� creada y
		/// falso en otro caso.
		/// Lo normal es que est� creada, pero puede ser �til durante el
		/// cierre para evitar usar el GameManager que podr�a haber sido
		/// destru�do antes de tiempo.
		/// </summary>
		/// <returns>Cierto si hay instancia creada.</returns>
		public static bool HasInstance()
		{
			return _instance != null;
		}

		//---------------------------------------------------------

		/// <summary>
		/// M�todo llamado en un momento temprano de la inicializaci�n.
		/// 
		/// En el momento de la carga, si ya hay otra instancia creada,
		/// nos destruimos (al GameObject completo)
		/// </summary>
		protected void Awake()
		{

			if (_instance != null)
			{
				// No somos la primera intancia. Se supone que somos un
				// GameManager de una escena que acaba de cargarse, pero
				// ya hab�a otro en DontDestroyOnLoad que se ha registrado
				// como la �nica instancia.
				// Transferimos la configuraci�n que es dependiente de la escena.
				// Esto permitir� al GameManager real mantener su estado interno
				// pero acceder a los elementos de la escena particulares o bien
				// olvidar los de la escena previa de la que venimos para que
				// sean efectivamente liberados.
				_instance.levelManager = levelManager;

				// Planificamos el aviso de que una nueva escena se ha cargado. Lo hacemos
				// en una corrutina, que no est� bien hacerlo en el Awake.
				_instance.StartCoroutine(_instance.SceneStarted());

				// Y ahora nos destru�mos del todo. DestroyImmediate y no Destroy para evitar
				// que se inicialicen el resto de componentes del GameObject para luego ser
				// destru�dos. Esto es importante dependiendo de si hay o no m�s managers
				// en el GameObject.
				DestroyImmediate(this.gameObject);
				return; // No es necesario, pero tranquiliza :-p
			}
			else
			{
				// Somos el primer GameManager.
				// Queremos sobrevivir a cambios de escena.
				_instance = this;
				DontDestroyOnLoad(this.gameObject);
				Init();
				_instance.StartCoroutine(_instance.SceneStarted());
			} // if-else somos instancia nueva o no.

		} // Awake

		//---------------------------------------------------------

		/// <summary>
		/// M�todo llamado cuando se destruye el componente.
		/// </summary>
		protected void OnDestroy()
		{
			if (this == _instance)
			{
				// �ramos la instancia de verdad, no un clon.
				_instance = null;
			} // if somos la instancia principal
		} // OnDestroy

		#endregion

		//---------------------------------------------------------
		//                 Gesti�n de la inicializaci�n
		//---------------------------------------------------------
		#region Gesti�n de la inicializaci�n

		/// <summary>
		/// Dispara la inicializaci�n.
		/// </summary>
		private void Init()
		{

			// De momento no hay inicializaci�n que valga :-D

		} // Init()

		#endregion

		// PlaceHolders

		//---------------------------------------------------------
		//                   M�todos p�blicos
		//---------------------------------------------------------
		#region M�todos p�blicos

		public ColorSpec GetColorSpec(Logic.EColor color)
		{
			foreach (var cs in colorSpecs)
				if (cs.colorType == color)
					return cs;

			// Crash.
			Debug.LogError("�El GameManager no conoce el color solicitado!" + color);
			return null;
		}

		#endregion

		//---------------------------------------------------------
		//   M�todos para la escena de men� principal
		//---------------------------------------------------------
		#region M�todos para la escena de men� principal

		/// <summary>
		/// Lanza la escena de cr�ditos
		/// </summary>
		public void ShowCredits()
		{

			ChangeScene(creditsScene);

		} // ShowCredits

		//---------------------------------------------------------

		/// <summary>
		/// Lanza la escena de selecci�n de personajes
		/// </summary>
		public void LaunchSelectPlayers()
		{

			ChangeScene(selectCharactersScene);

		} // ShowCredits

		#endregion

		//---------------------------------------------------------
		//   M�todos para la escena de selecci�n de personajes
		//---------------------------------------------------------
		#region M�todos para la escena de selecci�n de personajes

		/// <summary>
		/// Lanza la escena de nivel.
		/// 
		/// Es llamado desde la escena de selecci�n de personajes cuando
		/// todos se han elegido los personajes.
		/// </summary>
		/// <param name="players">Personajes y controles elegidos.</param>
		public void LaunchRandomLevel(PlayerConfig[] players)
		{
			_sinInstrucciones = false; // Venimos desde el men�
			Debug.Assert(players.Length > 0);
			this.players = players;
			ChangeScene(levelScene);

		} // LaunchRandomLevel

		//---------------------------------------------------------

		/// <summary>
		/// Relanza la escena de juego con la configuraci�n actual de los personajes.
		/// </summary>
		public void RelaunchLevel()
		{
			_sinInstrucciones = true;
			ChangeScene(levelScene);
		}

		#endregion

		//---------------------------------------------------------
		//               M�todos para varias escenas
		//---------------------------------------------------------
		#region M�todos para varias escenas

		/// <summary>
		/// Lanza la escena del men� principal. 
		/// 
		/// Es llamado desde la escena de juego cuando se termina la
		/// partida y desde la de cr�ditos.
		/// </summary>
		public void GoToMenu()
		{

			ChangeScene(mainMenuScene);

		} // LaunchRandomLevel

		#endregion

		//---------------------------------------------------------
		//              M�todos protegidos/privados
		//---------------------------------------------------------
		#region M�todos protegidos/privados

		/// <summary>
		/// Llamado tras cada cambio de escena (o al cargar la primera). Simula el evento
		/// Start() de Unity en un objeto que sobrevive al cambio de escenas. Para
		/// que funcione, las escenas destino tienen que incluir el prefab del GameManager.
		/// 
		/// Es llamado desde el Awake, ya sea por la primera instancia (en la escena
		/// de arranque) o por las instancias secundarias que se autodestruyen, para
		/// avisar al GameManager original.
		/// 
		/// Como se llama desde Awake, se crea como una corrutina. La idea es que se
		/// devuelva el control inmediatamente y se haga la inicializaci�n deseada
		/// en el siguiente frame.
		/// </summary>
		/// <returns>Corrutina</returns>
		private IEnumerator SceneStarted()
		{
			yield return 0;

			if (levelManager != null)
			{
				// Acabamos de lanzar la escena principal del juego
				StartLevel();
			} // if (levelmanager)

		} // SceneStarted

		//---------------------------------------------------------

		/// <summary>
		/// M�todo que cambia la escena actual por la indicada en el par�metro.
		/// Antes y despu�s de la carga fuerza la recolecci�n de basura, por eficiencia,
		/// dado que se espera que la carga tarde un tiempo, y dado que tenemos al
		/// usuario esperando podemos aprovechar para hacer limpieza y ahorrarnos alg�n
		/// tir�n en otro momento.
		/// </summary>
		/// <param name="index">�ndice de la escena (en el build settings)
		/// que se cargar�.</param>
		private static void ChangeScene(int index)
		{

			// De Unity Configuration Tips: Memory, Audio, and Textures
			// https://software.intel.com/en-us/blogs/2015/02/05/fix-memory-audio-texture-issues-in-unity
			//
			// "Since Unity's Auto Garbage Collection is usually only called when the heap is full
			// or there is not a large enough freeblock, consider calling (System.GC..Collect) before
			// and after loading a level (or put it on a timer) or otherwise cleanup at transition times."
			//
			// En realidad... todo esto es un oldie y dudo muuucho que sirva para algo, pero bueno :-)
			System.GC.Collect();
			UnityEngine.SceneManagement.SceneManager.LoadScene(index);
			System.GC.Collect();

		} // ChangeScene

		//---------------------------------------------------------

		/// <summary>
		/// Configura el LevelManager de la escena y pone en marcha
		/// la partida.
		/// </summary>
		/// <returns>Cierto si todo fue bien, y falso si hubo alg�n problema
		/// (idealmente no deber�a ocurrir).</returns>
		public bool StartLevel()
		{
			if (levelManager == null)
			{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
				Debug.LogError("[" + name + "]::" + GetType().Name + ": no se ha configurado LevelManager");
#endif
				// Esto estallar�...
				return false;
			}
			/*
			// En modo editor o builds de prueba, permitimos configurar en el
			// GameManager qu� jugadores van a jugar, sin tener que venir de
			// una escena anterior. Solo se usa si no nos lo han dado desde
			// fuera, lo que significa que estamos directamente en la escena de juego.
			for (int i = 0; i < players.Length; ++i)
			{
				levelManager.AddPlayer(players[i].character, players[i].bindings);
			}
			*/
			// TODO: de momento todo est� cableado. Mapa chusco y siempre con el
			// mismo n�mero de niveles.
			Logic.Map map = new Logic.Map();
			map.Init(1000, 20);


			levelManager.SetMap(map);

			// En modo editor o builds de prueba, permitimos configurar en el
			// GameManager qu� jugadores van a jugar, sin tener que venir de
			// una escena anterior. Solo se usa si no nos lo han dado desde
			// fuera, lo que significa que estamos directamente en la escena de juego.
			for (int i = 0; i < players.Length; ++i)
			{
				levelManager.AddPlayer(players[i].colorSpec, players[i].bindings);
			}

			levelManager.StartGame(_sinInstrucciones);

			return true;

		} // StartLevel

		#endregion

		//---------------------------------------------------------
		//                Atributos protegidos/privados
		//---------------------------------------------------------
		#region Atributos protegidos/privados

		/// <summary>
		/// Boleeano que indica si en el pr�ximo inicio de nivel hay o no que mostrar
		/// las instrucciones.
		/// </summary>
		private bool _sinInstrucciones;

		#endregion

	} // GameManager

} // namespace