//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SO;

namespace Mgr
{

	/// <summary>
	/// Clase para gestionar una "partida".
	/// </summary>
	public class LevelManager : MonoBehaviour
	{
		[System.Serializable]
		public struct MsgConfig
		{
			[TextArea(3, 10)]
			[Tooltip("Texto")]
			public string text;

			public Color color;

			public float staticTime;

			public float vanishTime;
		}

		//----------------------------------------------------------
		//                Propiedades p�blicas (inspector)
		//----------------------------------------------------------
		#region Propiedades p�blicas (inspector)

		[Header("Prefabs que se instancian")]
		[Tooltip("Prefab de los jugadores")]
		public PlayerController playerPrefab;

		[Tooltip("Prefab del tile")]
		public TileController tilePrefab;
		// TODO: en el futuro esto podr�a ser un array para que haya varios
		// y a�adir variedad. Tambien faltan los de los obst�culos.

		[Header("Referencias a objetos de la escena")]
		[Tooltip("C�mara del nivel que sigue la acci�n")]
		public CameraController levelCamera;

		[Tooltip("Etiqueta central para dar feedback")]
		public GameUI.LabelView label;

		[Header("Dificultad del nivel")]

		[Tooltip("Velocidad inicial de la c�mara y los jugadores (en tiles de mapa por segundo)")]
		public float initialVelocity = 3.0f;

		public MsgConfig[] initialMsg;

		public MsgConfig[] countDownMsg;

		public MsgConfig restartMsg;

		#endregion

		//----------------------------------------------------------
		//                      M�todos p�blicos
		//----------------------------------------------------------
		#region M�todos p�blicos

		/// <summary>
		/// Devuelve el mapa actual.
		/// </summary>
		/// <returns>Mapa</returns>
		public Logic.Map GetMap()
		{
			return _map;
		} // GetMap

		#endregion

		//----------------------------------------------------------
		//                 M�todos para el GameManager
		//----------------------------------------------------------
		#region M�todos para el GameManager

		public void SetMap(Logic.Map map)
		{
			_map = map;
		} // SetMap

		//----------------------------------------------------------

		/// <summary>
		/// A�ade un jugador nuevo al nivel. Se supone que no tendr� el
		/// mismo personaje ni controles que ning�n otro asignado previamente.
		/// *No* se comprueba que eso ocurra realmente.
		/// </summary>
		/// <param name="playerColor">Informaci�n del color</param>
		/// <param name="bindings">Asignaci�n de controles que usar�.</param>
		public void AddPlayer(ColorSpec playerColor, ControlBindings bindings)
		{
			_playerColors.Add(playerColor);
			_controlBindings.Add(bindings);
		} // AddPlayer

		//----------------------------------------------------------

		/// <summary>
		/// M�todo llamado para arrancar el nivel. Se instancian los
		/// caminos, los jugadores, etc�tera. Es llamado desde el
		/// GameManager
		/// <param name="sinInstrucciones"/>Cierto si se quieren quitar las instrucciones</param>
		/// </summary>
		public void StartGame(bool sinInstrucciones)
		{
			_skipInstructions = sinInstrucciones;
			Debug.Assert(_map != null);
			Debug.Assert(_players == null); // No hay jugadores anteriores

			// Instanciamos los caminos.
			// Esto no es muy elegante, que creamos todos los tiles a pincho...
			/*
			for (int p = 0; p < _map.NumPaths; ++p)
			{
				for (int x = 0; x < _map.PathLength; ++x)
				{
					InstanceTile(p, x, _map.GetTile(p, x));
				}

			}
			*/
			StartCoroutine(CreateMapCoroutine());

			_players = new PlayerController[_playerColors.Count];
			_numPlayers = _playerColors.Count;
			_deadPlayers = 0;
			for (int i = 0; i < _playerColors.Count; ++i)
			{
				ColorSpec c = _playerColors[i];
				ControlBindings b = _controlBindings[i];
				_players[i] = Instantiate<PlayerController>(playerPrefab);
				_players[i].Configure(this, i, c, b);

			} // for recorriendo los personajes registrados
			BackgroundGameController();


		} // StartGame

		//---------------------------------------------------------

		private IEnumerator CreateMapCoroutine()
		{
			for (int x = 0; x < _map.PathLength; ++x)
			{
				for (int p = 0; p < _map.NumPaths; ++p)
				{
					InstanceTile(p, x, _map.GetTile(p, x));
				}
				if (x > 20)
					yield return null;
			}

		} // CreateMapCoroutine

		//---------------------------------------------------------

