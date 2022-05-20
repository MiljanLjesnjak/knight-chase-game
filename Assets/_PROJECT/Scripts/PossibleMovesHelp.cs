using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleMovesHelp : MonoBehaviour
{
    [SerializeField]
    GameObject[] Spheres = default;
    
    bool show_moves_isOn;
    
    private void Awake()
    {
        if (MenuUI.show_moves == 0)
            show_moves_isOn = false;
        else
            show_moves_isOn = true;
    }

    public void ShowPossibleMoves()
    {

        foreach (GameObject sphere in Spheres)
        {
            Collider[] cols;
            cols = Physics.OverlapSphere(sphere.transform.position, 0.25f);

            foreach (Collider col in cols)
            {
                if (!col.CompareTag("Field"))
                {
                    sphere.transform.GetComponent<MeshRenderer>().enabled = false;
                }

                else
                {
                    sphere.transform.GetComponent<MeshRenderer>().enabled = true;
                    break;
                }
            }
        }

    }

    public void HideAllPossibleMoves()
    {
        if (!show_moves_isOn)
            return;

        foreach (GameObject spheres in Spheres)
            spheres.transform.GetComponent<MeshRenderer>().enabled = false;
    }


}
