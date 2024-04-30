using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class SignUp : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public TMP_InputField loginUsernameField;
    public TMP_InputField loginPasswordField;
    
    public UnityEngine.UI.Image passwordMatchImage;
    public UnityEngine.UI.Image usernameMatchImage;
    public Sprite cross, checkmark;

    public TextMeshProUGUI passwordMatchStatus;
    public TextMeshProUGUI usernameCorrectStatus;
    public TextMeshProUGUI statusText;

    public Button submitButton;
    public Button loginButton;

    bool statusActive = false, statusInfinite = false;
    float totalTime, timeElapsed = 0;

    public Animator SelectLine, ContainerPanel;

    public GameObject transitionPanel;

    // Start is called before the first frame update
    void Start()
    {
        passwordMatchStatus.text = null;
        passwordMatchImage.sprite = null;
        passwordMatchImage.color = new Color(0, 0, 0, 0);

        usernameCorrectStatus.text = null;
        usernameMatchImage.sprite = null;
        usernameMatchImage.color = new Color(0, 0, 0, 0);
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
    }

    public void CallRegister()
    {
        ActivateStatus(Color.white, "Please wait...", 0f);
        StartCoroutine(Register());
    }

    public void CallLogin()
    {
        ActivateStatus(Color.white, "Please wait...", 0f);
        StartCoroutine(Login());
    }

    public void ActivateLogin()
    {
        SelectLine.SetTrigger("Login");
        ContainerPanel.SetTrigger("Login");
    }

    public void ActivateSignUp()
    {
        SelectLine.SetTrigger("SignUp");
        ContainerPanel.SetTrigger("SignUp");
    }

    IEnumerator Register()
    {
        totalTime = 0f;

        WWWForm form = new();
        form.AddField("name", usernameField.text);
        form.AddField("password", passwordField.text);
        form.AddField("date", GetFormattedDate());

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/register.php", form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("User created.");
            ActivateStatus(Color.white, "Signed up successfully!", 3f);
            ActivateLogin();
        }
        else
        {
            Debug.Log("User creation failed. " + www.text);
            ActivateStatus(Color.red, www.text, 3f);
        }
    }

    IEnumerator Login()
    {
        totalTime = 0f;

        WWWForm form = new();
        form.AddField("name", loginUsernameField.text);
        form.AddField("password", loginPasswordField.text);

        WWW www = new WWW("https://sqlhandling.000webhostapp.com/login.php", form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("Welcome, " + loginUsernameField.text);
            ActivateStatus(Color.white, "Welcome, " + loginUsernameField.text + ".", 3f);
            UserPreferences.Instance._username = loginUsernameField.text;
            transitionPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Login failed. " + www.text);
            ActivateStatus(Color.red, www.text, 3f);
        }
    }

    string GetFormattedDate()
    {
        // Get the current date
        System.DateTime currentDate = System.DateTime.Now;

        // Format the date as "YYYY-MM-DD"
        string formattedDate = currentDate.ToString("yyyy-MM-dd");

        return formattedDate;
    }

    public void VerifyInput()
    {
        submitButton.interactable = (usernameField.text.Length >= 4 && passwordField.text.Length >= 6 && passwordField.text == confirmPasswordField.text);
        loginButton.interactable = (loginUsernameField.text != "" && loginPasswordField.text != "" && loginUsernameField.text.Trim() != null);

        passwordMatchImage.sprite = null;
        passwordMatchImage.color = new Color(255, 255, 255, 255);
        if (passwordField.text.Length >= 6)
        {
            passwordMatchStatus.text = passwordField.text == confirmPasswordField.text ? "Passwords match" : "Passwords do not match";
            passwordMatchImage.sprite = passwordField.text == confirmPasswordField.text ? checkmark : cross;
        }
        else if (passwordField.text.Length > 0)
        {
            passwordMatchStatus.text = "Password has to be atleast 6 characters";
            passwordMatchImage.sprite = cross;
        }
        else
        {
            passwordMatchStatus.text = null;
            passwordMatchImage.sprite = null;
            passwordMatchImage.color = new Color(0, 0, 0, 0);
        }

        usernameMatchImage.sprite = null;
        usernameMatchImage.color = new Color(255, 255, 255, 255);
        if (usernameField.text.Length >= 4)
        {
            usernameCorrectStatus.text = "Username accepted";
            usernameMatchImage.sprite = checkmark;
        }
        else if (usernameField.text.Length > 0)
        {
            usernameCorrectStatus.text = "Username has to be atleast 4 characters";
            usernameMatchImage.sprite = cross;
        }
        else
        {
            usernameCorrectStatus.text = null;
            usernameMatchImage.sprite = null;
            usernameMatchImage.color = new Color(0, 0, 0, 0 );
        }
    }

    void ActivateStatus(Color color, string text, float time)
    {
        statusText.text = text;
        statusText.color = color;
        statusActive = true;
        totalTime = time;
        if (time == 0f) statusInfinite = true;
        else statusInfinite = false;
    }

    void DeactivateStatus()
    {
        statusText.text = "";
        statusActive = false;
    }
}
