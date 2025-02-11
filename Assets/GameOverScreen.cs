using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using InfimaGames.LowPolyShooterPack;

public class GameOverScreen : MonoBehaviour
{
    public GameObject Player;
    public GameObject Skill;

    public Weapon Gun;

    public Health PlayerHealth;

    [SerializeField] ExpBar expBar;

    [SerializeField] UpgradePanelManager upgradePanelManager;
    [SerializeField] PauseManager pauseManager;
    public Text pointsText;
    public int points = 0;
    public int level = 1;

    [SerializeField] List<UpgradeData> upgrades;
    List<UpgradeData> selectedUpgrades;
    [SerializeField] List<UpgradeData> acquiredUpgrades;

    void Start()
    {
        expBar.UpdateExpSlider(points, ToLevelUp);
        expBar.UpdateLevelText(level);
    }

    public void Setup()
    {
        gameObject.SetActive(true);
        pointsText.text = points.ToString() + "  POINTS";
        pauseManager.PauseGame();
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("City");
    }

    public void ExitButton()
    {
        //SceneManager.LoadScene("MainMenu");
    }
    public void AddPoints(int point)
    {
        points = points + point;
        CheckLevelUp();
        expBar.UpdateExpSlider(points, ToLevelUp);
    }
    int ToLevelUp
    {
        get
        {
            return 1 + level;
        }
    }
    public void CheckLevelUp()
    {
        if(points >= ToLevelUp)
        {
            if (selectedUpgrades == null) { selectedUpgrades = new List<UpgradeData>(); }
            selectedUpgrades.Clear();
            selectedUpgrades.AddRange(GetUpgrades(4));

            upgradePanelManager.OpenPanel(selectedUpgrades);

            points -= ToLevelUp;
            level++;
            expBar.UpdateLevelText(level);
        }
    }

    public List<UpgradeData> GetUpgrades(int count)
    {
        List<UpgradeData> upgradeList = new List<UpgradeData>();
        HashSet<UpgradeData> uniqueNumbers = new HashSet<UpgradeData>();

        if (count > upgrades.Count)
        {
            count = upgrades.Count;
        }

        for(int i = 0; i < count; i++)
        {
            //upgradeList.Add(upgrades[UnityEngine.Random.Range(0, upgrades.Count)]);
            int randomNumber = UnityEngine.Random.Range(0, upgrades.Count);

            if (!uniqueNumbers.Contains(upgrades[randomNumber]))
            {
                uniqueNumbers.Add(upgrades[randomNumber]);
                upgradeList.Add(upgrades[randomNumber]);
            }
            else
            {
                i--;
            }

        }

        return upgradeList;
    }

    public void Upgrade(int selectedUpgradeId)
    {
        UpgradeData upgradeData = selectedUpgrades[selectedUpgradeId];

        if(acquiredUpgrades == null) { acquiredUpgrades = new List<UpgradeData>(); }

        if(upgradeData.upgradeType == UpgradeType.WeaponUnlock)
        {
            //enable script of the skill
            string scriptName = upgradeData.ScriptName;
            Type scriptType = Type.GetType(scriptName);
            MonoBehaviour scriptComponent = Player.GetComponent(scriptType) as MonoBehaviour;
            scriptComponent.enabled = true;
            //enable canva of the skill
            Transform childTransform = Skill.transform.Find(scriptName);
            if (childTransform != null)
            {
                // Enable the child GameObject
                childTransform.gameObject.SetActive(true);
            }
        }
        else if (upgradeData.upgradeType == UpgradeType.WeaponUpgrade)
        {
            Gun.AddDamage(upgradeData.AddDamage);
            Gun.AddFireRate(upgradeData.AddFireDelay);
        }
        else if (upgradeData.upgradeType == UpgradeType.ItemUnlock)
        {
            PlayerHealth.AddMaxHealth(upgradeData.AddMaxHealth);
            PlayerHealth.AddHealth(upgradeData.AddVampHealth);
        }

        acquiredUpgrades.Add(upgradeData);
        upgrades.Remove(upgradeData);
    }
}
