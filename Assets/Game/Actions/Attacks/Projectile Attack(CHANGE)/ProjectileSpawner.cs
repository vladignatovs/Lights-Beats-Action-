using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Spawner : MonoBehaviour
{
    public GameObject projectile;
    public float spawnRate = 5;
    public float timer = 0f;

    public float miniTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if(timer < spawnRate) {
            timer += Time.deltaTime;
        } else {
            timer = 0f;
            StartCoroutine(chooseTimeAttack());
        }

    }

    // Spawns a projectile at a given x value
    void spawnProjectile(float i) {
        Instantiate(projectile, new Vector3(i, transform.position.y, 0), transform.rotation);
    }
    // Spawns two projectiles: one at a given x value, the other at a given negative x value
    void spawnTwoParallelProjectiles(float i) {
        Instantiate(projectile, new Vector3(i, transform.position.y, 0), transform.rotation);
        Instantiate(projectile, new Vector3(-i, transform.position.y, 0), transform.rotation);
    }

    IEnumerator chooseTimeAttack() {
        int randomAttack = Random.Range(0, 4);
        switch(randomAttack) {
            case 0:
                return timeAttack1();
            case 1:
                return timeAttack2();
            case 2:
                return timeAttack3();
            case 3:
                return timeAttack4();
            default:
                return timeAttack1();
        }
    }

    IEnumerator timeAttack1() {
        for(float i = 16.75f; i >= 3f; i--) {
            spawnTwoParallelProjectiles(i);
            yield return new WaitForSeconds(0.33f);
        }
    }

    IEnumerator timeAttack2() {
        for(float i = 0f; i < 13.75f; i++) {
            if(i == 0f) {
                spawnProjectile(i);
            } else {
                spawnTwoParallelProjectiles(i);
            }
            yield return new WaitForSeconds(0.33f);
        }
    }

    IEnumerator timeAttack3() {
        for(float i = 16.75f; i > -13f; i--) {
            spawnProjectile(i);
            yield return new WaitForSeconds(0.12f);
        }
    }

    IEnumerator timeAttack4() {
        for(float i = -16.75f; i < 13f; i++) {
            spawnProjectile(i);
            yield return new WaitForSeconds(0.12f);
        }
    }

}
