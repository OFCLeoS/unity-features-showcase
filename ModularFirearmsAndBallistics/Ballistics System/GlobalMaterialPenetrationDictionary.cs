using System.Collections.Generic;

public static class GlobalMaterialPenetrationDictionary
{
    public static readonly IReadOnlyDictionary<MaterialType, float> MaterialPenetrationResistancePairs;
    
    static GlobalMaterialPenetrationDictionary()
    {
        MaterialPenetrationResistancePairs = new Dictionary<MaterialType, float>
        {
            {MaterialType.WOOD,1},
            {MaterialType.SHEET_METAL,0.5f},
            {MaterialType.CONCRETE,10},
        };
    }
}