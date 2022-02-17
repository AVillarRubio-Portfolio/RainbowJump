//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;

namespace GameUI
{

	public class LabelView : MonoBehaviour
	{
		//---------------------------------------------------------
		//                   Métodos de MonoBehaviour
		//---------------------------------------------------------
		#region Métodos de MonoBehaviour

		private void Start()
		{
			_text = GetComponent<Text>();
			if (_text == null)
			{
				Debug.LogError("No tengo Text");
				Destroy(gameObject);
			}
		}

		#endregion

		//---------------------------------------------------------
		//                   Métodos públicos
		//---------------------------------------------------------
		#region Métodos públicos

		public async Task ShowAndVanishAsync(string text, Color color, Vector3 endScale, float staticTime, float vanishTime)
		{
			_text.transform.localScale = Vector3.one;
			_text.text = text;
			_text.color = color;
			_text.enabled = true;
			Color endColor = color;
			// Espera casera del tiempo.
			float initTime = Time.time;
			float endTime = initTime + staticTime;
			while (Time.time < endTime)
			{
				await Task.Yield();
			}

			endColor.a = 0;
			var scaleAsync = BasicAnims.ScaleAnimAsync(transform, Vector3.one, endScale, vanishTime);
			var colorAsync = BasicAnims.ColorAnimAsync(_text, color, endColor, vanishTime);
			//Task.WaitAll(scaleAsync, colorAsync); // No sé por qué no puedo esperar a ambas así :-m Se cuelga Unity.
			await scaleAsync;
			await colorAsync;
			_text.enabled = false;
		}

		#endregion

		//----------------------------------------------------------
		//                     Atributos privados
		//----------------------------------------------------------
		#region Atributos privados

		/// <summary>
		/// Etiqueta que animamos y controlamos.
		/// </summary>
		Text _text;

		#endregion


	} // class LabelView


} // namespace GameUI