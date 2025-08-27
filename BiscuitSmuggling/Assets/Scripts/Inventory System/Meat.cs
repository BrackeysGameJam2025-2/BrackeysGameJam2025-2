using UnityEngine;

public class Meat : Item
{
    public override string GiveName()
    {
        return "Meat";
    }

    public override int MaxStacks()
    {
        return 10;
    }

    public override Sprite GiveItemImage()
    {
        return Resources.Load<Sprite>("UI/Item Image/Meat");
    }
}
