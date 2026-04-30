using UnityEngine;

[CreateAssetMenu(fileName = "GradeData", menuName = "Scriptable Objects/GradeData")]
public class EquipmentGradeData : ScriptableObject
{
    public Sprite normal;
    public Sprite rare;
    public Sprite epic;
    public Sprite legend;

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
}
