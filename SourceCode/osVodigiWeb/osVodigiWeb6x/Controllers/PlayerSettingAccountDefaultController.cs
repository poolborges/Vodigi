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
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class PlayerSettingAccountDefaultController : Controller
    {
        IPlayerSettingAccountDefaultRepository repository;

        public PlayerSettingAccountDefaultController()
            : this(new EntityPlayerSettingAccountDefaultRepository())
        { }

        public PlayerSettingAccountDefaultController(IPlayerSettingAccountDefaultRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /PlayerSettingAccountDefault/

        public ActionResult Index()
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // This is the list we are building to display
                List<PlayerSettingAccountDefaultView> accountdefaultviews = new List<PlayerSettingAccountDefaultView>();

                // First, get all the default system settings - this is the master list and has all the master setting names and default values
                IPlayerSettingSystemDefaultRepository systemdefaultrep = new EntityPlayerSettingSystemDefaultRepository();
                IEnumerable<PlayerSettingSystemDefault> systemdefaults = systemdefaultrep.GetAllPlayerSettingSystemDefaults();

                // Add an account default view for each system default
                IPlayerSettingTypeRepository typerepository = new EntityPlayerSettingTypeRepository();
                foreach (PlayerSettingSystemDefault systemdefault in systemdefaults)
                {
                    PlayerSettingAccountDefaultView accountdefaultview = new PlayerSettingAccountDefaultView();
                    accountdefaultview.PlayerSettingAccountDefaultID = 0;
                    accountdefaultview.AccountID = accountid;
                    accountdefaultview.PlayerSettingName = systemdefault.PlayerSettingName;
                    accountdefaultview.PlayerSettingTypeID = systemdefault.PlayerSettingTypeID;
                    accountdefaultview.PlayerSettingAccountDefaultValue = systemdefault.PlayerSettingSystemDefaultValue;
                    accountdefaultview.PlayerSettingDescription = systemdefault.PlayerSettingDescription;

                    // Add the player setting type name
                    PlayerSettingType type = typerepository.GetPlayerSettingType(systemdefault.PlayerSettingTypeID);
                    accountdefaultview.PlayerSettingTypeName = type.PlayerSettingTypeName;
                    accountdefaultviews.Add(accountdefaultview);
                }

                // If any account player setting defaults exist - update the settings
                IPlayerSettingAccountDefaultRepository accountdefaultrep = new EntityPlayerSettingAccountDefaultRepository();
                foreach (PlayerSettingAccountDefaultView accountdefaultview in accountdefaultviews)
                {
                    PlayerSettingAccountDefault accountdefault = accountdefaultrep.GetByPlayerSettingName(accountid, accountdefaultview.PlayerSettingName);
                    if (accountdefault != null)
                    {
                        accountdefaultview.PlayerSettingAccountDefaultID = accountdefault.PlayerSettingAccountDefaultID;
                        accountdefaultview.PlayerSettingAccountDefaultValue = accountdefault.PlayerSettingAccountDefaultValue;
                    }
                }

                accountdefaultviews.Sort();
                ViewResult result = View(accountdefaultviews);
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("PlayerSettingAccountDefault", "Index", ex);
                
            }
        }

        //
        // GET: /PlayerSettingAccountDefault/Edit/5

        public ActionResult Edit(string id)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                bool isid = true;
                try
                {
                    int x = Convert.ToInt32(id);
                }
                catch { isid = false; }

                PlayerSettingAccountDefault accountdefault = new PlayerSettingAccountDefault();
                IPlayerSettingSystemDefaultRepository systemdefaultrep = new EntityPlayerSettingSystemDefaultRepository();

                if (isid)
                {
                    IPlayerSettingAccountDefaultRepository accountdefaultrep = new EntityPlayerSettingAccountDefaultRepository();
                    accountdefault = accountdefaultrep.GetByPlayerSettingAccountDefaultID(Convert.ToInt32(id));
                }
                else
                {
                    PlayerSettingSystemDefault systemdefault = systemdefaultrep.GetByPlayerSettingName(id);
                    if (systemdefault != null)
                    {
                        accountdefault.PlayerSettingAccountDefaultID = 0;
                        accountdefault.AccountID = AuthUtils.GetAccountId();
                        accountdefault.PlayerSettingName = systemdefault.PlayerSettingName;
                        accountdefault.PlayerSettingTypeID = systemdefault.PlayerSettingTypeID;
                        accountdefault.PlayerSettingAccountDefaultValue = systemdefault.PlayerSettingSystemDefaultValue;
                    }
                }

                IPlayerSettingTypeRepository typerep = new EntityPlayerSettingTypeRepository();
                ViewData["PlayerSettingTypeName"] = typerep.GetPlayerSettingType(accountdefault.PlayerSettingTypeID).PlayerSettingTypeName;
                ViewData["PlayerSettingDescription"] = systemdefaultrep.GetByPlayerSettingName(accountdefault.PlayerSettingName).PlayerSettingDescription;
                ViewData["ValidationMessage"] = String.Empty;

                return View(accountdefault);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("PlayerSettingAccountDefault", "Edit", ex);
                
            }
        }

        //
        // POST: /PlayerSettingAccountDefault/Edit/5

        [HttpPost]
        public ActionResult Edit(PlayerSettingAccountDefault accountdefault)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    string validation = ValidateInput(accountdefault);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        IPlayerSettingTypeRepository typerep = new EntityPlayerSettingTypeRepository();
                        IPlayerSettingSystemDefaultRepository systemdefaultrep = new EntityPlayerSettingSystemDefaultRepository();
                        ViewData["PlayerSettingDescription"] = systemdefaultrep.GetByPlayerSettingName(accountdefault.PlayerSettingName).PlayerSettingDescription;
                        ViewData["PlayerSettingTypeName"] = typerep.GetPlayerSettingType(accountdefault.PlayerSettingTypeID).PlayerSettingTypeName;
                        ViewData["ValidationMessage"] = validation;
                        return View(accountdefault);
                    }

                    IPlayerSettingAccountDefaultRepository accountdefaultrep = new EntityPlayerSettingAccountDefaultRepository();
                    if (accountdefault.PlayerSettingAccountDefaultID == 0)
                    {
                        accountdefaultrep.CreatePlayerSettingAccountDefault(accountdefault);
                        CommonMethods.CreateActivityLog(AuthUtils.CheckAuthUser(), "PlayerSettingAccountDefault", "Create",
                                "Created player setting '" + accountdefault.PlayerSettingName + "' with value '" + accountdefault.PlayerSettingAccountDefaultValue + "'");
                    }
                    else
                    {
                        accountdefaultrep.UpdatePlayerSettingAccountDefault(accountdefault);
                        CommonMethods.CreateActivityLog(AuthUtils.CheckAuthUser(), "PlayerSettingAccountDefault", "Edit",
                                "Updated player setting '" + accountdefault.PlayerSettingName + "' to value '" + accountdefault.PlayerSettingAccountDefaultValue + "'");
                    }


                    return RedirectToAction("Index");
                }

                return View(accountdefault);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("PlayerSettingAccountDefault", "Edit POST", ex);
                
            }
        }

        private string ValidateInput(PlayerSettingAccountDefault accountdefault)
        {
            if (accountdefault.PlayerSettingTypeID == 1000000) // Integer
            {
                try
                {
                    int i = Convert.ToInt32(accountdefault.PlayerSettingAccountDefaultValue);
                }
                catch
                {
                    return "Please enter a valid integer value";
                }
            }
            else if (accountdefault.PlayerSettingTypeID == 1000001) // String
            {
                if (String.IsNullOrEmpty(accountdefault.PlayerSettingAccountDefaultValue))
                    return "Please enter a valid string value";
            }
            else if (accountdefault.PlayerSettingTypeID == 1000002) // Float
            {
                try
                {
                    double d = Convert.ToDouble(accountdefault.PlayerSettingAccountDefaultValue);
                }
                catch
                {
                    return "Please enter a valid floating point value";
                }
            }
            else if (accountdefault.PlayerSettingTypeID == 1000003) // Boolean
            {
                if (accountdefault.PlayerSettingAccountDefaultValue != "True" && accountdefault.PlayerSettingAccountDefaultValue != "False")
                    return "Please enter either True or False";
            }

            return String.Empty;
        }

    }
}
