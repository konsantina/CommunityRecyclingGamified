namespace CommunityRecyclingGamified.Enums
{
    public enum RedemptionStatus
    {
        Pending = 0,     // μόλις γίνει το POST
        Approved = 1,    // εγκρίθηκε
        Rejected = 2,    // απορρίφθηκε
        Fulfilled = 3,   // παραδόθηκε
        Cancelled = 4    // ακυρώθηκε από user
    }
}
