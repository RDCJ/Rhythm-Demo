using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Note;

public class JudgeTigger : MonoBehaviour
{
    public NoteBase note;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("JudgeLine"))
        {
            note.Activate();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("JudgeLine"))
        {
            note.Miss();
        }
    }
}
