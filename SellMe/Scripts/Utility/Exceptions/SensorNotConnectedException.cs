

namespace SuspiciousGames.SellMe.Utility.Exceptions
{
    [System.Serializable]
    public class SensorNotConnectedException : System.Exception
    {
        public SensorNotConnectedException() { }
        public SensorNotConnectedException(string message) : base(message) { }
        public SensorNotConnectedException(string message, System.Exception inner) : base(message, inner) { }
        protected SensorNotConnectedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}


