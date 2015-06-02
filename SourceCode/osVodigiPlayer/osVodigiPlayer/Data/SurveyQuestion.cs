
namespace osVodigiPlayer
{
    public class SurveyQuestion
    {
        public int SurveyQuestionID { get; set; }
        public int SurveyID { get; set; }
        public string SurveyQuestionText { get; set; }
        public bool AllowMultiselect { get; set; }
        public int SortOrder { get; set; }
    }
}
