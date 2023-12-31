using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Game.Component;
using Game.Service;
using Game.System;
using Game.System.Timing;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using ScriptableData;
using UnityEngine;

public class Startup : MonoBehaviour
{
    private EcsWorld world;
    private EcsSystems systems;
    private EcsSystems phisSystems;
    public static PlayerData PlayerData;

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private StaticData staticData;
    void Start()
    {
        world = new EcsWorld();
        var eventWorld = new EcsWorld();
        systems = new EcsSystems(world);
        phisSystems = new EcsSystems(world);
        EcsPhysicsEvents.ecsWorld = eventWorld;

        LoadData();
        
        phisSystems.AddWorld(eventWorld,Idents.EVENT_WORLD)
            
            .Add(new MoveApplySystem())
            .Add(new CanonDirectionRotationSystem())
            .Add(new CanonRotationSystem())
            .Add(new CollisionSystem())
            .Add(new MarbleSystem())
           

            .DelHerePhysics(Idents.EVENT_WORLD)
          
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.EVENT_WORLD))
#endif
            .Inject(new Fabric(world,staticData,sceneData))
            .Inject(sceneData)
            .Inject(staticData)
            .Inject(new MovementService(world))
            .Inject(new AnimationService(world))
            .Init();


        systems
            .AddWorld(eventWorld,Idents.EVENT_WORLD)
            .Add(new InitWorldSystem())
            
            .Add(new ShotButtonSystem())
            .Add(new X2System())
            .Add(new ShotSystem())
            .Add(new ToggleInputSystem())
            .Add(new HitSystem())
            .Add(new RiseFallViewSystem())
            .Add(new AddMarbleTickSystem())
            .Add(new DestroyDeadSystem())

            //.Add(new TickSystem())
            .Add(new UpdateBulletUISystem())

            .DelHere<CoinsChangedEventComponent>(Idents.EVENT_WORLD)
            .DelHere<ReleaseEvent>(Idents.EVENT_WORLD)
            .DelHere<X2Event>(Idents.EVENT_WORLD)
            .DelHere<ToggleInputEvent>(Idents.EVENT_WORLD)
            .DelHere<RiseEvent>(Idents.EVENT_WORLD)
            .DelHere<FallEvent>(Idents.EVENT_WORLD)
            .DelHere<HitEvent>(Idents.EVENT_WORLD)
            .DelHere<EndGameEvent>(Idents.EVENT_WORLD)
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.EVENT_WORLD))
#endif
            .Inject(new Fabric(world,staticData,sceneData))
            .Inject(sceneData)
            .Inject(staticData)
            .Inject(new MovementService(world))
            .Inject(new AnimationService(world))
            .InjectUgui(sceneData.EcsUguiEmitter,Idents.EVENT_WORLD)
            .Init();
    }

    private void LoadData()
    {
        PlayerData = new PlayerData();
        PlayerData.Level = PlayerPrefs.GetInt("Level");
    }
    
    public static void SaveData()
    {
        PlayerPrefs.SetInt("Level", PlayerData.Level);
      
    }


    void Update()
    {
        systems?.Run();
    }
    
    void FixedUpdate()
    {
        phisSystems?.Run();
    }

    private void OnDestroy()
    {
        SaveData();
        if (systems!=null)
        {
            systems.Destroy();
            systems = null;
        }
        if (phisSystems!=null)
        {
            phisSystems.Destroy();
            phisSystems = null;
        }

        if (world!=null)
        {
            world.Destroy();
            world = null;
        }  
        EcsPhysicsEvents.ecsWorld = null;

      
    }
}
