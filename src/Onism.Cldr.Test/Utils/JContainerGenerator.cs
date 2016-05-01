using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Test.Utils
{
    /// <summary>
    /// Represents a generator of <see cref="JContainer"/> tokens.
    /// Uses <see cref="DeterministicRandom"/> to generate randomized
    /// yet deterministic output.
    /// </summary>
    /// <remarks>
    /// <see cref="PatternCollectionGenerator"/> relies on a feature that
    /// the keys of properties are only non-negative numbers, chosen
    /// sequentially from 0 and not exceeding 9.
    /// </remarks>
    public class JContainerGenerator
    {
        private readonly DeterministicRandom random;

        /// <summary>
        /// Initializes a new instance of <see cref="JContainerGenerator"/>,
        /// using <see cref="DeterministicRandom"/> to get deterministic output.
        /// </summary>
        public JContainerGenerator(DeterministicRandom random)
        {
            this.random = random;
        }

        /// <summary>
        /// Generates a perfect k-ary tree.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/K-ary_tree.
        /// </remarks>
        public JContainer GeneratePerfectTree(int arity, int depth)
        {
            return GeneratePerfectObject(arity, depth);
        }

        private JContainer GeneratePerfectObject(int arity, int depth)
        {
            var root = new JObject();

            if (depth == 1)
            {
                for (var i = 0; i < arity; ++i)
                    root.Add(i.ToString(), i.ToString());

                return root;
            }

            for (var i = 0; i < arity; ++i)
                root.Add(i.ToString(), random.NextBool()
                    ? GeneratePerfectObject(arity, depth - 1)
                    : GeneratePerfectArray(arity, depth - 1));

            return root;
        }

        private JContainer GeneratePerfectArray(int arity, int depth)
        {
            var root = new JArray();

            if (depth == 1)
            {
                for (var i = 0; i < arity; ++i)
                    root.Add(i.ToString());

                return root;
            }

            for (var i = 0; i < arity; ++i)
                root.Add(random.NextBool()
                    ? GeneratePerfectObject(arity, depth - 1)
                    : GeneratePerfectArray(arity, depth - 1));

            return root;
        }
    }
}
