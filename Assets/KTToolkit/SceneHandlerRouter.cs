using UnityEngine;

namespace KTToolkit.SceneManager
{
    public class SceneHandlerRouter: MonoBehaviour
    {
        [SerializeField] private string loadScene;
        private string sceneManagerName = "SceneManager";
        private SceneHandler sceneManager;
        private void Start()
        {
            this.sceneManager = GameObject.Find(sceneManagerName).GetComponent<SceneHandler>();
        }
        public void SetScene()
        {
            sceneManager.SetScene(loadScene);
        }
    }
}