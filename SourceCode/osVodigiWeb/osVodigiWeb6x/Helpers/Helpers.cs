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
using System.Linq.Expressions;
using System.Web;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x
{
    public class Helpers
    {
        public static void SetupApplicationError(string controller, string action, string errormessage)
        {
            ApplicationError error = new ApplicationError();
            error.Controller = controller;
            error.Action = action;
            error.ErrorMessage = errormessage;
            HttpContext.Current.Session["ApplicationError"] = error;
        }
    }

    public static class DynamicOrderBy
    {
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty,
                                                           bool desc) where TEntity : class
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                                   source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }

    public static class CommonMethods
    {
        public static void CreateActivityLog(User user, string entitytype, string entityaction, string activitydetails)
        {
            try
            {
                IActivityLogRepository activityrep = new EntityActivityLogRepository();

                ActivityLog activitylog = new ActivityLog();
                activitylog.AccountID = user.AccountID;
                activitylog.UserID = user.UserID;
                activitylog.Username = user.Username;
                activitylog.EntityType = entitytype;
                activitylog.EntityAction = entityaction;
                activitylog.ActivityDateTime = DateTime.Now.ToUniversalTime();
                activitylog.ActivityDetails = activitydetails;

                activityrep.CreateActivityLog(activitylog);
            }
            catch { }
        }
    }
}