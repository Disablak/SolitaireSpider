using UnityEngine;
using UnityEngine.EventSystems;


public class RowUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int _id = -1;

    public void Layout()
    {

    }

    public void AddCard()
    {

    }

    public void Remove()
    {

    }

    public void OnPointerEnter( PointerEventData eventData )
    {
        Debug.Log( $"enter {_id}" );
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        Debug.Log( $"exit {_id}" );
    }
}
