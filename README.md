--- technical steps 

-- Users
Have attributes required to store data about themselves and some required to do tournaments

--History
I understood the history as the current count and duration regarding a tournament, which is in progress. (Since there would have been some redundancy in the /history, /score and /stats endpoints)
The ELO and total count of pushups is rather saved in the User itself.

--Tournaments
The tournaments themselves are also rather simple. A list of Users enter the tournament (with an initial amount and duration) and can then add push-ups while it's running.
I had some trouble getting a draw with multiple "drawees" to work (also during endpoint-handling), so because of time-constraints I omitted draws.

--HTTPREST-Server
I had to search for a while until I found some demos which I understood and as I later explain I would have probably liked to do the Server some other way.

The Server starts with 3 listeners and each HTTP-Message which is received creates a RequestContext (which splits and categorizes the message) 
and a ResponseContext (which sends the appropriate responses back to the client after dealing with them)

Depending on the requested endpoint and (if necessary) authentication via Token, a corresponding method is executed, which handles reading and writing from/to the database and creates an appropriate response.

The tournaments are handled in a seperate Thread and the handling of everything pertaining to the running/not running of them is a lot more DB-heavy than I'd like.

Since for every HTTP-message new contexts are created, I noticed rather late unfortunately that I had trouble getting variables to stick (when wrote to) outside of the Thread.
I read about "volatile" variables but unfortunately I would have had to re design the Server for that to work.


---Tests

The tests are mostly held simple, they test the general execution of the tournaments and adding push ups
and some Server-functions.

---Time spent
Design 5hrs
Logic 5hrs
HTTPREST-Server 20 hrs
Database-Desing/-Operations inside the program 15 hrs
Troubleshooting 15hrs
