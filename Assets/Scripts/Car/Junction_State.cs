using UnityEngine;

public class Junction_State : Car_Base
{
    public override void enterState(Car_State_Machine car)
    {
        car.currentShowState = Car_State_Machine.States.junction;
    }

    public override void fixedUpdateState(Car_State_Machine car)
    {
        car.Movement();

        float dis = Vector2.Distance(car.currentTarget.position, car.transform.position);

        if (dis < 3) car.switchState(car.movementState);
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
