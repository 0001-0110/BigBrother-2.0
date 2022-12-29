namespace Services
{
    // TODO this could be largely improved
    internal class IntService
    {
        public static int Min(params int[] values)
        {
            int min = values[0];

            foreach (int value in values)
                if (value < min)
                    min = value;

            return min;
        }

        public static int Max(params int[] values)
        {
            int min = values[0];

            foreach (int value in values)
                if (value < min)
                    min = value;

            return min;
        }
    }
}
