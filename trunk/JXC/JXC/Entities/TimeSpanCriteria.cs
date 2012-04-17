﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120416
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120416
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OEA.Library;
using OEA.MetaModel.Attributes;
using OEA.ManagedProperty;

namespace JXC
{
    [Serializable]
    public abstract class TimeSpanCriteria : Criteria
    {
        public TimeSpanCriteria()
        {
            this.TimeSpanType = TimeSpanType.LastMonth;
        }

        public static readonly Property<TimeSpanType> TimeSpanTypeProperty = P<TimeSpanCriteria>.Register(e => e.TimeSpanType, new PropertyMetadata<TimeSpanType>
        {
            PropertyChangedCallBack = (o, e) => (o as TimeSpanCriteria).OnTimeSpanTypeChanged(e)
        });
        public TimeSpanType TimeSpanType
        {
            get { return this.GetProperty(TimeSpanTypeProperty); }
            set { this.SetProperty(TimeSpanTypeProperty, value); }
        }
        protected virtual void OnTimeSpanTypeChanged(ManagedPropertyChangedEventArgs<TimeSpanType> e)
        {
            var today = DateTime.Today;
            switch (e.NewValue)
            {
                case TimeSpanType.Today:
                    this.From = this.To = today;
                    break;
                case TimeSpanType.Week:
                    var dayOfWeek = (int)today.DayOfWeek;
                    if (dayOfWeek == 0) dayOfWeek = 7;
                    dayOfWeek--;//0-6

                    var monday = today.AddDays(-dayOfWeek);
                    this.From = monday;
                    this.To = monday.AddDays(6);
                    break;
                case TimeSpanType.Month:
                    this.From = new DateTime(today.Year, today.Month, 1);
                    this.To = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                    break;
                case TimeSpanType.LastMonth:
                    this.From = today.AddDays(-30);
                    this.To = today;
                    break;
                case TimeSpanType.Year:
                    this.From = new DateTime(today.Year, 1, 1);
                    this.To = new DateTime(today.Year, 12, DateTime.DaysInMonth(today.Year, 12));
                    break;
                case TimeSpanType.All:
                    this.From = new DateTime(1800, 1, 1);
                    this.To = new DateTime(9999, 12, 31);
                    break;
                case TimeSpanType.Custom:
                    break;
                default:
                    break;
            }
        }

        public static readonly Property<DateTime> FromProperty = P<TimeSpanCriteria>.Register(e => e.From);
        public DateTime From
        {
            get { return this.GetProperty(FromProperty); }
            set { this.SetProperty(FromProperty, value); }
        }

        public static readonly Property<DateTime> ToProperty = P<TimeSpanCriteria>.Register(e => e.To);
        public DateTime To
        {
            get { return this.GetProperty(ToProperty); }
            set { this.SetProperty(ToProperty, value); }
        }
    }

    public enum TimeSpanType
    {
        [Label("自定义")]
        Custom,
        [Label("当天")]
        Today,
        [Label("本周")]
        Week,
        [Label("本月")]
        Month,
        [Label("最近一月")]
        LastMonth,
        [Label("本年")]
        Year,
        [Label("全部")]
        All
    }
}