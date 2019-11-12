using System;

namespace Account
{
    public class CloudAPI
    {
        public static string SignInUrl = "https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev/accounts/sign-in";
        public static string SignUpUrl = "https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev/accounts/sign-up";
        public static string SaveUrl = "https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev/saves";
        public static string ProfileUrl = "https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev/profiles";
    }

    [System.Serializable]
    public class CloudUserData
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    public class CloudResult
    {
        public TokenResult message;
        public bool code;
    }


    [System.Serializable]
    public class CloudSaveResult
    {
        public string[] message;
        public bool code;
    }


    [System.Serializable]
    public class TokenResult
    {
        public string username;
        public string access_token;
        public string refresh_token;
    }
}