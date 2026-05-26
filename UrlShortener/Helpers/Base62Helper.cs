namespace UrlShortener.Helpers;


public class Base62Helper
{
    private const string Base62Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(long id)
    {

        if (id == 0) return "000000";

        string result = "";
        while(id > 0) {
            int remainder = (int)(id % 62);
            result = Base62Chars[remainder] + result;
            id /= 62;
        }

        while(result.Length < 6) {
            result = "0" + result;
        }

        return result; 
    }
}