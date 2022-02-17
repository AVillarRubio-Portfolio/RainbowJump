//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using System.Collections.Generic;

namespace Logic
{

	/// <summary>
	/// Clase que guarda un mapa lógico. Tiene los métodos para crear
	/// el mapa y acceder a la información con la que construir el mapa
	/// visual en Unity.
	/// </summary>
	public class Map
	{
		//----------------------------------------------------------
		//                     Propiedades públicas
		//----------------------------------------------------------
		#region Propiedades públicas

		/// <summary>
		/// Número de caminos del mapa.
		/// </summary>
		public int NumPaths
		{
			get { return _tiles.Length; }
		}

		/// <summary>
		/// Longitud en tiles de los caminos.
		/// </summary>
		public int PathLength
		{
			// Asumimos que todos son igual de largos :-)
			get { return _tiles[0].Length;  }
		}

		#endregion

		//----------------------------------------------------------
		//                     Métodos públicos
		//----------------------------------------------------------
		#region Métodos públicos

		/// <summary>
		/// Método que construye un mapa aleatorio. Siempre tienen 6 caminos.
		/// Al principio tienen los 6 colores siempre en las mismas posiciones,
		/// y luego se van moviendo.
		/// 
		/// </summary>
		/// <param name="length">Longitud de los caminos</param>
		/// <param name="initialLength">Longitud inicial con los colores fijos</param>
		public void Init(int length, int initialLength)
		{
			if (length < initialLength)
				throw new System.Exception("No entiendes ná!");

			int numPaths = (int)EColor.NumColors;
			// Creamos los tiles, de momento predefinidos.
			_tiles = new Tile[numPaths][];
			bool[] hayColor = new bool[numPaths]; // Si tenemos camino de este color
			int[] lastChange = new int[numPaths]; // Último tile donde cambiamos este color
			int[] distancias = new int[numPaths]; // Array auxiliar para evitar news
			int[] lastObstacle = new int[numPaths]; // Último tile donde *este color* tuvo un obstáculo
			EColor[] currentColor = new EColor[numPaths]; // Colores de cada camino

			for (int p = 0; p < numPaths; ++p)
			{
				// Creamos el array para los tiles del camino, y creamos
				// los primeros initialLength tiles con el color fijo de
				// ese camino.
				hayColor[p] = true;
				lastChange[p] = lastObstacle[p] =0;
				currentColor[p] = (EColor)p;
				_tiles[p] = new Tile[length];
				for (int t = 0; t < initialLength; ++t)
				{
					Tile current = _tiles[p][t] = new Tile();
					current.color = currentColor[p];
				}
			}

			// Creamos los tiles de los caminos a partir de la posición
			// donde podemos empezar a cambiarlos.
			for (int t = initialLength; t < length; ++t)
			{
				// ¿Intercambiamos algún color?
				if (_random.Next(0, 3) == 0)
				{
					// ¡¡Sí!!
					// ¿Cuáles? Calculamos las distancias, anulando
					// los que han cambiado hace poco.
					int numIntercambiables = 0;
					for (int i = 0; i < numPaths; ++i)
					{
						distancias[i] = t - lastChange[i];
						if (distancias[i] < 5)
							// Este lo hemos cambiado hace muy poco. Le
							// damos cuartelillo.
							distancias[i] = 0;
						else
							++numIntercambiables;
					} // for calculando las distancias
					if (numIntercambiables > 1)
					{
						// Hay al menos dos que podemos intercambiar.
						// Los elegimos.
						int a, b;
						SeleccionPonderada(distancias, out a, out b);
						EColor temp;
						temp = currentColor[a];
						currentColor[a] = currentColor[b];
						currentColor[b] = temp;
						lastChange[a] = lastChange[b] = t;
					}
				} // if hemos decidido intercambiar caminos

				// ¿Ponemos algún obstáculo?
				if (_random.Next(0, 3) == 0)
				{
					// TODO: Algo falla. Se ponen a veces dos seguidos, y
					// diría que con menos densiada de la que yo esperaría.
					// ¡¡Sí!!
					// ¿En qué color? Calculamos las distancias al último
					// obstáculo de cada color.
					int numCandidatos = 0;
					for (int i = 0; i < numPaths; ++i) // Esto es de colores, en realidad
					{
						distancias[i] = t - lastObstacle[i];
						if (distancias[i] < 5)
							// Este ha tenido pincho hace muy poco. Le damos cuartelillo.
							distancias[i] = 0;
						else
							++numCandidatos;
					} // for calculando las distancias
					if (numCandidatos > 0)
					{
						// Hay al menos uno donde poner.
						EColor colorACambiar = (EColor)SeleccionPonderada(distancias);
						// ¿Dónde está?
						int i = 0;
						while (currentColor[i] != colorACambiar)
							++i;
						if (t != lastChange[i])
						{
							// Tenemos el camino, y no ha cambiado ahora mismo.
							// ¡¡obstáculo!!
							lastObstacle[i] = t;
						}
					}
				} // if hemos decidido intercambiar caminos

				// Configuramos el tile.
				for (int p = 0; p < numPaths; ++p)
				{
					Tile current = _tiles[p][t] = new Tile();
					if (currentColor[p] != EColor.Black && t == lastChange[p])
						// Acabamos de cambiar este camino. Ponemos un tile blanco
						// de cortesía.
						current.color = EColor.White;
					else
						current.color = currentColor[p];
					if (lastObstacle[(int)currentColor[p]] == t)
						current.obstacle = true;
					// Añadimos vallas sin ton ni son, para que al menos se vean XD
					current.northWall = _random.Next(0, 10) == 0;
					current.southWall = _random.Next(0, 10) == 0;

					current.newLife = _random.Next(0, 20) == 0;
				} // for recorriendo los caminos
			} // for recorriendo hacia delante el mapa


		} // Init

