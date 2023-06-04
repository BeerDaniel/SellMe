
namespace SuspiciousGames.SellMe.Core.Adventures
{
    [System.Serializable]
    public struct AdventureLengthStepWrapper
    {
        public AdventureLength length;
        public int maxSteps;

        public AdventureLengthStepWrapper(AdventureLength l , int steps)
        {
            length = l;
            maxSteps = steps;
        }
    }
}

