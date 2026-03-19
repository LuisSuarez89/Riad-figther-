#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SetupRoadFighter : EditorWindow
{
    [MenuItem("Tools/Configurar Road Fighter")]
    public static void Configurar()
    {
        // 1. Instanciar el coche del jugador (Car_4)
        GameObject carPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Simple Vehicle Pack/Prefabs/Mobile/Car_4.prefab");
        GameObject car = null;
        if (carPrefab != null)
        {
            car = (GameObject)PrefabUtility.InstantiatePrefab(carPrefab);
            car.transform.position = new Vector3(0, 0.5f, 0);
            
            if (car.GetComponent<PlayerCarController>() == null)
            {
                car.AddComponent<PlayerCarController>();
            }

            Rigidbody rb = car.GetComponent<Rigidbody>();
            if (rb == null) rb = car.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 1500f;
            
            car.tag = "Player";
            if (car.GetComponent<Collider>() == null)
            {
                BoxCollider bc = car.AddComponent<BoxCollider>();
                bc.center = new Vector3(0, 0.8f, 0);
                bc.size = new Vector3(2f, 1.5f, 4.5f);
            }
        }
        else
        {
            Debug.LogError("No se pudo encontrar el prefab del Car_4 en Assets/Simple Vehicle Pack/Prefabs/Mobile/Car_4.prefab");
            return;
        }

        // 2. Extraer dimensiones de la pista (z0_)
        GameObject[] allZ0 = GameObject.FindObjectsOfType<GameObject>();
        GameObject z0Road = null;
        
        foreach(GameObject obj in allZ0)
        {
            if (obj.name.StartsWith("z0_"))
            {
                MeshFilter mf = obj.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    string meshName = mf.sharedMesh.name.ToLower();
                    // Agregar colisionadores a los meshes de concreto o asfalto
                    if (meshName.Contains("concrete") || obj.name.Contains("concrete") || meshName.Contains("road"))
                    {
                        if (obj.GetComponent<Collider>() == null)
                        {
                            obj.AddComponent<MeshCollider>();
                        }
                    }
                }

                Renderer r = obj.GetComponent<Renderer>();
                if (r != null)
                {
                    if (z0Road == null || r.bounds.size.x > z0Road.GetComponent<Renderer>().bounds.size.x)
                    {
                        z0Road = obj;
                    }
                }
            }
        }

        if (z0Road != null)
        {
            Renderer rend = z0Road.GetComponent<Renderer>();
            // El ancho de la pista será el menor valor entre X y Z de sus bounds (ya que podría estar rotada)
            float width = Mathf.Min(rend.bounds.size.x, rend.bounds.size.z);
            
            // Margen para que el coche no toque los bordes (reducido a un 85% de la pista)
            float usableWidth = width * 0.85f; 
            float maxOffset = usableWidth / 2f;
            
            PlayerCarController pcc = car.GetComponent<PlayerCarController>();
            SerializedObject pccSo = new SerializedObject(pcc);
            pccSo.FindProperty("maxHorizontalOffset").floatValue = maxOffset;
            pccSo.FindProperty("laneWidth").floatValue = maxOffset; 
            pccSo.ApplyModifiedProperties();

            // Asignar la posición y rotación exacta solicitada
            car.transform.position = new Vector3(275f, 20f, 335f);
            car.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            
            Debug.Log($"Detectada pista '{z0Road.name}' con ancho estimado {width}. Posición: 275,20,335 Rotación: Y=90");
        }

        // 3. Configurar la Cámara
        Camera cam = Camera.main;
        if (cam != null && car != null)
        {
            CameraFollow3D follow = cam.gameObject.GetComponent<CameraFollow3D>();
            if (follow == null) follow = cam.gameObject.AddComponent<CameraFollow3D>();
            follow.target = car.transform;
            // Ajustar posición inicial orientativamente
            cam.transform.position = car.transform.position + car.transform.rotation * follow.localOffset;
            cam.transform.rotation = car.transform.rotation * Quaternion.Euler(15f, 0, 0);
        }

        // 4. GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            GameObject gmObj = new GameObject("GameManager");
            gm = gmObj.AddComponent<GameManager>();
        }

        // 5. Traffic Spawner
        TrafficSpawner ts = FindObjectOfType<TrafficSpawner>();
        if (ts == null)
        {
            GameObject tsObj = new GameObject("TrafficSpawner");
            ts = tsObj.AddComponent<TrafficSpawner>();
        }

        if (car != null)
        {
            SerializedObject so = new SerializedObject(ts);
            so.FindProperty("player").objectReferenceValue = car.transform;
            
            // Ajustar TrafficSpawner a la pista
            if (z0Road != null)
            {
                Renderer rend = z0Road.GetComponent<Renderer>();
                float usableWidth = rend.bounds.size.x * 0.85f;
                int lanes = 3;
                if (usableWidth > 15f) lanes = 4;
                if (usableWidth > 20f) lanes = 5;
                
                so.FindProperty("laneCount").intValue = lanes;
                so.FindProperty("laneWidth").floatValue = usableWidth / lanes;
            }

            GameObject[] traffic = new GameObject[] {
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Simple Vehicle Pack/Prefabs/Mobile/Car_1.prefab"),
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Simple Vehicle Pack/Prefabs/Mobile/Car_2.prefab"),
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Simple Vehicle Pack/Prefabs/Mobile/Car_3.prefab"),
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Simple Vehicle Pack/Prefabs/Mobile/Bus_1.prefab"),
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Simple Vehicle Pack/Prefabs/Mobile/Bus_2.prefab"),
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Simple Vehicle Pack/Prefabs/Mobile/Taxi.prefab")
            };
            
            SerializedProperty trafficProp = so.FindProperty("trafficPrefabs");
            trafficProp.ClearArray();
            int added = 0;
            for(int i = 0; i < traffic.Length; i++) {
                if (traffic[i] != null) {
                    trafficProp.InsertArrayElementAtIndex(added);
                    trafficProp.GetArrayElementAtIndex(added).objectReferenceValue = traffic[i];
                    added++;
                }
            }
            so.ApplyModifiedProperties();
        }

        Debug.Log("¡Escena configurada exitosamente y ajustada a la pista! Ahora puedes presionar Play.");
    }
}
#endif
