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
	/// Enumerado con los distintos tipos de color que hay en el juego.
	/// 
	/// No soy fan del nombre EColor, pero poner Color nos iba a generar
	/// colisiones de nombre todo el rato y as� es m�s pr�ctico
	/// </summary>
	public enum EColor
	{

		// TODO: De momento dejo Color1, Color2, como gen�ricos a falta de
		// algo mejor XD
		Color1, Color2, Color3, Color4,
		// TODO: de momento meto estos, aunque no sean de jugadores.
		// Quiz� no tengan sentido aqu� :-m
		White, Black,

		// N�mero de colores
		NumColors
	} // EColor

	//----------------------------------------------------------

	/// <summary>
	/// Clase para m�todos de extensi�n.
	/// </summary>
	public static class EColorExtensions
	{
		/// <summary>
		/// Devuelve cierto si el color es uno de los v�lidos para un jugador.
		/// </summary>
		/// <param name="color">color</param>
		/// <returns>Cierto si es un color v�lido para jugador</returns>
		public static bool IsPlayerColor(this EColor color)
		{
			return color <= EColor.Color4;
		}

		/// <summary>
		/// Devuelve cierto si el color es un "comod�n". No ser�
		/// color de jugador, pero si el jugador est� en un camino de este
		/// color, se permitir�.
		/// </summary>
		/// <param name="color">Color a analizar</param>
		/// <returns>Cierto si es un color "comod�n" v�lido para todos.</returns>
		public static bool IsWildcardColor(this EColor color)
		{
			return color == EColor.White;
		}

		/// <summary>
		/// Devuelve cierto si el color es de "muerte". No ser�
		/// color de jugador, pero si el jugador est� en un camino de este
		/// color, morir�.
		/// </summary>
		/// <param name="color">Color a analizar</param>
		/// <returns>Cierto si es un color "de muerte".</returns>
		public static bool IsDeathColor(this EColor color)
		{
			return color == EColor.Black;
		}

		/// <summary>
		/// Devuelve cierto si el color actual (this) es compatible con el
		/// del par�metro (son iguales o uno es el comod�n). Si un jugador
		/// tiene un color y acaba en un camino del otro, este m�todo indica
		/// si es v�lido.
		/// </summary>
		/// <param name="color">Color a analizar</param>
		/// <param name="second">Color con el que compararlo.</param>
		/// <returns></returns>
		public static bool IsCompatible(this EColor color, EColor second)
		{
			return color == second || color.IsWildcardColor() || second.IsWildcardColor();
		}

	} // class EColorExtensions

} // namespace