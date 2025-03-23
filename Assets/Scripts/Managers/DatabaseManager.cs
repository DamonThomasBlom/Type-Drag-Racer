using UnityEngine;
using BestHTTP;
using System;
using Newtonsoft.Json;
using System.Text;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
using Sirenix.OdinInspector;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using Fusion;

public class DatabaseManager : MonoBehaviour
{
    #region SINGLETON

    public static DatabaseManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private const string BASE_URL = "http://127.0.0.1:8090/api/collections/";

    [Button]
    public void RegisterUser(string username, string email, string password, Action<SimpleServerResponse> callback)
    {
        HTTPRequest request = new HTTPRequest(new Uri($"{BASE_URL}users/records"), HTTPMethods.Post, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull registration
                if (res.IsSuccess)
                {
                    callbackResponse.IsSuccess = true;
                    callback.Invoke(callbackResponse);

                    UserData userData = JsonConvert.DeserializeObject<UserData>(res.DataAsText);

                    //Send verification email here
                    VerifyEmail(userData.Email, (callback) => { });
                }
                else // Unsuccessful registration
                {
                    RegisterErrorResponse errorResponse = JsonConvert.DeserializeObject<RegisterErrorResponse>(res.DataAsText);

                    callbackResponse.IsSuccess = false;
                    switch (errorResponse.Data.Keys.First())
                    {
                        case "username":
                            callbackResponse.Message = "Username has already been taken.";
                            break;
                        default:
                            callbackResponse.Message = errorResponse.Data.Keys.First() + " - " + errorResponse.Data[errorResponse.Data.Keys.First()].Message;
                            break;
                    }

                    callback.Invoke(callbackResponse);
                }
            }
            else
            {
                callbackResponse.IsSuccess = false;
                callbackResponse.Message = "Register error, check your internet connection!";
                callback.Invoke(callbackResponse);
            }

        });

        object jsonBody = new
        {
            password = password,
            passwordConfirm = password,
            email = email,
            emailVisibility = true,
            username = username
        };

        request.SetHeader("Content-Type", "application/json");
        request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonBody));

        request.Send();
    }

    [Button]
    public void VerifyEmail(string email, Action<SimpleServerResponse> callback)
    {
        HTTPRequest request = new HTTPRequest(new Uri($"{BASE_URL}users/request-verification"), HTTPMethods.Post, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull registration
                if (res.IsSuccess)
                {
                    callbackResponse.IsSuccess = true;
                    callback.Invoke(callbackResponse);

                    ToastManager.Instance.ShowToast("Email verification sent.");
                    Debug.Log("Email verifcation sent to - " + email);
                }
                else // Unsuccessful verifcation
                {
                    RegisterErrorResponse errorResponse = JsonConvert.DeserializeObject<RegisterErrorResponse>(res.DataAsText);

                    callbackResponse.IsSuccess = false;
                    callbackResponse.Message = errorResponse.Data.Keys.First() + " - " + errorResponse.Data[errorResponse.Data.Keys.First()].Message;

                    callback.Invoke(callbackResponse);
                    Debug.LogError("Email verification error - " + callbackResponse.Message);
                }
            }
            else
            {
                callbackResponse.IsSuccess = false;
                callbackResponse.Message = "Register error, check your internet connection!";
                callback.Invoke(callbackResponse);
            }
        });

        object jsonBody = new
        {
            email = email
        };

        request.SetHeader("Content-Type", "application/json");
        request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonBody));

        request.Send();
    }

    [Button]
    public void Login(string email, string password, Action<SimpleServerResponse> callback)
    {
        HTTPRequest request = new HTTPRequest(new Uri($"{BASE_URL}users/auth-with-password"), HTTPMethods.Post, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull login
                if (res.IsSuccess)
                {
                    LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(res.DataAsText);

                    // Assign all user data here
                    Player.Instance.PlayerData = loginResponse.UserData;
                    Player.Instance.Token = loginResponse.Token;

                    callbackResponse.IsSuccess = true;
                    callback.Invoke(callbackResponse);
                }
                else // Unsuccessful login
                {
                    callbackResponse.IsSuccess = false;
                    callbackResponse.Message = "Failed to authenticate.";

                    callback.Invoke(callbackResponse);
                }
            }
            else
            {
                callbackResponse.IsSuccess = false;
                callbackResponse.Message = "Server sent an error!";
                callback.Invoke(callbackResponse);
            }

        });

        object jsonBody = new
        {
            password = password,
            identity = email,
        };

        request.SetHeader("Content-Type", "application/json");
        request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonBody));

        request.Send();
    }

    [Button]
    public void PostLeaderboardStat(object jsonBody)
    {
        HTTPRequest request = new HTTPRequest(new Uri($"{BASE_URL}leaderboard/records"), HTTPMethods.Post, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull login
                if (res.IsSuccess)
                    Debug.Log("Posted leaderboard stat successfully!");
                else // Unsuccessful login
                    Debug.LogError("Error posting leaderboard stat!");
            }
            else
                Debug.LogError("Error posting leaderboard stat!");
        });

        request.SetHeader("Content-Type", "application/json");
        request.SetHeader("Authorization", $"Bearer {Player.Instance.Token}");
        request.RawData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonBody));

        request.Send();
    }

    //string Top10Leaderboard = $"{BASE_URL}leaderboard/records?sort=-wpm&perPage=10&page=1";
    //string Top10ByGameMode = $"{BASE_URL}leaderboard/records?sort=-wpm&perPage=10&page=1&filter=(race_distance='{gamemode}')";
    //string PlayerRankGlobal = $"{BASE_URL}leaderboard/records?filter=(wpm>'{userWPM}')&perPage=1";  // Use totalItems for rank
    //string PlayerRankByGameMode = $"{BASE_URL}leaderboard/records?filter=(wpm>'{userWPM}' %26%26 race_distance='{gameMode}')&perPage=1"; // Not working yet
    //string PlayerBestWPM = $"{BASE_URL}leaderboard/records?filter=(username='{Player.Instance.PlayerName}')&sort=-wpm&perPage=1";
    //string PlayerBestWPMForRaceDistance = $"{BASE_URL}leaderboard/records?filter=username='{Player.Instance.PlayerName}' %26%26 race_distance='{raceDistance}'&sort=-wpm&perPage=1"; // Not working yet

    public void GetTop10LeaderBoard(Action<LeaderBoardResponse> callback)
    {
        string urlTop10Leaderboard = $"{BASE_URL}leaderboard/records?sort=-wpm&perPage=10&page=1";

        HTTPRequest request = new HTTPRequest(new Uri(urlTop10Leaderboard), HTTPMethods.Get, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull login
                if (res.IsSuccess)
                {
                    Debug.Log("Got top 10 leaderboard stat successfully!");
                    callback.Invoke(JsonConvert.DeserializeObject<LeaderBoardResponse>(res.DataAsText));
                }
                else
                {
                    Debug.LogError("Error getting top 10 leaderboard!");
                    callback.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Error getting top 10 leaderboard!");
                callback.Invoke(null);
            }
        });

        request.SetHeader("Authorization", $"Bearer {Player.Instance.Token}");
        request.Send();
    }

    public void GetTop10LeaderBoardByRace(RaceDistance raceDistance, Action<LeaderBoardResponse> callback)
    {
        string urlTop10RaceDistanceLeaderboard = $"{BASE_URL}leaderboard/records?sort=-wpm&perPage=10&page=1&filter=(race_distance='{raceDistance.GetDescription()}')";

        HTTPRequest request = new HTTPRequest(new Uri(urlTop10RaceDistanceLeaderboard), HTTPMethods.Get, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull login
                if (res.IsSuccess)
                {
                    Debug.Log($"Got top 10 leaderboard by race distance {raceDistance.GetDescription()} stat successfully!");
                    callback.Invoke(JsonConvert.DeserializeObject<LeaderBoardResponse>(res.DataAsText));
                }
                else
                {
                    Debug.LogError("Error getting top 10 leaderboard!");
                    callback.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Error getting top 10 leaderboard!");
                callback.Invoke(null);
            }
        });

        request.SetHeader("Authorization", $"Bearer {Player.Instance.Token}");
        request.Send();
    }

    public void GetPlayerGlobalRank(int playerWpm, Action<LeaderBoardResponse> callback)
    {
        string urlPlayerGlobalRank = $"{BASE_URL}leaderboard/records?filter=(wpm>='{playerWpm}')&perPage=1";

        HTTPRequest request = new HTTPRequest(new Uri(urlPlayerGlobalRank), HTTPMethods.Get, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull login
                if (res.IsSuccess)
                {
                    Debug.Log($"Got players global rank successfully!");
                    callback.Invoke(JsonConvert.DeserializeObject<LeaderBoardResponse>(res.DataAsText));
                }
                else
                {
                    Debug.LogError("Error getting player global rank!");
                    callback.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Error getting player global rank!");
                callback.Invoke(null);
            }
        });

        request.SetHeader("Authorization", $"Bearer {Player.Instance.Token}");
        request.Send();
    }

    public void GetPlayerRankForRace(int playerWpm, RaceDistance raceDistance, Action<LeaderBoardResponse> callback)
    {
        string urlPlayerRankRace = $"{BASE_URL}leaderboard/records?filter=(wpm>='{playerWpm}' %26%26 race_distance='{raceDistance.GetDescription()}')&perPage=1";

        HTTPRequest request = new HTTPRequest(new Uri(urlPlayerRankRace), HTTPMethods.Get, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull login
                if (res.IsSuccess)
                {
                    Debug.Log($"Got players global rank successfully!");
                    callback.Invoke(JsonConvert.DeserializeObject<LeaderBoardResponse>(res.DataAsText));
                }
                else
                {
                    Debug.LogError("Error getting player global rank!");
                    callback.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Error getting player global rank!");
                callback.Invoke(null);
            }
        });

        request.SetHeader("Authorization", $"Bearer {Player.Instance.Token}");
        request.Send();
    }

    public void GetPlayerBestWPM(Action<LeaderBoardResponse> callback)
    {
        string playerBestWpmUrl = $"{BASE_URL}leaderboard/records?filter=(username='{Player.Instance.PlayerName}')&sort=-wpm&perPage=1";

        HTTPRequest request = new HTTPRequest(new Uri(playerBestWpmUrl), HTTPMethods.Get, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull login
                if (res.IsSuccess)
                {
                    Debug.Log("Got players best WPM!");
                    callback.Invoke(JsonConvert.DeserializeObject<LeaderBoardResponse>(res.DataAsText));
                }
                else
                {
                    Debug.LogError("Error getting players best WPM!");
                    callback.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Error getting players best WPM!");
                callback.Invoke(null);
            }
        });

        request.SetHeader("Authorization", $"Bearer {Player.Instance.Token}");
        request.Send();
    }

    public void GetPlayerBestWPMForRace(RaceDistance raceDistance, Action<LeaderBoardResponse> callback)
    {
        string playerBestWpmUrlForRace = $"{BASE_URL}leaderboard/records?filter=username='{Player.Instance.PlayerName}' %26%26 race_distance='{raceDistance.GetDescription()}'&sort=-wpm&perPage=1";

        HTTPRequest request = new HTTPRequest(new Uri(playerBestWpmUrlForRace), HTTPMethods.Get, (req, res) =>
        {
            // Handle server response here
            SimpleServerResponse callbackResponse = new();
            if (req.State == HTTPRequestStates.Finished)
            {
                // Successfull login
                if (res.IsSuccess)
                {
                    Debug.Log("Got players best WPM for race!");
                    callback.Invoke(JsonConvert.DeserializeObject<LeaderBoardResponse>(res.DataAsText));
                }
                else
                {
                    Debug.LogError("Error getting players best WPM for race!");
                    callback.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Error getting players best WPM for race!");
                callback.Invoke(null);
            }
        });

        request.SetHeader("Authorization", $"Bearer {Player.Instance.Token}");
        request.Send();
    }
}

