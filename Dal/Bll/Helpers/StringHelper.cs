namespace Threesixty.Dal.Bll.Helpers
{
    internal class StringHelper
    {
        public static string ExtractFileExtensionFromBase64(string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return null;

            var comaParts = base64.Split(',');
            if (comaParts == null || comaParts.Length == 0)
                return null;

            var slashParts = comaParts[0].Split('/');
            if (slashParts == null || slashParts.Length < 2)
                return null;

            var p = slashParts[1].Split(';');
            if (p == null || p.Length < 2)
                return null;

            return p[0];
        }
    }
}
