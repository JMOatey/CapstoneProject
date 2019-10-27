using System;
using System.Net.Http;
using System.Web;

/*
 *  
 */
public class ResponseObject
{
    public string mResponseMessage = string.Empty;
    public bool mSuccess;

    public ResponseObject(string ResponseMessage, bool Success)
    {
        this.mResponseMessage = ResponseMessage;
        this.mResponseCode = Success;
    }
};


public class ResourceAPI
{
    // URL is hard coded at the moment but this will change
    private string mBaseURL = new string("https://92g8gmwng0.execute-api.us-east-1.amazonaws.com/dev");
    private string mResourceName = string.Empty;
    private HttpClient mClient;

    public BaseAPI(string ResourceName, HttpClient Client)
    {
        this.mResourceName = ResourceName;
        this.mClient = Client;
    }

    /*
     *  Create an object in the database
     */
    public ResponseObject Create(string AccessToken, string data)
    {
        try
        {

        }
        catch(HttpRequestException e)
        {
            ResponseObject response = new ResponseObject("", false);
            return response;
        }
    }

    /*
     *  Read an object in the database
     */
    public ResponseObject Read(string AccessToken, int ResourceID = -1)
    {
        try
        {
            UriBuilder uri = new UriBuilder(this.mBaseURL);
            if(ResourceID >= 0)
            {
                NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);
                query["id"] = ResourceID; 
                query["token"] = AccessToken;

                uri.Query = query.ToString();
            }

            HttpResponseMessage responseMessage = await this.mClient.GetAsync(uri.ToString());

            responseMessage.EnsureSuccessStatusCode();

            string responseBody = await responseMessage.Content.ReadStringAsync();
            int responseCode = responseMessage.StatusCode;

            ResponseObject response = new ResponseObject(responseBody, true);
            return response;
        }
        catch(HttpRequestException e)
        {
            ResponseObject response = new ResponseObject("", false);
            return response;
        }
    } 

    /*
     *  Update an object in the database
     */
    public ResponseObject Update(string AccessToken, int ResourceID, string data)
    {
        try
        {

        }
        catch(HttpRequestException e)
        {
            ResponseObject response = new ResponseObject("", false);
            return response;
        }
    }

    /*
     *  Delete an object in the database
     */
    public ResponseObject Delete(string AccessToken, int ResourceID)
    {
        try
        {

        }
        catch(HttpRequestException e)
        {
            ResponseObject response = new ResponseObject("", false);
            return response;
        }
    }
};


public class AccountAPI
{
    private HttpClient mClient;

    public AccountAPI(HttpClient Client)
    {
        this.mClient = Client;
    }

    /*
     *  Creates a user account in Cognito with the given properties
     *
     *  Good response:
     *      message:    <access_token>
     *      code:       1
     */
    public ResponseObject SignUp(string UserName, string Password)
    {

    }

    /*
     *  Delete an object in the database
     */
    public ResponseObject SignIn(string UserName, string Password)
    {

    }
};


public class CloudAPI
{
    public BaseAPI Profile;
    public BaseAPI Save;
    public AccountAPI Account;

    public CloudAPI()
    {
        HttpClient Client = new HttpClient();

        this.Profile = new ResourceAPI("profiles", Client);
        this.Save = new ResourceAPI("saves", Client);
        this.Account = new AccountAPI(Client);
    }
};