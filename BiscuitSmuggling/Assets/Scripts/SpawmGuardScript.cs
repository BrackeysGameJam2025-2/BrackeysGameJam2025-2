using UnityEngine;

public class SpawmGuardScript : MonoBehaviour
{
    [SerializeField] Transform GuardSpawnPoint;
    [SerializeField] GameObject FatGuard;
    [SerializeField] GameObject VeteranGuard;

    private void Start()
    {
        GuardsType currentGuardType = GameManager.Instance.GetCurrentGuardType();
        Debug.Log(currentGuardType);

        if (currentGuardType == GuardsType.Fat)
        {
            Instantiate(FatGuard, GuardSpawnPoint.position, Quaternion.identity);
        }
        else if (currentGuardType == GuardsType.Veteran)
        {
            Instantiate(VeteranGuard, GuardSpawnPoint.position, Quaternion.identity);
        }
    }
}
