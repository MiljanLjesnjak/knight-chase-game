using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour
{
    float scan_timer = 0;
    float scan_target_time = 0.2f;

    float capture_time = 1f;

    bool pause_scan = false;


    private void Update()
    {
        //Scans every scan_target_time seconds
        scan_timer += Time.deltaTime;
        if (scan_timer >= scan_target_time && pause_scan == false)
        {
            ScanArea();
            scan_timer = 0;
        }
    }

    Vector3 king_pos, knight_pos, direction;
    IEnumerator TryCapture()
    {
        yield return new WaitForSeconds(capture_time);

        //Checks if knight is still attackble after capture_time seconds passed
        Collider[] col = Physics.OverlapBox(transform.position + Vector3.up, new Vector3(1, 0.5f, 1), Quaternion.identity);
        foreach (Collider c in col)
        {
            if (c.gameObject.CompareTag("Player"))
            {

                king_pos = transform.position;
                king_pos = new Vector3(Mathf.RoundToInt(king_pos.x), king_pos.y, Mathf.RoundToInt(king_pos.z));

                knight_pos = GameObject.Find("Player (Knight)").transform.position;
                knight_pos = new Vector3(Mathf.RoundToInt(knight_pos.x), knight_pos.y, Mathf.RoundToInt(knight_pos.z));

                direction = knight_pos - king_pos;
                direction.y = 0;

                //King moves to capture
                for (int i = 0; i < 20; i++)
                {
                    transform.Translate(direction / 20);


                    yield return null;
                }

                yield return new WaitForSeconds(1f);

                //King returns to initial position
                for (int i = 0; i < 20; i++)
                {
                    transform.Translate(-direction / 20);

                    yield return null;
                }
            }
        }

        pause_scan = false;
    }



    //Scans area for player knight
    void ScanArea()
    {
        scan_timer = 0;

        Collider[] col = Physics.OverlapBox(transform.position + Vector3.up, new Vector3(1, 0.5f, 1), Quaternion.identity);

        foreach (Collider c in col)
        {

            if (c.gameObject.CompareTag("Player"))
            {
                pause_scan = true;

                StartCoroutine(TryCapture());
                
                return;
            }

        }
    }



}
