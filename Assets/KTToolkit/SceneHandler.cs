using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;

namespace KTToolkit.SceneManager
{
    [ExecuteInEditMode]
    public class SceneHandler : MonoBehaviour
    {
        [SerializeField] private string[] scenes;
        public string[] Scenes
        {
            get => this.scenes;
            private set => this.scenes = value;
        }
        [SerializeField] private string FirstSceneName = "Start";

        /// <summary>
        /// cache
        /// </summary>
        private IMixedRealitySceneSystem sceneSystem;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            /// Update contentSceneNames and loadedStatus On Content Loaded
            sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
            sceneSystem.OnContentLoaded += CheckLoadContentStatus;
            if (scenes.Contains(FirstSceneName)) sceneSystem.LoadContent(FirstSceneName, LoadSceneMode.Single);
        }

        /// <summary>
        /// Get Scene`s Status From MixedRealityToolKit SceneManager
        /// </summary>
        /// <parm name="mode"> mode = contentSceneNames[] updated </parm>
        private void CheckLoadContentStatus(IEnumerable<string> mode) {
            IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();

            string[] contentSceneNames = sceneSystem.ContentSceneNames;
            bool[] loadStatus = new bool[contentSceneNames.Length];

            for (int i = 0; i < contentSceneNames.Length; i++)
            {
                loadStatus[i] = sceneSystem.IsContentLoaded(contentSceneNames[i]);
                // Debug.Log("loadstatus = " + loadStatus[i]);
            }
        }

        public void SetScene(string scene)
        {
            if(!scenes.Contains(scene)) return;

            IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
            sceneSystem.LoadContent(scene, LoadSceneMode.Single);
        }

#if UNITY_EDITOR
        private void InitSceneList()
        {
            Regex reg = new Regex(@".*Contents/(.+)\.unity", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/Scenes/Contents" });
            scenes = new string[guids.Length];
            for (var idx = 0; idx < guids.Length; idx++)
            {
                string filePath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[idx]);
                Match m = reg.Match(filePath);
                string fileName = m.Groups[1].Value;
                scenes[idx] = fileName;
            }
        }
#endif
    }
}
