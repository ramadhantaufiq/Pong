using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    //Script, Collider, dan rigidbody bola
    public BallControl ball;
    CircleCollider2D ballCollider;
    Rigidbody2D ballRigidbody;

    //Bola "bayangan" yang akan ditampilkan di titik tumbukan
    public GameObject ballAtCollision;

    // Start is called before the first frame update
    void Start()
    {
        ballRigidbody = ball.GetComponent<Rigidbody2D>();
        ballCollider = ball.GetComponent<CircleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        //Inisiasi status pantulan lintasan , yang hanya akan ditampilkan 
        //Jika lintasan bertumbukan dengan objek tertentu
        bool drawBallAtCollision = false;

        //Titik tumbukan yang digeser 
        Vector2 offsetHitPoint = new Vector2();

        //Tentukkan titik tumbukan dengan deteksi pergerakan lingkaran
        RaycastHit2D[] circleCastHit2DArray = Physics2D.CircleCastAll(ballRigidbody.position, ballCollider.radius, ballRigidbody.velocity.normalized);

        //Untuk setiap titik tumbukan 
        foreach (RaycastHit2D circleCastHit2D in circleCastHit2DArray)
        {
            //Jika terjadi tumbukan, dan tumbukan tersebut tidak dengan bola
            //Karena garis lintasan digambar dari titik tengah bola
            if (circleCastHit2D.collider != null && circleCastHit2D.collider.GetComponent<BallControl>() == null)
            {
               //Garis lintasan akan digambar dari titik tengah bola saat ini 
               //ke titik tengah bola pada saat tumbukan
               //Yaitu sebuah titi yang di-offset dari titik tumbukan berdasar vektor normal titik
               //Tersebut sebesar jari jari bola

               //tentukan titik tumbukan
               Vector2 hitPoint = circleCastHit2D.point;

               //tentukan normal di titik tumbukan 
               Vector2 hitNormal = circleCastHit2D.normal;

               //Tentukan offSetHitPoint, titik tengah
               offsetHitPoint = hitPoint + hitNormal * ballCollider.radius;

               //Gambar garis lintasan 
               DottedLine.DottedLine.Instance.DrawDottedLine(ball.transform.position, offsetHitPoint);

               //kalau bukan sidewall
               if (circleCastHit2D.collider.GetComponent<SideWall>() == null)
               {
                   //HItung vektor datang 
                   Vector2 inVector = (offsetHitPoint - ball.TrajectoryOrigin).normalized;

                   //Hitung vektor keluar
                   Vector2 outVector = Vector2.Reflect(inVector, hitNormal);

                   //hitung dot 
                   float outDot = Vector2.Dot(outVector, hitNormal);
                   if (outDot > -1.0f && outDot < 1.0)
                   {
                        // Gambar lintasan pantulannya
                        DottedLine.DottedLine.Instance.DrawDottedLine(offsetHitPoint, offsetHitPoint + outVector * 10.0f);

                        //Untuk menggambar bola "Bayangan"
                        drawBallAtCollision = true;
                   }
               }
               // Hanya gambar lintasan untuk satu titik tumbukan, jadi keluar dari loop
               break;
            }
        }

        //Jika true
        if (drawBallAtCollision)
        {
            //gambar bola bayangan 
            ballAtCollision.transform.position = offsetHitPoint;
            ballAtCollision.SetActive(true);
        } else 
        {
            //Sembunyikan bola bayangan 
            ballAtCollision.SetActive(false);

        }
    }
}
