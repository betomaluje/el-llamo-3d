using System.Collections.Generic;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Randomizer
{
    private int _amount;
    List<int> bannedIndexes = new List<int>();

    public int Amount { get => _amount; }

    public Randomizer(int amount)
    {
        Assert.IsTrue(amount > 0, "Randomizer can work only with amounts > 0");
        _amount = amount;
        bannedIndexes = new List<int>(_amount);

    }

    /// <summary>
    /// Select without repeat twice the same value.
    /// </summary>
    public int SelectNoRepeat()
    {
        if (_amount == 1)
        {
            return 0;
        }

        int index = Random.Range(0, _amount);

        int attempts = 1000;

        while (bannedIndexes.Contains(index) && attempts > 0)
        {
            attempts--;
            index = Random.Range(0, _amount);
        }

        bannedIndexes.Add(index);

        return index;
    }

    public void ClearBanned()
    {
        bannedIndexes.Clear();
    }
}