public class SimpleServerResponse
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; }
}

public partial class LoginResponse
{
    [JsonProperty("record")]
    public UserData UserData { get; set; }

    [JsonProperty("token")]
    public string Token { get; set; }
}

[Serializable]
public class UserData
{
    [JsonProperty("collectionId")]
    public string CollectionId { get; set; }

    [JsonProperty("collectionName")]
    public string CollectionName { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("emailVisibility")]
    public bool EmailVisibility { get; set; }

    [JsonProperty("verified")]
    public bool Verified { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("created")]
    public DateTimeOffset Created { get; set; }

    [JsonProperty("updated")]
    public DateTimeOffset Updated { get; set; }
}

public partial class RegisterErrorResponse
{
    public Dictionary<string, ValidationError> Data { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("status")]
    public long Status { get; set; }
}

public partial class ValidationError
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}

public partial class LeaderBoardResponse
{
    [JsonProperty("items")]
    public List<LeaderboardDatabaseItem> Items { get; set; }

    [JsonProperty("page")]
    public long Page { get; set; }

    [JsonProperty("perPage")]
    public long PerPage { get; set; }

    [JsonProperty("totalItems")]
    public long TotalItems { get; set; }

    [JsonProperty("totalPages")]
    public long TotalPages { get; set; }
}

public partial class LeaderboardDatabaseItem
{
    [JsonProperty("accuracy")]
    public float Accuracy { get; set; }

    [JsonProperty("collectionId")]
    public string CollectionId { get; set; }

    [JsonProperty("collectionName")]
    public string CollectionName { get; set; }

    [JsonProperty("created")]
    public DateTimeOffset Created { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("race_distance")]
    public string RaceDistance { get; set; }

    [JsonProperty("time")]
    public float Time { get; set; }

    [JsonProperty("updated")]
    public DateTimeOffset Updated { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("wpm")]
    public long Wpm { get; set; }
}