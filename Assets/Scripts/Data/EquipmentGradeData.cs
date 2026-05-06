using UnityEngine;

[CreateAssetMenu(fileName = "GradeData", menuName = "Scriptable Objects/GradeData")]
public class EquipmentGradeData : ScriptableObject
{
    public Sprite normal;
    public Sprite rare;
    public Sprite epic;
    public Sprite legend;

    public Sprite normalBackground;
    public Sprite rareBackground;
    public Sprite epicBackground;
    public Sprite legendBackground;

    public Sprite GetSprite(GradeType grade)
    {
        return grade switch
        {
            GradeType.Normal => normal,
            GradeType.Rare => rare,
            GradeType.Epic => epic,
            GradeType.Legend => legend,
            _ => null
        };
    }

    public Sprite GetSpriteBackground(GradeType grade)
    {
        return grade switch
        {
            GradeType.Normal => normalBackground,
            GradeType.Rare => rareBackground,
            GradeType.Epic => epicBackground,
            GradeType.Legend => legendBackground,
            _ => null
        };
    }
}
