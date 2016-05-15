namespace Onism.Cldr.Tools.Subsetting
{
    /// <summary>
    /// Represents a decision of how to handle a JSON token when
    /// subsetting is performed (to remove or not to remove).
    /// </summary>
    public class Decision
    {
        private readonly int time;
        public bool Remove { get; }

        public Decision(int time, bool remove)
        {
            this.time = time;
            this.Remove = remove;
        }

        /// <summary>
        /// Gets the decision that has occured later in time.
        /// </summary>
        public static Decision GetNewer(Decision a, Decision b)
        {
            if (a == null)
                return b;

            if (b == null)
                return a;

            return a.time > b.time ? a : b;
        }
    }
}