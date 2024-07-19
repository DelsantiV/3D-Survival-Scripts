using UnityEditor;
using UnityEngine;

namespace Invector.vCamera
{
    [CustomEditor(typeof(vThirdPersonCameraUpgraded))]
    [CanEditMultipleObjects]
    public class vThirdPersonCameraEditorUpgraded : Editor
    {
        GUISkin skin;
        vThirdPersonCameraUpgraded tpCamera;      
        private Texture2D m_Logo = null;

        void OnSceneGUI()
        {
            if (Application.isPlaying)
                return;
            tpCamera = (vThirdPersonCameraUpgraded)target;
        }

        void OnEnable()
        {
            m_Logo = (Texture2D)Resources.Load("tp_camera", typeof(Texture2D));
            tpCamera = (vThirdPersonCameraUpgraded)target;
            tpCamera.indexLookPoint = 0;
        }

        public override void OnInspectorGUI()
        {
            if (!skin) skin = Resources.Load("vSkin") as GUISkin;
            GUI.skin = skin;

            GUILayout.BeginVertical("THIRD PERSON CAMERA LITE BY Invector", "window");

            GUILayout.Space(30);

            EditorGUILayout.BeginVertical();

            base.OnInspectorGUI();

            GUILayout.Space(10);

            GUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            GUILayout.Space(2);           
        }       
    }
}