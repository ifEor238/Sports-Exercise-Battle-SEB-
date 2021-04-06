using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SWE1_SEB_Eory
{
    public class ResponseContext
    {
        private RequestContext requestContext;
        static readonly string dbString = "Host=localhost;Username=postgres;Password=postgres;Database=sbeDB";
        //public volatile bool tournamentIsRunning = false;
        Tournament tourn = new Tournament();
        //volatile int userCount = 0;

        public ResponseContext(RequestContext requestContext)
        {
            this.requestContext = requestContext;
        }

        private string CreateResponse(string status, string contentType, string content)
        {
            var response = "";
            response += "HTTP/1.1 " + status + "\r\n" +
                   "Content-Type: " + contentType + "\r\n" +
                   "Content-Length: " + content.Length + "\r\n" +
                   "\r\n" +
                   content;
            return response;
        }

        public string ExecuteRequest()
        {
            Thread t = new Thread(new ThreadStart(Tournament));
            if (requestContext.Method.ToUpper().Equals("GET"))
            {
                if (requestContext.endpoint.ToLower().Contains("/users/"))
                {
                    var username = requestContext.endpoint.Substring(7);
                    if (matchAuthToken(username))
                    {
                        return showProfileInfo(username);
                    }
                    else
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "BAD TOKEN");
                    }
                }
                if (requestContext.endpoint.ToLower().Contains("/stats"))
                {
                    var username = requestContext.endpoint.Substring(7);
                    if (matchAuthToken(username))
                    {
                        return showStats(username);
                    }
                    else
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "BAD TOKEN");
                    }
                }
                if (requestContext.endpoint.ToLower().Contains("/score"))
                {
                    if (matchAuthToken())
                    {
                        return showScore();
                    }
                    else
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "BAD TOKEN");
                    }
                }
                if (requestContext.endpoint.ToLower().Contains("/history/"))
                {
                    var username = requestContext.endpoint.Substring(9);
                    if (matchAuthToken(username))
                    {
                        return showHistory(username);
                    }
                    else
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "BAD TOKEN");
                    }
                }
                if (requestContext.endpoint.ToLower().Contains("/tournament"))
                {
                    if (matchAuthToken())
                    {
                        return showTournament();
                    }
                    else
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "BAD TOKEN");
                    }
                }
                else
                {
                    return CreateResponse("400 Bad Request", "text/plain", "GET: Error");
                }
            }
            else if (requestContext.Method.ToUpper().Equals("POST"))
            {
                if (requestContext.endpoint.ToLower().Equals("/users"))
                {
                    return Register();
                }
                if (requestContext.endpoint.ToLower().Contains("/history/"))
                {
                    var username = requestContext.endpoint.Substring(9);
                    if (matchAuthToken(username))
                    {
                        if(!t.IsAlive)
                        {    
                            t.Start();
                            return Tournament(username);
                        }
                        else
                        {
                            return Tournament(username);
                        }
                        
                    }
                    else
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "BAD TOKEN");
                    }
                }
                if (requestContext.endpoint.ToLower().Contains("/extra/"))
                {
                    var username = requestContext.endpoint.Substring(7);
                    if (matchAuthToken(username))
                    {
                        return handleHandstand(username);
                    }
                    else
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "BAD TOKEN");
                    }
                }
                if (requestContext.endpoint.ToLower().Equals("/sessions"))
                {
                    string token = Login();
                    if (token is null)
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "User or password is incorrect!");
                    }
                    return CreateResponse("200 OK", "text/plain", "Logged in");
                }
                else
                {
                    return CreateResponse("400 Bad Request", "text/plain", "POST: Error");
                }
            }
            else if (requestContext.Method.ToUpper().Equals("DELETE"))
            {
                return CreateResponse("400 Bad Request", "text/plain", "DELETE: Error");
            }
            else if (requestContext.Method.ToUpper().Equals("PUT"))
            {
                if (requestContext.endpoint.ToLower().Contains("/users/"))
                {
                    var username = requestContext.endpoint.Substring(7);
                    if (matchAuthToken(username))
                    {
                        return UpdateProfile(username);
                    }
                    else
                    {
                        return CreateResponse("400 Bad Request", "text/plain", "BAD TOKEN");
                    }
                }
                else
                {
                    return CreateResponse("400 Bad Request", "text/plain", "PUT: Error");
                }
            }
            else
            {
                return CreateResponse("400 Bad Request", "text/plain", "No HTTTP-Method given");
            }
        }

        private string UpdateProfile(string username)
        {
            User newUser = new User();
            try
            {
                newUser = JsonConvert.DeserializeObject<User>(requestContext.content);
            }
            catch (JsonSerializationException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "JSON-Error");
            }

            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            var cmd = new NpgsqlCommand("UPDATE users SET name = @name, bio = @bio, image = @image WHERE username= @username", conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("name", newUser.Name);
            cmd.Parameters.AddWithValue("bio", newUser.Bio);
            cmd.Parameters.AddWithValue("image", newUser.Image);
            cmd.Prepare();

            try
            {
                cmd.ExecuteNonQuery();
                return CreateResponse("200 OK", "text/plain", "Profile updated.");
            }
            catch (PostgresException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "Profile Update Error.");
            }
        }

        private string Register()
        {
            User newUser = new User();
            try
            {
                newUser = JsonConvert.DeserializeObject<User>(requestContext.content);
            }
            catch (JsonSerializationException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "JSON-Error");
            }
            string authTK = " Basic " + newUser.Username + "-sebToken";
            string startingELO = "100";
            string TestCount = "10";

            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            var cmd = new NpgsqlCommand("INSERT INTO users (username, password, authtoken, elo, totalcount) VALUES (@username, @password, @token, @selo, @tcount)", conn);
            cmd.Parameters.AddWithValue("username", newUser.Username);
            cmd.Parameters.AddWithValue("password", newUser.Password);
            cmd.Parameters.AddWithValue("token", authTK);
            cmd.Parameters.AddWithValue("selo", startingELO);
            cmd.Parameters.AddWithValue("tcount", TestCount);
            cmd.Prepare();

            try
            {
                cmd.ExecuteNonQuery();
                return CreateResponse("200 OK", "text/plain", "User registered.");
            }
            catch (PostgresException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "User is already registered.");
            }
        }

        public string Login()
        {
            User user = new User();
            try
            {
                user = JsonConvert.DeserializeObject<User>(requestContext.content);
            }
            catch (JsonSerializationException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "JSON-Error");
            }

            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            var cmd = new NpgsqlCommand("SELECT authtoken FROM users where username = @username and password = @password", conn);
            cmd.Parameters.AddWithValue("username", user.Username);
            cmd.Parameters.AddWithValue("password", user.Password);
            cmd.Prepare();

            try
            {
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    return (string)reader[0];
                }

                return null;
            }
            catch (PostgresException)
            {
                return null;
            }
        }

        public Boolean matchAuthToken()
        {
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            string token = requestContext.AuthorizationToken;

            var cmd = new NpgsqlCommand("SELECT username, authtoken FROM users where authtoken = @token", conn);
            cmd.Parameters.AddWithValue("token", token);
            cmd.Prepare();
            var user = new User();

            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    user.Username = (string)reader[0];
                    user.AuthToken = (string)reader[1];
                    if (user.AuthToken != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (PostgresException)
            {
                return false;
            }
        }

        public Boolean matchAuthToken(string username)
        {
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            string token = requestContext.AuthorizationToken;

            var cmd = new NpgsqlCommand("SELECT username, authtoken FROM users where username = @username", conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();
            var user = new User();

            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    user.Username = (string)reader[0];
                    user.AuthToken = (string)reader[1];
                    if (user.Username.Equals(username) && user.AuthToken.Equals(token))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (PostgresException)
            {
                return false;
            }
        }

        public string showProfileInfo(string username)
        {
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            string token = requestContext.AuthorizationToken;

            var cmd = new NpgsqlCommand("SELECT username, name, bio, image FROM users where username = @username", conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();
            var user = new User();

            try
            {
                var usertmp = new User();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usertmp.Username = (string)reader[0];
                    usertmp.Name = (reader[1] == DBNull.Value) ? string.Empty : (string)reader[1];
                    usertmp.Bio = (reader[2] == DBNull.Value) ? string.Empty : (string)reader[2];
                    usertmp.Image = (reader[3] == DBNull.Value) ? string.Empty : (string)reader[3];
                }
                return CreateResponse("200 OK", "application/json", 
                    JsonConvert.SerializeObject(usertmp, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }));

            }
            catch (PostgresException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "Profile Info Error");
            }
        }

        public string showStats(string username)
        {
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            string token = requestContext.AuthorizationToken;

            var cmd = new NpgsqlCommand("SELECT name, elo, totalcount FROM users where username = @username", conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();

            try
            {
                var usertmp = new User();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usertmp.Name = (reader[0] == DBNull.Value) ? string.Empty : (string)reader[0];
                    usertmp.ELO = (reader[1] == DBNull.Value) ? string.Empty : (string)reader[1];
                    usertmp.totalCount = (reader[2] == DBNull.Value) ? string.Empty : (string)reader[2];
                }
                return CreateResponse("200 OK", "application/json",
                    JsonConvert.SerializeObject(usertmp, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }));

            }
            catch (PostgresException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "Library Error");
            }
        }

        public string showScore()
        {
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            string token = requestContext.AuthorizationToken;

            var cmd = new NpgsqlCommand("SELECT name, elo, totalcount FROM users", conn);
            cmd.Prepare();

            try
            {
                List<User> userlisttmp = new List<User>();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var usertmp = new User();
                    usertmp.Name = (reader[0] == DBNull.Value) ? string.Empty : (string)reader[0];
                    usertmp.ELO = (reader[1] == DBNull.Value) ? string.Empty : (string)reader[1];
                    usertmp.totalCount = (reader[2] == DBNull.Value) ? string.Empty : (string)reader[2];
                    userlisttmp.Add(usertmp);
                }
                return CreateResponse("200 OK", "application/json",
                    JsonConvert.SerializeObject(userlisttmp, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }));

            }
            catch (PostgresException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "Library Error");
            }
        }
        public string showHistory(string username)
        {
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            var cmd = new NpgsqlCommand("SELECT starttime FROM tournament WHERE running = 'true'", conn);
            cmd.Prepare();

            var starttime = "";
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    starttime = (reader[0] == DBNull.Value) ? string.Empty : (string)reader[0];
                }
                reader.Close();
                var cmd2 = new NpgsqlCommand("SELECT username, count, duration FROM tournplayers WHERE tournid = (SELECT tournid FROM tournament WHERE running = 'true') AND username = @username", conn);
                cmd2.Parameters.AddWithValue("username", username);
                cmd2.Prepare();

                List<User> userlisttmp = new List<User>();
                var reader2 = cmd2.ExecuteReader();



                while (reader2.Read())
                {
                    var usertmp = new User();
                    usertmp.Name = (reader2[0] == DBNull.Value) ? string.Empty : (string)reader2[0];
                    usertmp.currentCount = (reader2[1] == DBNull.Value) ? string.Empty : (string)reader2[1];
                    usertmp.currentDuration = (reader2[2] == DBNull.Value) ? string.Empty : (string)reader2[2];
                    userlisttmp.Add(usertmp);
                }
                return CreateResponse("200 OK", "application/json", "Tournament is running! (Started: " + starttime + ") History:" +
                    JsonConvert.SerializeObject(userlisttmp, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }));

                //return CreateResponse("200 OK", "text/plain", "Tournament is running!");
            }
            else
            {
                return CreateResponse("200 OK", "text/plain", " NO Tournament is running!");
            }
        }

        public string showTournament()
        {
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            var cmd = new NpgsqlCommand("SELECT starttime FROM tournament WHERE running = 'true'", conn);
            cmd.Prepare();

            var starttime = "";
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    starttime = (reader[0] == DBNull.Value) ? string.Empty : (string)reader[0];
                }
                reader.Close();
                var cmd2 = new NpgsqlCommand("SELECT username, count, duration FROM tournplayers WHERE tournid = (SELECT tournid FROM tournament WHERE running = 'true')", conn);
                cmd2.Prepare();

                List<User> userlisttmp = new List<User>();
                var reader2 = cmd2.ExecuteReader();

             

                while (reader2.Read())
                {
                    var usertmp = new User();
                    usertmp.Name = (reader2[0] == DBNull.Value) ? string.Empty : (string)reader2[0];
                    usertmp.currentCount = (reader2[1] == DBNull.Value) ? string.Empty : (string)reader2[1];
                    usertmp.currentDuration = (reader2[2] == DBNull.Value) ? string.Empty : (string)reader2[2];
                    userlisttmp.Add(usertmp);
                }
                return CreateResponse("200 OK", "application/json", "Tournament is running! (Started: " + starttime + ")" +
                    JsonConvert.SerializeObject(userlisttmp, Formatting.Indented, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }));

               //return CreateResponse("200 OK", "text/plain", "Tournament is running!");
            }
            else
            {
                reader.Close();
                NpgsqlCommand command = new NpgsqlCommand("SELECT COUNT(*) FROM tournament WHERE running = 'false'", conn);

                // Execute the query and obtain the value of the first column of the first row
                Int64 sqlCount = (Int64)command.ExecuteScalar();
                return CreateResponse("200 OK", "text/plain", " NO Tournament is running! (" + sqlCount + " completed)");
            }
            

        }

        public void Tournament()
        {
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            var cmdInit = new NpgsqlCommand("SELECT * FROM tournament WHERE running = 'true'", conn);
            cmdInit.Prepare();


            var readerInit = cmdInit.ExecuteReader();
            if (readerInit.HasRows)
            {
                readerInit.Close();
            }

            else
            {
                readerInit.Close();
                var starttime = DateTime.Now.ToString();
                var cmd = new NpgsqlCommand("INSERT INTO tournament (starttime, running) VALUES( @start, 'true'); ", conn);
                cmd.Parameters.AddWithValue("start", starttime);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                Thread.Sleep(120000);

                var cmd2 = new NpgsqlCommand("SELECT username, count, duration FROM tournplayers WHERE tournid = (SELECT tournid FROM tournament WHERE running = 'true')", conn);
                cmd2.Prepare();


                string tmpCount;
                string tmpDur;
                var reader = cmd2.ExecuteReader();

                while (reader.Read())
                {
                    var usertmp = new User();
                    usertmp.Name = (reader[0] == DBNull.Value) ? string.Empty : (string)reader[0];
                    tmpCount = (reader[1] == DBNull.Value) ? string.Empty : (string)reader[1];
                    tmpDur = (reader[2] == DBNull.Value) ? string.Empty : (string)reader[2];
                    tourn.AddUser(new User(usertmp.Name), Int32.Parse(tmpCount), Int32.Parse(tmpDur));
                }

                reader.Close();


                tourn.handleTournament();
                Console.WriteLine(tourn.winner.Name);

                var currELO = 0;
                var currTotal = 0;

                var cmdEndSel = new NpgsqlCommand("SELECT elo, totalcount FROM users WHERE username = @username", conn);
                cmdEndSel.Parameters.AddWithValue("username", tourn.winner.Name);
                cmdEndSel.Prepare();


                var readerEndSel = cmdEndSel.ExecuteReader();
                if (readerEndSel.HasRows)
                {
                    while (readerEndSel.Read())
                    {
                        currELO = Int32.Parse((readerInit[0] == DBNull.Value) ? string.Empty : (string)readerInit[0]);
                        currTotal = Int32.Parse((readerInit[1] == DBNull.Value) ? string.Empty : (string)readerInit[1]);
                    }
                    readerEndSel.Close();

                    var cmdupd = new NpgsqlCommand("UPDATE users SET elo = @newelo WHERE username = @username", conn);
                    cmdupd.Parameters.AddWithValue("username", tourn.winner.Name);
                    cmdupd.Parameters.AddWithValue("newelo", (currELO + 2));
                    cmdupd.Prepare();

                    cmdupd.ExecuteNonQuery();

                    var cmdupd2 = new NpgsqlCommand("UPDATE users SET elo = @newelo WHERE username != @username", conn);
                    cmdupd2.Parameters.AddWithValue("username", tourn.winner.Name);
                    cmdupd2.Parameters.AddWithValue("newelo", (currELO - 1));
                    cmdupd2.Prepare();

                    cmdupd2.ExecuteNonQuery();

                    tourn.cleanTournament();



                    var cmd3 = new NpgsqlCommand("UPDATE tournament SET running = 'false' WHERE starttime = @start", conn);
                    cmd3.Parameters.AddWithValue("start", starttime);
                    cmd3.Prepare();

                    cmd3.ExecuteNonQuery();
                }

            }
        }

            public string Tournament(string username)
        {
            Thread.Sleep(300);
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            History histmp = new History();
            try
            {
                histmp = JsonConvert.DeserializeObject<History>(requestContext.content);
            }
            catch (JsonSerializationException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "JSON-Error");
            }

            var starttime = DateTime.Now.ToString();
            var currCount = 0;
            var currDur = 0;
            var currTotalCount = 0;

            var cmdUpdTotalSel = new NpgsqlCommand("SELECT totalcount FROM users WHERE username = @username", conn);
            cmdUpdTotalSel.Parameters.AddWithValue("username", username);
            cmdUpdTotalSel.Prepare();


            var readerUpdTotal = cmdUpdTotalSel.ExecuteReader();
                while (readerUpdTotal.Read())
                {
                    currTotalCount = Int32.Parse((readerUpdTotal[0] == DBNull.Value) ? string.Empty : (string)readerUpdTotal[0]);
                }

                readerUpdTotal.Close();

            var cmdupd = new NpgsqlCommand("UPDATE users SET totalcount = @newtotcount WHERE username = @username", conn);
            cmdupd.Parameters.AddWithValue("username", username);
            cmdupd.Parameters.AddWithValue("newtotcount", (currTotalCount + histmp.Count));
            cmdupd.Prepare();

            cmdupd.ExecuteNonQuery();

            var cmdInit = new NpgsqlCommand("SELECT count, duration FROM tournplayers WHERE tournid = (SELECT tournid FROM tournament WHERE running = 'true')" +
                "AND username = @username", conn);
                cmdInit.Parameters.AddWithValue("username", username);
                cmdInit.Prepare();


            var readerInit = cmdInit.ExecuteReader();
            if (readerInit.HasRows)
            {
                while (readerInit.Read())
                {
                    currCount = Int32.Parse((readerInit[0] == DBNull.Value) ? string.Empty : (string)readerInit[0]);
                    currDur = Int32.Parse((readerInit[1] == DBNull.Value) ? string.Empty : (string)readerInit[1]);
                }
                readerInit.Close();

                var cmdupd2 = new NpgsqlCommand("UPDATE tournplayers SET count = @newcount, duration = @newdur WHERE username = @username AND " +
                    "tournid = (SELECT tournid FROM tournament WHERE running = 'true')", conn);
                cmdupd2.Parameters.AddWithValue("username", username);
                cmdupd2.Parameters.AddWithValue("newcount", (histmp.Count + currCount));
                cmdupd2.Parameters.AddWithValue("newdur", (histmp.Duration + currDur));
                cmdupd2.Prepare();

                cmdupd2.ExecuteNonQuery();

            }
            else
            {
            readerInit.Close();
            var cmd = new NpgsqlCommand("INSERT INTO tournplayers (username, tournid, count, duration) " +
            "VALUES( @username, (SELECT tournid FROM tournament WHERE running = 'true'), @cnt, @duration)", conn);
            //+ "ON CONFLICT ON CONSTRAINT tournplayers_pkey " +
            //"DO UPDATE SET count = count + @cnt, duration = duration + @duration; ", conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("cnt", histmp.Count);
            cmd.Parameters.AddWithValue("duration", histmp.Duration);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            }

            return CreateResponse("200 OK", "text/plain", "Entry added.");
                
            
        }

        public string handleHandstand(string username)
        {
            Thread.Sleep(300);
            using var conn = new NpgsqlConnection(dbString);
            conn.Open();

            HandstandEntry htmp = new HandstandEntry();
            try
            {
                htmp = JsonConvert.DeserializeObject<HandstandEntry>(requestContext.content);
            }
            catch (JsonSerializationException)
            {
                return CreateResponse("400 Bad Request", "text/plain", "JSON-Error");
            }

            var starttime = DateTime.Now.ToString();
            var currELO = 0;

            var cmd = new NpgsqlCommand("SELECT elo FROM users WHERE username = @username", conn);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Prepare();


            var readerEndSel = cmd.ExecuteReader();
            if (readerEndSel.HasRows)
            {
                while (readerEndSel.Read())
                {
                    currELO = Int32.Parse((readerEndSel[0] == DBNull.Value) ? string.Empty : (string)readerEndSel[0]);
                }
                readerEndSel.Close();

                var cmdupd = new NpgsqlCommand("UPDATE users SET elo = @newelo WHERE username = @username", conn);
                cmdupd.Parameters.AddWithValue("username", username);
                cmdupd.Parameters.AddWithValue("newelo", (currELO + htmp.Count));
                cmdupd.Prepare();

                cmdupd.ExecuteNonQuery();

                cmd.ExecuteNonQuery();

            }

            return CreateResponse("200 OK", "text/plain", "Entry added.");


        }



    }
}
