//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using UnityEngine;

/// <summary>
/// Clase que controla el desplazamiento de la cámara del nivel.
/// La gestiona el LevelManager estableciendo su velocidad de desplazamiento.
/// </summary>
public class CameraController : MonoBehaviour
{
	//----------------------------------------------------------
	//                     Métodos públicos
	//----------------------------------------------------------
	#region Métodos públicos

	/// <summary>
	/// Establece la velocidad de desplazamiento (en el eje X). Se
	/// aplicará esta velocidad en el siguiente update.
	/// </summary>
	/// <param name="v"></param>
	public void SetVelocity(float v)
	{
		_velocity = v;
	} // SetVelocity

	#endregion

	//----------------------------------------------------------
	//                     Métodos de MonoBehaviour
	//----------------------------------------------------------
	#region Métodos de MonoBehaviour


	/// <summary>
	/// Método llamado por Unity en cada frame. Actualizamos la posición.
	/// </summary>
	void Update()
	{
		transform.position += new Vector3(_velocity * Time.deltaTime, 0, 0);
	}

	#endregion

	//----------------------------------------------------------
	//                     Atributos privados
	//----------------------------------------------------------
	#region Atributos privados

	/// <summary>
	/// Velocidad de desplazamiento (en el eje X) por unidad de tiempo.
	/// </summary>
	private float _velocity = 0.0f;

	#endregion

} // class CameraController

