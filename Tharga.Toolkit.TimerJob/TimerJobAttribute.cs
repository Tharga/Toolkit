using System;

namespace Tharga.Toolkit.TimerJob;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TimerJobAttribute : Attribute
{
    //https://crontab.guru/
    //* * * * *         Every minute.
    //0 * * * *         Top of every hour.
    //0,1,2 * * * *     Every hour at minutes 0, 1, and 2.
    //*/2 * * * *       Every two minutes.
    //1-55 * * * *      Every minute through the 55th minute.
    //* 1,10,20 * * *   Every 1st, 10th, and 20th hours.
    //*/1 * * * *       At every minute
    //0 */1 * * *       Every hour at minute 0
    //0 3 * * *"        At 03:00
    public string CronExpression { get; set; }
    public bool RunOnStart { get; set; } = true;
}