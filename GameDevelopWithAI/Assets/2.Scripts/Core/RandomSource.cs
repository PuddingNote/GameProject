using System;

namespace SlimeExperiment.Core
{
    public interface IRandomSource
    {
        int NextInclusive(int minValue, int maxValue);
        float NextPercent();
    }

    public sealed class SystemRandomSource : IRandomSource
    {
        private readonly Random random;

        public SystemRandomSource()
        {
            random = new Random();
        }

        public SystemRandomSource(int seed)
        {
            random = new Random(seed);
        }

        public int NextInclusive(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue + 1);
        }

        public float NextPercent()
        {
            return (float)(random.NextDouble() * 100d);
        }
    }
}
