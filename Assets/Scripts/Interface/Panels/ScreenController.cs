using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ScreenController
    {
        /// <summary>
        /// Changes the game screen size temporarly with the given height and width
        /// TODO: Soon give the option to choose the refresh rate
        /// </summary>
        /// <param name="h"></param>
        /// <param name="w"></param>
        public static void ChangeScreenSize(int height, int width, bool fullscreen )
        {
            Screen.SetResolution(width, height, fullscreen);
            Save();
        }

        /// <summary>
        /// Saves the current screen size
        /// </summary>
        /// <param name="h"></param>
        /// <param name="w"></param>
        private static void Save()
        {
            PlayerPrefs.SetInt("screen_width", Screen.width);
            PlayerPrefs.SetInt("screen_height", Screen.height);
            PlayerPrefs.SetInt("screen_fullscreen", Screen.fullScreen ? 1 : 0);
        }

        /// <summary>
        /// Loads the player prefs saved options and instantly apply on screen
        /// </summary>
        private static void Load()
        {

        }

        /// <summary>
        /// Returns a list of possible resolutions
        /// </summary>
        public static Resolution[] GetPossibleResolutions()
        {
            return Screen.resolutions;
        }

        /// <summary>
        /// Gets the current resolution
        /// </summary>
        /// <returns></returns>
        public static Resolution CurrentResolution()
        {
            return Screen.currentResolution;
        }

        /// <summary>
        /// Retrieves the current screen size
        /// </summary>
        private static void GetScreenSize()
        {
            int h = Screen.height;
            int w = Screen.width;
            Debug.Log("My screen height = " + h + " and my width is = " + w);
        }
    }
}
