using System;
using System.Collections;

namespace EnvironmentControllers
{
    public class TrainingEnvironmentController : EnvironmentController
    {
        public override void Reset()
        {
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return null;
            OnTurnStarted?.Invoke(this);
        }
    }
}
