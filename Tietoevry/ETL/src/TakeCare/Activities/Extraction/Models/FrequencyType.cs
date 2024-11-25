namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Models
{
    public class FrequencyType
    {
        public int Id { get; set; }

        /*
         The id of the frequency type.
            0 - the content property contains a FrequencySingle object
            1 - the content property contains a FrequencyDaily object
            2 - the content property contains a FrequencyScheduleDay object
            3 - the content property contains a FrequencyScheduleWeek object
            4 - the content property contains a FrequencyContinuous object
            5 - the content property contains a FrequencyIrregular object
         */


        public string Name { get; set; }
    }
}
