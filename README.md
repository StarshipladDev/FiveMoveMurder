# FiveMoveMurderFest

[Screenshot]:ScreenShot.PNG
![This is Alt text][Screenshot]
This app is a port of a homebrew RPG system (http://www.starshiplad.com/FMMF.pdf) .
It is primarily combat-based, however I plan for campaign managment tools to be added.


# Features(Planned)

*Players declare a move/action for a character. Other side declares counter action, to which non-moved friendlies may again counter-move*

# Features(Planned)

*Basic A.I for enemy units*

*All actioning characters roll inititave then perform their actions in initave order ,regardless of whether it can be done by then*
*(E.G shooting at unit 'x' when unit x already shot by unit 'y' in same turn)*

# Next Update

Week Ending 24/11/19

# Next Build

Week Ending 24/11/19

* Add MainForm_Load to UML
* Add sortMoves to UML
* Add team selector in starshiplad.com/upload.php
* Bugtest
* Merge Advanced AI and Master
* Add basic instructions to README

# Skill developing

I plan on this project improving my skills in the following:

>General C#, specifficaly objects, GUI and mouse interactions

>General SQL and database managment

>SQL interaction via client-side programs

>QA and 'client'(I.E my friends) feedback and improvemnt

>Further documentation improvement(This markdown file being an example)

# Installing and Compiling:

All files other than FMMF.zip are for development purposes to look through my code.
To run the program, download FMMF.zip, uncompress it and run FiveMoveMurderFest.exe 

The SQL connection will not work in the development code as I have removed my SQL user data
If you have your own SQL database to connect to using mySQL with the correct file names, it will work fine.

**However** , you might be able to get these details out of the release build.I don't know.
If you don't trust the DLLs that I used in the release build, you can happily delete them, however SQL units will not work.


All images should be saved in the folder 'Pictures'.
All audio should be saved in the folder 'Audio'
