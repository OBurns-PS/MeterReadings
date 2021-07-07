using Crudinski.Data.Attributes;
using Crudinski.Data.RelationshipManager;
using Crudinski.Entity.Logic.Items;
using System.Collections.Generic;

namespace MeterReadings.Model.Objects
{
    public class Account : DataItem
    {
        [PrimaryKey]
        [Identity]
        public int AccountID { get; set; }

        [MappedField]
        public string AccountNumber
        {
            get
            {
                return _accountNumber;
            }
            set
            {
                _accountNumber = value;
                OnPropertyChanged();
            }
        }

        [NavigationProperty]
        public List<Person> Person { get; set; }

        [NavigationProperty]
        public List<MeterReading> MeterReading { get; set; }

        public Account()
        {
            Person = new List<Person>();
            MeterReading = new List<MeterReading>();
        }

        private string _accountNumber;
    }
}
