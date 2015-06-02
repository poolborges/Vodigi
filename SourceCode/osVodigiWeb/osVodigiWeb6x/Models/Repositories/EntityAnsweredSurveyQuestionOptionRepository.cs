/* ----------------------------------------------------------------------------------------
    Vodigi - Open Source Interactive Digital Signage
    Copyright (C) 2005-2013  JMC Publications, LLC

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
---------------------------------------------------------------------------------------- */

using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace osVodigiWeb6x.Models
{
    public class EntityAnsweredSurveyQuestionOptionRepository : IAnsweredSurveyQuestionOptionRepository
    {
        private VodigiContext db = new VodigiContext();

        public void CreateAnsweredSurveyQuestionOption(AnsweredSurveyQuestionOption option)
        {
            // Prevent duplicate option entries in this answered survey
            var query = from answeredsurveyquestionoption in db.AnsweredSurveyQuestionOptions
                        select answeredsurveyquestionoption;
            query = query.Where(asvos => asvos.AnsweredSurveyID.Equals(option.AnsweredSurveyID));
            query = query.Where(asvos => asvos.AnsweredSurveyQuestionOptionID.Equals(option.AnsweredSurveyQuestionOptionID));
            List<AnsweredSurveyQuestionOption> answeredsurveyquestionoptions = query.ToList();
            if (answeredsurveyquestionoptions == null || answeredsurveyquestionoptions.Count == 0)
            {
                db.AnsweredSurveyQuestionOptions.Add(option);
                db.SaveChanges();
            }
        }

        public void UpdateAnsweredSurveyQuestionOption(AnsweredSurveyQuestionOption answeredsurveyquestionoption)
        {
            db.Entry(answeredsurveyquestionoption).State = EntityState.Modified;
            db.SaveChanges();
        }

        public AnsweredSurveyQuestionOption GetAnsweredSurveyQuestionOption(int id)
        {
            AnsweredSurveyQuestionOption option = db.AnsweredSurveyQuestionOptions.Find(id);

            return option;
        }

        public IEnumerable<AnsweredSurveyQuestionOption> GetBySurveyQuestionOptionId(int id)
        {
            var query = from answeredsurveyquestionoption in db.AnsweredSurveyQuestionOptions
                        select answeredsurveyquestionoption;
            query = query.Where(asvos => asvos.SurveyQuestionOptionID.Equals(id));

            List<AnsweredSurveyQuestionOption> answeredsurveyquestionoptions = query.ToList();

            return answeredsurveyquestionoptions;
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}