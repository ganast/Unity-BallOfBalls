using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using System.IO;

public class SimManager : MonoBehaviour {

    [SerializeField]
    private GameObject ballGameObjectPrefab = null;

    [SerializeField]
    private Transform ballsGameObjectParent = null;

    [SerializeField]
    private float ballScale = 1.0f;

    [SerializeField]
    private GameObject boxGameObjectPrefab = null;

    [SerializeField]
    private float spawnExtent = 4000;

    [SerializeField]
    private int spawnCount = 1000;

    [SerializeField]
    private float initialSpeed = 1000;

    [SerializeField]
    private float forceFactor = 1000;

    [SerializeField]
    private bool isBoundingBoxEnabled = true;

    [SerializeField]
    private bool isDOTSModeEnabled = true;

    private GameObject boundingBoxGameObject = null;

    private Entity ballEntityPrefab = Entity.Null;

    private Entity boundingBoxEntityPrefab = Entity.Null;

    private Entity boundingBoxEntity = Entity.Null;

    private static SimManager inst = null;

    public static SimManager GetSimManager() {
        return inst;
    }

    public void Reset() {

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (boundingBoxGameObject != null) {
            Destroy(boundingBoxGameObject);
            boundingBoxGameObject = null;
        }

        if (boundingBoxEntity != Entity.Null) {
            entityManager.DestroyEntity(boundingBoxEntity);
            boundingBoxEntity = Entity.Null;
        }

        ClearChildGameObjects(ballsGameObjectParent);
        ClearEntitiesWithTag<BallEntityTag>(entityManager);

        ballGameObjectPrefab.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
        CompositeScale compositeScaleComponent = entityManager.GetComponentData<Unity.Transforms.CompositeScale>(ballEntityPrefab);
        compositeScaleComponent.Value = new float4x4(
            ballScale,  0,          0,          0,
            0,          ballScale,  0,          0,
            0,          0,          ballScale,  0,
            0,          0,          0,          1
        );
        entityManager.SetComponentData(ballEntityPrefab, compositeScaleComponent);

        if (isDOTSModeEnabled) {

            if (isBoundingBoxEnabled) {
                boundingBoxEntity = entityManager.Instantiate(boundingBoxEntityPrefab);
            }

            SpawnEntities(spawnCount, ballEntityPrefab, spawnExtent, initialSpeed, entityManager);
        }
        else {

            if (isBoundingBoxEnabled) {
                boundingBoxGameObject = Instantiate(boxGameObjectPrefab);
            }

            SpawnGameObjects(spawnCount, ballGameObjectPrefab, spawnExtent, initialSpeed, ballsGameObjectParent);
        }
    }

    public void SwitchMode() {

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (boundingBoxGameObject != null) {
            Destroy(boundingBoxGameObject);
            boundingBoxGameObject = null;
        }

        if (boundingBoxEntity != Entity.Null) {
            entityManager.DestroyEntity(boundingBoxEntity);
            boundingBoxEntity = Entity.Null;
        }

        if (isDOTSModeEnabled) {
            if (isBoundingBoxEnabled) {
                boundingBoxEntity = entityManager.Instantiate(boundingBoxEntityPrefab);
            }
            EntitiesFromGameObjects(ballEntityPrefab, ballsGameObjectParent, entityManager);
        }
        else {
            if (isBoundingBoxEnabled) {
                boundingBoxGameObject = Instantiate(boxGameObjectPrefab);
            }
            GameObjectsFromEntities(ballGameObjectPrefab, ballsGameObjectParent, entityManager);
        }
    }

    public void ToggleBoundingBox() {

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (isDOTSModeEnabled) {
            if (boundingBoxGameObject != null) {
                Destroy(boundingBoxGameObject);
                boundingBoxGameObject = null;
            }
            if (boundingBoxEntity != Entity.Null) {
                entityManager.DestroyEntity(boundingBoxEntity);
                boundingBoxEntity = Entity.Null;
            }
            else {
                boundingBoxEntity = entityManager.Instantiate(boundingBoxEntityPrefab);
            }
        }
        else {
            if (boundingBoxEntity != Entity.Null) {
                entityManager.DestroyEntity(boundingBoxEntity);
                boundingBoxEntity = Entity.Null;
            }
            if (boundingBoxGameObject != null) {
                Destroy(boundingBoxGameObject);
                boundingBoxGameObject = null;
            }
            else {
                boundingBoxGameObject = Instantiate(boxGameObjectPrefab);
            }
        }
    }

