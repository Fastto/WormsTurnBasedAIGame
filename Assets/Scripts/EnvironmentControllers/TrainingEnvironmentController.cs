using System;
using System.Collections;

namespace EnvironmentControllers
{
    public class TrainingEnvironmentController : EnvironmentController
    {
        public override void Reset()
        {
            // starting new turn 1 frame later to avoid intersection with another logic 
            // during the EndTimer phase within the training
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return null;
            OnTurnStarted?.Invoke(this);
        }
    }
}
