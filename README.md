<h1>Documentation</h1>

<b>Website:</b> https://10fastfingers.com/typing-test/portuguese

<h3>Goals:</h3>

This automation was developed for the RPA challenge on the 10fastfingers website, in which we will take each word from the table, and insert it in the bar below, leaving a space, within 1 minute. Afterwards, collect the information.

<h3>Challenge/Scope</h3>

![Challenge](https://github.com/JoaoCarlosAp/Desafio10FastGingersRPA/assets/48108997/2a395379-89ba-4db3-bb50-2f77687f8763)
![Scope](https://github.com/JoaoCarlosAp/Desafio10FastGingersRPA/assets/48108997/5b1d13c6-fc87-42ac-b6b3-192cf48d180b)

<h3>Dependencies:</h3>

- EEPLUS: library to manage spreadsheets.
- Selenium: library for browsing the website.

<h3>Comments:</h3>

- Difficulty reaching the screen with the words, if restsharp is done.
- In selenium, if you leave the screen hidden, it is not possible to retrieve cookies.
- A spreadsheet was used to save the information, to better facilitate the test.

<h3>Improvements:</h3>

- Use migration to save a database.
- Use a cloud database.
- Create method that checks and updates ChromiDriver alone.
