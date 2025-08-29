using UnityEngine;

public class dogtag : Item
{
    public override string GiveName()
    {
        return "Dogtag";
    }

    public override int MaxStacks()
    {
        return 5;
    }

    public override Sprite GiveItemImage()
    {
        return Resources.Load<Sprite>("UI/Item Image/Dogtag");
    }
}
