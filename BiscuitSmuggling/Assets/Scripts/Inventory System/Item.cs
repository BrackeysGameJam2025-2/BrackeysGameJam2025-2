using System.ComponentModel.Design;
using UnityEngine;
[System.Serializable]
public abstract class Item
{
    public abstract string GiveName();

    public virtual int MaxStacks()
    {
        return 10;
    }

    public virtual Sprite GiveItemImage()
    {
        return Resources.Load<Sprite>("UI/Item Image/No Item Image Icon");
    }
}
