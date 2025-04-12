using UnityEngine;

namespace WormComponents
{
   public class HumanInput : MonoBehaviour
   {
      public WormController m_WormController;

      private void Update()
      {
         var xAxis = Input.GetAxis("Horizontal") > 0f ? 1 : Input.GetAxis("Horizontal") < 0f ? -1 : 0;
         var yAxis = Input.GetAxis("Vertical") > 0f ? 1 : Input.GetAxis("Vertical") < 0f ? -1 : 0; 
         var jumpButton = Input.GetButtonDown("Jump"); 
         var fireButton = Input.GetButtonDown("Fire1");
         var cancelButton = Input.GetButtonDown("Fire2");

         switch (m_WormController.m_State.m_State)
         {
            case WormState.States.MOVING:
               m_WormController.m_ControllingSignals.m_HorizontalMoving = xAxis;
               m_WormController.m_ControllingSignals.m_Jump = jumpButton;
               m_WormController.m_ControllingSignals.m_Aim = fireButton;
               break;
            case WormState.States.SHOOTING:
               m_WormController.m_ControllingSignals.m_Aimning = xAxis;
               m_WormController.m_ControllingSignals.m_PowerChanging = yAxis;
               m_WormController.m_ControllingSignals.m_AimCancel = cancelButton;
               m_WormController.m_ControllingSignals.m_Fire = fireButton;
               break;
            case WormState.States.ESCAPING:
               m_WormController.m_ControllingSignals.m_HorizontalMoving = xAxis;
               m_WormController.m_ControllingSignals.m_Jump = jumpButton;
               break;
            default:
               break;
         }
      }
   }
}
