using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;
    public GameObject Lock;
    public Button button;

    [SerializeField] private LevelData leveldata;

    public bool isLocked;

    void Start()
    {
        itemName.text = leveldata.levelName;
        itemPrice.text = ("$") + leveldata.levelPrice.ToString();
        button.interactable = false;
    }

    public void unlock()
    {
        isLocked = false;
        Lock.SetActive(false);
        button.interactable = true;
    }
}
