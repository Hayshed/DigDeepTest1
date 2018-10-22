using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour {


    Dictionary<Character, GameObject> characterGameObjectMap;

    Dictionary<string, Sprite> characterSprites;

    // Use this for initialization
    void Start () {

        LoadSprites();

        characterGameObjectMap = new Dictionary<Character, GameObject>();

        WorldController.Instance.World.RegisterCharacterCreated(OnCharacterCreated);

        //Check for pre-existing chracters, which won't do the callback
        foreach (Character c in WorldController.Instance.World.characterList) {
            OnCharacterCreated(c);
        }
    }

    void LoadSprites() {
        characterSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Characters/");

        Debug.Log("LOADED RESOURCE:");
        foreach (Sprite s in sprites) {
            Debug.Log(s);
            characterSprites[s.name] = s;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    // When a character is created by world, make a visual gameobject for it 
    void OnCharacterCreated(Character c) {
        Debug.Log("OnCharacterCreated");

        GameObject cha_go = new GameObject();

        characterGameObjectMap.Add(c, cha_go);

        cha_go.name = "Goblin";
        cha_go.transform.position = new Vector3(c.X, c.Y, 0);
        cha_go.transform.SetParent(this.transform, true);

        SpriteRenderer sr = cha_go.AddComponent<SpriteRenderer>();

        sr.sprite = characterSprites["Goblin"];
        sr.sortingLayerName = "Characters";

        // Regeister us wanting to know when the character changes
        c.RegisterOnChangedCallback(OnCharacterChanged);
    }

    // When a character is changed, change the visual gameobject to represent the change
    void OnCharacterChanged(Character c) {

        if (characterGameObjectMap.ContainsKey(c) == false) {
            Debug.LogError("OnCharacterChanged -- trying to chnage visuals for a character not in our map");
        }

        GameObject cha_go = characterGameObjectMap[c];
        cha_go.transform.position = new Vector3(c.X, c.Y, 0);   // update the visuals for movement

    }

}
