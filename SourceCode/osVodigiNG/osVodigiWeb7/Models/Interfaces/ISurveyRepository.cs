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

namespace osVodigiWeb6x.Models
{
    public interface ISurveyRepository
    {
        void CreateSurvey(Survey survey);
        void UpdateSurvey(Survey survey);
        Survey GetSurvey(int surveyid);
        IEnumerable<Survey> GetAllSurveys(int accountid);
        IEnumerable<Survey> GetActiveSurveys(int accountid);
        IEnumerable<Survey> GetApprovedSurveys(int accountid);
        IEnumerable<Survey> GetSurveyPage(int accountid, string surveyname, bool onlyapproved, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount);
        int GetSurveyRecordCount(int accountid, string surveyname, bool onlyapproved, bool includeinactive);
        int SaveChanges();
    }
}