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
using Microsoft.EntityFrameworkCore;

namespace osVodigiWeb6x.Models
{
    public class EntityAccountRepository : IAccountRepository
    {
        private VodigiContext db = new VodigiContext();

        public Account GetAccount(int id)
        {
            Account account = db.Accounts.Find(id);

            return account;
        }

        public IEnumerable<Account> GetActiveAccounts()
        {
            var query = from account in db.Accounts
                        select account;
            query = query.Where(accts => accts.IsActive == true);
            query = query.OrderBy(a => a.AccountName);

            List<Account> accounts = query.ToList();

            return accounts;
        }

        public IEnumerable<Account> GetAccountByName(string accountname)
        {
            var query = from account in db.Accounts
                        select account;
            query = query.Where(accts => accts.AccountName == accountname);
            query = query.OrderBy(a => a.IsActive);

            List<Account> accounts = query.ToList();

            return accounts;
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            var query = from account in db.Accounts
                        select account;
            query = query.OrderBy(a => a.AccountName);

            List<Account> accounts = query.ToList();

            return accounts;
        }

        public IEnumerable<Account> GetAccountPage(string accountname, string description, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from account in db.Accounts
                        select account;
            if (!String.IsNullOrEmpty(accountname))
                query = query.Where(accts => accts.AccountName.StartsWith(accountname));
            if (!String.IsNullOrEmpty(description))
                query = query.Where(accts => accts.AccountDescription.Contains(description));
            if (!includeinactive)
                query = query.Where(accts => accts.IsActive == true);

            // Apply the ordering
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<Account> accounts = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return accounts;
        }

        public int GetAccountRecordCount(string accountname, string description, bool includeinactive)
        {
            var query = from account in db.Accounts
                        select account;
            if (!String.IsNullOrEmpty(accountname))
                query = query.Where(accts => accts.AccountName.StartsWith(accountname));
            if (!String.IsNullOrEmpty(description))
                query = query.Where(accts => accts.AccountDescription.Contains(description));
            if (!includeinactive)
                query = query.Where(accts => accts.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreateAccount(Account account)
        {
            db.Accounts.Add(account);
            db.SaveChanges();
        }

        public void UpdateAccount(Account account)
        {
            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

    }
}