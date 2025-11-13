using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WarningScript : MonoBehaviour {

    [SerializeField] Button noButton;
    [SerializeField] Button yesButton;

    TaskCompletionSource<bool> tcs;

    void Awake() {
        yesButton.onClick.AddListener(() => Close(true));
        noButton.onClick.AddListener(() => Close(false));
    }

    public Task<bool> WaitForUser() {
        gameObject.SetActive(true);
        tcs = new TaskCompletionSource<bool>();
        return tcs.Task;
    }

    private void Close( bool result ) {
        gameObject.SetActive(false);
        tcs?.TrySetResult(result);
    }
}
