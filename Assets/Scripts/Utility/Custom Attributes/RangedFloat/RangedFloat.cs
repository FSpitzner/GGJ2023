using System;

[Serializable]
public struct RangedFloat {
    public float minValue;
    public float maxValue;
	///<summary>Checks if the value is within the ranged float</summary>
	public bool Contains(float value)
	{
		if (value <=maxValue && value >=minValue)
		{ return true; }
		else
		{ return false; }
	}
}