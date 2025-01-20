using System.Collections;
using UnityEngine;

public class Stop_State : Car_Base
{
    float timer;
    public override void enterState(Car_State_Machine car)
    {
        car.currentShowState = Car_State_Machine.States.stop;
        car.currentSpeed = 0f;
        timer = 0f;
        car.StartCoroutine(Wait());

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(3f);
            //car.switchState(car.overTakingState);
            car.switchState(car.movementState);
        }
    }

    public override void fixedUpdateState(Car_State_Machine car)
    {
        if (!car.IsCarInFront())
            car.switchState(car.movementState);

        timer += Time.deltaTime;

        if(timer > 10f)
        {
            car.StartCoroutine(car.CarDetectedCoroutine(car.GetCar()));
        }
        if (timer > 12f)
        {
            car.switchState(car.movementState);
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
