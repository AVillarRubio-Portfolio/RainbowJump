//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using UnityEngine;

/// <summary>
/// Clase para configurar un tile gráfico.
/// </summary>
public class TileController : MonoBehaviour
{
	//----------------------------------------------------------
	//                Propiedades públicas (inspector)
	//----------------------------------------------------------
	#region Propiedades públicas (inspector)

	[Tooltip("Mallas de los tiles con el material a los que se les establece el color")]
	public MeshRenderer[] groundTiles;

	[Tooltip("Game objects con la representación de las vallas del lado superior")]
	public GameObject[] northFences;

	[Tooltip("Game objects con la representación de las vallas del lado inferior")]
	public GameObject[] southFences;

	[Tooltip("Game objects con la representación de los pinchos (obstáculo que hay que saltar)")]
	public GameObject[] obstacles;

	[Tooltip("Game object de la nueva vida")]
	public GameObject newLifeGO;

	#endregion

	//----------------------------------------------------------
	//                     Métodos públicos
	//----------------------------------------------------------
	#region Métodos públicos

	/// <summary>
	/// Recibe la información de un tile lógico y la utiliza para configurar el tile
	/// visual del prefab.
	/// </summary>
	/// <param name="color">Color del tile</param>
	/// <param name="northFence">Cierto si hay que hacer visible la valla superior</param>
	/// <param name="southFence">Cierto si hay que hacer visible la valla inferior</param>
	/// <param name="obstacle">Cierto si hay que hacer visible el obstáculo</param>
	public void Configure(Color color, bool northFence, bool southFence, bool obstacle, bool newLife)
	{

		foreach (var ms in groundTiles) {
			ms.material.color = color;
		}
		foreach (var go in northFences)
		{
			go.SetActive(northFence);
		}
		foreach (var go in southFences)
		{
			go.SetActive(southFence);
		}
		foreach (var go in obstacles)
		{
			go.SetActive(obstacle);
		}
		newLifeGO.SetActive(newLife);

	} // Configure

	#endregion

} // class TileController
