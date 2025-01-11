using UnityEngine;

public abstract class Car_Base
{
    public abstract void enterState(Car_State_Machine car);
    public abstract void fixedUpdateState(Car_State_Machine car);
    public abstract void updateState(Car_State_Machine car);
    public abstract void onCollisionEnter(Car_State_Machine car, Collision2D collision);
    public abstract void onCollisionExit(Car_State_Machine car, Collision2D collisionexit);
    public abstract void onTriggerEnter(Car_State_Machine car, Collider2D collision);
    public abstract void onTriggerStay(Car_State_Machine car, Collider2D collision);
    public abstract void onTriggerExit(Car_State_Machine car, Collider2D collisionexit);
    
}
