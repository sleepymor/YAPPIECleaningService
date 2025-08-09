using UnityEngine;

public class MissionListeners : MonoBehaviour
{
    public string[] missionsToUnlock;  // misi yang harus selesai dulu
    public string missionToLock;  // misi yang harus selesai dulu

    void Start()
    {
        // Nonaktifkan semua child di awal
        SetChildrenActive(false);
    }

    void Update()
    {
        if (MissionManager.instance.IsMissionCompleted(missionToLock))
        {
            return;
        }

        if (AreAllMissionsCompleted())
        {
            // Nyalakan semua child jika misi sudah selesai semua
            SetChildrenActive(true);
        }
        else
        {
            // Nonaktifkan semua child jika misi belum selesai
            SetChildrenActive(false);
        }
    }

    bool AreAllMissionsCompleted()
    {
        foreach (string missionName in missionsToUnlock)
        {
            if (!MissionManager.instance.IsMissionCompleted(missionName))
                return false;
        }
        return true;
    }

    // Fungsi untuk mengaktifkan/nonaktifkan semua child
    void SetChildrenActive(bool isActive)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }
}
