using UnityEngine;

public class PseudoRandomNumberGenerator
{
    private float[] numberTolerances;
    private readonly int _forOthersPortion;
    
    public PseudoRandomNumberGenerator(int numberOfOptions)
    {
        numberTolerances = new float[numberOfOptions];
        for (int i = 0; i < numberOfOptions; i++)
        {
            numberTolerances[i] = 1.0f / numberOfOptions;
        }

        _forOthersPortion = numberOfOptions - 1;
    }

    public int GetPseudoRandomNumber()
    {
        float randomNumber = Random.value;
        // float result = 0;
        int result = 0;
        float countValue = 0;
        int index = 0;
        for (int i = 0; i < numberTolerances.Length; i++)
        {
            if (randomNumber < numberTolerances[i] + countValue)
            {
                index = i;
                // result = 1.0f / numberTolerances.Length * (i);
                result = i;
                break;
            }

            countValue += numberTolerances[i];
        }

        float minusValue = numberTolerances[index] / 2;
        for (int i = 0; i < numberTolerances.Length; i++)
        {
            if (i == index)
            {
                numberTolerances[i] -= minusValue;
            }
            else
            {
                numberTolerances[i] += minusValue / _forOthersPortion;
            }
        }

        return result;
    }
}
