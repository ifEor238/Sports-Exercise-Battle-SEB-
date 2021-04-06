using System;
using System.Collections.Generic;
using NUnit.Framework;
using SWE1_SEB_Eory;

namespace SWE1_SEB_Eory_Test
{
    class UnitTests
    {


        [Test]
        public void TestAddCount()
        {
            History his1 = new History(20, 50);

            his1.addCount(20);

            Assert.AreEqual(his1.Count, 40);
        }

        [Test]
        public void TestAddDuration()
        {
            History his1 = new History(20, 50);

            his1.addDuration(20);

            Assert.AreEqual(his1.Duration, 70);
        }

        [Test]
        public void TestAddDurationError()
        {
            History his1 = new History(20, 50);

            Assert.Throws<InvalidOperationException>(() => his1.addDuration(100));
        }

        [Test]
        public void TestTournamentSimple()
        {
            User user1 = new User("Test 1");

            History his1 = new History(25, 40);

            Tournament tourn = new Tournament();

            tourn.AddUser(user1, his1.Count, his1.Duration);

            tourn.handleTournament();

            Assert.AreEqual(tourn.winner, user1);
        }

        [Test]
        public void TestTournament3PlayersWin2InitialValues()
        {
            User user1 = new User("Test 1");
            User user2 = new User("Test 2");
            User user3 = new User("Test 3");

            History his1 = new History(25, 40);
            History his2 = new History(30, 60);
            History his3 = new History(20, 30);

            Tournament tourn = new Tournament();

            tourn.AddUser(user1, his1.Count, his1.Duration);
            tourn.AddUser(user2, his2.Count, his2.Duration);
            tourn.AddUser(user3, his3.Count, his3.Duration);

            tourn.handleTournament();

            Assert.AreEqual(tourn.winner, user2);
        }

        [Test]
        public void TestTournament3PlayersWin3ChangedValues()
        {
            User user1 = new User("Test 1");
            User user2 = new User("Test 2");
            User user3 = new User("Test 3");

            History his1 = new History(25, 40);
            History his2 = new History(30, 60);
            History his3 = new History(20, 30);

            Tournament tourn = new Tournament();

            tourn.AddUser(user1, his1.Count, his1.Duration);
            tourn.AddUser(user2, his2.Count, his2.Duration);
            tourn.AddUser(user3, his3.Count, his3.Duration);

            tourn.updateHistory(user3, 30, 50);

            tourn.handleTournament();

            Assert.AreEqual(tourn.winner, user3);
        }

        [Test]
        public void TestPOST()
        {
            var recv = @"POST /sessions HTTP/1.1
            Host: localhost:10001
            User-Agent: curl/7.55.1
            Accept: */*
            Content-Type: application/json
            Content-Length: 12

            Testing POST";
            var list = new List<string>();
            var requestContext = new RequestContext(recv, list);

            Assert.AreEqual("POST", requestContext.Method);
        }

        [Test]
        public void TestGET()
        {
            var recv = @"GET /users/kienboec HTTP/1.1
            Host: localhost:10001
            User-Agent: curl/7.55.1
            Accept: */*
            Authorization: Basic altenhof-sebToken
            ";
            var list = new List<string>();
            var requestContext = new RequestContext(recv, list);

            Assert.AreEqual("GET", requestContext.Method);
        }

        [Test]
        public void TestPUT()
        {
            var recv = @"PUT /users/kienboec HTTP/1.1
            Host: localhost:10001
            User-Agent: curl/7.55.1
            Accept: */*
            Authorization: Basic altenhof-sebToken
            ";
            var list = new List<string>();
            var requestContext = new RequestContext(recv, list);

            Assert.AreEqual("PUT", requestContext.Method);
        }

        [Test]
        public void TestDelete()
        {
            var recv = @"DELETE /users/kienboec HTTP/1.1
            Host: localhost:10001
            User-Agent: curl/7.55.1
            Accept: */*
            Authorization: Basic altenhof-sebToken
            ";
            var list = new List<string>();
            var requestContext = new RequestContext(recv, list);

            Assert.AreEqual("DELETE", requestContext.Method);
        }


    }
}
