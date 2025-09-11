using UnityEngine;
using UnityEngine.UI;

public class RollButton : MonoBehaviour
{
    private void Awake()
    {
        transform.GetComponent<Button>().onClick.AddListener(OnRoll);
    }

    void Start()
    {
        
    }

    private void OnRoll()
    {
        Debug.Log("Rolled the shop items!");
    }
}
