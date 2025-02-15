using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using UnityEngine.UI;

public class CloudSaveManager : MonoBehaviour
{
    // UI elements to display saved data
    public Text playerDataText;

    // Example player data to save
    private int playerScore = 1000;

    private void Start()
    {
        // Initialize Unity Services
        InitializeUnityServices();
    }

    // Initialize Unity services (Cloud Save and Authentication)
    async void InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();  // Ensure sign-in is complete
            Debug.Log("Unity Services initialized.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error initializing Unity Services: " + e.Message);
        }
    }

    // Save player data to Unity Cloud Save
    public async void SaveDataToCloud()
    {
        try
        {
            // Create a dictionary to store player data
            var playerData = new Dictionary<string, object>
            {
                { "playerScore", playerScore }
            };

            // Save the data to Unity Cloud Save using SaveAsync
            await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);  // Updated method
            Debug.Log("Player data saved to the cloud.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving data to the cloud: " + e.Message);
        }
    }

    // Load player data from Unity Cloud Save
    public async void LoadDataFromCloud()
    {
        try
        {
            // Convert List to HashSet
            var keys = new HashSet<string> { "playerScore" };

            // Retrieve player data from the cloud
            var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (playerData.ContainsKey("playerScore"))
            {
                // Convert the stored value to int manually
                string scoreString = playerData["playerScore"].ToString();
                if (int.TryParse(scoreString, out int loadedScore))
                {
                    playerScore = loadedScore;
                    playerDataText.text = $"Player Score: {playerScore}";
                    Debug.Log("Player data loaded from the cloud.");
                }
                else
                {
                    Debug.LogWarning("Failed to parse player score from cloud data.");
                }
            }
            else
            {
                Debug.LogWarning("Player data not found in the cloud.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading data from the cloud: " + e.Message);
        }
    }
}
