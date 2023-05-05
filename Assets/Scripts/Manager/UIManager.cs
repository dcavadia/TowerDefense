using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonComponent <UIManager>
{
    public TurretSelectionPanel turretSelectionPanel;
    public PlayerInfoPanel playerInfoPanel;
    public GameOverPanel gameOverPanel;
    public GameWinPanel winPanel;

    public void WinPopUp()
    {
        winPanel.gameObject.SetActive(true);
    }

    public void LosePopUp()
    {
        gameOverPanel.gameObject.SetActive(true);
    }
}
