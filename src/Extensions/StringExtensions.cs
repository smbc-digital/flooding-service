namespace flooding_service.Mappers
{
    public static class StringExtensions
    {
        public static string WhereIsTheFloodToReadableText(this string value)
        {
            switch (value)
            {
                case "pavement":
                    return "On a pavement";
                case "road":
                    return "On a road";
                default:
                    return value;
            }
        }

    }
}