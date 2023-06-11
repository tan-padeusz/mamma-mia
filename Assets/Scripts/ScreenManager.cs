using System;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private Canvas[] screens;
    private readonly Dictionary<GameScreen, Canvas> _screens = new Dictionary<GameScreen, Canvas>();

    private void Start()
    {
        var enumValues = (GameScreen[]) Enum.GetValues(typeof(GameScreen));
        for (var index = 0; index < this.screens.Length; index++)
            this._screens.Add(enumValues[index], this.screens[index]);
        this.EnableScreen(GameScreen.Menu);
    }

    public void EnableScreen(GameScreen gameScreen)
    {
        foreach (var (key, value) in this._screens)
            value.enabled = key == gameScreen;
    }
}
