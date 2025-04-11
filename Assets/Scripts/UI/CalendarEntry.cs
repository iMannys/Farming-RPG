using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class CalendarEntry : MonoBehaviour
{
    //The date
    [SerializeField]
    TextMeshProUGUI dateText;
    [SerializeField]
    Image icon;
    //The colour of the entry
    Image entry;
    //The colours of the day
    [SerializeField]
    Color weekday, sat, sun, today;
    public GameTimestamp.Season season; 
    string eventDescription;

    // Start is called before the first frame update
    void OnEnable()
    {
        icon.gameObject.SetActive(false);
        entry = GetComponent<Image>();
    }

    //For days with special events
    public void Display(int date, GameTimestamp timestamp, GameTimestamp.DayOfTheWeek day, Sprite eventSprite, string eventDescription)
    {
        icon.gameObject.SetActive(false);
        GameTimestamp currentTime = TimeManager.Instance.GetGameTimestamp();
        dateText.text = date.ToString();
        Color colorToSet = weekday; 
        switch (day)
        {
            case GameTimestamp.DayOfTheWeek.Saturday:
                colorToSet = sat;
                break;
            case GameTimestamp.DayOfTheWeek.Sunday:
                colorToSet = sun;
                break;
            default:
                colorToSet = weekday;
                break; 
        }
        if (date == currentTime.day && timestamp.season == currentTime.season && timestamp.year == currentTime.year)
        {
            colorToSet = today;
        }
        entry.color = colorToSet;
        if (eventSprite != null)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = eventSprite;
        }
    }

    //For normal days
    public void Display(int date, GameTimestamp timestamp, GameTimestamp.DayOfTheWeek day)
    {

        Display(date, timestamp, day, null, "Just an ordinary day");

    }

    //For null entries
    public void EmptyEntry()
    {
        icon.gameObject.SetActive(false);
        entry.color = Color.clear;
        dateText.text = "";
    }
}