using Crudinski.Data.RelationshipManager;
using Crudinski.Entity.Logic.Context;
using MeterReadings.Model.Objects;

namespace MeterReadings.Model
{
    public class MeterReadingsDataHouse : DataHouse
    {
        protected override void DefineRelationships(RelationshipDefinitionManager relationshipModelBuilder)
        {
            relationshipModelBuilder.OneToMany<Account, Person>(
                x => x.Account, x => x.Person);
            relationshipModelBuilder.OneToMany<Account, MeterReading>(
                x => x.Account, x => x.MeterReading);
        }
    }
}
