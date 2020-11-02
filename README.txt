COMP 3973 Assignment 1
URLs:
  App: https://a00996409-comp3793-asn1.azurewebsites.net/
  Github: https://github.com/SeijiStory/COMP3793-Asn1

Team Member(s):
Seiji Story, A00996409, story.seiji@protonmail.com

What You Have Not Completed: N/A
  Checklist:
  [X] The assignment consists of two views [...]
  [X] When the user clicks on a book [...]
  [X] The names of students in the team should display on the home page
  [X] Only authenticated users can access the book and book details views
  [X] [...] seed the appropriate identity framework database tables with
      the following user: [...]
  [X] Users do not see menu items that they do not have access to unless
      they are authenticated
  [X] Deploy your application to Azure

Any Major Challenges:
  The biggest challenge was probably figuring out how to serialize the
  JSON object into a book object; the JSON layout does not exacly fit
  the model of the Book model, so I had to write a custom function to
  serialize the Book models from JSON. A secondary challenge was dealing
  with unexpected inputs; I had assumed that ISBN_10 would always come
  as a string that was convertable to int, so when the application ran
  into an ISBN with an X in it, it crashed, so I had to actually log the
  output and explore the API to realize that the ISBNs could contain 
  non-integer values.

Any Special Instructions For Testing Your App: N/A
