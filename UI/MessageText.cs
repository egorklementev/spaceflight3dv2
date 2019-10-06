using UnityEngine;
using TMPro;

public class MessageText : MonoBehaviour {

    private TextMeshProUGUI messageText;
    private Animator messageAnimator;

    private void Awake()
    {
        messageText = GetComponent<TextMeshProUGUI>();
        messageAnimator = GetComponent<Animator>();
    }

    public enum ScreenPosition
    {
        TOP,
        BOTTOM,
        CENTER,
        RIGHT,
        LEFT
    }

    public void DisplayMessage(string message)
    {
        DisplayMessage(message, 1.0f, ScreenPosition.CENTER);
    }
    public void DisplayMessage(string message, ScreenPosition pos)
    {
        DisplayMessage(message, 1.0f, pos);
    }
    public void DisplayMessage(string message, float timeParam)
    {
        DisplayMessage(message, timeParam, ScreenPosition.CENTER);
    }

    /// <summary>
    /// A function for displaying a message on the screen
    /// </summary>
    /// <param name="message">The string representing the actual message</param>
    /// <param name="timeParam">Time in seconds to be displayed</param>
    /// <param name="pos">The position on the screen</param>
    public void DisplayMessage(string message, float timeParam, ScreenPosition pos)
    {
        messageText.text = message;

        switch (pos)
        {
            case ScreenPosition.TOP:
                messageAnimator.SetTrigger("DisplayTop");
                break;
            case ScreenPosition.BOTTOM:
                messageAnimator.SetTrigger("DisplayBottom");
                break;
            case ScreenPosition.CENTER:
                messageAnimator.SetTrigger("DisplayCenter");
                break;
            case ScreenPosition.RIGHT:
                messageAnimator.SetTrigger("DisplayRight");
                break;
            case ScreenPosition.LEFT:
                messageAnimator.SetTrigger("DisplayLeft");
                break;
            default:
                messageAnimator.SetTrigger("DisplayCenter");
                break;
        }

        try
        {
            messageAnimator.SetFloat("TimeParam", 1f / timeParam);
        }
        catch (System.DivideByZeroException e)
        {
            Debug.LogException(e);
            throw;
        }
    }

}
