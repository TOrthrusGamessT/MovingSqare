using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

public class FireBase : MonoBehaviour
{
    private FirebaseApp app;
    private void Start()
    {
        Init();
    }


    private void Init()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public static void LogCustomEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if (parameters == null)
        {
            FirebaseAnalytics.LogEvent(eventName);
            return;
        }

        var firebaseParams = new List<Parameter>();
        foreach (var param in parameters)
        {
            if (param.Value is int intVal)
                firebaseParams.Add(new Parameter(param.Key, intVal));
            else if (param.Value is float floatVal)
                firebaseParams.Add(new Parameter(param.Key, floatVal));
            else if (param.Value is double doubleVal)
                firebaseParams.Add(new Parameter(param.Key, doubleVal));
            else
                firebaseParams.Add(new Parameter(param.Key, param.Value.ToString()));
        }

        FirebaseAnalytics.LogEvent(eventName, firebaseParams.ToArray());
    }



}
