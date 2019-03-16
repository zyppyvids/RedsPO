using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using NUnit.Framework;
using Moq;
using Business;
using static UnitTesting.UnitTestMethods;

namespace Tests
{
    [TestFixture]
    public class UserBusinessTests
    {
        private Mock<DbSet<Event>> _mockEvents;
        private Mock<DbSet<Reminder>> _mockReminders;
        private Mock<DbSet<Task>> _mockTasks;
        private Mock<DbSet<User>> _mockUsers;

        private Mock<PODbContext> _mockContext;

        [SetUp]
        public void Setup()
        {
            List<User> users = new List<User>()
            {
                new User() { UserId = 1, UserName = "userName1", PasswordHash = "passwordHash", FirstName = "firstName", LastName = "lastName" },
                new User() { UserId = 2, UserName = "userName2", PasswordHash = "passwordHash", FirstName = "firstName", LastName = "lastName" }
            };

            List<Event> events = new List<Event>()
            {
                new Event() { EventId = 1, Name = "name", DueTime = DateTime.Now, UserId = 1},
                new Event() { EventId = 2, Name = "name", DueTime = DateTime.Now, UserId = 2}
            };

            List<Reminder> reminders = new List<Reminder>()
            {
                new Reminder() { ReminderId = 1, Name = "name", DueTime = DateTime.Now, UserId = 1},
                new Reminder() { ReminderId = 2, Name = "name", DueTime = DateTime.Now, UserId = 1}
            };

            List<Task> tasks = new List<Task>()
            {
                new Task() { TaskId = 1, Name = "name", Date = DateTime.Now, IsDone = false, UserId = 1},
                new Task() { TaskId = 2, Name = "name", Date = DateTime.Now, IsDone = false, UserId = 1},
            };

            _mockEvents = GetQueryableMockDbSet(events);

            _mockReminders = GetQueryableMockDbSet(reminders);

            _mockTasks = GetQueryableMockDbSet(tasks);

            _mockUsers = GetQueryableMockDbSet(users);

            _mockContext = new Mock<PODbContext>();
            _mockContext.Setup(m => m.Events).Returns(_mockEvents.Object);
            _mockContext.Setup(m => m.Reminders).Returns(_mockReminders.Object);
            _mockContext.Setup(m => m.Tasks).Returns(_mockTasks.Object);
            _mockContext.Setup(m => m.Users).Returns(_mockUsers.Object);

        }

        [Test]
        public void TestRegisterNewUserToTheDatabase()
        {
            UserBusiness mockUserBusiness = new UserBusiness(_mockContext.Object);

            User mockUser = new User() { UserId = 3, UserName = "userName", FirstName = "firstName", LastName = "lastName", PasswordHash = "passwordHash"};

            mockUserBusiness.Register(mockUser);

            Assert.Contains(mockUser, mockUserBusiness.GetPODbContext.Users.ToList(), "User not registered properly!");
        }

        [Test]
        public void TestRegisterNullUserToTheDatabase()
        {
            UserBusiness mockUserBusiness = new UserBusiness(_mockContext.Object);

            User mockUser = null;

            Assert.Catch(() => mockUserBusiness.Register(mockUser), "Null user registered to the database!");
        }

        [Test]
        public void TestFetchAllUsersFromTheDatabase()
        {
            UserBusiness mockUserBusiness = new UserBusiness(_mockContext.Object);

            List<User> mockUsers = mockUserBusiness.FetchAllUsers();

            Assert.AreEqual(mockUsers.Count(), mockUserBusiness.GetPODbContext.Users.Count(), "Not all users are fetched!");
        }

        [Test]
        public void TestFalseFetchAllUsersFromTheDatabase()
        {
            UserBusiness mockUserBusiness = new UserBusiness(_mockContext.Object);

            List<User> mockUsers = mockUserBusiness.FetchAllUsers();

            mockUsers.Add(new User());

            Assert.AreNotEqual(mockUsers.Count(), mockUserBusiness.GetPODbContext.Users.Count(), "Not all users are fetched!");
        }

        [Test]
        public void TestFetchUserFromTheDatabase()
        {
            string userName = "userName1";
            string passwordHash = "passwordHash";

            int expectedId = 1;

            UserBusiness mockUserBusiness = new UserBusiness(_mockContext.Object);

            User mockUser = mockUserBusiness.FetchUser(userName, passwordHash);

            Assert.AreEqual(expectedId, mockUser.UserId,  "User not fetched correctly!");
        }

        [Test]
        public void TestFalseFetchUserFromTheDatabase()
        {
            string userName = "nonExisting";
            string passwordHash = "nonExisting";

            UserBusiness mockUserBusiness = new UserBusiness(_mockContext.Object);

            User mockUser = mockUserBusiness.FetchUser(userName, passwordHash);

            Assert.AreEqual(null, mockUser, "Non existent user fetched!");
        }

        [Test]
        public void TestUserIsExistingInTheDatabase()
        {
            UserBusiness mockUserBusiness = new UserBusiness(_mockContext.Object);

            User existingUser = mockUserBusiness.GetPODbContext.Users.ToList()[0];

            Assert.True(mockUserBusiness.IsExisting(existingUser), "IsExisting() returns False for existing User!");
        }

        [Test]
        public void TestUserNotExistingInTheDatabase()
        {
            UserBusiness mockUserBusiness = new UserBusiness(_mockContext.Object);

            User nonExistingUser = new User();

            Assert.False(mockUserBusiness.IsExisting(nonExistingUser), "IsExisting() returns True for non existing User!");
        }

        [Test]
        public void TestHashPassword()
        {
            string text = "testText";

            string expectedHash = "85AC464C2F22837C991C50FDAA5FACC25A09FD7664B5D50427F69B4DF7744BDC";

            string returnedHash = UserBusiness.HashPassword(text);

            Assert.True(expectedHash == returnedHash, "Hashing of passwords is incorrect!");
        }

        [Test]
        public void TestHashNullPassword()
        {
            string text = null;

            Assert.Catch(() => UserBusiness.HashPassword(text), "Null password is hashed!");
        }
    }
}