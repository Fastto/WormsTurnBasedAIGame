namespace EnvironmentControllers
{
    public class TrainingEnvironmentController : EnvironmentController
    {
        public override void Reset()
        {
            OnTurnStarted?.Invoke(this);
        }
    }
}
