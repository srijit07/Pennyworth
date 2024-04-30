using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExpenseDetails : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDetails()
    {
        GameObject scriptHub = GameObject.FindGameObjectWithTag("ScriptHub");
        HomeScreenManager homeScreenManager = scriptHub.GetComponent<HomeScreenManager>();

        string id = gameObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text[4..];
        string type = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        string message = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        string amount = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;
        string date = gameObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text;
        string time = gameObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text;

        homeScreenManager.ShowExpenseDetails(id, type, message, amount, date, time);
    }
}
