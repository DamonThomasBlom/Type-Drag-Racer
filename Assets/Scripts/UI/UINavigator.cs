using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UINavigator : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NavigateTab(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? -1 : 1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) NavigateDirection(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) NavigateDirection(Vector2.down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) NavigateDirection(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) NavigateDirection(Vector2.right);
    }

    private void NavigateTab(int direction)
    {
        List<Selectable> selectableElements = new List<Selectable>();

        foreach (Selectable selectable in Selectable.allSelectablesArray)
        {
            if (selectable.gameObject.activeInHierarchy && selectable.interactable)
            {
                selectableElements.Add(selectable);
            }
        }

        if (selectableElements.Count == 0) return;

        selectableElements.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        GameObject current = EventSystem.current.currentSelectedGameObject;
        int currentIndex = selectableElements.FindIndex(s => s.gameObject == current);
        int nextIndex = (currentIndex + direction + selectableElements.Count) % selectableElements.Count;

        EventSystem.current.SetSelectedGameObject(selectableElements[nextIndex].gameObject);
    }

    private void NavigateDirection(Vector2 direction)
    {
        if (EventSystem.current.currentSelectedGameObject == null) return;

        Selectable current = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        if (current == null) return;

        Selectable next = null;
        if (direction == Vector2.up) next = current.FindSelectableOnUp();
        else if (direction == Vector2.down) next = current.FindSelectableOnDown();
        else if (direction == Vector2.left) next = current.FindSelectableOnLeft();
        else if (direction == Vector2.right) next = current.FindSelectableOnRight();

        if (next != null)
        {
            EventSystem.current.SetSelectedGameObject(next.gameObject);
        }
    }
}
