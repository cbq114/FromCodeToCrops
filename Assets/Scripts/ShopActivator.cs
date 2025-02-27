using UnityEngine;
using UnityEngine.InputSystem;

public class ShopActivator : MonoBehaviour
{
    private bool canOpen;

    // Update is called once per frame
    void Update()
    {
        if(canOpen == true)
        {
            if(Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
            {
                if(UIController.instance.theShop.gameObject.activeSelf == false)
                {
                    UIController.instance.theShop.OpenClose();

                    AudioManager.instance.PlaySFX(0);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            canOpen = false;
        }
    }
}
