using UnityEngine;

public class CheckpointUIScript : MonoBehaviour
{
    [SerializeField] TrackCheckpoints trackCheckpoints;

    private void Start()
    {
        trackCheckpoints.OnPlayerCorrectCheckpoint += OnCorrect;
        trackCheckpoints.OnPlayerIncorrectCheckpoint += OnIncorrect;

        Hide();
    }

    private void OnCorrect(object sender, System.EventArgs e){
        Hide();
    }

    private void OnIncorrect(object sender, System.EventArgs e){
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
