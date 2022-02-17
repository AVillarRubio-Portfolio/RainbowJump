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
	/// Scriptable Object para mantener la información sobre los colores.
	/// </summary>
	[CreateAssetMenu(fileName = "Color", menuName = "ScriptableObjects/Color", order = 1)]
	public class ColorSpec : ScriptableObject
	{
		// Igual esto es matar moscas a cañonazos... Lo hacemos por si
		// viene bien en el futuro pensando en posibles efectos, poder elegir
		// personaje en un supuesto HUD de escena inicial o cosas así.

		[Tooltip("Tipo de color asociado a este scriptable object")]
		public Logic.EColor colorType;

		[Tooltip("Color (visual) asociado utilizado como MainColor")]
		public Color color;

		[Tooltip("Material a establecer en el modelo del jugador que use este color.")]
		public Material playerMaterial;

		[Tooltip("Nombre del color")]
		public string colorName;

	} // ColorSpec

} // namespace
