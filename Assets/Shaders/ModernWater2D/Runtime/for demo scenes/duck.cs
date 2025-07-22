using System.Collections;
using UnityEngine;

namespace Water2D
{

    public class duck : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        [SerializeField] float pathLength = 5;
        [SerializeField] bool flip = false;

        bool right = true;

        Vector2 NextPoint()
        {
            Vector2 s = transform.position;
            s += new Vector2(((flip ? right : !right) ? 1 : -1) * pathLength, 0);
            return s;
        }

        void Start()
        {
            StartCoroutine(goToP());
        }


        IEnumerator goToP()
        {
            Vector2 pp = NextPoint();
            flip = !flip;
            float xs = transform.position.x;
            float xe = pp.x;
            float xc = transform.position.x;
            float t = 0;
            float time = 10 / speed;

            while (t < time)
            {
                t += Time.deltaTime;
                float per = t / time;
                xc = Mathf.SmoothStep(xs, xe, per);
                transform.position = new Vector2(xc, transform.position.y);
                yield return new WaitForEndOfFrame();
            }
            transform.position = pp;
            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            }
            StartCoroutine(goToP());
        }

    }

}