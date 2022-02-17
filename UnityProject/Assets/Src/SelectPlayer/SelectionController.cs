//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control general de la pantalla de selección de personajes
/// </summary>
/// Simplemente comprueba si hay algún stand ya con personaje/jugador
/// para habilitar el botón "Jugar".
/// También es el que se encarga de capturar el evento de jugar
/// para configurar el GameManager con los jugadores seleccionados
/// y lanzar el juego.
public class SelectionController : MonoBehaviour
{

    /// <summary>
    /// Botón de "play" que se habilita cuando hay algún jugador.
    /// </summary>
    public UnityEngine.UI.Button playButton;

    /// <summary>
    /// Texto del botón del play cuando está habilitado.
    /// </summary>
    public UnityEngine.UI.Text textButtonEnabled;

    /// <summary>
    /// Texto del botón del play cuando está deshabilitado.
    /// </summary>
    public UnityEngine.UI.Text textButtonDisabled;

    /// <summary>
    /// Control de cada jugador para saber si están o no
    /// seleccionados (por polling...)
    /// </summary>
    public StandController[] jugadores;

    private bool algunJugador = false;

    // Start is called before the first frame update
    void Start()
    {
        playButton.interactable = false;
        textButtonEnabled.enabled = false;
        textButtonDisabled.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        algunJugador = false;
        foreach (var s in jugadores)
            algunJugador |= s.isEnabled;

        if (algunJugador)
        {

            // Ya se ha activado algún jugador
            // ¿Hay colores repetidos? Soy tope chungo.
            bool repetidos = false;
            for (int i = 0; i < jugadores.Length; ++i)
                if (jugadores[i].isEnabled)
                    jugadores[i].SetError(false);
            for (int i = 0; i < jugadores.Length; ++i)
			{
                bool esteRepetido = false;
                if (jugadores[i].isEnabled && !jugadores[i].inError)
				{
                    for (int j = i + 1; j < jugadores.Length /*&& !repetidos*/; ++j)
					{
                        // No salimos por si hay triple empate.
                        if (jugadores[j].isEnabled && jugadores[i].currentIndex == jugadores[j].currentIndex)
                        {
                            // Ouch. Colisión de color.
                            esteRepetido = true;
                            jugadores[i].SetError(true);
                            jugadores[j].SetError(true);
                        }
                    }
                    if (!esteRepetido)
                        jugadores[i].SetError(false);
                }
                repetidos |= esteRepetido;
			} // for i
            // Habilitamos el botón
            textButtonEnabled.enabled = playButton.interactable = !repetidos;
            textButtonDisabled.enabled = repetidos;
        }
    }

    public void LaunchGame()
    {

        // Configuramos el GameManager
        List<Mgr.GameManager.PlayerConfig> pc = new List<Mgr.GameManager.PlayerConfig>();
        foreach (var s in jugadores)
        {
            if (s.isEnabled)
            {
                Mgr.GameManager.PlayerConfig newPlayer = new Mgr.GameManager.PlayerConfig();
                newPlayer.colorSpec = s.GetColorSpec();
                newPlayer.bindings = s.bindings;
                pc.Add(newPlayer);
            }
        }

        // Lanzamos el nivel con esos jugadores.
        Mgr.GameManager.Instance.LaunchRandomLevel(pc.ToArray());

    }

}
