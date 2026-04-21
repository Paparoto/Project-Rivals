using UnityEngine;

public class CustomerSkinManager : MonoBehaviour
{
    public GameObject[] skinPrefabs;
    private Animator skinAnimator;

    public void ApplyRandomSkin()
    {
        if (skinPrefabs == null || skinPrefabs.Length == 0)
        {
            Debug.LogWarning("Aucun skin assigné dans CustomerSkinManager !");
            return;
        }

        int index = Random.Range(0, skinPrefabs.Length);
        GameObject skinPrefab = skinPrefabs[index];

        GameObject skinInstance = Instantiate(skinPrefab, transform);
        skinInstance.transform.localPosition = Vector3.zero;
        skinInstance.transform.localRotation = Quaternion.identity;
        skinInstance.transform.localScale    = Vector3.one;

        // Récupérer l'Animator sur le skin
        skinAnimator = skinInstance.GetComponent<Animator>();
        if (skinAnimator == null)
            skinAnimator = skinInstance.GetComponentInChildren<Animator>();

        var capsuleRenderer = GetComponent<MeshRenderer>();
        if (capsuleRenderer != null)
            capsuleRenderer.enabled = false;
    }

        public void SetRunning(bool running)
        {
            if (skinAnimator != null)
                skinAnimator.SetBool("isWalking", running);
                skinAnimator.SetBool("isWalking2", running);
        }
}