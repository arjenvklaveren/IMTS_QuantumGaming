using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace Game.Test
{
    public class DeltaMoveTest : MonoBehaviour
    {
        [SerializeField] float moveMult = 5.0f;
        bool canMove = false;

        private void Start()
        {
            StartCoroutine(MoveLimiter());
        }

        void Update()
        {
            if(canMove) Move();
            if (UnityEngine.Input.GetKeyDown(KeyCode.K))
            {
                ResetMove();
            }
        }

        private void ResetMove()
        {
            transform.position = Vector3.zero;
            StartCoroutine(MoveLimiter());
        }

        void Move()
        {
            Vector2 moveDir = new Vector2(1, 0);
            Vector2 moveSpeed = new Vector2(1 * moveMult, 1 * moveMult);
            Vector2 translation = moveDir * moveSpeed * Time.deltaTime;
            transform.Translate(translation);
            //Debug.Log(transform.position.ToString("F4"));
        }

        IEnumerator MoveLimiter()
        {
            canMove = true;
            yield return new WaitForSeconds(1);
            canMove = false;
            Debug.Log("STOP: " + transform.position.ToString("F4"));
        }
    }
}
