//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using UnityEngine;

namespace SO
{

	/// <summary>
	/// Clase para crear los controles para cada uno de los jugadores. Luego será usada por los player controller
	/// para poder mover a ese jugador
	/// </summary>
	[CreateAssetMenu(fileName = "controlBinding", menuName = "ScriptableObjects/ControlBinding", order = 2)]
	public class ControlBindings : ScriptableObject
	{

		[Tooltip("Nombre del eje X. No se usa en el juego :-D")]
		public string xAxis;

		[Tooltip("Nombre del eje Y. Se usa para cambiar de camino")]
		public string yAxis;

		[Tooltip("Tecla/botón de acción")]
		public string actionButton;

	} // class ControlBindings

} // namespace