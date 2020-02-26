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
using System.Web;
using System.Web.Mvc;
using osVodigiWeb6x.Exceptions;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class ApplicationErrorController : Controller
    {
        //
        // GET: /ApplicationError/

        public ActionResult Index()
        {
            try
            {
                ApplicationError error = (ApplicationError)Session["ApplicationError"];
                ViewData["Controller"] = error.Controller;
                ViewData["Action"] = error.Action;
                ViewData["ErrorMessage"] = error.ErrorMessage;

            }
            catch { }

            return View();
        }

        public ActionResult Error(int statusCode, Exception exception)
        {
            Response.StatusCode = statusCode;
            ViewBag.StatusCode = statusCode.ToString();
            ViewBag.ErrorMessage = exception.ToString();


            if (exception.GetType() == typeof(HttpException))
            {
                ViewBag.ErrorMessage = ((HttpException)exception).GetHtmlErrorMessage();
            }
            else if (exception.GetType() == typeof(NotAuthcException))
            {
                ViewBag.ErrorMessage = ((NotAuthcException)exception).Message;
            }
            else if (exception.GetType() == typeof(NotAuthzException))
            {
                ViewBag.ErrorMessage = ((NotAuthzException)exception).Message;
            }
            else
            {
                ViewBag.ErrorMessage = "Vodigi server internal error. Detail omited";
            }


            return View();
        }

    }
}
