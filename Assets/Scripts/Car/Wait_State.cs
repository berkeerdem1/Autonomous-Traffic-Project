using System.Collections;
using UnityEngine;

public class Wait_State : Car_Base
{
    public override void enterState(Car_State_Machine car)
    {
        car.currentShowState = Car_State_Machine.States.wait;

        car.StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(3f);

            car.GetAIPath().destination = car.currentTarget.position;
            car.GetAIPath().canSearch = true; // Yol aramayý etkinleþtir
            car.switchState(car.movementState);
        }
    }

    public override void fixedUpdateState(Car_State_Machine car)
    {
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
