using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour {

    #region Private members

    private bool _isCausingDamage = false;

    #endregion

    #region Public Members

    public float DamageRepeatRate = 0.1f;

    public int DamageAmount = 1;

    public bool Repeating = true;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        _isCausingDamage = true;

        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        ///
        /// this is for testplayer
        ///
        Testplayer testplayer = other.gameObject.GetComponent<Testplayer>();
        ///
        /// till to there
        ///

        if (testplayer != null)
        {
           
            if (Repeating)
            {
                // Repeating damage
                StartCoroutine(TakeDamage(testplayer, DamageRepeatRate));
            }
            else
            {
                // Just one time damage
                testplayer.TakeDamage(DamageAmount);
            }
        }


        if (player != null)
        {
            if (Repeating)
            {
                // Repeating damage
                StartCoroutine(TakeDamage(player, DamageRepeatRate));
            }
            else
            {
                // Just one time damage
                player.TakeDamage(DamageAmount);
            }
        }
    }

    ///
    /// this is for testplayer
    ///
    IEnumerator TakeDamage(Testplayer player, float repeatRate)
    {
        while (_isCausingDamage)
        {
            player.TakeDamage(DamageAmount);
            TakeDamage(player, repeatRate);

            if (player.IsDead)
            {
                _isCausingDamage = false;
            }

            yield return new WaitForSeconds(repeatRate);
        }
    }
    ///
    /// till to there
    ///

    IEnumerator TakeDamage(PlayerController player, float repeatRate)
    {
        while (_isCausingDamage)
        {
            player.TakeDamage(DamageAmount);
            TakeDamage(player, repeatRate);

            if (player.IsDead)
                _isCausingDamage = false;

            yield return new WaitForSeconds(repeatRate);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _isCausingDamage = false;
        }
    }
}
