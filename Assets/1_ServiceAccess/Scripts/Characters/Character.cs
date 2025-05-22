using System;
using UnityEngine;

namespace Excercise1
{
    public class Character : MonoBehaviour, ICharacter
    {
        [SerializeField] protected string id;
        [SerializeField] protected CharacterService characterService;

        protected virtual void OnEnable()
        {
            characterService = CharacterService.Instance;

            if (characterService ==  null )
            {
                Debug.LogError("CharacterService.Instance is null. Make sure there´s one in the scene.");
            }
            
            characterService.TryAddCharacter(id, this);
             
            //TODO: Add to CharacterService. The id should be the given serialized field. 
        }

        protected virtual void OnDisable()
        {
            characterService.TryRemoveCharacter(id);
            //TODO: Remove from CharacterService.
        }
    }
}