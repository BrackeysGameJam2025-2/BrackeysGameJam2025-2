using cherrydev;
using UnityEngine;

public class LeaveLocationTrigger : MonoBehaviour
{
    private bool _entered;

    [SerializeField]
    private DialogNodeGraph m_info;

    private bool CanExit() => !ClankerHivemind.Exists ||
        (!ClankerHivemind.Instance.AnyoneChases && !ClankerHivemind.Instance.IsBusted);

    private void Update()
    {
        if (_entered)
        {
            if (!CanExit())
            {
                _entered = false;
                DialogManager.Instance.HideInteractInfo();
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                TransitionManager.GoToWarehouse();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && CanExit())
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
