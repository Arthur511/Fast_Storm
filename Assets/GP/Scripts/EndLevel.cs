using UnityEngine;

public class EndLevel : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            MainGame.Instance.UIManager.DisplayScorePanel(MainGame.Instance.PlayerController.Score);
            MainGame.Instance.PlayerController.IsOnPause = true;
        }
    }

}
