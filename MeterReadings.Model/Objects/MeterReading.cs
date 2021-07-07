using Crudinski.Data.Attributes;
using Crudinski.Data.RelationshipManager;
using Crudinski.Entity.Logic.Items;
using System;

namespace MeterReadings.Model.Objects
{
    public class MeterReading : DataItem
    {
        [PrimaryKey]
        [Identity]
        public int MeterReadingID { get; set; }

        [ForeignKey]
        public int AccountID { get; set; }

        [MappedField]
        public DateTime MeterReadingDateTime
        {
            get
            {
                return _meterReadingDateTime;
            }
            set
            {
                _meterReadingDateTime = value;
                OnPropertyChanged();
            }
        }

        [MappedField]
        public int MeterReadValue
        {
            get
            {
                return _meterReadValue;
            }
            set
            {
                _meterReadValue = value;
                OnPropertyChanged();
            }
        }

        [NavigationProperty]
        public Account Account { get; set; }

        private DateTime _meterReadingDateTime;
        private int _meterReadValue;
    }
}
