using TMPro;
using UnityEngine;

public class NameSuggestions : MonoBehaviour
{
    [SerializeField] [ReadOnlyEditor] private string selectedName;

    [SerializeField] private string[] names;

    [SerializeField] TMP_Text suggestionText;
    [SerializeField] TMP_InputField nameInput;

    private void Start()
    {
        RerollName();
    }

    // Update is called once per frame
    void Update()
    {
        //reroll name when there is no text in the input and the user presses backspace or delete
        if ((Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete)) && nameInput.text.Length <= 0)
            RerollName();

        //apply name when there is no input and tab is pressed
        else if (Input.GetKeyDown(KeyCode.Tab) && nameInput.text.Length <= 0)
            SelectName();
    }

    void RerollName()
    {
        selectedName = names[Random.Range(0, names.Length)];
        DisplaySuggestion();
    }

    void DisplaySuggestion()
    {
        suggestionText.text = selectedName;
    }

    void SelectName()
    {
        nameInput.text = selectedName;
        //nameInput.caretPosition = selectedName.Length;
        nameInput.selectionStringAnchorPosition = 0;
        nameInput.selectionStringFocusPosition = selectedName.Length;
    }
}
