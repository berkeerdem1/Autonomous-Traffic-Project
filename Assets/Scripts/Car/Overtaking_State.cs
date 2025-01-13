using Pathfinding;
using System.Collections;
using UnityEngine;

public class Overtaking_State : Car_Base
{
    public override void enterState(Car_State_Machine car)
    {
        car.currentShowState = Car_State_Machine.States.overtaking;
        car.currentSpeed = 6;
        car.StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(6f);
            car.switchState(car.movementState);
        }
    }

    public override void fixedUpdateState(Car_State_Machine car)
    {
        car.StartOvertaking();

        // Check overtaking status
        if (car.isOvertaking && car.GetAIPath().reachedDestination)
        {
            car.EndOvertaking(); // If overtaking is complete, return to current target
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
