using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarInteractable : InteractableObject
{
    public override void Pickup()
    {
        CalendarUIListing calendar = UIManager.Instance.calendar;
        calendar.gameObject.SetActive(true);
        UIManager.Instance.darkBackground.SetActive(true);
        calendar.RenderCalendar(TimeManager.Instance.GetGameTimestamp());
    }
}
