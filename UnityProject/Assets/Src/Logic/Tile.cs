//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

namespace Logic
{

	/// <summary>
	/// Clase que almacena la informaci�n de un tile l�gico. No tiene
	/// nada visual. Es �til para crear el mapa l�gico.
	/// </summary>
	public class Tile
	{

		/// <summary>
		/// Color del tile.
		/// </summary>
		public EColor color;
		// TODO: est� la idea volando de poder poner dos colores
		// a la vez para que el camino sirva para dos jugadores, pero
		// eso est� por ver XD

		/// <summary>
		/// Cierto si es un tile de obst�culo en el que el usuario tendr�
		/// que saltar.
		/// </summary>
		public bool obstacle;

		/// <summary>
		/// Cierto si tiene pared en el lado "norte", lo que impide
		/// saltar al camino que haya arriba.
		/// </summary>
		public bool northWall;

		/// <summary>
		/// Cierto si tiene pared en el lado "sur", lo que impide
		/// saltar al camino que haya abajo.
		/// </summary>
		public bool southWall;

		public bool newLife;

	} // class Tile

} // namespace