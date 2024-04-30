using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class HomeScreenManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI welcomeUsername;
    [SerializeField]
    GameObject AddExpensePanel, ViewExpensesPanel;
    [SerializeField]
    Animator animationPanel;
    [SerializeField]
    float autoRefreshTime;
    float refreshTimer;
    [SerializeField]
    Animator creditsAnimator, transitionPanelAnimator;
    string previousAmount = "0", newAmount;
    int numberOfExpenses = 0;
    string latestVersionNumber;

    #region HomeScreenVariables
    [SerializeField]
    TextMeshProUGUI totalExpenditureText;
    #endregion

    #region AddExpenseVariables
    [SerializeField]
    GameObject typeAlreadyAdded, typeHasToBeAdded;
    [SerializeField]
    TMP_Dropdown type;
    [SerializeField]
    TMP_InputField addType;
    [SerializeField]
    Button addTypeButton;
    [SerializeField]
    TMP_InputField message;
    [SerializeField]
    TMP_InputField amount;
    [SerializeField]
    TMP_InputField dateDay, dateMonth, dateYear;
    [SerializeField]
    TMP_InputField timeHour, timeMinute;
    [SerializeField]
    TMP_Dropdown timeAmPm;
    [SerializeField]
    TextMeshProUGUI addStatusText;
    #endregion

    #region ViewExpenseVariables
    [SerializeField]
    GameObject expensePrefab;
    [SerializeField]
    GameObject verticalLayoutGroupParent;
    [SerializeField]
    TMP_Dropdown timeFrame;
    [SerializeField]
    TMP_Dropdown orderBy;
    [SerializeField]
    TextMeshProUGUI statusViewExpenses;
    [SerializeField]
    ScrollRect viewScroll;
    [SerializeField]
    GameObject detailsPanel;
    [SerializeField]
    TextMeshProUGUI detailedType, detailedMessage, detailedAmount, detailedDate, detailedTime;
    [SerializeField]
    GameObject modifyExpense;
    #endregion

    #region AccountVariables
    [SerializeField]
    TextMeshProUGUI name, username;
    #endregion

    #region HomeScreenNavigationVariables
    string currentlyViewing = "home";
    [SerializeField]
    GameObject homePanel;
    [SerializeField]
    GameObject addExpensePanel;
    [SerializeField]
    GameObject viewExpensesPanel;
    [SerializeField]
    GameObject accountPanel;
    [SerializeField]
    ScrollRect homeScroll;
    [SerializeField]
    GameObject pieChartHolder;
    [SerializeField]
    GameObject pieChartSegmentPrefab;
    [SerializeField]
    TextMeshProUGUI pieChartStatus;
    [SerializeField]
    TextMeshProUGUI estimatedExpenditure;
    [SerializeField]
    GameObject labelPrefab;
    [SerializeField]
    Image pieChartImage;
    [SerializeField]
    Animator versionWarningAnimator;
    [SerializeField]
    TextMeshProUGUI thisVersion;
    [SerializeField]
    TextMeshProUGUI thisVersionLabel;
    [SerializeField]
    TextMeshProUGUI latestVersionLabel;

    Animator aHomePanel, aAddExpensePanel, aViewExpensesPanel, aAccountPanel;
    #endregion

    #region CurrentExpenseDetails
    public string c_id, c_type, c_message, c_amount, c_date, c_time;
    #endregion

    #region ModifyExpense
    [SerializeField]
    GameObject m_typeAlreadyAdded, m_typeHasToBeAdded;
    [SerializeField]
    TMP_Dropdown m_type;
    [SerializeField]
    TMP_InputField m_addType;
    [SerializeField]
    TMP_InputField m_message;
    [SerializeField]
    TMP_InputField m_amount;
    [SerializeField]
    TMP_InputField m_date_dd;
    [SerializeField]
    TMP_InputField m_date_mm;
    [SerializeField]
    TMP_InputField m_date_yyyy;
    [SerializeField]
    TMP_InputField m_time_hh;
    [SerializeField]
    TMP_InputField m_time_mm;
    [SerializeField]
    TMP_Dropdown m_time_ampm;
    #endregion

    bool addTypeBool = false;
    float totalTime, timeElapsed = 0;
    bool statusActive = false, statusInfinite = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (UserPreferences.Instance != null)
        {
            welcomeUsername.text = UserPreferences.Instance._username;
            username.text = "Username: " + UserPreferences.Instance._username;
            name.text = UserPreferences.Instance._username;
        }
        else
        {
            welcomeUsername.text = "DThePro";
            username.text = "Username: " + "DThePro";
            name.text = "DThePro";
        }
    }

    void Start()
    {
        
        
        aHomePanel = homePanel.GetComponent<Animator>();
        aAddExpensePanel = addExpensePanel.GetComponent<Animator>();
        aViewExpensesPanel = viewExpensesPanel.GetComponent<Animator>();
        aAccountPanel = accountPanel.GetComponent<Animator>();

        pieChartStatus.text = "NO EXPENSES YET.";

        timeFrame.value = 3;
        orderBy.value = 0;
        type.value = -1;
        CheckVersion();
        ViewExpensesRefresh();
        StartCoroutine(FetchTotalExpense());
    }

    // Update is called once per frame
    void Update()
    {
        if (statusActive && !statusInfinite)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > totalTime)
            {
                timeElapsed = 0;
                DeactivateStatus();
            }
        }

        refreshTimer += Time.deltaTime;
        if (refreshTimer > autoRefreshTime)
        {
            AutoRefresh();
        }

        // Debug.Log(c_id);
    }

    void ActivateStatus(Color color, string text, float time)
    {
        addStatusText.text = text;
        addStatusText.color = color;
        statusActive = true;
        totalTime = time;
        if (time == 0f) statusInfinite = true;
        else statusInfinite = false;
    }

    void DeactivateStatus()
    {
        addStatusText.text = "";
        statusActive = false;
    }

    public void LogOut()
    {
        transitionPanelAnimator.SetTrigger("OutroAnimationHomescreen"); 
    }

    public void AddExpenseFinal()
    {
        bool errorFlag = true;

        string _type = !addTypeBool ? type.options[type.value].text : addType.text;

        string _message = message.text;

        float _amount;
        if (float.TryParse(amount.text, out _amount)) _amount = _amount;
        else;   // Error handling

        System.DateTime currentDateTime = System.DateTime.Now;
        string __day = currentDateTime.Day.ToString(), __month = currentDateTime.Month.ToString(), __year = currentDateTime.Year.ToString();
        int _day = currentDateTime.Day, _month = currentDateTime.Month, _year = currentDateTime.Year;
        if (dateDay.text.NullIfEmpty() != null && dateMonth.text.NullIfEmpty() != null && dateYear.text.NullIfEmpty() != null)
        {
            int.TryParse(dateDay.text, out _day);
            int.TryParse(dateMonth.text, out _month);
            int.TryParse(dateYear.text, out _year);
            if (dateMonth.text == "1" ||
                dateMonth.text == "3" ||
                dateMonth.text == "5" ||
                dateMonth.text == "7" ||
                dateMonth.text == "8" ||
                dateMonth.text == "10" ||
                dateMonth.text == "12" ||
                dateMonth.text == "01" ||
                dateMonth.text == "03" ||
                dateMonth.text == "05" ||
                dateMonth.text == "07" ||
                dateMonth.text == "08")
            {
                if (_day >= 1 && _day <= 31)
                {
                    __day = _day.ToString();
                    __month = _month.ToString();
                    __year = _year.ToString();
                    errorFlag = false;
                }
                else errorFlag = true;
            }
            else if (dateMonth.text == "4" ||
                    dateMonth.text == "6" ||
                    dateMonth.text == "9" ||
                    dateMonth.text == "11" ||
                    dateMonth.text == "04" ||
                    dateMonth.text == "06" ||
                    dateMonth.text == "09")
            {
                if (_day >= 1 && _day <= 30)
                {
                    __day = _day.ToString();
                    __month = _month.ToString();
                    __year = _year.ToString();
                    errorFlag = false;
                }
                else errorFlag = true;
            }
            else if (dateMonth.text == "2" || dateMonth.text == "02")
            {
                if (_year % 400 == 0 || (_year % 400 != 0 && _year % 4 == 0))
                {
                    if (_day >= 1 && _day <= 29)
                    {
                        __day = _day.ToString();
                        __month = _month.ToString();
                        __year = _year.ToString();
                        errorFlag = false;
                    }
                    else errorFlag = true;
                }
                else
                {
                    if (_day >= 1 && _day <= 28)
                    {
                        __day = _day.ToString();
                        __month = _month.ToString();
                        __year = _year.ToString();
                        errorFlag = false;
                    }
                    else errorFlag = true;
                }
            }
        }
        else errorFlag = false;

        string __hour = currentDateTime.Hour.ToString(), __minute = currentDateTime.Minute.ToString();
        int _hour = currentDateTime.Hour, _minute = currentDateTime.Minute;
        if (timeHour.text != "" && timeMinute.text != "")
        {
            int.TryParse(timeHour.text, out _hour);
            int.TryParse(timeMinute.text, out _minute);
            if (_hour >= 1 && _hour <= 12 && _minute >= 0 && _minute <= 60)
            {
                if ((timeAmPm.value == 0 && _hour != 12) || (timeAmPm.value == 1 && _hour == 12))
                {
                    __hour = _hour.ToString();
                }
                else if ((timeAmPm.value == 1 && _hour != 12) || (timeAmPm.value == 0 && _hour == 12))
                {
                    __hour = (_hour + 12).ToString();
                    if (__hour == "24") __hour = "00";
                }
                __minute = _minute.ToString();
                errorFlag = false;
            }
            else errorFlag = true;
        }
        else errorFlag = false;

        // Debug.Log(__hour);

        if (message.text.NullIfEmpty() == null || amount.text.NullIfEmpty() == null) errorFlag = true;

        if (errorFlag)
        {
            ActivateStatus(Color.red, "One or more required fields are incorrect or empty. Please retry.", 5f);
        }
        else
        {
            StartCoroutine(AddExpenseFinalCoroutine(_type, _message, _amount.ToString(), __day, __month, __year, __hour, __minute));
            // ClearFeilds();
            AddExpenseRefresh();
        }
    }

    void ClearFields()
    {
        typeAlreadyAdded.SetActive(true);
        typeHasToBeAdded.SetActive(false);
        m_typeAlreadyAdded.SetActive(true);
        m_typeHasToBeAdded.SetActive(false);
        type.value = -1;
        addType.text = "";
        message.text = "";
        amount.text = "";
        dateDay.text = "";
        dateMonth.text = "";
        dateYear.text = "";
        timeHour.text = "";
        timeMinute.text = "";
    }

    IEnumerator AddExpenseFinalCoroutine(string _type, string _message, string _amount, string _day, string _month, string _year, string _hour, string _minute)
    {
        totalTime = 0f;

        WWWForm form = new();
        if (UserPreferences.Instance != null)
            form.AddField("name", UserPreferences.Instance._username);
        else
            form.AddField("name", "DThePro");
        form.AddField("type", _type);
        form.AddField("message", _message);
        form.AddField("amount", _amount);
        form.AddField("date", (_year + "-" + _month + "-" + _day));
        // Debug.Log((_year + "-" + _month + "-" + _day));
        form.AddField("time", (_hour + ":" + _minute + ":00"));

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/expense.php", form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("User created.");
            ActivateStatus(Color.white, "Expense Added.", 5f);
            ClearFields();
            AddExpenseRefresh();
            // ActivateLogin();
        }
        else
        {
            Debug.Log("User creation failed. " + www.text);
            ActivateStatus(Color.red, "An error ocurred. Please try again.", 5f);
        }
    }

    IEnumerator FetchExpenses(string filename)
    {
        WWWForm form = new();
        if (UserPreferences.Instance != null)
            form.AddField("name", UserPreferences.Instance._username);
        else
            form.AddField("name", "DThePro");

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/" + filename, form);
        yield return www;

        if (www.isDone && www.error == null) FormatExpense(www);
    }

    IEnumerator FetchTotalExpense()
    {
        WWWForm form = new();
        if (UserPreferences.Instance != null)
            form.AddField("name", UserPreferences.Instance._username);
        else
            form.AddField("name", "DThePro");

        WWW wwwG = new WWW("https://sqlhandling.000webhostapp.com/TotalExpense_1Month.php", form);
        yield return wwwG;

        if (wwwG.isDone)
        {
            SetTotalExpense(wwwG);
        }
    }

    void SetTotalExpense(WWW www)
    {
        if (www.error == null)
        {
            StartCoroutine(FetchExpenses("displayAll.php"));

            if (numberOfExpenses != 0)
            {
                totalExpenditureText.text = "Rs. " + www.text;
                newAmount = www.text;
            }
            else
            {
                totalExpenditureText.text = "Rs. 0";
                newAmount = "0";
            }
        }
    }

    IEnumerator FetchTypesValues()
    {
        WWWForm form = new();
        if (UserPreferences.Instance != null)
            form.AddField("name", UserPreferences.Instance._username);
        else
            form.AddField("name", "DThePro");

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/TotalExpenseOfAllTypes.php", form);
        yield return www;

        if (www.isDone && www.text != "") DrawPieChart(www);
    }

    IEnumerator FetchTypes()
    {
        WWWForm form = new();
        if (UserPreferences.Instance != null)
            form.AddField("name", UserPreferences.Instance._username);
        else
            form.AddField("name", "DThePro");

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/AllTypes.php", form);
        yield return www;

        if (www.isDone && www.text != "") AddTypesToDropdown(www);
    }

    IEnumerator Delete(string id)
    {
        WWWForm form = new();
        
        Debug.Log(id);
        
        form.AddField("id", id);

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/deleteAll.php", form);
        yield return www;

        Debug.LogWarning(www.text);
        CloseExpenseDetails();
        ViewExpensesRefresh();
    }

    IEnumerator FetchEstimatedExpenditure()
    {
        WWWForm form = new();
        if (UserPreferences.Instance != null)
            form.AddField("name", UserPreferences.Instance._username);
        else
            form.AddField("name", "DThePro");

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/Estimated.php", form);
        yield return www;

        if (www.isDone && www.text != "")
        {
            estimatedExpenditure.text = "Rs. " + www.text;
        }
    }

    IEnumerator LoadPieChartImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isNetworkError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("No error.");
            Texture2D myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite newSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));

            pieChartImage.sprite = newSprite;
            pieChartImage.color = Color.white;
        }
    }

    IEnumerator FetchVersionNumber()
    {
        WWW www = new WWW("https://sqlhandling.000webhostapp.com/version.php");
        yield return www;

        if (www.isDone && www.error == null)
        {
            latestVersionNumber = www.text;
            latestVersionLabel.text = "Latest version: " + latestVersionNumber;
            thisVersionLabel.text = "Installed version: " + thisVersion.text;
            if (latestVersionNumber != thisVersion.text) versionWarningAnimator.SetBool("inOutdatedVersion", true);
            else versionWarningAnimator.SetBool("inOutdatedVersion", false);
        }
    }

    IEnumerator ModifyExpense(string _type, string _message, string _amount, string _day, string _month, string _year, string _hour, string _minute)
    {
        totalTime = 0f;

        Debug.Log(c_id + " " + _type + " " + _amount + " " + _message + " " + (_year + "-" + _month + "-" + _day) + " " + (_hour + ":" + _minute + ":00"));
        WWWForm form = new();
        form.AddField("id", c_id);
        form.AddField("type", _type);
        form.AddField("message", _message);
        form.AddField("amount", _amount);
        form.AddField("date", (_year + "-" + _month + "-" + _day));
        // Debug.Log((_year + "-" + _month + "-" + _day));
        form.AddField("time", (_hour + ":" + _minute + ":00"));

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/EditExpense.php", form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("User created.");
            // Debug.Log(c_id + " " + _type + " " + _amount + " " + _message + " " + (_year + "-" + _month + "-" + _day) + " " + (_hour + ":" + _minute + ":00"));
            ActivateStatus(Color.white, "Expense Added.", 5f);
            ClearFields();
            AddExpenseRefresh();
            // ActivateLogin();
        }
        else
        {
            Debug.Log("User creation failed. " + www.text);
            // ActivateStatus(Color.red, "An error ocurred. Please try again.", 5f);
        }
    }

    void CheckVersion()
    {
        StartCoroutine(FetchVersionNumber());
    }

    public void CloseVersionWarning() { versionWarningAnimator.SetBool("inOutdatedVersion", false); }

    static void DeleteAllChildren(GameObject parent)
    {
        // Loop through all child objects and destroy them
        int childCount = parent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }


    void FormatExpense(WWW www)
    {
        DeleteAllChildren(verticalLayoutGroupParent);

        List<string> rows = www.text.Split(new[] { "&%<$" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        numberOfExpenses = rows.Count;

        // foreach (string row in rows) Debug.Log(row);

        // Debug.Log(rows.Count);
        if (rows.Count != 0)
        {
            foreach (string row in rows)
            {

                List<string> values = row.Split(new[] { "*@%>" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                // Debug.Log(values[2]);
                string id = values[0];
                string type = values[2];
                string message = values[3];
                string amount = values[4];
                string date = values[5];
                string time = values[6];

                GameObject expense = Instantiate(expensePrefab);
                expense.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = type;
                expense.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
                expense.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Rs. " + amount;
                expense.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = date;
                expense.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = id;
                expense.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = time;

                expense.transform.SetParent(verticalLayoutGroupParent.transform, false);
            }
            statusViewExpenses.text = "";
            pieChartStatus.text = "LOADING CHART...";
        }
        else
        {
            statusViewExpenses.text = "No expenses found.";
            pieChartStatus.text = "NO EXPENSES YET.";
        }
    }

    void AddTypesToDropdown(WWW www)
    {
        type.ClearOptions();
        m_type.ClearOptions();
        AddOption("Food");
        AddOption("Clothing");
        AddOption("Travel");
        AddOption("Groceries");
        AddOption("Electronics");


        Debug.Log(www.text);
        List<string> types = www.text.Split(new[] { "*@%>" }, StringSplitOptions.RemoveEmptyEntries).ToList();

        foreach (string _type in types)
        {
            if (_type.ToUpper() != "FOOD" && _type.ToUpper() != "CLOTHING" && _type.ToUpper() != "TRAVEL" && _type.ToUpper() != "GROCERIES" && _type.ToUpper() != "ELECTRONICS")
                AddOption(_type);
        }

        ClearFields();
        ActivateModifyExpensePanel2();
    }

    void AddOption(string optionText)
    {
        TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(optionText);
        type.options.Add(optionData);
        m_type.options.Add(optionData);
    }

    public void ViewExpensesRefresh()
    {
        string valueCombo = timeFrame.value + "" + orderBy.value;
        switch (valueCombo)
        {
            case "00": StartCoroutine(FetchExpenses("LastMonth.php")); break;
            case "10": StartCoroutine(FetchExpenses("Last3Months.php")); break;
            case "20": StartCoroutine(FetchExpenses("Last6Months.php")); break;
            case "30": StartCoroutine(FetchExpenses("displayAll.php")); break;
            case "01": StartCoroutine(FetchExpenses("LastMonth_OldestFirst.php")); break;
            case "11": StartCoroutine(FetchExpenses("Last3Months_OldestFirst.php")); break;
            case "21": StartCoroutine(FetchExpenses("Last6Months_OldestFirst.php")); break;
            case "31": StartCoroutine(FetchExpenses("displayAll_OldestFirst.php")); break;
            case "02": StartCoroutine(FetchExpenses("LastMonth_TypeWise.php")); break;
            case "12": StartCoroutine(FetchExpenses("Last3Months_TypeWise.php")); break;
            case "22": StartCoroutine(FetchExpenses("Last6Months_TypeWise.php")); break;
            case "32": StartCoroutine(FetchExpenses("displayAll_TypeWise.php")); break;
        }
    }

    public void HomeScreenRefresh()
    {
        StartCoroutine(FetchTypesValues());
        StartCoroutine(FetchTotalExpense());
        StartCoroutine(FetchEstimatedExpenditure());
    }

    public void AddExpenseRefresh()
    {
        StartCoroutine(FetchTypes());
    }

    public void SwitchToAddType()
    {
        typeAlreadyAdded.SetActive(false);
        typeHasToBeAdded.SetActive(true);
        addTypeBool = true;
    }

    public void SwitchToAddTypeModifyExpense()
    {
        m_typeAlreadyAdded.SetActive(false);
        m_typeHasToBeAdded.SetActive(true);
        addTypeBool = true;
    }

    public void AddExpenseWindow()
    {
        if (currentlyViewing == "home")
        {
            currentlyViewing = "add expense";
            aHomePanel.SetBool("inHome", false);
            aAddExpensePanel.SetBool("inAddExpense", true);
            ClearFields();
            AddExpenseRefresh();
        }
    }

    public void ViewExpensesWindow()
    {
        if (currentlyViewing == "home")
        {
            currentlyViewing = "view expenses";
            aHomePanel.SetBool("inHome", false);
            aViewExpensesPanel.SetBool("inViewExpenses", true);
            timeFrame.value = 3;
            ViewExpensesRefresh();
        }
    }

    public void AccountWindow()
    {
        if (currentlyViewing == "home")
        {
            currentlyViewing = "account";
            aHomePanel.SetBool("inHome", false);
            aAccountPanel.SetBool("inAccount", true);
        }
    }

    public void Home()
    {
        currentlyViewing = "home";
        aHomePanel.SetBool("inHome", true);
        aAddExpensePanel.SetBool("inAddExpense", false);
        aAccountPanel.SetBool("inAccount", false);
        aViewExpensesPanel.SetBool("inViewExpenses", false);
        CloseExpenseDetails();
        CloseCredits();
        HomeScreenRefresh();
    }

    public void OpenCredits()
    {
        creditsAnimator.SetBool("inCredits", true);
    }

    public void CloseCredits()
    {
        creditsAnimator.SetBool("inCredits", false);
    }

    void AutoRefresh()
    {
        switch (currentlyViewing)
        {
            case "home": HomeScreenRefresh(); break;
            case "view expenses": ViewExpensesRefresh(); break;
        }
        refreshTimer = 0;
    }

    void DrawPieChart(WWW www)
    {
        if (newAmount != previousAmount && newAmount != "0")
        {
            // Debug.Log(www.text);
            // DeleteAllChildren(pieChartHolder);

            string oriUrl = "https://image-charts.com/chart?chs=300x300&chd=t:60,40,70,80,110&cht=p3&chl=Hello%7CWorld%7CHi%7CBye%7CTom";
            string url = "https://image-charts.com/chart?chs=300x300&chd=t:";

            Debug.Log(www.text);
            List<string> types = www.text.Split(new[] { "&%<$" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string type in types) Debug.Log(type);

            float totalExpenditure = 1f;

            foreach (string type in types)
            {
                List<string> typeValues = type.Split(new[] { "*@%>" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                try
                {
                    float.TryParse(typeValues[1], out float typeAmount);
                    totalExpenditure += typeAmount;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            Debug.Log(totalExpenditure);

            foreach (string type in types)
            {
                List<string> typeValues = type.Split(new[] { "*@%>" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                // Debug.Log(typeValues);
                try
                {
                    float.TryParse(typeValues[1], out float typeAmount);

                    float typePercentage = (typeAmount / totalExpenditure) * 100;

                    url += typePercentage.ToString() + ",";
                }
                catch (Exception ex)
                {
                    break;
                }
            }

            url += "&cht=p3&chl=";

            foreach (string type in types)
            {
                List<string> typeValues = type.Split(new[] { "*@%>" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                url += typeValues[0] + "%7C";
            }

            url = url.Substring(0, url.Length - 3);

            // Debug.Log(url);

            pieChartImage.color = new Color(0, 0, 0, 0);
            StartCoroutine(LoadPieChartImage(url));

            previousAmount = newAmount;
        }

        if (newAmount == "0")
        {
            pieChartImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void ShowExpenseDetails(string id, string type, string message, string amount, string date, string time)
    {
        detailsPanel.GetComponent<Animator>().SetBool("inDetails", true);

        detailedType.text = type;
        detailedMessage.text = message;
        detailedAmount.text = amount;
        detailedDate.text = date;
        detailedTime.text = time;

        c_id = id;
        c_type = type;
        c_message = message;
        c_amount = amount;
        c_date = date;
        c_time = time;
    }

    public void ActivateModifyExpensePanel()
    {
        AddExpenseRefresh();

        modifyExpense.GetComponent<Animator>().SetBool("modifyingExpense", true);
        m_typeAlreadyAdded.SetActive(true);
        m_typeHasToBeAdded.SetActive(false);
    }
    
    public void ActivateModifyExpensePanel2()
    {
        int i;
        for (i = 0; i < m_type.options.Count; i++)
        {
            if (c_type == type.options[i].text)
            {
                break;
            }
        }

        if (true) m_type.value = i;

        m_message.text = c_message;
        m_amount.text = c_amount[4..];
        m_date_dd.text = c_date.Substring(8, 2);
        m_date_mm.text = c_date.Substring(5, 2);
        m_date_yyyy.text = c_date[..4];
        m_time_hh.text = ConvertTimeTo12HourFormat(c_time)[..2];
        m_time_mm.text = ConvertTimeTo12HourFormat(c_time).Substring(3, 2);
        string ampm = ConvertTimeTo12HourFormat(c_time).Substring(9, 2);
        if (ampm == "AM") m_time_ampm.value = 0;
        else m_time_ampm.value = 1;
    }

    public void ModifyExpenseFinal()
    {
        bool errorFlag = true;

        string _type = !addTypeBool ? m_type.options[m_type.value].text : m_addType.text;

        string _message = m_message.text;

        string _amount = m_amount.text;

        System.DateTime currentDateTime = System.DateTime.Now;
        string __day = currentDateTime.Day.ToString(), __month = currentDateTime.Month.ToString(), __year = currentDateTime.Year.ToString();
        int _day = currentDateTime.Day, _month = currentDateTime.Month, _year = currentDateTime.Year;
        if (m_date_dd.text.NullIfEmpty() != null && m_date_mm.text.NullIfEmpty() != null && m_date_yyyy.text.NullIfEmpty() != null)
        {
            int.TryParse(m_date_dd.text, out _day);
            int.TryParse(m_date_mm.text, out _month);
            int.TryParse(m_date_yyyy.text, out _year);
            if (m_date_mm.text == "1" ||
                m_date_mm.text == "3" ||
                m_date_mm.text == "5" ||
                m_date_mm.text == "7" ||
                m_date_mm.text == "8" ||
                m_date_mm.text == "10" ||
                m_date_mm.text == "12" ||
                m_date_mm.text == "01" ||
                m_date_mm.text == "03" ||
                m_date_mm.text == "05" ||
                m_date_mm.text == "07" ||
                m_date_mm.text == "08")
            {
                if (_day >= 1 && _day <= 31)
                {
                    __day = _day.ToString();
                    __month = _month.ToString();
                    __year = _year.ToString();
                    errorFlag = false;
                }
                else errorFlag = true;
            }
            else if (m_date_mm.text == "4" ||
                    m_date_mm.text == "6" ||
                    m_date_mm.text == "9" ||
                    m_date_mm.text == "11" ||
                    m_date_mm.text == "04" ||
                    m_date_mm.text == "06" ||
                    m_date_mm.text == "09")
            {
                if (_day >= 1 && _day <= 30)
                {
                    __day = _day.ToString();
                    __month = _month.ToString();
                    __year = _year.ToString();
                    errorFlag = false;
                }
                else errorFlag = true;
            }
            else if (m_date_mm.text == "2" || m_date_mm.text == "02")
            {
                if (_year % 400 == 0 || (_year % 400 != 0 && _year % 4 == 0))
                {
                    if (_day >= 1 && _day <= 29)
                    {
                        __day = _day.ToString();
                        __month = _month.ToString();
                        __year = _year.ToString();
                        errorFlag = false;
                    }
                    else errorFlag = true;
                }
                else
                {
                    if (_day >= 1 && _day <= 28)
                    {
                        __day = _day.ToString();
                        __month = _month.ToString();
                        __year = _year.ToString();
                        errorFlag = false;
                    }
                    else errorFlag = true;
                }
            }
        }
        else errorFlag = false;

        string __hour = currentDateTime.Hour.ToString(), __minute = currentDateTime.Minute.ToString();
        int _hour = currentDateTime.Hour, _minute = currentDateTime.Minute;
        if (m_time_hh.text != "" && m_time_mm.text != "")
        {
            int.TryParse(m_time_hh.text, out _hour);
            int.TryParse(m_time_mm.text, out _minute);
            if (_hour >= 1 && _hour <= 12 && _minute >= 0 && _minute <= 60)
            {
                if ((m_time_ampm.value == 0 && _hour != 12) || (m_time_ampm.value == 1 && _hour == 12))
                {
                    __hour = _hour.ToString();
                }
                else if ((m_time_ampm.value == 1 && _hour != 12) || (m_time_ampm.value == 0 && _hour == 12))
                {
                    __hour = (_hour + 12).ToString();
                    if (__hour == "24") __hour = "00";
                }
                __minute = _minute.ToString();
                errorFlag = false;
            }
            else errorFlag = true;
        }
        else errorFlag = false;

        // Debug.Log(__hour);

        if (m_message.text.NullIfEmpty() == null || m_amount.text.NullIfEmpty() == null) errorFlag = true;

        if (errorFlag)
        {
            ActivateStatus(Color.red, "One or more required fields are incorrect or empty. Please retry.", 5f);
        }
        else
        {
            StartCoroutine(ModifyExpense(_type, _message, _amount, __day, __month, __year, __hour, __minute));
            // ClearFeilds();
            DeactivateModifyExpensePanel();
            ViewExpensesRefresh();
            CloseExpenseDetails();
        }
    }

    public void DeactivateModifyExpensePanel()
    {
        modifyExpense.GetComponent<Animator>().SetBool("modifyingExpense", false);
    }

    public string ConvertTimeTo12HourFormat(string time24Hour)
    {
        // Attempt to parse the 24-hour time string into hours, minutes, and seconds
        string[] timeParts = time24Hour.Split(':');
        int hours, minutes, seconds;
        if (timeParts.Length != 3 ||
            !int.TryParse(timeParts[0], out hours) ||
            !int.TryParse(timeParts[1], out minutes) ||
            !int.TryParse(timeParts[2], out seconds))
        {
            return "Invalid time format. Please use HH:MM:SS (24-hour).";
        }

        // Determine the AM/PM period and adjust hours accordingly
        string period = hours >= 12 ? "PM" : "AM";
        hours = hours % 12;  // Convert to 12-hour format
        hours = hours == 0 ? 12 : hours;  // Handle midnight (00:00 in 24-hour format)

        // Format the 12-hour time string with consistent padding for minutes and seconds
        return $"{hours:00}:{minutes:00}:{seconds:00} {period}";
    }

    public void CloseExpenseDetails()
    {
        detailsPanel.GetComponent<Animator>().SetBool("inDetails", false);
    }

    public void DeleteExpense()
    {
        StartCoroutine(Delete(c_id));
        Debug.Log(c_id);
        HomeScreenRefresh();
    }

    public void OpenDownloadURL()
    {
        Application.OpenURL("https://drive.google.com/drive/folders/1QqqI44mpub7ipkzNKp46_kodNVuUmLiX?usp=sharing");
    }
}
