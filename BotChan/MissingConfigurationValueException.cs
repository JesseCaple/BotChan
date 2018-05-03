using System;
using System.Runtime.Serialization;

namespace BotChan
{
    [Serializable]
    internal class MissingConfigurationValueException : Exception
    {
        public MissingConfigurationValueException(string propertyName)
            : base($"Missing config property value for {propertyName}.")
        {
        }
    }
}