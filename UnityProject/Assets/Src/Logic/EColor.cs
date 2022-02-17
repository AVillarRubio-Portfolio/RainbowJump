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
	/// colisiones de nombre todo el rato y así es más práctico
	/// </summary>
	public enum EColor
	{

		// TODO: De momento dejo Color1, Color2, como genéricos a falta de
		// algo mejor XD
		Color1, Color2, Color3, Color4,
		// TODO: de momento meto estos, aunque no sean de jugadores.
		// Quizá no tengan sentido aquí :-m
		White, Black,

		// Número de colores
		NumColors
	} // EColor

	//----------------------------------------------------------

	/// <summary>
	/// Clase para métodos de extensión.
	/// </summary>
	public static class EColorExtensions
	{
		/// <summary>
		/// Devuelve cierto si el color es uno de los válidos para un jugador.
		/// </summary>
		/// <param name="color">color</param>
		/// <returns>Cierto si es un color válido para jugador</returns>
		public static bool IsPlayerColor(this EColor color)
		{
			return color <= EColor.Color4;
		}

		/// <summary>
		/// Devuelve cierto si el color es un "comodín". No será
		/// color de jugador, pero si el jugador está en un camino de este
		/// color, se permitirá.
		/// </summary>
		/// <param name="color">Color a analizar</param>
		/// <returns>Cierto si es un color "comodín" válido para todos.</returns>
		public static bool IsWildcardColor(this EColor color)
		{
			return color == EColor.White;
		}

		/// <summary>
		/// Devuelve cierto si el color es de "muerte". No será
		/// color de jugador, pero si el jugador está en un camino de este
		/// color, morirá.
		/// </summary>
		/// <param name="color">Color a analizar</param>
		/// <returns>Cierto si es un color "de muerte".</returns>
		public static bool IsDeathColor(this EColor color)
		{
			return color == EColor.Black;
		}

		/// <summary>
		/// Devuelve cierto si el color actual (this) es compatible con el
		/// del parámetro (son iguales o uno es el comodín). Si un jugador
		/// tiene un color y acaba en un camino del otro, este método indica
		/// si es válido.
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