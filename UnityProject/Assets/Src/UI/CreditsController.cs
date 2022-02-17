using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time - startTime) > 23)
            BackToMainMenu();
    }

    public void BackToMainMenu()
	{
        Mgr.GameManager.Instance.GoToMenu();
	}

    private float startTime;

} // CreditsController
