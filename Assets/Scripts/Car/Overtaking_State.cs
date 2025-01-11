using Pathfinding;
using System.Collections;
using UnityEngine;

public class Overtaking_State : Car_Base
{
    public override void enterState(Car_State_Machine car)
    {
        car.currentShowState = Car_State_Machine.States.overtaking;

        car.StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);
            car.AIPath().maxSpeed = car.currentSpeed;
            car.AIPath().destination = car.target.position;
            //car.switchState(car.movementState);
        }
    }

    public override void fixedUpdateState(Car_State_Machine car)
    {
        car.StartOvertaking();

        // Sollama durumunu kontrol et
        if (car.isOvertaking && car.AIPath().reachedDestination)
        {
            car.EndOvertaking(); // Sollama tamamlandýysa, orijinal hedefe dön
            car.switchState(car.movementState);
            return;
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
