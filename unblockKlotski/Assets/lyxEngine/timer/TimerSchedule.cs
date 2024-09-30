using UnityEngine;
using System.Collections;


/// <summary>
/// usage:
/*  Timer.Schedule(this, numTile * 0.03f + 0.7f, () =>
        {
            DialogController.instance.ShowDialog(DialogType.Complete);
        });
*/// </summary>
//定时执行
public class TimerSchedule
{
    private static MonoBehaviour behaviour;
    public delegate void Task();


    public static Coroutine Schedule(MonoBehaviour _behaviour, float delay, Task task)
    {
        behaviour = _behaviour;
        return behaviour.StartCoroutine(DoTask(task, delay));
    }

    private static IEnumerator DoTask(Task task, float delay)
    {
        yield return new WaitForSeconds(delay);
        task();
    }
}
