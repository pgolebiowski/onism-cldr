using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Onism.Cldr.Test.Utils
{
    /// <summary>
    /// Once initialized with a bit array, this object returns values
    /// in a deterministic way. Facilitates recreating scenarios where some randomness is needed.
    /// </summary>
    public class DeterministicRandom
    {
        private readonly BitArray bitArray;
        private int index = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeterministicRandom"/> class,
        /// using a random bit array with the specified length.
        /// </summary>
        public DeterministicRandom(int bitArrayLength)
        {
            if (bitArrayLength <= 0)
                throw new ArgumentException("Length must be greater than 0.");

            this.bitArray = new BitArray(bitArrayLength);
            var random = new Random();

            for (var i = 0; i < bitArrayLength; ++i)
                this.bitArray[i] = random.NextDouble() >= 0.5;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeterministicRandom"/> class,
        /// using a string of bits (1 and 0) as an input.
        /// </summary>
        public DeterministicRandom(string bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            if (bits.Any(b => (b != '0') && (b != '1')))
                throw new ArgumentException("Only '0' and '1' are allowed.");

            this.bitArray = new BitArray(bits.Length);
            for (var i = 0; i < bits.Length; ++i)
                this.bitArray[i] = bits[i] == '1';
        }

        /// <summary>
        /// Reads next bit and returns a boolean value.
        /// </summary>
        public bool NextBool()
        {
            var bit = this.bitArray[index++];

            if (index >= bitArray.Length)
                index = 0;

            return bit;
        }

        /// <summary>
        /// Reads next 32 bits and returns a non-negative integer.
        /// </summary>
        public int NextInt()
        {
            var bits = Enumerable.Range(1, 32).Select(i => NextBool()).ToArray();
            var result = new int[1];
            new BitArray(bits).CopyTo(result, 0);

            if (result[0] < 0)
                result[0] = -result[0];

            return result[0];
        }

        /// <summary>
        /// Gets the serialized bit array. 
        /// </summary>
        public override string ToString()
        {
            var builder = new StringBuilder(this.bitArray.Length);

            foreach (bool bit in this.bitArray)
                builder.Append(bit ? 1 : 0);

            return builder.ToString();
        }
    }
}