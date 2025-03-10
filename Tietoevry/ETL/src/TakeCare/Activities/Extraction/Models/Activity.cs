namespace TakeCare.Migration.OpenEhr.Activities.Extraction.Models
{
    public class Activity
    {
        public string Id { get; set; }
        public Guid Guid { get; set; }
        public int Status { get; set; }
        /*
         *   0 - Normal
            -2 - Cancelleds
            -3 - Not performed*/
        public Created Created { get; set; }
        public LastSaved LastSaved { get; set; }
        public string? ExplanationAndInstruction { get; set; }
        public Term Term { get; set; }
        public Profession Profession { get; set; }
        public LinkedDocument? LinkedDocument { get; set; }
        public Signed? Signed { get; set; }
        public UserData? SignerUser { get; set; }
        public Locked? Locked { get; set; }
        public Priority? Priority { get; set; }
        public string? CompletedSetBySystemDateTime { get; set; }
        public int? CompletedSetByUserDate { get; set; }
        public int? CompletedSetByUserTime { get; set; }

        public string? CompletedSetByUserDateTime { get; set; } // combined date and time

        public Frequency Frequency { get; set; } // contains combined date and time in freq content
        public string? BasedOnActivityId { get; set; }
        public List<PlannedDateTime>? PlannedAt { get; set; } // contains combined date time
    }
}