    /**************************************************************************/

    /**
     * 
     */
    public static void ClearChildGameObjects(Transform parent) {
        for (int i = parent.childCount - 1; i >= 0; i--) {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    /**
     * 
     */
    public static GameObject SpawnGameObject(GameObject prefab, Vector3 translation, Quaternion rotation, Vector3 velocity, Transform parent) {

        GameObject gameObject = GameObject.Instantiate(prefab, translation, rotation, parent);

        // gameObject.name = "Ball";

        gameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);

        return gameObject;
    }

    /**
     * 
     */
    public static GameObject SpawnGameObject(GameObject prefab, float extent, float speed, Transform parent) {

        Vector3 translation = GetRandomTranslation(extent);
        Quaternion rotation = GetRandomRotation(180.0f, 90.0f);
        Vector3 velocity = GetRandomVelocity(speed);

        return SpawnGameObject(prefab, translation, rotation, velocity, parent);
    }

    /**
     * 
     */
    public static void SpawnGameObjects(int count, GameObject prefab, float extent, float speed, Transform parent) {
        for (int i = 0; i <= count - 1; i++) {
            SpawnGameObject(prefab, extent, speed, parent);
        }
    }

    /**************************************************************************/

    public static void ClearEntitiesWithTag<T>(EntityManager entityManager) where T: IComponentData {
        EntityQuery q = entityManager.CreateEntityQuery(ComponentType.ReadOnly<BallEntityTag>());
        entityManager.DestroyEntity(q);
    }

    public static Entity SpawnEntity(Entity prefab, Vector3 translation, Quaternion rotation, Vector3 velocity, EntityManager entityManager) {

        Entity entity = entityManager.Instantiate(prefab);

        //entityManager.SetName(ball, "Ball");

        // entityManager.SetComponentData(ball, new Translation { Value = position });
        Translation translationComponent = entityManager.GetComponentData<Translation>(entity);
        translationComponent.Value = translation;
        entityManager.SetComponentData(entity, translationComponent);

        // entityManager.SetComponentData(ball, new Translation { Value = position });
        Rotation rotationComponent = entityManager.GetComponentData<Rotation>(entity);
        rotationComponent.Value = rotation;
        entityManager.SetComponentData(entity, rotationComponent);

        // entityManager.SetComponentData(ball, new PhysicsVelocity { Linear = velocity });
        PhysicsVelocity physicsVelocityComponent = entityManager.GetComponentData<PhysicsVelocity>(entity);
        physicsVelocityComponent.Linear = velocity;
        entityManager.SetComponentData(entity, physicsVelocityComponent);

        return entity;
    }

    public static Entity SpawnEntity(Entity prefab, float extent, float speed, EntityManager entityManager) {

        Vector3 translation = GetRandomTranslation(extent);
        Quaternion rotation = GetRandomRotation(180.0f, 90.0f);
        Vector3 velocity = GetRandomVelocity(speed);

        return SpawnEntity(prefab,  translation, rotation, velocity, entityManager);
    }

    public static void SpawnEntities(int count, Entity prefab, float extent, float speed, EntityManager entityManager) {
        for (int i = 0; i <= count - 1; i++) {
            SpawnEntity(prefab, extent, speed, entityManager);
        }
    }

    /**************************************************************************/

    public static void EntitiesFromGameObjects(Entity prefab, Transform parent, EntityManager entityManager) {
        for (int i = parent.childCount - 1; i >= 0; i--) {
            GameObject gameObject = parent.GetChild(i).gameObject;
            Vector3 translation = gameObject.transform.position;
            Quaternion rotation = gameObject.transform.rotation;
            Vector3 velocity = gameObject.GetComponent<Rigidbody>().velocity;
            Destroy(parent.GetChild(i).gameObject);
            SpawnEntity(prefab, translation, rotation, velocity, entityManager);
        }
    }

    public static void GameObjectsFromEntities(GameObject prefab, Transform parent, EntityManager entityManager) {
        EntityQuery q = entityManager.CreateEntityQuery(ComponentType.ReadOnly<BallEntityTag>());
        // NativeArray<Entity> entities = q.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> entities = q.ToEntityArray(Allocator.Persistent);
        foreach (Entity entity in entities) {
            Vector3 translation = entityManager.GetComponentData<Translation>(entity).Value;
            Quaternion rotation = entityManager.GetComponentData<Rotation>(entity).Value;
            Vector3 velocity = entityManager.GetComponentData<PhysicsVelocity>(entity).Linear;
            SpawnGameObject(prefab, translation, rotation, velocity, parent);
            // entityManager.DestroyEntity(entity);
        }
        entityManager.DestroyEntity(entities);
        entities.Dispose();
    }

    /**************************************************************************/

    public static Vector3 GetRandomTranslation(float extent) {
        return new Vector3(
            UnityEngine.Random.Range(-extent, +extent),
            UnityEngine.Random.Range(-extent, +extent),
            UnityEngine.Random.Range(-extent, +extent)
        );
    }

    public static Quaternion GetRandomRotation(float yawExtent, float pitchExtent) {
        return Quaternion.Euler(
            0.0f,
            UnityEngine.Random.Range(-yawExtent, +yawExtent),
            UnityEngine.Random.Range(-pitchExtent, +pitchExtent)
        );
    }

    public static Vector3 GetRandomVelocity(float speed) {
        return new Vector3(
            UnityEngine.Random.Range(-1.0f, 1.0f),
            UnityEngine.Random.Range(-1.0f, 1.0f),
            UnityEngine.Random.Range(-1.0f, 1.0f)
        ).normalized * speed;
    }

    /**************************************************************************/

    public static void DumpComponentTypes(Entity entity, EntityManager entityManager, string path) {
        NativeArray<ComponentType> comps = entityManager.GetComponentTypes(entity);
        StreamWriter f = new StreamWriter(File.OpenWrite(path));
        foreach (ComponentType t in comps) {
            f.WriteLine(t.ToString());
        }
        f.Close();
    }

    /**************************************************************************/

    public void Awake() {
        inst = this;
    }

    public void Start() {

        BlobAssetStore bas = new BlobAssetStore();

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(
            World.DefaultGameObjectInjectionWorld, bas);

        ballEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
            ballGameObjectPrefab, settings);

        boundingBoxEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
            boxGameObjectPrefab, settings);

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        entityManager.AddComponent<BallEntityTag>(ballEntityPrefab);
        
