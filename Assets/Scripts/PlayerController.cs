using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using System.Xml;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 1.5f;

    private Animator animator;
    int isMovingID;

    private EntityManager entityManager;
    private Entity playerEntity;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        isMovingID = Animator.StringToHash("IsMoving");
    }

    private void Start()
    {
        //Find and cache the Player entity from ECS world to this Monobehavior class
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerTag>());
        if (entityQuery.TryGetSingletonEntity<PlayerTag>(out Entity entity))
        {
            playerEntity = entity;
            if (entityManager.HasComponent<LocalTransform>(playerEntity))
            {
                //Update the Player entity's position on ECS world based on Player gameobject
                var playerEntityTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
                playerEntityTransform.Position = (float3)transform.position;
                entityManager.SetComponentData(playerEntity, playerEntityTransform);
            }
        }

    }

    private void Update()
    {
        var moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        animator.SetBool(isMovingID, math.lengthsq(moveInput) != 0);

        if (math.lengthsq(moveInput) == 0)
        {
            return;
        }

        moveInput = math.normalize(moveInput);
        if (playerEntity != Entity.Null && entityManager.HasComponent<LocalTransform>(playerEntity))
        {
            transform.position += moveInput * playerSpeed * Time.deltaTime;
            transform.rotation = quaternion.LookRotation(moveInput, math.up());

            //Update the Player entity's position on ECS world based on Player gameobject
            var playerEntityTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
            playerEntityTransform.Position = (float3)transform.position;
            entityManager.SetComponentData(playerEntity, playerEntityTransform);
        }
    }
}
