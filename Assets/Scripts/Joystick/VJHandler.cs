using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class VJHandler : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform jsContainer;
    public RectTransform joystick;

    public Vector3 InputDirection;

    private static VJHandler _instance;
    public static VJHandler instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
    }
    public void OnDrag(PointerEventData ped)
    {
        Vector2 position = Vector2.zero;

        //To get InputDirection
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                (jsContainer,
                ped.position,
                ped.pressEventCamera,
                out position);

        float x = position.x;
        float y = position.y;

        InputDirection = new Vector3(x, y, 0);
        InputDirection = (InputDirection.magnitude > .5f) ? InputDirection.normalized / 2 : InputDirection;

        //to define the area in which joystick can move around
        joystick.anchoredPosition = new Vector3(InputDirection.x, InputDirection.y);

    }

    public void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public void OnPointerUp(PointerEventData ped)
    {
        InputDirection = Vector3.zero;
        joystick.anchoredPosition = Vector3.zero;
    }
}