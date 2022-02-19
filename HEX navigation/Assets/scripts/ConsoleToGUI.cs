using UnityEngine;

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {        
        static string myLog = "";
        private string output;
        private string stack;

        GUIStyle consoleStyle;
        Texture2D consoleBackground;


        void OnEnable()
        {
            Application.logMessageReceived += Log;

            consoleBackground = new Texture2D(1, 1, TextureFormat.RGBAFloat, false);
            consoleBackground.SetPixel(0, 0, new Color(1, 1, 1, 0.25f));
            consoleBackground.Apply(); 
            consoleStyle = new GUIStyle(GUIStyle.none);
            consoleStyle.fontSize = 24;
            consoleStyle.normal.textColor = Color.blue;
            consoleStyle.normal.background = consoleBackground;            
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            if (!output.Contains("NullRef")) { myLog = output + "\n" + myLog; }
            
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            {
                myLog = GUI.TextArea(new Rect(Screen.width-560, Screen.height-125, 500, 100), myLog, consoleStyle);
            }
        }
        
    }
}
