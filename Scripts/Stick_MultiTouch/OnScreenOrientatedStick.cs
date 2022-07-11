#if ENABLE_INPUT_SYSTEM
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

////TODO: custom icon for OnScreenOrientatedStick component

namespace UnityEngine.InputSystem.OnScreen
{
    /// <summary>
    /// A stick control displayed on screen and moved around by touch or other pointer
    /// input.
    /// </summary>
    [AddComponentMenu("Input/On-Screen Orientated Stick")]
    public class OnScreenOrientatedStick : OnScreenControl
    {
        public enum StickMode
        {
            Regular,
            Horisontal,
            Vertical,
        }

        private void OnPointerDown(Vector2 pos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), pos, cam: null, out m_PointerDownPos);
        }

        private void OnDrag(Vector2 pos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), pos, cam: null, out var position);
            var delta = position - m_PointerDownPos;

            delta.x *= m_StickOrintation.x;
            delta.y *= m_StickOrintation.y;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            SendValueToControl(newPos);
        }

        private void OnPointerUp()
        {
            ((RectTransform)transform).anchoredPosition = m_StartPos;
            SendValueToControl(Vector2.zero);
        }

        private void PressStart(InputAction press, InputAction position)
        {
            if (m_CurPressAction != null) return;

            Vector2 pos = position.ReadValue<Vector2>();
            GameObject guiObj = GUIFromPosition(pos);
            if (guiObj == this.gameObject)
            {
                OnPointerDown(pos);
                m_CurPressAction = press;
                m_CurPosAction = position;
            }
        }

        private void PressEnd(InputAction press)
        {
            if (m_CurPressAction == press)
            {
                OnPointerUp();
                m_CurPressAction = null;
                m_CurPosAction = null;
            }
        }

        private static GameObject GUIFromPosition(Vector2 pos)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = pos;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);

            return raysastResults.Count > 0 ? raysastResults[0].gameObject : null;
        }

        private void Awake()
        {
            m_FirstTouch = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
            m_SecondTouch = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch1/press");
            m_ThirdTouch = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch2/press");

            m_FirstTouchPos = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/touch0/position");
            m_SecondTouchPos = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/touch1/position");
            m_ThirdTouchPos = new InputAction(type: InputActionType.PassThrough, binding: "<Touchscreen>/touch2/position");

            m_FirstTouch.started += _ => PressStart(m_FirstTouch, m_FirstTouchPos);
            m_SecondTouch.started += _ => PressStart(m_SecondTouch, m_SecondTouchPos);
            m_ThirdTouch.started += _ => PressStart(m_ThirdTouch, m_ThirdTouchPos);

            m_FirstTouch.canceled += _ => PressEnd(m_FirstTouch);
            m_SecondTouch.canceled += _ => PressEnd(m_SecondTouch);
            m_ThirdTouch.canceled += _ => PressEnd(m_ThirdTouch);
        }

        override protected void OnEnable()
        {
            m_FirstTouch.Enable();
            m_FirstTouchPos.Enable();
            m_SecondTouch.Enable();
            m_SecondTouchPos.Enable();
            m_ThirdTouch.Enable();
            m_ThirdTouchPos.Enable();
        }

        override protected void OnDisable()
        {
            m_FirstTouch.Disable();
            m_FirstTouchPos.Disable();
            m_SecondTouch.Disable();
            m_SecondTouchPos.Disable();
            m_ThirdTouch.Disable();
            m_ThirdTouchPos.Disable();
        }

        private void Start()
        {
            m_StartPos = ((RectTransform)transform).anchoredPosition;

            if (m_StickMode == StickMode.Regular)
                m_StickOrintation = Vector2.one;
            else if (m_StickMode == StickMode.Horisontal)
                m_StickOrintation = Vector2.right;
            else
                m_StickOrintation = Vector2.up;
        }

        private void Update()
        {
            if(m_CurPressAction != null && m_CurPressAction.phase == InputActionPhase.Performed)
            {
                OnDrag(m_CurPosAction.ReadValue<Vector2>());
            }
        }

        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRange = 50;

        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        [SerializeField]
        private StickMode m_StickMode = StickMode.Regular;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;
        private Vector2 m_StickOrintation;

        private InputAction m_FirstTouch;
        private InputAction m_FirstTouchPos;
        private InputAction m_SecondTouch;
        private InputAction m_SecondTouchPos;
        private InputAction m_ThirdTouch;
        private InputAction m_ThirdTouchPos;

        private InputAction m_CurPressAction;
        private InputAction m_CurPosAction;
        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}
#endif
