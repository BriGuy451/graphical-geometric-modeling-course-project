using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CoroutineUtils
{
    public static IEnumerator WaitTimeBetweenUnitsOfWork(float timeToWait, Action firstFunction, Action secondFunction)
    {
        Debug.Log($"!!-First Unit of Work {firstFunction.ToString()}");
        firstFunction();

        Debug.Log($"!!-Waiting {timeToWait}");
        yield return new WaitForSeconds(timeToWait);

        Debug.Log($"!!-Second Unit of Work {secondFunction.ToString()}");
        secondFunction();
    }

    public static IEnumerator LoadSceneAndPerformAction(string sceneName, Action actionToPerform)
    {
        LogFromMethod("LoadAndPerformAction");

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
        loadSceneAsync.allowSceneActivation = true;

        LogFromMethod("Waiting");
        while (!loadSceneAsync.isDone)
        {
            yield return null;
        }

        LogFromMethod("Performing Action");
        actionToPerform();
    }

    private static void LogFromMethod(string message)
    {
        Debug.Log($"CoroutineUtils:{message}");
    }

}