		/// <summary>
		/// Controlador general del juego como m�todo as�ncrono.
		/// </summary>
		public async void BackgroundGameController()
		{
			// TODO: Mejorar esto.
			// Usar un TextMeshPro y hacer algo m�s para que el juego no empiece
			// tan bruscamente.

			int i = 0;
			if (!_skipInstructions)
			{
				foreach (var data in initialMsg)
				{
					await label.ShowAndVanishAsync(data.text, data.color, 10 * Vector3.one, data.staticTime, data.vanishTime);
				}
			}
			// No esperamos en el �ltimo.
			foreach (var data in countDownMsg)
			{
				var r = label.ShowAndVanishAsync(data.text, data.color, 10 * Vector3.one, data.staticTime, data.vanishTime);
				if (i < countDownMsg.Length - 1)
					await r;
				++i;
			}

			levelCamera.SetVelocity(initialVelocity * 2);
			for (i = 0; i < _playerColors.Count; ++i)
				_players[i].SetVelocity(initialVelocity * 2);

			float currentVelocity = initialVelocity * 2;

			float nextVelChange = Time.time + 5.0f;
			// Esperamos a que termine la partida.
			while (_deadPlayers < _numPlayers - 1)
			{
				if (nextVelChange <= Time.time)
				{
					nextVelChange = Time.time + 5.0f;
					currentVelocity += 1.0f;
					for (i = 0; i < _playerColors.Count; ++i)
						if (!_players[i].IsDead())
							_players[i].SetVelocity(currentVelocity);
					levelCamera.SetVelocity(currentVelocity);

				}
				await Task.Yield();
			}

			// Paramos todos los jugadores.
			levelCamera.SetVelocity(0);
			PlayerController ganador = null;
			for (i = 0; i < _playerColors.Count; ++i)
			{
				if (!_players[i].IsDead())
				{
					// Este es el que ha ganado.
					_players[i].SetVelocity(0);
					_players[i].WinAnimation();
					ganador = _players[i];
				}
			}
			SO.ColorSpec colorSpec = ganador.GetColorSpec();
			await label.ShowAndVanishAsync("GANA EL JUGADOR " + colorSpec.colorName, colorSpec.color, 10 * Vector3.one, 5, 1f);


			label.ShowAndVanishAsync(restartMsg.text, restartMsg.color, 10 * Vector3.one, restartMsg.staticTime, restartMsg.vanishTime);

			float endTime = Time.time + 3.0f;
			while(Time.time <= endTime)
			{
				for (int p = 0; p < _numPlayers; ++p)
				{
					string buttonName;
					buttonName = _players[p].GetControlBindings().actionButton;
					if (Input.GetButtonDown(buttonName))
					{
						GameManager.Instance.RelaunchLevel();
						return;
					}
				}
				await Task.Yield();
			}

			// No quieren que jugemos m�s.
			GameManager.Instance.GoToMenu();
			
		} // BackgroundGameController

		#endregion

		//----------------------------------------------------------
		//                 M�todos para el PlayerController
		//----------------------------------------------------------
		#region M�todos para los PlayerController

		/// <summary>
		/// M�todo llamado cuando el jugador pierde una vida. El propio
		/// PlayerController se encargar� de gestionar su cambio de posici�n.
		/// Se avisa por si se metiera HUD o algo en alg�n momento.
		/// </summary>
		/// <param name="player"></param>
		public void PlayerDead(PlayerController player)
		{

			// TODO: mensaje?

		} // PlayerDead

		//----------------------------------------------------------

		/// <summary>
		/// M�todo llamado cuando el jugador muere definitivamente (pierde
		/// todas sus vidas). Si todos mueren, la partida termina.
		/// Antes de llamar a este m�todo se llama tambi�n a PlayerDead
		/// </summary>
		/// <param name="player">Jugador que ha perdido todas las vidas.</param>
		public void PlayerKilled(PlayerController player)
		{
			++_deadPlayers;
			player.SetVelocity(0);
			player.FreezeForever();
			player.gameObject.SetActive(false); // Lo ocultamos
		} // PlayerKilled

		#endregion

		//----------------------------------------------------------
		//                  M�todos de MonoBehaviour
		//----------------------------------------------------------
		#region M�todos de MonoBehaviour

		#endregion

		//----------------------------------------------------------
		//                     M�todos privados
		//----------------------------------------------------------
		#region M�todos privados

		/// <summary>
		/// Instancia un tile a partir de los datos l�gicos.
		/// </summary>
		/// <param name="path">N�mero de camino al que pertenece el tile</param>
		/// <param name="i">�ndice i del tile tile (0-based, de 1 en 1)</param>
		/// <param name="tile">Informaci�n l�gica del tile</param>
		private void InstanceTile(int path, int i, Logic.Tile tile)
		{
			TileController newTile = Instantiate<TileController>(tilePrefab, transform);

			newTile.transform.position = new Vector3(i * 2, 0, path * 4);

			ColorSpec colorSpec = GameManager.Instance.GetColorSpec(tile.color);
			newTile.Configure(colorSpec.color, tile.northWall, tile.southWall, tile.obstacle, tile.newLife);

			// TODO: conservarlos para destruirlos. Esto igual deber�a llevarse a un
			// MapController si crece mucho.

		} // InstanceTile

		#endregion

		//----------------------------------------------------------
		//                     Atributos privados
		//----------------------------------------------------------
		#region Atributos privados

		/// <summary>
		/// Mapa l�gico que se est� jugando.
		/// </summary>
		Logic.Map _map;

		/// <summary>
		/// Lista con los colores de los jugadores. Est�n en el mismo
		/// orden que la asignaci�n de teclas.
		/// </summary>
		List<ColorSpec> _playerColors = new List<ColorSpec>();

		/// <summary>
		/// Lista con la asignaci�n de controles de cada jugador. Est�n en
		/// el mismo orden que la informaci�n de los colores.
		/// </summary>
		List<ControlBindings> _controlBindings = new List<ControlBindings>();

		/// <summary>
		/// Controladores de los jugadores creados din�micamente al empezar.
		/// </summary>
		PlayerController[] _players;

		/// <summary>
		/// Cierto si queremos saltar las etiquetas de las instrucciones al principio.
		/// </summary>
		bool _skipInstructions = false;

		/// <summary>
		/// N�mero de jugadores que tenemos.
		/// </summary>
		int _numPlayers;

		/// <summary>
		/// N�mero de jugadores que quedan vivos.
		/// </summary>
		volatile int _deadPlayers;

		#endregion

	} // LevelManager

} // namespace