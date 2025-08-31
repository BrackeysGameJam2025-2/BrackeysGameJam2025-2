using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUp : MonoBehaviour
{
    [SerializeField]
    private string[] m_FmodBanks = { "Master", "SFX", "Dialog", "Soundtrack" };


    private IEnumerator Start()
    {
        foreach (var bank in m_FmodBanks)
        {
            RuntimeManager.LoadBank(bank, true); // async load
        }

        // Wait for all banks to finish loading
        bool loading = true;
        while (loading)
        {
            if (RuntimeManager.HasBankLoaded(m_FmodBanks[^1]))
            {
                loading = false;
            }
            yield return null;
        }

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
