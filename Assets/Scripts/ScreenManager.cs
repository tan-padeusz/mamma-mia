using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private Canvas[] screens;
    private readonly Dictionary<Screen, Canvas> _screens = new Dictionary<Screen, Canvas>();

    private void Start()
    {
        var enumValues = (Screen[]) Enum.GetValues(typeof(Screen));
        for (var index = 0; index < this.screens.Length; index++)
            this._screens.Add(enumValues[index], this.screens[index]);
    }

    public void EnableScreen(Screen screen)
    {
        foreach (var (key, value) in this._screens)
            value.enabled = key == screen;
    }
}
