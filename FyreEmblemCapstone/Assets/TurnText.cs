using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Text text = this.GetComponent<Text>();
        if(TurnManager.Instance != null) {
            if(TurnManager.Instance.CurrentUnit != null) {
                if(TurnManager.Instance.CurrentUnit.name != null)
                {
                    text.text = TurnManager.Instance.CurrentUnit.name;
                }
            }
        }
    }
}
