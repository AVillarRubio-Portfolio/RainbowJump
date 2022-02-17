//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;
using System.Threading;

namespace GameUI
{

	/// <summary>
	/// Clase con m�todos est�ticos que proporcionan m�todos as�ncronos para
	/// hacer animaciones sencillas por c�digo.
	/// </summary>
	public class BasicAnims
	{

		//---------------------------------------------------------
		//                Animaciones del transform
		//---------------------------------------------------------
		#region Animaciones del transform

		/// <summary>
		/// Realiza una animaci�n de la escala local de un Transform.
		/// </summary>
		/// <param name="t">Transform a animar.</param>
		/// <param name="initScale">Escala inicial</param>
		/// <param name="endScale">Escala final</param>
		/// <param name="seconds">Duraci�n (en segundos) de la animaci�n.</param>
		/// <returns>Corrutina</returns>
		public static async Task ScaleAnimAsync(Transform t, Vector3 initScale, Vector3 endScale, float seconds)
		{

			float initTime = Time.time;
			float endTime = initTime + seconds;

			t.localScale = initScale;
			while (Time.time < endTime)
			{
				t.localScale = Vector3.Lerp(initScale, endScale, (Time.time - initTime) / seconds);
				await Task.Yield();
			}
			t.localScale = endScale;

		} // ScaleAnimCoroutine

		#endregion

		//---------------------------------------------------------
		//                Animaciones sobre UI.Text
		//---------------------------------------------------------
		#region Animaciones sobre UI.Text

		/// <summary>
		/// Realiza una animaci�n del color de una UI.Text. Es �til t�picamente para hacer aparecer o desaparecer
		/// de forma progresiva un texto modificando su alfa.
		/// </summary>
		/// <param name="image">Texto a animar.</param>
		/// <param name="initColor">Color inicial</param>
		/// <param name="endColor">Color final</param>
		/// <param name="seconds">Duraci�n (en segundos) de la animaci�n.</param>
		public static async Task ColorAnimAsync(Text text, Color initColor, Color endColor, float seconds)
		{
			float initTime = Time.time;
			float endTime = initTime + seconds;

			text.color = initColor;
			while (Time.time < endTime)
			{
				text.color = Color.Lerp(initColor, endColor, (Time.time - initTime) / seconds);
				await Task.Yield();
			}
			text.color = endColor;

		} // ColorAnimAsync

		#endregion

	} // BasicAnims

} // namespace