using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidThemeControl : MonoBehaviour
{
    public static void StatusBarControl(bool _isVisible)
    {
        ApplicationChrome.statusBarState = _isVisible ? ApplicationChrome.States.Visible : ApplicationChrome.States.Hidden;
    }

    public static void StatusBarColorControl(uint _colorValue)
    {
        ApplicationChrome.statusBarColor = _colorValue;
    }

    public static void NavigationBarControl(bool _isVisible)
    {
        ApplicationChrome.navigationBarState = _isVisible ? ApplicationChrome.States.Visible : ApplicationChrome.States.Hidden;
    }

    public static void NavigationBarColorControl(uint _colorValue)
    {
        ApplicationChrome.navigationBarColor = _colorValue;
    }
}
