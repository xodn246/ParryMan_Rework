using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This class helps the Dialogue System's demo work with the New Input System. It
    /// registers the inputs defined in DemoInputControls for use with the Dialogue 
    /// System's Input Device Manager.
    /// </summary>
    public class DemoInputRegistration : MonoBehaviour
    {

#if USE_NEW_INPUT

        private Input_Player controls;

        // Track which instance of this script registered the inputs, to prevent
        // another instance from accidentally unregistering them.
        protected static bool isRegistered = false;
        private bool didIRegister = false;

        void Awake()
        {
            controls = new Input_Player();
        }

        void OnEnable()
        {
            if (!isRegistered)
            {
                isRegistered = true;
                didIRegister = true;
                controls.Enable();
                InputDeviceManager.RegisterInputAction("Dialogue", controls.CutScene.Dialogue);
            }
        }

        void OnDisable()
        {
            if (didIRegister)
            {
                isRegistered = false;
                didIRegister = false;
                controls.Disable();
                InputDeviceManager.RegisterInputAction("Dialogue", controls.CutScene.Dialogue);
            }
        }

#endif

    }
}
