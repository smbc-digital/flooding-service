namespace flooding_service.Extensions
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

        public static string WhereIsTheFloodingComingFromToReadableText(this string value)
        {
            switch (value)
            {
                case "riverOrStream":
                    return "River or stream";
                case "culvert":
                    return "Culverted watercourse";
                default:
                    return value;
            }
        }
    }
}