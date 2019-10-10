using Sitecore;
using Sitecore.Forms.Mvc.Attributes;
using Sitecore.Forms.Mvc.TypeConverters;
using Sitecore.Forms.Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;


namespace Sitecore.Support.Forms.Mvc.ViewModels.Fields
{
    public class DateField : ValuedFieldViewModel<string>
    {
      public object Day
      {
        get;
        set;
      }

      public object Month
      {
        get;
        set;
      }

      public object Year
      {
        get;
        set;
      }

      public string DayTitle
      {
        get;
        set;
      }

      public string MonthTitle
      {
        get;
        set;
      }

      public string YearTitle
      {
        get;
        set;
      }

      [DefaultValue("yyyy-MMMM-dd")]
      public string DateFormat
      {
        get;
        set;
      }

      public List<SelectListItem> Years
      {
        get;
        private set;
      }

      public List<SelectListItem> Months
      {
        get;
        private set;
      }

      public List<SelectListItem> Days
      {
        get;
        private set;
      }

      [TypeConverter(typeof(IsoDateTimeConverter))]
      public DateTime StartDate
      {
        get;
        set;
      }

      [TypeConverter(typeof(IsoDateTimeConverter))]
      public DateTime EndDate
      {
        get;
        set;
      }

      [DateRangeValidation]
      [ParameterName("SelectedDate")]
      public override string Value
      {
        get
        {
          return base.Value;
        }
        set
        {
          base.Value = value;
          OnValueUpdated();
        }
      }

      public override string ResultParameters => DateFormat;

      protected void OnValueUpdated()
      {
        if (!string.IsNullOrEmpty(Value))
        {
          DateTime dateTime = DateUtil.IsoDateToDateTime(Value);
          Day = dateTime.Day;
          Month = dateTime.Month;
          Year = dateTime.Year;
        }
        InitItems();
      }
    
    public override void Initialize()
      {
        if (string.IsNullOrEmpty(DateFormat))
        {
          DateFormat = "yyyy-MMMM-dd";
        }
        if (StartDate == DateTime.MinValue)
        {
          StartDate = DateUtil.IsoDateToDateTime("20000101T120000");
        }
        if (EndDate == DateTime.MinValue)
        {
          EndDate = DateTime.Now.AddYears(1).Date;
        }
        Years = new List<SelectListItem>();
        Months = new List<SelectListItem>();
        Days = new List<SelectListItem>();
        InitItems();
      }

      private void InitItems()
      {
        List<string> list = new List<string>(DateFormat.Split('-'));
        list.Reverse();
        list.ForEach(InitDate);
      }

      private void InitDate(string marker)
      {
        DateTime? current = string.IsNullOrEmpty(Value) ? null : new DateTime?(DateUtil.IsoDateToDateTime(Value));
        switch (marker.ToLower()[0])
        {
          case 'd':
            InitDays(current);
            break;
          case 'm':
            InitMonth(marker, current);
            break;
          case 'y':
            InitYears(marker, current);
            break;
        }
      }

      private void InitYears(string marker, DateTime? current)
      {
        DateTime dateTime = new DateTime(StartDate.Year - 1, 1, 1);
        Years.Clear();
        for (int i = StartDate.Year; i <= EndDate.Year; i++)
        {
          dateTime = dateTime.AddYears(1);
          SelectListItem item = new SelectListItem
          {
            Text = string.Format("{0:" + marker + "}", dateTime),
            Value = i.ToString(CultureInfo.InvariantCulture),
            Selected = (current.HasValue && current.Value.Year == i)
          };
          Years.Add(item);
        }
      }

      private void InitDays(DateTime? current)
      {
        Days.Clear();
        int num = current.HasValue ? DateTime.DaysInMonth(current.Value.Year, current.Value.Month) : 31;
        for (int i = 1; i <= 31; i++)
        {
          if (i <= num)
          {
            Days.Add(new SelectListItem
            {
              Selected = (current.HasValue && current.Value.Day == i),
              Text = i.ToString(CultureInfo.InvariantCulture),
              Value = i.ToString(CultureInfo.InvariantCulture)
            });
          }
        }
      }

      private void InitMonth(string marker, DateTime? current)
      {
        DateTime dateTime = default(DateTime);
        Months.Clear();
        for (int i = 1; i <= 12; i++)
        {
          Months.Add(new SelectListItem
          {
            Selected = (current.HasValue && current.Value.Month == i),
            Text = string.Format("{0:" + marker + "}", dateTime.AddMonths(i - 1)),
            Value = i.ToString(CultureInfo.InvariantCulture)
          });
        }
      }
    }
}