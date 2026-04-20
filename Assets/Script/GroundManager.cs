using UnityEngine;

public class GroundManager : MonoBehaviour{

    public int largeur = 100; 
    public int hauteur = 150; 
    public float tailleCellule = 1.0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 

        for (int i = 0; i <= hauteur; i+=10)
        {
            Vector3 debut = transform.position + new Vector3(0-largeur/2, 0, i * tailleCellule-hauteur/2);
            Vector3 fin = transform.position + new Vector3(largeur * tailleCellule/2, 0, i * tailleCellule-hauteur/2);
            Gizmos.DrawLine(debut, fin);
        }

        for (int j = 0; j <= largeur; j+=10)
        {
            Vector3 debut = transform.position + new Vector3(j * tailleCellule-largeur/2, 0, 0-hauteur/2);
            Vector3 fin = transform.position + new Vector3(j * tailleCellule-largeur/2, 0, hauteur * tailleCellule-hauteur/2);
            Gizmos.DrawLine(debut, fin);
        }
    }
}