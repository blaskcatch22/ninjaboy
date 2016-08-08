using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class menuActions : MonoBehaviour {

	public void reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void quit()
    {
        Application.Quit();
    }
}
