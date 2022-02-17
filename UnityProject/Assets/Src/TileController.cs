//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using UnityEngine;

/// <summary>
/// Clase para configurar un tile gr�fico.
/// </summary>
public class TileController : MonoBehaviour
{
	//----------------------------------------------------------
	//                Propiedades p�blicas (inspector)
	//----------------------------------------------------------
	#region Propiedades p�blicas (inspector)

	[Tooltip("Mallas de los tiles con el material a los que se les establece el color")]
	public MeshRenderer[] groundTiles;

	[Tooltip("Game objects con la representaci�n de las vallas del lado superior")]
	public GameObject[] northFences;

	[Tooltip("Game objects con la representaci�n de las vallas del lado inferior")]
	public GameObject[] southFences;

	[Tooltip("Game objects con la representaci�n de los pinchos (obst�culo que hay que saltar)")]
	public GameObject[] obstacles;

	[Tooltip("Game object de la nueva vida")]
	public GameObject newLifeGO;

	#endregion

	//----------------------------------------------------------
	//                     M�todos p�blicos
	//----------------------------------------------------------
	#region M�todos p�blicos

	/// <summary>
	/// Recibe la informaci�n de un tile l�gico y la utiliza para configurar el tile
	/// visual del prefab.
	/// </summary>
	/// <param name="color">Color del tile</param>
	/// <param name="northFence">Cierto si hay que hacer visible la valla superior</param>
	/// <param name="southFence">Cierto si hay que hacer visible la valla inferior</param>
	/// <param name="obstacle">Cierto si hay que hacer visible el obst�culo</param>
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
