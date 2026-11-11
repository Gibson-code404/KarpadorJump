using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Eine einzige Stelle für alle Eingaben.
// Funktioniert mit dem alten Input Manager UND mit dem neuen Input System.
public static class GameInput
{
    public static bool JumpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        bool key = kb != null && (kb.spaceKey.wasPressedThisFrame
                               || kb.upArrowKey.wasPressedThisFrame
                               || kb.wKey.wasPressedThisFrame);
        bool click = mouse != null && mouse.leftButton.wasPressedThisFrame;
        return key || click;
#else
        return Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.UpArrow)
            || Input.GetKeyDown(KeyCode.W)
            || Input.GetMouseButtonDown(0);
#endif
    }

    public static bool PausePressed()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        return kb != null && (kb.escapeKey.wasPressedThisFrame || kb.pKey.wasPressedThisFrame);
#else
        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
#endif
    }
    public static float MoveAxis()
    {
    #if ENABLE_INPUT_SYSTEM
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb == null) return 0f;
        float x = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        return x;
    #else
        return Input.GetAxisRaw("Horizontal");
    #endif
        
    }
}