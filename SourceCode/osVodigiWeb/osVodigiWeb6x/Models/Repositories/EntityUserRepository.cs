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
    public class EntityUserRepository : IUserRepository
    {
        private VodigiContext db = new VodigiContext();

        public User GetUser(int id)
        {
            User user = db.Users.Find(id);

            return user;
        }

        public User GetUserByUsername(string username)
        {
            var query = from user in db.Users
                        select user;
            query = query.Where(us => us.Username.Equals(username));

            List<User> users = query.ToList();

            if (users.Count > 0)
                return users[0];
            else
                return null;
        }

        public User ValidateUser(string username, string password)
        {
            var query = from user in db.Users
                        select user;
            query = query.Where(us => us.Username.Equals(username));
            query = query.Where(us => us.Password.Equals(password));
            query = query.Where(us => us.IsActive == true);

            List<User> users = query.ToList();

            if (users.Count > 0)
                return users[0];
            else
                return null;
        }

        public IEnumerable<User> GetUserPage(int accountid, string username, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from user in db.Users
                        select user;
            if (accountid > 0)
                query = query.Where(us => us.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(username))
                query = query.Where(us => us.Username.StartsWith(username));
            if (!includeinactive)
                query = query.Where(us => us.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<User> users = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return users;
        }

        public int GetUserRecordCount(int accountid, string username, bool includeinactive)
        {
            var query = from user in db.Users
                        select user;
            if (accountid > 0)
                query = query.Where(us => us.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(username))
                query = query.Where(us => us.Username.StartsWith(username));
            if (!includeinactive)
                query = query.Where(us => us.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreateUser(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}