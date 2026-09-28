using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



public class EndDevice : MonoBehaviour
{
    public static EndDevice current { get; private set; }
    void Awake()
    {
        current = this;
    }

    public OldItemData.ItemType acceptedType = OldItemData.ItemType.Item;
    public List<OldItemData> receivedItems = new List<OldItemData>();

    public int Quota = 3; // Number of items required to end the game
    [SerializeField] private Inventory playerInventory;

    private bool isGrinding;

    [Header("Grinder Visual")]
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private Transform grindPoint;

    [SerializeField] private float lowerDuration = 2f;

    [Header("Sparks")]
    [SerializeField] private ParticleSystem sparks;
    [SerializeField] private float sparksDuration = 0.8f;

    [Header("Floating Value")]
    [SerializeField] private NumberVisual numberVisual;
    [SerializeField] private Transform valueSpawnPoint;

    [Header("Audio")]
    [SerializeField] private AudioSource grinderAudioSource;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    public void TryReceiveFromInventory()
    {
        if (isGrinding)
        {
            Debug.Log("Grinder is currently processing an item.");
            return;
        }

        if (playerInventory == null)
        {
            Debug.LogError("No inventory found!");
            return;
        }
        if (playerInventory.itemSlots == null ||
        playerInventory.itemSlots.Length == 0)
        {
            Debug.LogError("Inventory has no item slots.");
            return;
        }
        OldItemSlot selectedSlot = playerInventory.itemSlots[playerInventory.currentIndex];
        if (selectedSlot.itemInSlot == null)
        {
            Debug.Log("No item in selected slot.");
            return;
        }
        if (selectedSlot.itemInSlot.itemType != acceptedType)
        {
            Debug.Log("Selected item is not the correct type.");
            return;
        }
        

        OldItemData received = selectedSlot.itemInSlot;
        playerInventory.RemoveItem(received);
        receivedItems.Add(received);

        
        
        int itemValue = received.value + Random.Range(1, 10);
        GameManager.current.addMoney(itemValue);
        GameManager.current.totalLootCollected++;
        GameManager.current.totalMoneyEarned += itemValue;

        isGrinding = true;

        StartCoroutine(GrindItem(received, itemValue));

       
        //GameManager.current.currentQuota++;
        


        Debug.Log($"Device received: {received.itemName}");
        if (receivedItems.Count >= Quota)
        {
            GameManager.current.canStartGame = true;
        }
    }

    private IEnumerator GrindItem(OldItemData itemData, int itemValue)
    {
        
        if (itemData.pickupPrefab == null)
        {
            Debug.LogWarning(
                $"No pickupPrefab assigned to {itemData.itemName}"
            );

            isGrinding = false;
            yield break;
        }

        if (itemSpawnPoint == null)
        {
            Debug.LogError(
                "Grinder Item Spawn Point has not been assigned!"
            );

            isGrinding = false;
            yield break;
        }

        if (grindPoint == null)
        {
            Debug.LogError(
                "Grinder Grind Point has not been assigned!"
            );

            isGrinding = false;
            yield break;
        }

        GameObject grindingItem = Instantiate
        (
            itemData.pickupPrefab,
            itemSpawnPoint.position,
            itemSpawnPoint.rotation
        );
        Rigidbody rb = grindingItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider[] colliders = grindingItem.GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        Vector3 startPosition = itemSpawnPoint.position;
        Vector3 endPosition = grindPoint.position;

        float elapsed = 0f;

        while (elapsed < lowerDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / lowerDuration;

            
            t = Mathf.SmoothStep(0f, 1f, t);

            grindingItem.transform.position =
                Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        grindingItem.transform.position = endPosition;


        Debug.Log("Item has reached the grinder. Starting sparks.");

        if (animator != null)
        {
            animator.SetBool("Grinding", true);
        }

        if (sparks != null)
        {
            sparks.Play();
        }

        if (SoundManager.current != null && grinderAudioSource != null)
        {
            SoundManager.current.PlayLoop("Grinder", grinderAudioSource);
        }


        yield return new WaitForSeconds(sparksDuration);

        if (animator != null)
        {
            animator.SetBool("Grinding", false);
        }

        if (sparks != null)
        {
            sparks.Stop();
        }

        if (SoundManager.current != null && grinderAudioSource != null)
        {
            SoundManager.current.StopLoop(grinderAudioSource);
        }

        Destroy(grindingItem);

        
        if (numberVisual != null && valueSpawnPoint != null)
        {
           
            NumberVisual valueText = Instantiate(numberVisual, valueSpawnPoint.position, valueSpawnPoint.rotation);

            valueText.ShowValue(itemValue);

            if (SoundManager.current != null && grinderAudioSource != null)
            {
                SoundManager.current.PlayOneShotSFX("Cash", grinderAudioSource);
            }
        }
        else
        {
            Debug.LogWarning("floating number not workin");
        }


            isGrinding = false;
    }

}