//----------------------------------------------------------
// Rainbow Color
// Juego desarrollado para la Global Game Game, 2022
// por el equipo de profesores organizadores de FDI-UCM
//----------------------------------------------------------
// Todos los derechos reservados
//----------------------------------------------------------

using UnityEngine;

using Mgr;

namespace UI
{
    public class MainMenuController : MonoBehaviour
    {

        public void PlayClick()
        {
            GameManager.Instance.LaunchSelectPlayers();
        }

        public void CreditsClick()
        {
            GameManager.Instance.ShowCredits();
        }

        public void Exit()
        {
            Application.Quit();
        }

    }

} // namespace