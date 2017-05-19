using Sitecore.Data.Items;
using Sitecore.Form.Core.Configuration;
using Sitecore.Forms.Core.Attributes;
using Sitecore.Forms.Mvc.Data.TypeProviders;
using Sitecore.Forms.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Sitecore.Support.Forms.Mvc.Models.Fields
{
    public class DateField : FieldModel
    {
        public DateField(Item item) : base(item)
        {
            this.Initialize();
        }

        private void InitDate(string marker)
        {
            DateTime current = DateUtil.IsoDateToDateTime(this.Value.ToString());
            char ch = marker.ToLower()[0];
            switch (ch)
            {
                case 'd':
                    this.InitDays(current);
                    return;

                case 'm':
                    this.InitMonth(marker, current);
                    return;
            }
            if (ch == 'y')
            {
                this.InitYears(marker, current);
            }
        }

        private void InitDays(DateTime current)
        {
            this.Days.Clear();
            for (int i = 1; i <= DateTime.DaysInMonth(current.Year, current.Month); i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Selected = current.Day == i,
                    Text = i.ToString(CultureInfo.InvariantCulture),
                    Value = i.ToString(CultureInfo.InvariantCulture)
                };
                this.Days.Add(item);
            }
        }

        private void Initialize()
        {
            this.DateFormat = "yyyy-MMMM-dd";
            if (this.StartDate == DateTime.MinValue)
            {
                this.StartDate = DateUtil.IsoDateToDateTime("20000101T120000");
            }
            if (this.EndDate == DateTime.MinValue)
            {
                this.EndDate = DateTime.Now.AddYears(1).Date;
            }
            this.DayTitle = ResourceManager.Localize("DAY");
            this.MonthTitle = ResourceManager.Localize("MONTH");
            this.YearTitle = ResourceManager.Localize("YEAR");
            this.Years = new List<SelectListItem>();
            this.Months = new List<SelectListItem>();
            this.Days = new List<SelectListItem>();
            KeyValuePair<string, string> pair = base.ParametersDictionary.FirstOrDefault<KeyValuePair<string, string>>(x => x.Key.ToLower() == "selecteddate");
            this.Value = ((pair.Key != null) && !string.IsNullOrEmpty(pair.Value)) ? pair.Value : DateUtil.ToIsoDate(DateTime.Now);
            List<string> list = new List<string>(this.DateFormat.Split(new char[] { '-' }));
            SitecorSupport = DateUtil.IsoDateToDateTime(Value.ToString());
            list.Reverse();
            list.ForEach(new Action<string>(this.InitDate));
        }

        private void InitMonth(string marker, DateTime current)
        {
            DateTime time = new DateTime();
            this.Months.Clear();
            for (int i = 1; i <= 12; i++)
            {
                SelectListItem item = new SelectListItem
                {
                    Selected = current.Month == i,
                    Text = string.Format("{0:" + marker + "}", time.AddMonths(i - 1)),
                    Value = i.ToString(CultureInfo.InvariantCulture)
                };
                this.Months.Add(item);
            }
        }

        private void InitYears(string marker, DateTime current)
        {
            DateTime time = new DateTime(this.StartDate.Year - 1, 1, 1);
            this.Years.Clear();
            for (int i = this.StartDate.Year; i <= this.EndDate.Year; i++)
            {
                time = time.AddYears(1);
                SelectListItem item = new SelectListItem
                {
                    Text = string.Format("{0:" + marker + "}", time),
                    Value = i.ToString(CultureInfo.InvariantCulture),
                    Selected = current.Year == i
                };
                this.Years.Add(item);
            }
        }

        protected override void OnValueUpdated()
        {
            DateTime time = DateUtil.IsoDateToDateTime(this.Value.ToString());
            //this.Day = time.Day;
            //this.Month = time.Month;
            //this.Year = time.Year;
        }

        [DefaultValue("yyyy-MMMM-dd")]
        public string DateFormat { get; set; }

        public object Day { get; set; }

        public List<SelectListItem> Days { get; private set; }

        public string DayTitle { get; set; }

        [Converter(typeof(IsoDateTimeConverter))]
        public DateTime EndDate { get; set; }

        public object Month { get; set; }

        public List<SelectListItem> Months { get; private set; }

        public string MonthTitle { get; set; }

        public override string ResultParameters =>
            this.DateFormat;

        [Converter(typeof(IsoDateTimeConverter))]
        public DateTime StartDate { get; set; }

        [DateRangeValidation(), DataType(DataType.Date)]
        public override object Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        public object Year { get; set; }

        public List<SelectListItem> Years { get; private set; }

        public string YearTitle { get; set; }
        public DateTime SitecorSupport { get; set; }
    }
}