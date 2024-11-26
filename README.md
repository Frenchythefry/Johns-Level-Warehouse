# How to use:
## As a player:
> - Create a folder in your Mods directory (assuming you already installed MelonLoader, and put the LevelEditor.dll into the mods folder) called Levels
> - If you have a .zip, .7zip, .rar, etc. of a level, extract it into said Levels directory. If you double click on the folder in there, you should have three .txt files and a folder. If this isn't the case, and you have 1 folder in there, move that folder into Levels, and delete the extra one.
> - If you have a folder, just drop it into Levels!
> - Next, run the game, and click play. In world select you should see a new UI element that has an input field and a submit button. Enter the name of the level (the name of the folder containing the level) into this. Then hit submit! **ONLY HIT SUBMIT ONCE**
> - It will take a second to load. You might see rocketman and the moonlight district. This is good. (If you want details as to why, DM me)
> - If you followed these steps correctly, you'll see a level appear!
## As a creator:
> ### Here's the hard part. This will be complicated, but don't worry. Once I finish the mod fully, I plan to make a tool (like TasHelper) to make this much easier
> - Step 1: Create a folder and call it whatever you want. Its easier if the name is shorter, as you have to type it in.
> - Step 2: In this folder, create 4 .txt files. One should be called rules.txt, one should be called tilepos.txt, one should be called tileposGrapple.txt and one should be called other.txt. Also, make a folder called Palette.
> - Step 3: Take whatever images you want your level to be made of and put them in here. The tiles should be square, or have transparency to fill it in to make it one. Also, I would name them 1, 2, 3, etc. so the order is correct (hopefully).
> - Step 4: Open tilepos.txt. In here, you basically build your level. Each "block" is separated by a comma, so one line would be like '1, 1, 1, 0'. A zero represents an empty space. Any number above that represents a tile from your palette. **This is for Non-Grappleable Objects**
> - Step 5: Do the same thing you did for tilepos.txt for tileposGrapple.txt. **This is for Grappleable Objects**
> - Step 6: Once you finish putting your tiles in place, count the width and height of the text you put (so numbers across and lines). Then, put the width as the second line of rules.txt (**The second line. Not the first. Don't ask why.**), and the height as the third line. 
> - Step 7: Considering that the bottom left corner of tilepos.txt is at the position '0,0', the 4th and 5th lines of rules.txt should be the x and y of the player's start position.
> - Step 8: Next, open other.txt. Again considering that the bottom left corner of tilepos.txt is at the position '0,0', put 'win, x, y' with x as the x of the factory and y as the y of the factory.
> - Step 9: Lastly, put 'checkpoint, x, y' for every checkpoint (you can have as many as you want) with x as the x position of the checkpoint and y as the y position.
> - Now, follow the steps under as a player to try it out!!
