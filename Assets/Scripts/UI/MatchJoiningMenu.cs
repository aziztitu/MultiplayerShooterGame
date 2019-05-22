using System;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchJoiningMenu : Bolt.GlobalEventListener
{
    public string sceneToGoBack = "Multiplayer Menu";

    public GameObject matchList;
    
    public GameObject matchListItemPrefab;

    private void Awake()
    {
        if (!GameManager.Instance.isLoggedIn)
        {
            SceneManager.LoadScene(AuthMenu.authMenuSceneName);
            return;
        }
     
        HelperUtilities.UpdateCursorLock(false);
        if (BoltNetwork.IsRunning)
        {
            BoltLauncher.Shutdown();
        }
        
        FindSessions();
    }

    public void FindSessions()
    {
        BoltLauncher.StartClient();
    }

    public override void BoltStartDone()
    {
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        for (int i = 0; i < matchList.transform.childCount; i++)
        {
            Destroy(matchList.transform.GetChild(i).gameObject);
        }
        
        foreach (KeyValuePair<Guid, UdpSession> pair in sessionList)
        {
            UdpSession session = pair.Value;

            if (session.Source == UdpSessionSource.Photon)
            {
                GameObject matchListItem = Instantiate(matchListItemPrefab, matchList.transform);
                MatchListItemUI matchListItemUi = matchListItem.GetComponent<MatchListItemUI>();
                matchListItemUi.SetSessionData(session);
            }
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene(sceneToGoBack);
    }
}