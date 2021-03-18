using UnityEngine;
using System.Linq;

namespace KTToolkit.Extensions
{
    public static class GameObjectUtil
    {
        /// <summary>
        /// ヒエラルキーに存在するゲームオブジェクトが実際に存在した場合に削除を行います
        /// </summary>
        /// <param name="name">削除したいゲームオブジェクト名</param>
        public static void DestroyIfExist(string name)
        {
            var gameobject = GameObject.Find(name);
            if (gameobject == null)
            {
                return;
            }
            GameObject.Destroy(gameobject);
        }
        /// <summary>
        /// ゲームオブジェクトにコンポーネントがアタッチされているかをチェックする
        /// </summary>
        /// <typeparam name="T">コンポーネント名</typeparam>
        /// <param name="self">自身のゲームオブジェクト(自動参照)</param>
        /// <returns>bool true = 存在する ; false = 存在しない</returns>
        public static bool HasComponent<T>(this GameObject self) where T : Component
        {
            return self.GetComponent<T>() != null;
        }
        /// <summary>
        /// ルートになるオブジェクトを返す
        /// </summary>
        /// <param name="self">自身のゲームオブジェクト(自動取得)</param>
        /// <returns>ルートオブジェクト</returns>
        public static GameObject GetRoot(this GameObject self)
        {
            var t = self.transform;
            for (; ; )
            {
                var parent = t.parent;
                if (parent == null)
                {
                    break;
                }
                t = parent;
            }
            return t.gameObject;
        }
        /// <summary>
        /// 子要素のげーーむオブジェクトを取得する
        /// </summary>
        /// <param name="self">自身のゲームオブジェクト(自動参照)</param>
        /// <param name="name">検索するオブジェクト名</param>
        /// <returns></returns>
        public static GameObject GetChildObject(this GameObject self, string name)
        {
            var t = self.transform;
            return t.Find(name).gameObject;
        }
        /// <summary>
        /// 深い階層まで子オブジェクトを名前で検索して GameObject 型で取得します
        /// </summary>
        /// <param name="self">GameObject 型のインスタンス</param>
        /// <param name="name">検索するオブジェクトの名前</param>
        /// <param name="includeInactive">非アクティブなオブジェクトも検索する場合 true</param>
        /// <returns>子オブジェクト</returns>
        public static GameObject FindDeep(
            this GameObject self,
            string name,
            bool includeInactive = false)
        {
            var children = self.GetComponentsInChildren<Transform>(includeInactive);
            foreach (var transform in children)
            {
                if (transform.name == name)
                {
                    return transform.gameObject;
                }
            }
            return null;
        }
    }
}