        // entityManager.SetName(boundingBoxEntityPrefab, "BoundingBoxPrefab");
        // entityManager.SetName(ballEntityPrefab, "BallPrefab");

        // bas.Dispose();

        Reset();
    }

    public void Update() {
        
    }

    /**************************************************************************/

    public float GetBallScale() {
        return ballScale;
    }

    public void SetBallScale(float ballScale) {
        if (ballScale > 0) {
            this.ballScale = ballScale;
        }
    }

    public int GetSpawnCount() {
        return spawnCount;
    }

    public void SetSpawnCount(int spawnCount) {
        if (spawnCount > 0) {
            this.spawnCount = spawnCount;
        }
    }

    public float GetSpawnExtent() {
        return spawnExtent;
    }

    public void SetSpawnExtent(float spawnExtent) {
        if (spawnExtent > 0) {
            this.spawnExtent = spawnExtent;
        }
    }

    public float GetInitialSpeed() {
        return initialSpeed;
    }

    public void SetInitialSpeed(float initialSpeed) {
        if (initialSpeed > 0) {
            this.initialSpeed = initialSpeed;
        }
    }

    public float GetForceFactor() {
        return forceFactor;
    }

    public void SetForceFactor(float forceFactor) {
        if (forceFactor > 0) {
            this.forceFactor = forceFactor;
        }
    }

    public bool IsBoundingBoxEnabled() {
        return isBoundingBoxEnabled;
    }

    public void SetBoundingBoxEnabled(bool isBoundingBoxEnabled) {
        this.isBoundingBoxEnabled = isBoundingBoxEnabled;
    }

    public bool IsDOTSModeEnabled() {
        return isDOTSModeEnabled;
    }

    public void SetDOTSModeEnabled(bool isDOTSModeEnabled) {
        this.isDOTSModeEnabled = isDOTSModeEnabled;
    }

    private class BallEntityTag: IComponentData {

    }
}
