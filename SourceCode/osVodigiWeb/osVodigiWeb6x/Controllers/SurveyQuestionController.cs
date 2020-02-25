﻿/* ----------------------------------------------------------------------------------------
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
using System.Web;
using System.Web.Mvc;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class SurveyQuestionController : Controller
    {
        ISurveyQuestionRepository repository;

        public SurveyQuestionController()
            : this(new EntitySurveyQuestionRepository())
        { }

        public SurveyQuestionController(ISurveyQuestionRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /SurveyQuestion/Create/surveyid

        public ActionResult Create(int id)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewSurveyQuestion(id));
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("SurveyQuestion", "Create", ex);
                
            }
        }

        //
        // POST: /SurveyQuestion/Create/surveyid

        [HttpPost]
        public ActionResult Create(int id, SurveyQuestion surveyquestion)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    surveyquestion.SurveyID = id;
                    // Note: Proper sort order is applied in the repository

                    string validation = ValidateInput(surveyquestion);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(surveyquestion);
                    }
                    else
                    {
                        repository.CreateSurveyQuestion(surveyquestion);

                        CommonMethods.CreateActivityLog((User)Session["User"], "SurveyQuestion", "Add",
                            "Added survey question '" + surveyquestion.SurveyQuestionText + "' - ID: " + surveyquestion.SurveyQuestionID.ToString());

                        return RedirectToAction("Edit", "Survey", new { id = surveyquestion.SurveyID });
                    }
                }
                return View(surveyquestion);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("SurveyQuestion", "Create POST", ex);
                
            }
        }

        //
        // GET: /SurveyQuestion/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                SurveyQuestion surveyquestion = repository.GetSurveyQuestion(id);
                ViewData["ValidationMessage"] = String.Empty;

                return View(surveyquestion);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("SurveyQuestion", "Edit", ex);
                
            }
        }

        //
        // POST: /SurveyQuestion/Edit/5

        [HttpPost]
        public ActionResult Edit(SurveyQuestion surveyquestion)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    string validation = ValidateInput(surveyquestion);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(surveyquestion);
                    }

                    repository.UpdateSurveyQuestion(surveyquestion);

                    CommonMethods.CreateActivityLog((User)Session["User"], "SurveyQuestion", "Edit",
                                                    "Edited survey question '" + surveyquestion.SurveyQuestionText + "' - ID: " + surveyquestion.SurveyQuestionID.ToString());

                    return RedirectToAction("Edit", "Survey", new { id = surveyquestion.SurveyID });
                }

                return View(surveyquestion);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("SurveyQuestion", "Edit POST", ex);
                
            }
        }


        //
        // GET: /SurveyQuestion/Delete/5

        public ActionResult Delete(int id)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                SurveyQuestion surveyquestion = repository.GetSurveyQuestion(id);
                int surveyid = surveyquestion.SurveyID;

                repository.DeleteSurveyQuestion(surveyquestion);

                return RedirectToAction("Edit", "Survey", new { id = surveyid });
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("SurveyQuestion", "Delete", ex);
                
            }
        }

        //
        // GET: /SurveyQuestion/MoveUp/5

        public ActionResult MoveUp(int id)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                SurveyQuestion surveyquestion = repository.GetSurveyQuestion(id);
                int surveyid = surveyquestion.SurveyID;

                repository.MoveSurveyQuestion(surveyquestion, true);

                return RedirectToAction("Edit", "Survey", new { id = surveyid });
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("SurveyQuestion", "MoveUp", ex);
                
            }
        }

        //
        // GET: /SurveyQuestion/MoveDown/5

        public ActionResult MoveDown(int id)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                SurveyQuestion surveyquestion = repository.GetSurveyQuestion(id);
                int surveyid = surveyquestion.SurveyID;

                repository.MoveSurveyQuestion(surveyquestion, false);

                return RedirectToAction("Edit", "Survey", new { id = surveyid });
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("SurveyQuestion", "MoveDown", ex);
                
            }
        }

        //
        // Support Methods

        private SurveyQuestion CreateNewSurveyQuestion(int id)
        {
            SurveyQuestion surveyquestion = new SurveyQuestion();
            surveyquestion.SurveyQuestionID = 0;
            surveyquestion.SurveyID = id;
            surveyquestion.SurveyQuestionText = String.Empty;
            surveyquestion.AllowMultiSelect = false;
            surveyquestion.SortOrder = 1;

            return surveyquestion;
        }

        private string ValidateInput(SurveyQuestion surveyquestion)
        {
            if (surveyquestion.SurveyID == 0)
                return "Survey ID is not valid.";

            if (String.IsNullOrEmpty(surveyquestion.SurveyQuestionText))
                return "Survey Question Text is required.";

            return String.Empty;
        }
    }
}
