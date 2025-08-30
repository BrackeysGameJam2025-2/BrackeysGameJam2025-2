public class MoneyInteractoveObject : InteractiveObject
{
    /*public override void Interact()
    {
        behavior.OnInteractionResult += OnAccept;
        behavior.Interact();
    }*/
    protected override void OnAccept(bool obj)
    {
        behavior.OnInteractionResult -= OnAccept;
        if (!preventMultipleInteractions)
            return;
        if (obj)
        {
            playerInArea.PlayerOutArea(this);
            gameObject.SetActive(false);
        }
    }
}
