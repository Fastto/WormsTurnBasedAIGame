using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using WormComponents;

namespace EnvironmentControllers
{
    public class PlayingEnvironmentController : EnvironmentController
    {
        public List<TeamSO> m_TeamsSettings;
        protected List<Team> m_Teams = new List<Team>();

        private void Start()
        {
            m_TeamsSettings.ForEach(ts =>
            {
                var team = new Team(this, ts);
                m_Teams.Add(team);
            });
            
            StartCoroutine(GameLoop());
        }

        private IEnumerator GameLoop()
        {
            yield return null;
            while (true) //todo: start game from menu
            {
                yield return null;
                Reset();
                
                OnGameStarted?.Invoke(this);
                int currentTeamId = 0;
                while (!IsGameFinished())
                {
                    //new turn
                    currentTeamId = GetNextTeamId(currentTeamId);
                    var worm = m_Teams[currentTeamId].GetNextWorm();
                    worm.m_State.m_State = WormState.States.MOVING;
                    OnTurnStarted?.Invoke(this);
                    while (worm.m_State.m_State != WormState.States.IDLE && !IsGameFinished())
                    {
                        yield return null;
                    }
                    OnTurnFinished?.Invoke(this);
                }
            }
            
        }
        
        public override void Reset()
        {
            m_Teams.ForEach(team => team.DestroyAllWorms());
            m_Teams.ForEach(team => team.Reset());

            PlaceWormRandomly(new List<WormController>{m_Teams[0].GetWorms()[0], m_Teams[1].GetWorms()[0]});
        }

        /**
         * Game is finished when theres are no more then one teams have live worms
         */
        protected bool IsGameFinished()
        {
            int numberOfTeamsWithActiveWorms = 0;
            m_Teams.ForEach(team => 
                numberOfTeamsWithActiveWorms += team.GetWorms().Count > 0 ? 1 : 0);

            return numberOfTeamsWithActiveWorms <= 1;
        }

        protected int GetNextTeamId(int currentId)
        {
            currentId += 1;
            if (currentId == m_Teams.Count)
                currentId = 0;

            while (m_Teams[currentId].GetWorms().Count == 0)
            {
                currentId++;
                if (currentId == m_Teams.Count)
                    currentId = 0;
            }
            
            return currentId;
        }
        
    }
}
