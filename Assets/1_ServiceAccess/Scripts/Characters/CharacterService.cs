using System.Collections.Generic;
using UnityEngine;

namespace Excercise1
{
    public class CharacterService : MonoBehaviour
    {
        //Intancia accesible globalmente
        public static CharacterService Instance {  get; private set; }

        private readonly Dictionary<string, ICharacter> _charactersById = new();

        private void Awake()
        {
            //si ya existe una intancia que no sea esta se destruye el objeto actual
            if(Instance != null && Instance != this)
            {
                Debug.LogWarning("Another instance of CharacterService was foun and destroyed.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public bool TryAddCharacter(string id, ICharacter character)
            => _charactersById.TryAdd(id, character);
        public bool TryRemoveCharacter(string id)
            => _charactersById.Remove(id);

        public ICharacter GetCharacterById(string id)
        {
            _charactersById.TryGetValue(id, out ICharacter character);
            return character;
        }
    }
}
