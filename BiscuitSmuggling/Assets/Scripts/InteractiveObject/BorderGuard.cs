using cherrydev;
using UnityEngine;


[CreateAssetMenu(fileName = "BorderGuard", menuName = "Scriptable Objects/InteraciveObjectBechavior/BorderGuard")]
public class BorderGuard : InteractiveObjectBehavior
{
    [SerializeField]
    private GuardsType[] guardsTypes;
    [SerializeField]
    private string passBorder = "passBorder";

    [SerializeField]
    private string money = "moneyFatty";
    [SerializeField]
    private ItemMetadata bribeItem;

    public override void Interact()
    {
        // Check current guard type from GameManager and set corresponding dialog
        if (GameManager.Instance != null)
        {
            GuardsType currentGuardType = GameManager.Instance.GetCurrentGuardType();
            int guardTypeIndex = (int)currentGuardType;

            // Ensure the index is valid for both guardsTypes and dialogGraphs arrays
            if (dialogGraphs != null && dialogGraphs.Length > guardTypeIndex &&
                guardsTypes != null && guardsTypes.Length > guardTypeIndex)
            {
                // Index of guard type = index of dialog
                chosenGraph = dialogGraphs[guardTypeIndex];
                Debug.Log($"Selected Guard Type: {guardsTypes[guardTypeIndex]} (index: {guardTypeIndex}) with Dialog Graph: {chosenGraph.name}");

                // Start the dialog with the chosen graph
                DialogManager.Instance.ShowDialog(dialogType, chosenGraph, this);
            }
            else
            {
                Debug.LogWarning($"Guard type index {guardTypeIndex} is out of range for dialogGraphs or guardsTypes arrays.");

                // Fallback to base implementation
                base.Interact();
            }
        }
        else
        {
            Debug.LogWarning("GameManager instance not found. Using fallback dialog selection.");

            // Fallback to base implementation
            base.Interact();
        }
    }

    public override void Accept()
    {
        // Start searching cars using GameManager.
        GameManager.Instance?.StartSearchingCars();
        Debug.Log("Car search started.");
    }

    public override void Prepare(DialogBehaviour dialogBehaviour)
    {

        DialogManager.Instance.CurrentDialog.SetVariableValue(money, Inventory.Instance.GetItem(bribeItem));
        Inventory.Instance.PrepareInventoryInfo.PrepareBartender(dialogBehaviour);
    }

    public override void Reject()
    {
        // Check if game ended in success.
        var dialogManager = DialogManager.Instance;
        if (dialogManager != null && dialogManager.CurrentDialog != null)
        {
            var variablesHandler = dialogManager.CurrentDialog.VariablesHandler;
            if (variablesHandler != null)
            {
                bool passedBorder = variablesHandler.GetVariableValue<bool>(passBorder);
                if (passedBorder)
                {
                    Debug.Log("Player successfully passed the border.");
                }
                else
                {
                    Debug.Log("Player failed to pass the border.");
                }
            }
            else
            {
                Debug.LogWarning("VariablesHandler is not set in the current dialog.");
            }
        }
        else
        {
            Debug.LogWarning("DialogManager or CurrentDialog is not set.");
        }
    }
}
