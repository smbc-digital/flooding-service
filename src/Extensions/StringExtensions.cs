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
                case "parkOrFootpath":
                    return "In a park or public footpath";
                case "privateLand":
                    return "On private land";
                case "home":
                    return "In a home";
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