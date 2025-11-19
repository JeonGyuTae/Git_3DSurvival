using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnimalType
{
    Carnivore,
    Herbivore,
}

/// <summary>
/// AnimalData SO 데이터
/// Animal Data가 가지는 필드는 다음과 같다.
/// 
/// - animalName :  동물 이름
/// - description:  동물 설명
/// - maxHp :       동물 체력
/// - atk   :       동물 공격력
/// </summary>
[CreateAssetMenu(fileName = "NewAnimal", menuName = "Animal/newAnimalData")]
public class AnimalData : ScriptableObject
{
    [Header("Animal Info")]
    public string animalName;
    public string description;
    public AnimalType type;

    [Header("Stat Info")]
    public float maxHp;
    public float atk;

    [Header("Spawn Info")]
    public GameObject prefab;
    public float respawnTime;
}
