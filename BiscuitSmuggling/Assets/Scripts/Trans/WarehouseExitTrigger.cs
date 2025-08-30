using UnityEngine;

public class WarehouseExitTrigger : MonoBehaviour
{
    private bool _entered;

    private void Update()
    {
        if (_entered && Input.GetKeyDown(KeyCode.E))
        {
            WhereToGoMenu.Instance.Show();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _entered = true;
            InteractLabel.Instance.Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _entered = false;
            InteractLabel.Instance.Hide();
        }
    }
}
