using System;
using osVodigiWeb7x.Models;

namespace osVodigiWeb7.Domain.Business
{
    public class SurveyManager
    {
        ISurveyRepository surveyRepository;
        IAnsweredSurveyRepository answeredSurveyRepository;
        ISurveyQuestionRepository surveyQuestionRepository;
        ISurveyQuestionOptionRepository surveyQuestionOptionRepository;
        IAnsweredSurveyQuestionOptionRepository answeredSurveyQuestionOptionRepository;

        public SurveyManager(ISurveyRepository _surveyRepository,
            IAnsweredSurveyRepository _answeredSurveyRepository,

        ISurveyQuestionRepository _surveyQuestionRepository,
        ISurveyQuestionOptionRepository _surveyQuestionOptionRepository,
        IAnsweredSurveyQuestionOptionRepository _answeredSurveyQuestionOptionRepository)
        {
            surveyRepository = _surveyRepository;
            answeredSurveyRepository = _answeredSurveyRepository;
            surveyQuestionRepository = _surveyQuestionRepository;
            surveyQuestionOptionRepository = _surveyQuestionOptionRepository;
            answeredSurveyQuestionOptionRepository = _answeredSurveyQuestionOptionRepository;
        }



        public ISurveyRepository GetISurveyRepository()
        {
            return surveyRepository;
        }

        public IAnsweredSurveyRepository GetIAnsweredSurveyRepository()
        {
            return answeredSurveyRepository;
        }

        public ISurveyQuestionRepository GetISurveyQuestionRepository()
        {
            return surveyQuestionRepository;
        }

        public ISurveyQuestionOptionRepository GetISurveyQuestionOptionRepository()
        {
            return surveyQuestionOptionRepository;
        }
        public IAnsweredSurveyQuestionOptionRepository GetIAnsweredSurveyQuestionOptionRepository()
        {
            return answeredSurveyQuestionOptionRepository;
        }
    }
}
