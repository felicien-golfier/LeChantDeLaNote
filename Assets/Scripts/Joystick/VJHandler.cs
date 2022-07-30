using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class VJHandler : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform jsContainer;
    public RectTransform joystick;

    public Vector2 InputDirection;

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
        //To get InputDirection
        RectTransformUtility.ScreenPointToLocalPointInRectangle
                (jsContainer,
                ped.position,
                ped.pressEventCamera,
                out InputDirection);
        InputDirection.x /= GetComponent<RectTransform>().rect.size.x/2;
        InputDirection.y /= GetComponent<RectTransform>().rect.size.y/2;
        InputDirection = (InputDirection.magnitude > .5f) ? InputDirection.normalized / 2 : InputDirection;
        //to define the area in which joystick can move around
        joystick.anchoredPosition = new Vector3(
            InputDirection.x*GetComponentInChildren<CircleCollider2D>().radius,
            InputDirection.y * GetComponentInChildren<CircleCollider2D>().radius);
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