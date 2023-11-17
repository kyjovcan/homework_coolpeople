	Hello there, i would like to first apologize for taking longer to send the results, at first i thought this should be some problem to see IF i can code, but after i started to work on this i found out this is just to see HOW i can code, immediately after opening the codes i started to notice a lot of small problems, details, bugs that didnt make sense or were a bit pain to look at, here is just main parts i improved, changed or added. 
	
	I used Visual Studio 2022, project is just running on IIS Express, plus i added a simple React app which i enjoyed working on, i guess you will have React installed (if not, install node, npm, npm install) and in the client app folder just "npm start" to get it running on localhost:3000, backend is running automaticaly with swagger. For tests, just open Test Explorer and run them, i created just the most crucial ones for Job controller, as you mentioned not to create everything, i focused just on main methods. This all took me probably ~10-12 hours to make, 2 evenings after work and few hours today, hope it is enough for a quick show, we can talk about further upgrades on the call. 

	Quick list of updates: 
- changed the architecture a bit, moved functionality to separate folders
- improved readability 
	- naming convention weird 
	- fix notation, upper/lowercase names 
	- general code cleanup, formatting
- improved extendability - simple changes like switching from if...else to switch of file types - easier to maintain and extend 
- added dependency injections for logging and context to simplify creation of them
- added cors, proper routing for http requests

- controllers mistakes / upgrades
	- switch from just returning "something" to returning Action Results instead - 201 Created / 404 not found / 500 bad request...
	- validate model (TranslatorModel) using FromBody attributes
	- change argument exceptions to action results - bad requests 
	- change "Single" function on top of DBresults to "SingleOrDefault", there might be null result
	- error and null handling
	- statutes.All provides the same functionality as statutes.where.... ==0
	- document load provides parsing we need already
	- redundant usage of strings instead of variables - changed
	- dont compare saveChanges > 0 to see if Save worked, catch exception
	- unreliable notification service proper handling
	- moved jobs and translators to own separate models, with constructors and basic functionality
	- namespace was mismatched - Controllers vs Controlers
	- didnt touch logging, but would simplify and update a bit 
	- searching for job in translators fixed
	- 
- added frontend - simple react app with one page, shows table with Jobs and Translators
	- added functionality to add new Job/Translator 
	- for Jobs added dropdown menu to choose the Translator, change status for the Job 
		- added to show work with States
		- also for additional point - only can assign Cetrified translators 
		- also updated Job model, so that every job has info about its translator assigned 
	- styled it just a bit with bootstrap, implemented just part of functionality as you wished
- didnt touch database, logging, security, auth, but that would be some next steps to work on, also another thing i miss is delete functionality for jobs and translators, i would add this as next step
- another next step - move data related functionality to Services folder, to have it separated from controllers, currently it is very easy - _context.SaveChanges() - but in the future those CRUD functions can be bigger, harder, would be better to have them separated from main logic
- added tests just for creating and updating of jobs, to show really simple testing scenarious for some of the action results from controllers, every test is passable with green tick for me