		//----------------------------------------------------------

		/// <summary>
		/// Recibe una serie de "pesos" de varios valores, y elige uno
		/// de forma probabilística según esos pesos.
		/// </summary>
		/// <param name="pesos">Array de pesos</param>
		/// <param name="suma">Suma de los valores del array. Si no se conoce, se puede omitir y se calcula dentro</param>
		/// <returns>Índice elegido (entre 0 y pesos.Length)</returns>
		private int SeleccionPonderada(int[] pesos, int suma = 0)
		{
			int i, r;
			if (suma == 0)
			{
				// No sabemos la suma. La calculamos.
				for (i = 0; i < pesos.Length; ++i)
					suma += pesos[i];
			}
			r = _random.Next(0, suma);
			i = 0;
			while (r > pesos[i])
			{
				r -= pesos[i];
				++i;
			}
			return i;
		} // SeleccionPonderada

		//----------------------------------------------------------

		/// <summary>
		/// Recibe una serie de "pesos" de varios valores, y elige dos distintos
		/// de forma probabilística según esos pesos.
		/// *Se debe garantizar* que en pesos hay al menos dos valores distintos de 0
		/// o entraremos en un bucle infinito...
		/// </summary>
		/// <param name="pesos">Array de pesos</param>
		/// <param name="a">Primer elegido</param>
		/// <param name="b">Segundo elegido</param>
		private void SeleccionPonderada(int[] pesos, out int a, out int b)
		{
			int suma = 0;
			int i;
			for (i = 0; i < pesos.Length; ++i)
				suma += pesos[i];
			a = SeleccionPonderada(pesos, suma);
			do
			{
				b = SeleccionPonderada(pesos, suma);
			} while (a == b);

		} // SeleccionPonderada

		//----------------------------------------------------------

		/// <summary>
		/// Devuelve la información lógica de un tile.
		/// </summary>
		/// <param name="path">Camino del que se quiere obtener el tile (0-based)</param>
		/// <param name="n">Índice del tile del camino (0-based)</param>
		/// <returns>Tile correspondiente</returns>
		public Tile GetTile(int path, int n)
		{
			return _tiles[path][n];
		}

		//----------------------------------------------------------

		/// <summary>
		/// Devuelve el camino (entre 0 y NumPaths - 1) donde debe comenzar el
		/// jugador del color del parámetro.
		/// </summary>
		/// <param name="color">Color del jugador cuyo camino inicial se desea obtener.</param>
		/// <returns>Número del camino (0-based)</returns>
		public int GetColorInitialPath(EColor color)
		{
			// TODO:
			return (int)color;
		} // GetColorInitialPath

		//----------------------------------------------------------

		/// <summary>
		/// Dado un número de tile (posición de avance en los caminos) devuelve
		/// el número de camino que tiene el color del parámetro. En primer
		/// lugar se busca el camino con ese color. Si no hay, se busca un blanco.
		/// Si no hay, se devolverá -1 (no debería ocurrir)
		/// </summary>
		/// <param name="n">Posición del camino</param>
		/// <param name="color">Color buscado</param>
		/// <returns></returns>
		public int GetPathForColor(int n, EColor color)
		{
			List<int> delColor = new List<int>();
			List<int> comodines = new List<int>();

			for (int i = 0; i < NumPaths; ++i)
			{
				if (_tiles[i][n].color == color)
					delColor.Add(i);
				else if (_tiles[i][n].color.IsWildcardColor())
					comodines.Add(i);
			}

			List<int> aUsar;
			if (delColor.Count > 0)
				aUsar = delColor;
			else if (comodines.Count > 0)
				aUsar = comodines;
			else
				return -1;

			return aUsar[_random.Next(0, aUsar.Count)];

		} // GetPathForColor

		//----------------------------------------------------------

		/// <summary>
		/// Devuelve cierto si se puede ir al camino siguiente dado el camino y posición
		/// actual. Se entiende por "camino siguiente" pasar de estar en el camino n a
		/// estar en el camino n+1.
		/// </summary>
		/// <param name="path">Camino por el que se pregunta.</param>
		/// <param name="n">Posición (tile)</param>
		public bool CanGoNextPath(int path, int n)
		{
			// Podremos si no estamos ya en el último, y si el actual no tiene
			// valla arriba o el siguiente valla abajo.
			return path != (NumPaths - 1) &&
			       !_tiles[path][n].northWall && !_tiles[path + 1][n].southWall;

		} // CanGoNextPath

		//----------------------------------------------------------

		/// <summary>
		/// Devuelve cierto si se puede ir al camino anterior dado el camino y posición
		/// actual. Se entiende por "camino anterior " pasar de estar en el camino n a
		/// estar en el camino n-1.
		/// </summary>
		/// <param name="path">Camino por el que se pregunta.</param>
		/// <param name="n">Posición (tile)</param>
		public bool CanGoPrevPath(int path, int n)
		{

			// Podremos si no estamos ya en el último, y si el actual no tiene
			// valla arriba o el siguiente valla abajo.
			return path > 0 &&
				   !_tiles[path][n].southWall && !_tiles[path - 1][n].northWall;

		} // CanGoPrevPath

		#endregion

		//----------------------------------------------------------
		//                     Atributos privados
		//----------------------------------------------------------
		#region Atributos privados

		/// <summary>
		/// Tiles del mapa (_tiles[camino][tile]).
		/// </summary>
		private Tile[][] _tiles;

		/// <summary>
		/// Generador de números aleatorios.
		/// </summary>
		static System.Random _random = new System.Random();

		#endregion


	} // class Map

} // namespace