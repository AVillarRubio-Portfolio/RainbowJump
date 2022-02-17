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
/// Clase para controlar el comportamiento de un "stand" de
/// selección de personaje.
/// </summary>
/// Se configura con el input al que representa; cuando se
/// pulsa el botón de acción se activa de forma a partir de
/// ese momento se podrá seleccionar el personaje que se
/// quiere manejar asociado a ese jugador.
public class StandController : MonoBehaviour
{
    /// <summary>
    /// Objeto que tiene por debajo el stand a mostrar
    /// cuando se ha activado (base, luces, ...)
    /// </summary>
    public GameObject standEnabled;

    /// <summary>
    /// Objeto que tiene por debajo el stand que se
    /// muestra cuando está desactivado (aún no se
    /// ha dado al botón de "acción" asociado al jugador).
    /// </summary>
    public GameObject standDisabled;

    /// <summary>
    /// Teclas utilizadas para este stand
    /// </summary>
    public SO.ControlBindings bindings;

    /// <summary>
    /// Punto donde se coloca el personaje seleccionado.
    /// </summary>
    public Transform standPoint;
#if NO

    /// <summary>
    /// TextMesh en el que poner el nombre
    /// </summary>
    public TextMesh textoNombre;
#endif

    /// <summary>
    /// Sprite con los controles del jugador.
    /// </summary>
    public SpriteRenderer instructionsSprite;

    public PlayerController playerPrefab;

    public int initialColor = 0;

    /// <summary>
    /// A true cuando el stand ya ha sido activado por la tecla
    /// de activación y está seleccionando el personaje.
    /// </summary>
    [HideInInspector]
    public bool isEnabled;

    /// <summary>
    /// GameObject con el player instanciado una vez que se ha pulsado la
    /// tecla de acción de estos bindings.
    /// </summary>
    private PlayerController currentPlayer;

    /// <summary>
    /// Índice al array de colores actual.
    /// </summary>
    [HideInInspector]
    public int currentIndex;

#if NO
    /// <summary>
    /// Índice al array anterior por el que empieza cuando
    /// se activa.
    /// </summary>
    public int initialCharacter = 0;


    /// <summary>
    /// Game object actual con el personaje seleccionado
    /// (cuelga de StandPoint).
    /// </summary>
    private GameObject currentPlayer;

    /// <summary>
    /// Índice al Character actual seleccionado (al array characters).
    /// </summary>
    [HideInInspector]
    public int currentIndex;

#endif
    
    private bool ignoreAxis = false;

    // Habilita y desabilita lo que corresponda
    void Start()
    {
        // Sanity-check
        if ((standEnabled == null) || (standDisabled == null) || (bindings == null))
        {
            Debug.Log("StandController mal configurado. Nos destruimos.");
            DestroyImmediate(this);
        }
        isEnabled = false;
        standEnabled.SetActive(false);
        standDisabled.SetActive(true);

    }

    // Comprueba las teclas y actúa en consecuencia
    void Update()
    {
        if (Input.GetButtonDown(bindings.actionButton) && !isEnabled)
        {
            // Nos activamos
            isEnabled = true;
            standEnabled.SetActive(true);
            standDisabled.SetActive(false);
            currentPlayer = Instantiate(playerPrefab, standPoint);
            currentPlayer.FreezeForever();
            selectCharacter(initialColor);
        }

        if (!isEnabled)
            return;

        float x = Input.GetAxis(bindings.xAxis);
        float y = Input.GetAxis(bindings.yAxis);
        if (!ignoreAxis)
        {
            if ((Mathf.Abs(x) > 0.2) || (Mathf.Abs(y) > 0.2))
            {
                int newIndex = currentIndex;
                if (Mathf.Abs(x) > 0.2) {
                    if (x > 0.2)
                    {
                        newIndex++;
                    }
                    else
                        newIndex--;
                }
                else {
                    if (y > 0.2)
                    {
                        newIndex--;
                    }
                    else
                        newIndex++;
                }

                // HACK: cableo que hay 4 colores, que en el GameManager
                // están todos XD
                // Además tienen que estar en orden...
                newIndex += 4;
                newIndex %= 4;
                selectCharacter(newIndex);
            }
        }
        ignoreAxis = Mathf.Abs(x) > 0.2 || Mathf.Abs(y) > 0.2;
    }

    /// <summary>
    /// Devuelve la especificación del color del color seleccionado.
    /// </summary>
    /// <returns></returns>
    public SO.ColorSpec GetColorSpec()
	{
        return Mgr.GameManager.Instance.GetColorSpec((Logic.EColor)currentIndex);

    }

    public void SetError(bool error)
	{
        if (error)
        {
            instructionsSprite.color = Color.red;
        }
        else
            selectCharacter(currentIndex);
        inError = error;
	}

    void selectCharacter(int index)
    {

        currentIndex = index; // Antes que lo siguiente, que lo usa XD
        // Configuramos el personaje.
        currentPlayer.ConfigureColor(GetColorSpec());
        Color c = GetColorSpec().color;
        //c.a = 128;
        instructionsSprite.color = c;

    }

    public bool inError;

}
