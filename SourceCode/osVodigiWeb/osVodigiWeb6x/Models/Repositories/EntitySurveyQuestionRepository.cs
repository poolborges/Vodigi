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
    public class EntitySurveyQuestionRepository : ISurveyQuestionRepository
    {
        private VodigiContext db = new VodigiContext();

        public IEnumerable<SurveyQuestion> GetSurveyQuestions(int surveyid)
        {
            var query = from surveyquestion in db.SurveyQuestions
                        select surveyquestion;
            query = query.Where(sqs => sqs.SurveyID.Equals(surveyid));
            query = query.OrderBy("SortOrder", false);

            List<SurveyQuestion> surveyquestions = query.ToList();

            return surveyquestions;
        }

        public SurveyQuestion GetSurveyQuestion(int id)
        {
            SurveyQuestion surveyquestion = db.SurveyQuestions.Find(id);
            return surveyquestion;
        }

        public void DeleteSurveyQuestion(SurveyQuestion question)
        {
            var query = from surveyquestion in db.SurveyQuestions
                        select surveyquestion;

            query = query.Where(sqs => sqs.SurveyID.Equals(question.SurveyID));
            query = query.OrderBy("SortOrder", false);

            List<SurveyQuestion> surveyquestions = query.ToList();

            bool found = false;
            foreach (SurveyQuestion sq in surveyquestions)
            {
                if (found)
                {
                    sq.SortOrder -= 1;
                    db.Entry(sq).State = EntityState.Modified;
                }
                if (sq.SurveyQuestionID == question.SurveyQuestionID)
                {
                    found = true;
                    db.SurveyQuestions.Remove(sq);
                }
            }

            db.SaveChanges();
        }

        public void CreateSurveyQuestion(SurveyQuestion question)
        {
            // Get the maximum sort order
            var query = from surveyquestion in db.SurveyQuestions
                        select surveyquestion;

            query = query.Where(sqs => sqs.SurveyID.Equals(question.SurveyID));
            query = query.OrderBy("SortOrder", true);

            List<SurveyQuestion> surveyquestions = query.ToList();

            int maxsortorder = 0;
            if (surveyquestions.Count > 0)
                maxsortorder = surveyquestions[0].SortOrder;

            question.SortOrder = maxsortorder + 1;
            db.SurveyQuestions.Add(question);
            db.SaveChanges();
        }

        public void MoveSurveyQuestion(SurveyQuestion question, bool ismoveup)
        {
            var query = from surveyquestion in db.SurveyQuestions
                        select surveyquestion;
            query = query.Where(sqs => sqs.SurveyID.Equals(question.SurveyID));
            query = query.OrderBy("SortOrder", false);

            List<SurveyQuestion> surveyquestions = query.ToList();

            // Get the current and max sort orders
            int currentsortorder = question.SortOrder;
            int maxsortorder = 1;
            foreach (SurveyQuestion sq in surveyquestions)
            {
                if (sq.SortOrder > maxsortorder)
                    maxsortorder = sq.SortOrder;
            }

            // Adjust the appropriate sort orders
            foreach (SurveyQuestion sq in surveyquestions)
            {
                if (ismoveup)
                {
                    if (sq.SurveyQuestionID == question.SurveyQuestionID) // move current question up
                    {
                        if (currentsortorder > 1)
                            question.SortOrder -= 1;
                    }
                    else // find the previous item and increment it
                    {
                        if (sq.SortOrder == currentsortorder - 1)
                        {
                            sq.SortOrder += 1;
                            db.Entry(sq).State = EntityState.Modified;
                        }
                    }
                }
                else
                {
                    if (sq.SurveyQuestionID == question.SurveyQuestionID) // move current question down
                    {
                        if (currentsortorder < maxsortorder)
                            question.SortOrder += 1;
                    }
                    else // find the next item and decrement it
                    {
                        if (sq.SortOrder == currentsortorder + 1)
                        {
                            sq.SortOrder -= 1;
                            db.Entry(sq).State = EntityState.Modified;
                        }
                    }
                }
            }

            db.SaveChanges();
        }

        public void UpdateSurveyQuestion(SurveyQuestion surveyquestion)
        {
            db.Entry(surveyquestion).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}