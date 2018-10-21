using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LoadoutManager : MonoBehaviour {

    public static LoadoutManager Instance;

    private Loadout currentLoadout;

    public Text LoadoutNameText;
    public Image PrimaryWeaponImage;
    public Image SecondaryWeaponImage;

    public List<LoadoutWeapon> AvailableWeapons = new List<LoadoutWeapon>();


    private void Awake()
    {
        Instance = this;
    }

    public Loadout GetLoadout(int loadoutNumber, Transform gunManagerTransform)
    {
        Loadout loadout = LoadLoadout(loadoutNumber);
        foreach (Transform gun in gunManagerTransform)
        {
            if (loadout.PrimaryWeaponName == gun.name || loadout.SecondaryWeaponName == gun.name)
            {
                gun.GetComponent<Gun>().enabled = true;
            }
            else
            {
                gun.GetComponent<Gun>().enabled = false;
            }
        }
        return loadout;
    } 

    public void SelectLoadout(int loadoutNumber)
    {
        Loadout data = LoadLoadout(loadoutNumber);
        if (data != null)
        {
            currentLoadout = data;
            LoadoutNameText.text = data.LoadoutName;
            PrimaryWeaponImage.sprite = FindLoadoutWeaponByName(data.PrimaryWeaponName).WeaponSprite;
            SecondaryWeaponImage.sprite = FindLoadoutWeaponByName(data.SecondaryWeaponName).WeaponSprite;
        }
        else
        {
            Loadout data2 = SaveLoadout(loadoutNumber, FindLoadoutWeaponByName("AWP_DragonLore").WeaponName, FindLoadoutWeaponByName("Glock 18").WeaponName);
            LoadoutNameText.text = data2.LoadoutName;
            PrimaryWeaponImage.sprite = FindLoadoutWeaponByName(data2.PrimaryWeaponName).WeaponSprite;
            SecondaryWeaponImage.sprite = FindLoadoutWeaponByName(data2.SecondaryWeaponName).WeaponSprite;
            currentLoadout = data2;
        }
    }

    public void OnClick_ChangeWeapon(string _weaponName)
    {
        if (currentLoadout == null)
        {
            return;
        }

        LoadoutWeapon weapon = FindLoadoutWeaponByName(_weaponName);

        if (weapon != null)
        {
            if (weapon.Primary)
            {
                currentLoadout.PrimaryWeaponName = weapon.WeaponName;
                PrimaryWeaponImage.sprite = weapon.WeaponSprite;
            }
            else
            {
                currentLoadout.SecondaryWeaponName = weapon.WeaponName;
                SecondaryWeaponImage.sprite = weapon.WeaponSprite;
            }

            SaveLoadout(currentLoadout.LoadoutNumber, currentLoadout.PrimaryWeaponName, currentLoadout.SecondaryWeaponName);
            LoadoutNameText.text = currentLoadout.LoadoutName;
            PrimaryWeaponImage.sprite = FindLoadoutWeaponByName(currentLoadout.PrimaryWeaponName).WeaponSprite;
            SecondaryWeaponImage.sprite = FindLoadoutWeaponByName(currentLoadout.SecondaryWeaponName).WeaponSprite;
        }
    }

    private Loadout SaveLoadout(int number, string primaryWeaponName, string secondaryWeaponName)
    {
        string loadoutName = "Loadout" + number.ToString();
        string path = Application.persistentDataPath + "/" + loadoutName + ".loadout";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);

        Loadout data = new Loadout(loadoutName, number)
        {
            PrimaryWeaponName = primaryWeaponName,
            SecondaryWeaponName = secondaryWeaponName
        };

        bf.Serialize(file, data);
        file.Close();
        return data;
    }

    public Loadout LoadLoadout(int number)
    {
        string loadoutName = "Loadout" + number.ToString();
        string path = Application.persistentDataPath + "/" + loadoutName + ".loadout";

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            Loadout data = (Loadout)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            return null;
        }
    }

    private LoadoutWeapon FindLoadoutWeaponByName(string weaponName)
    {
        int index = AvailableWeapons.FindIndex(x => x.WeaponName == weaponName);

        if (index == -1)
        {
            return null;
        }
        else
        {
            return AvailableWeapons[index];
        }
    }

    public void OnClick_DeleteLoadouts()
    {
        for (int i = 1; i < 5; i++)
        {
            string path = Application.persistentDataPath + "/Loadout" + i.ToString() + ".loadout";

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
[System.Serializable]
public class LoadoutWeapon
{
    public string WeaponName;
    public Sprite WeaponSprite;
    public bool Primary;
}

[System.Serializable]
public class Loadout
{
    public string LoadoutName;
    public int LoadoutNumber;
    public string PrimaryWeaponName;
    public string SecondaryWeaponName;

    public Loadout(string loadoutName, int loadoutNumber)
    {
        LoadoutName = loadoutName;
        LoadoutNumber = loadoutNumber;
    }
}