using cherrydev;
using UnityEngine;

public class WarehouseExitTrigger : MonoBehaviour
{
    private bool _entered;

    [SerializeField]
    private DialogNodeGraph m_info;

    private void Update()
    {
        if (_entered && Input.GetKeyDown(KeyCode.E))
        {
            _entered = false;
            WhereToGoMenu.Instance.Show();
            DialogManager.Instance.HideInteractInfo();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _entered = true;
            DialogManager.Instance.ShowInteractInfo(m_info);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _entered = false;
            DialogManager.Instance.HideInteractInfo();
        }
    }
}
