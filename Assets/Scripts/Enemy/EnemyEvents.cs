using System;
using UnityEngine;

public static class EnemyEvents
{
    public static Action<GameObject> OnEnemyDamaged;
    public static Action<GameObject> OnEnemyDied;
    public static Action<GameObject> OnEnemyStomped;
}
