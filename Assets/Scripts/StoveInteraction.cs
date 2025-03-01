using UnityEngine;
using UnityEngine.InputSystem;

public class StoveInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            UIController.instance.ShowMessage("Nhấn E để nấu ăn");
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    
    private void Update()
    {
        if (playerInRange && (Keyboard.current.eKey.wasPressedThisFrame || Input.GetKeyDown(KeyCode.E)))
        {
            CookingSystem.instance.OpenCookingPanel();
        }
    }
}