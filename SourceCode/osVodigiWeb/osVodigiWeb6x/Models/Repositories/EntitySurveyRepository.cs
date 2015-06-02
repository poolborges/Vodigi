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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace osVodigiWeb6x.Models
{
    public class EntitySurveyRepository : ISurveyRepository
    {
        private VodigiContext db = new VodigiContext();

        public Survey GetSurvey(int id)
        {
            Survey survey = db.Surveys.Find(id);

            return survey;
        }

        public IEnumerable<Survey> GetAllSurveys(int accountid)
        {
            var query = from survey in db.Surveys
                        select survey;
            query = query.Where(svs => svs.AccountID.Equals(accountid));
            query = query.OrderBy("SurveyName", false);

            List<Survey> surveys = query.ToList();

            return surveys;
        }

        public IEnumerable<Survey> GetActiveSurveys(int accountid)
        {
            var query = from survey in db.Surveys
                        select survey;
            query = query.Where(svs => svs.AccountID.Equals(accountid));
            query = query.Where(svs => svs.IsActive == true);
            query = query.OrderBy("SurveyName", false);

            List<Survey> surveys = query.ToList();

            return surveys;
        }

        public IEnumerable<Survey> GetApprovedSurveys(int accountid)
        {
            var query = from survey in db.Surveys
                        select survey;
            query = query.Where(svs => svs.AccountID.Equals(accountid));
            query = query.Where(svs => svs.IsApproved == true);
            query = query.OrderBy("SurveyName", false);

            List<Survey> surveys = query.ToList();

            return surveys;
        }

        public IEnumerable<Survey> GetSurveyPage(int accountid, string surveyname, bool onlyapproved, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from survey in db.Surveys
                        select survey;

            query = query.Where(svs => svs.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(surveyname))
                query = query.Where(svs => svs.SurveyName.StartsWith(surveyname));
            if (onlyapproved)
                query = query.Where(svs => svs.IsApproved == true);
            if (!includeinactive)
                query = query.Where(svs => svs.IsActive == true);

            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<Survey> surveys = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return surveys;
        }

        public int GetSurveyRecordCount(int accountid, string surveyname, bool onlyapproved, bool includeinactive)
        {
            var query = from survey in db.Surveys
                        select survey;

            query = query.Where(svs => svs.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(surveyname))
                query = query.Where(svs => svs.SurveyName.StartsWith(surveyname));
            if (onlyapproved)
                query = query.Where(svs => svs.IsApproved == true);
            if (!includeinactive)
                query = query.Where(svs => svs.IsActive == true);

            return query.Count();
        }

        public void CreateSurvey(Survey survey)
        {
            db.Surveys.Add(survey);
            db.SaveChanges();
        }

        public void UpdateSurvey(Survey survey)
        {
            db.Entry(survey).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}