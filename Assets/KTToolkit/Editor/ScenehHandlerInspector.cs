using UnityEngine;
using UnityEditor;

namespace KTToolkit.SceneManager
{
    [CustomEditor(typeof(SceneHandler), true)]//拡張するクラスを指定
    public class ScenehHandlerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            //元のInspector部分を表示
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            //targetを変換して対象を取得
            SceneHandler boneManager = target as SceneHandler;
            //PublicMethodを実行する用のボタン
            if (GUILayout.Button("Initialize Scene Build List"))
            {
                boneManager.SendMessage("InitSceneList", null, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
