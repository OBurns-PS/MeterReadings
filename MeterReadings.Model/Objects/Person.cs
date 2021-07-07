using Crudinski.Data.Attributes;
using Crudinski.Data.RelationshipManager;
using Crudinski.Entity.Logic.Items;

namespace MeterReadings.Model.Objects
{
    public class Person : DataItem
    {
        [PrimaryKey]
        [Identity]
        public int PersonID { get; set; }

        [ForeignKey]
        public int AccountID { get; set; }

        [MappedField]
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        [MappedField]
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        [NavigationProperty]
        public Account Account { get; set; }

        private string _firstName, _lastName;

    }
}
