using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    Button play_btn;
    Button editor_btn;

    public GameObject game_mgr;
    public GameObject composition_editor;

    private void Awake()
    {
        play_btn = transform.Find("play_btn").GetComponent<Button>();
        editor_btn = transform.Find("editor_btn").GetComponent<Button>();
        play_btn.onClick.AddListener(() => {
            Instantiate(game_mgr, transform.parent);
        });

        editor_btn.onClick.AddListener(() => {
            Instantiate(composition_editor, transform.parent);
        });
    }
}
