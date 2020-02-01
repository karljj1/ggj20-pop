using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class ControlsDebug : MonoBehaviour
{
    InputSystem m_Controls;
    float m_MoveDir;

    void Awake()
    {
        m_Controls = new InputSystem();
        m_Controls.Game.Jump.performed += Jump;
        m_Controls.Game.Drop.performed += Drop;
    }

    void OnEnable()
    {
        m_Controls.Enable();
    }

    void OnDisable()
    {
        m_Controls.Disable();
    }

    void Update()
    {
        m_MoveDir = m_Controls.Game.Move.ReadValue<float>();
    }

    void Jump(CallbackContext _) 
    {
        Debug.Log("Jump");
    }

    void Drop(CallbackContext _)
    {
        Debug.Log("Drop");
    }

    void OnGUI()
    {
        GUILayout.Label("Move " + m_MoveDir);
    }

}
