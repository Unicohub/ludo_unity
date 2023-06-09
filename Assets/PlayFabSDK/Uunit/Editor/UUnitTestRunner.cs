/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PlayFab.UUnit
{
    public class StaticTestRunner
    {
        private static void ClearDebugLog()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        [MenuItem("PlayFab/UUnit/Run Tests Immediately%#t")]
        public static void TestImmediately()
        {
            ClearDebugLog();
            UUnitTestSuite suite = new UUnitTestSuite();
            suite.FindAndAddAllTestCases(typeof(UUnitTestCase));
            suite.RunAllTests();
            UUnitTestResult result = suite.GetResults();

            if (!result.AllTestsPassed())
            {
                Debug.LogWarning(result.Summary());
                throw new Exception("Not all tests passed.");
            }
            else
            {
                Debug.Log(result.Summary());
            }
        }

        /*[MenuItem("UUnit/Run Tests Incrementally")] Not quite ready for prime-time*/
        public static void TestIncrementally()
        {
            var runnerGOs = GameObject.FindObjectsOfType<UnityIncrementalTestRunner>();
            if (!Application.isPlaying)
                for (int i = 1; i < runnerGOs.Length; i++)
                    GameObject.DestroyImmediate(runnerGOs[i]); // You should only have 1 unit-test runner

            EditorApplication.isPlaying = true; // Start the player

            if (runnerGOs.Length == 0)
            {
                // If there is no unit-test runner active, create one
                GameObject newRunner = new GameObject();
                newRunner.AddComponent<UnityIncrementalTestRunner>();
            }
        }
    }
}
