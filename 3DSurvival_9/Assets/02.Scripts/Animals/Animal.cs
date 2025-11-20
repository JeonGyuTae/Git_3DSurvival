using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animal 스크립트
/// Animal에 대한 Interact를 처리하는 스크립트이다.
/// </summary>
public class Animal : MonoBehaviour, IInteractable, IDamageable, ICullable
{
    [SerializeField] private AnimalData data;

    protected AIController controller;

    [Header("Compnent")]
    private Rigidbody _rigidbody;
    private AnimalConditionHandler conditionHandler;
    private AnimalAnimationHandler animationHandler;
    protected SkinnedMeshRenderer skinnedMeshRenderer;
    private ParticleSystem fx_dead;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        animationHandler = GetComponent<AnimalAnimationHandler>();
        conditionHandler = GetComponent<AnimalConditionHandler>();;
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        SetController();

        fx_dead = GetComponentInChildren<ParticleSystem>();

        Init();
    }

    private void SetController()
    {
        switch(data.type)
        {
            case AnimalType.Herbivore:
                controller = GetComponent<HerbivoreAIController>();
                break;
            case AnimalType.Carnivore:
                controller = GetComponent<CarnivoreAIController>();
                break;
            case AnimalType.Partner:
                controller = GetComponent<DogAIController>();
                break;
            default:
                Debug.Log("타입이 없습니다.");
                break;
        }
    }

    public void Init()
    {
        // 체력 설정
        conditionHandler.SetHealth(data.maxHp);

        // SkinnedMeshRenderer 설정
        skinnedMeshRenderer.enabled = true;

        // controller Init
        controller.Init();

        DisableCullComponents();
    }

    public virtual InteractableType GetInteractableType()
    {
        if (data.type == AnimalType.Partner) return InteractableType.NPC;
        return InteractableType.Animal;
    }

    public string GetInteractPrompt()
    {
        return data.animalName;
    }

    public void ShowInteractUI()
    {

    }

    public void HideInteractUI()
    {
        
    }

    public virtual void OnInteract()
    {
        if(TryGetComponent<DogAIController>(out  var controller))
        {
            controller.SetCanActive();
        }
    }

    public void TakeDamage(int damage)
    {
        
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        controller.OnHit(damage, hitPosition);
    }

    public void EnableCullComponents()
    {
        if (controller == null) return;
        if (controller.IsDeaded) return;

        skinnedMeshRenderer.enabled = true;
        controller.enabled = true;
    }

    public virtual void DisableCullComponents()
    {
        if (data.type == AnimalType.Partner) return;

        skinnedMeshRenderer.enabled = false;
        controller.enabled = false;
    }

    #region 프로퍼티

    public AnimalData Data { get { return data; } }
    public AnimalAnimationHandler AnimationHandler { get { return animationHandler; } }
    public AnimalConditionHandler ConditionHandler { get { return conditionHandler; } }
    public Rigidbody Rb { get { return _rigidbody; } } 
    public SkinnedMeshRenderer SkinnedMeshRenderer { get { return skinnedMeshRenderer; }}
    public ParticleSystem FX_Dead { get { return fx_dead; } }

    #endregion
}
