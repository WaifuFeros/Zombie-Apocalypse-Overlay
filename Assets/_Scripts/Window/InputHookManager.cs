using UnityEngine;

public class InputHookManager : MonoBehaviour
{
    [Header("Hook Settings")]
    public bool enableKeyboardHook = true;  // Permet d'activer ou désactiver le hook clavier dans l'inspector
    public bool enableMouseHook = true;     // Permet d'activer ou désactiver le hook souris dans l'inspector

    void Start()
    {
        // Active les hooks selon les paramètres
        if (enableKeyboardHook)
            InputHook.EnableHook(InputHook.HookType.Keyboard);

        if (enableMouseHook)
            InputHook.EnableHook(InputHook.HookType.Mouse);
    }

    private void OnDestroy() => DeactivateInputHooks();

    private void OnApplicationQuit() => DeactivateInputHooks();


    private void DeactivateInputHooks()
    {
        // Désactive les hooks lorsque l'objet est détruit
        if (enableKeyboardHook)
            InputHook.DisableHook(InputHook.HookType.Keyboard);

        if (enableMouseHook)
            InputHook.DisableHook(InputHook.HookType.Mouse);
    }
}