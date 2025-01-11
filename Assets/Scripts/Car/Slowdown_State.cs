using System.Collections;
using UnityEngine;

public class Slowdown_State : Car_Base
{
    public override void enterState(Car_State_Machine car)
    {
        car.currentShowState = Car_State_Machine.States.slowdown;

        car.AIPath().maxSpeed = 2.7f;

        car.StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);
            car.AIPath().maxSpeed = car.currentSpeed;
            car.switchState(car.movementState);
        }
    }

    public override void fixedUpdateState(Car_State_Machine car)
    {
        if (car.target != null)
        {
            car.AIPath().destination = car.target.position;
        }
    }

    public override void onCollisionEnter(Car_State_Machine car, Collision2D collision)
    {
    }

    public override void onCollisionExit(Car_State_Machine car, Collision2D collisionexit)
    {
    }

    public override void onTriggerEnter(Car_State_Machine car, Collider2D collision)
    {
    }

    public override void onTriggerExit(Car_State_Machine car, Collider2D collisionexit)
    {
    }

    public override void onTriggerStay(Car_State_Machine car, Collider2D collision)
    {
    }

    public override void updateState(Car_State_Machine car)
    {
    }
}
