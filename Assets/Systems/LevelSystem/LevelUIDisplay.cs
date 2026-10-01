using UnityEngine;

public class LevelUIDisplay : MonoBehaviour
{
 
    private int price;
    private LevelData levelData;



    void Start()
    {
        price = levelData.levelPrice;
    }
}
