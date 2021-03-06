﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Business
{
    public class ReminderBusiness
    {
        private PODbContext _poDbContext;

        public PODbContext GetPODbContext => _poDbContext;

        public ReminderBusiness(PODbContext poDbContext)
        {
            _poDbContext = poDbContext;
        }

        /// <summary>Adds the reminder.</summary>
        /// <param name="userReminder">The user reminder.</param>
        public void AddReminder(Reminder userReminder)
        {
            if (userReminder == null)
                throw new InvalidOperationException("Reminder should not be null!");

            //Adds the user reminder
            _poDbContext.Reminders.Add(userReminder);
            _poDbContext.SaveChanges();
        }

        /// <summary>Modifies the reminder.</summary>
        /// <param name="userReminder">The user reminder.</param>
        /// <param name="user">The user.</param>
        public void ModifyReminder(Reminder userReminder, User user)
        {
            Reminder @reminder = _poDbContext.Reminders.Find(userReminder.ReminderId);
            if (@reminder == null || @reminder.UserId != user.UserId)
            {
                throw new InvalidOperationException("Reminder either does not exist or is in another user!");
                //Warning: Reminder either does not exist or is in another user
            }
            else
            {
                //Modifies the user reminder
                _poDbContext.Entry(@reminder).CurrentValues.SetValues(userReminder);
                _poDbContext.SaveChanges();
            }
        }

        /// <summary>Removes the reminder.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        public void RemoveReminder(int id, User user)
        {
            Reminder @reminder = _poDbContext.Reminders.Find(id);
            if (reminder == null || @reminder.UserId != user.UserId)
            {
                throw new InvalidOperationException("Reminder either does not exist or is in another user!");
                //Warning: Reminder either does not exist or is in another user
            }
            else
            {
                //Removes the user reminder
                _poDbContext.Reminders.Remove(@reminder);
                _poDbContext.SaveChanges();
            }
        }

        /// <summary>Fetches the reminder by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        public Reminder FetchReminderById(int id, User user)
        {
            Reminder @reminder = _poDbContext.Reminders.Find(id);
            if (@reminder == null || @reminder.UserId != user.UserId)
            {
                throw new InvalidOperationException("Reminder either does not exist or is in another user!");
                //Warning: Reminder either does not exist or is in another user
            }
            else
            {
                //Returns the user reminder
                return @reminder;
            }
        }

        /// <summary>Lists all reminders.</summary>
        /// <param name="user">The user.</param>
        public List<Reminder> ListAllReminders(User user)
        {
            //Returns a List with all user reminders
            return _poDbContext.Reminders.Where(r => r.UserId == user.UserId).ToList();
        }

        /// <summary>Lists all reminders by date.</summary>
        /// <param name="date">The date.</param>
        /// <param name="user">The user.</param>
        public List<Reminder> ListAllRemindersByDate(DateTime date, User user)
        {
            //Returns a List with all user reminders on a certain date
            return _poDbContext.Reminders.Where(r => DbFunctions.TruncateTime(r.DueTime) == date.Date && r.UserId == user.UserId).ToList();
        }

        /// <summary>Removes all reminders.</summary>
        /// <param name="user">The user.</param>
        public void RemoveAllReminders(User user)
        {
            //Removes all user reminders
            _poDbContext.Reminders.RemoveRange(_poDbContext.Reminders.Where(x => x.UserId == user.UserId));
            _poDbContext.SaveChanges();
        }
    }
}
