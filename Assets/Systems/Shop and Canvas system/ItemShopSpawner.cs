using UnityEngine;

public class ItemShopSpawner : MonoBehaviour
{
    [SerializeField]
    private Transform itemSpawnTransform;
    public void SpawnItem(OldItemData purchasedItem)
    {
        if (itemSpawnTransform != null)
        {
            Instantiate(purchasedItem.pickupPrefab, itemSpawnTransform.position, itemSpawnTransform.rotation);
        }

    }
}
