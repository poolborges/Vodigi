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
    public class EntitySurveyQuestionOptionRepository : ISurveyQuestionOptionRepository
    {
        private VodigiContext db = new VodigiContext();

        public IEnumerable<SurveyQuestionOption> GetSurveyQuestionOptions(int surveyquestionid)
        {
            var query = from surveyquestionoption in db.SurveyQuestionOptions
                        select surveyquestionoption;
            query = query.Where(sqs => sqs.SurveyQuestionID.Equals(surveyquestionid));
            query = query.OrderBy("SortOrder", false);

            List<SurveyQuestionOption> surveyquestionoptions = query.ToList();

            return surveyquestionoptions;
        }

        public SurveyQuestionOption GetSurveyQuestionOption(int id)
        {
            SurveyQuestionOption surveyquestionoption = db.SurveyQuestionOptions.Find(id);
            return surveyquestionoption;
        }

        public void DeleteSurveyQuestionOption(SurveyQuestionOption option)
        {
            var query = from surveyquestionoption in db.SurveyQuestionOptions
                        select surveyquestionoption;

            query = query.Where(sqos => sqos.SurveyQuestionID.Equals(option.SurveyQuestionID));
            query = query.OrderBy("SortOrder", false);

            List<SurveyQuestionOption> surveyquestionoptions = query.ToList();

            bool found = false;
            foreach (SurveyQuestionOption sqo in surveyquestionoptions)
            {
                if (found)
                {
                    sqo.SortOrder -= 1;
                    db.Entry(sqo).State = EntityState.Modified;
                }
                if (sqo.SurveyQuestionOptionID == option.SurveyQuestionOptionID)
                {
                    found = true;
                    db.SurveyQuestionOptions.Remove(sqo);
                }
            }

            db.SaveChanges();
        }

        public void CreateSurveyQuestionOption(SurveyQuestionOption option)
        {
            // Get the maximum sort order
            var query = from surveyquestionoption in db.SurveyQuestionOptions
                        select surveyquestionoption;
            query = query.Where(sqos => sqos.SurveyQuestionID.Equals(option.SurveyQuestionID));
            query = query.OrderBy("SortOrder", true);

            List<SurveyQuestionOption> surveyquestionoptions = query.ToList();

            int maxsortorder = 0;
            if (surveyquestionoptions.Count > 0)
                maxsortorder = surveyquestionoptions[0].SortOrder;

            option.SortOrder = maxsortorder + 1;
            db.SurveyQuestionOptions.Add(option);
            db.SaveChanges();
        }

        public void MoveSurveyQuestionOption(SurveyQuestionOption option, bool ismoveup)
        {
            var query = from surveyquestionoption in db.SurveyQuestionOptions
                        select surveyquestionoption;
            query = query.Where(sqs => sqs.SurveyQuestionID.Equals(option.SurveyQuestionID));
            query = query.OrderBy("SortOrder", false);

            List<SurveyQuestionOption> surveyquestionoptions = query.ToList();

            // Get the current and max sort orders
            int currentsortorder = option.SortOrder;
            int maxsortorder = 1;
            foreach (SurveyQuestionOption sqo in surveyquestionoptions)
            {
                if (sqo.SortOrder > maxsortorder)
                    maxsortorder = sqo.SortOrder;
            }

            // Adjust the appropriate sort orders
            foreach (SurveyQuestionOption sqo in surveyquestionoptions)
            {
                if (ismoveup)
                {
                    if (sqo.SurveyQuestionOptionID == option.SurveyQuestionOptionID) // move current question up
                    {
                        if (currentsortorder > 1)
                            option.SortOrder -= 1;
                    }
                    else // find the previous item and increment it
                    {
                        if (sqo.SortOrder == currentsortorder - 1)
                        {
                            sqo.SortOrder += 1;
                            db.Entry(sqo).State = EntityState.Modified;
                        }
                    }
                }
                else
                {
                    if (sqo.SurveyQuestionOptionID == option.SurveyQuestionOptionID) // move current question down
                    {
                        if (currentsortorder < maxsortorder)
                            option.SortOrder += 1;
                    }
                    else // find the next item and decrement it
                    {
                        if (sqo.SortOrder == currentsortorder + 1)
                        {
                            sqo.SortOrder -= 1;
                            db.Entry(sqo).State = EntityState.Modified;
                        }
                    }
                }
            }

            db.SaveChanges();
        }

        public void UpdateSurveyQuestionOption(SurveyQuestionOption surveyquestionoption)
        {
            db.Entry(surveyquestionoption).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}