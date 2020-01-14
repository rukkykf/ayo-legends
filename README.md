# README

The Ayo Legends game is a hybrid implementation of the popular Yoruba game Ayo. There are varying mechanics of play but the mechanics
used in this implementation are the mechanics used in Myriad Softwares Awale Shareware.

### What is this repository for?

The Ayo Legends game served as my senior project in college. To summarize essential parts of the project briefly:

There are two types of games in literature, games of progression and emergent games. Traditionally, the Ayo game
is an emergent game and this means that the game's challenge is derived primarily from playing according to a set of rules.
Other examples of emergent games are chess, checkers, monopoly.

Games of progression however present the game challenge to the player
in levels of difficulty that are usually driven by a story line. A good example of a game of progression is Candy Crush.
Hybrid games incorporate elements of emergence and progression and most modern games are hybrids like Devil May Cry, Assassin's Creed etc.
The goal of this project was to implement the Ayo game as a hybrid game by incorporating progression mechanics
that are facilitated by a storyline. Another feature that was added to the Ayo Legends game is the Multiplayer feature
that allows friends play the game on different devices when they are connected via a LAN or any network. The multiplayer features were implemented
using Unity's UNet system and at the moment, they're a bit rusty because I only tested with localhost, didn't host it on Unity Cloud, for now
but hopefully, soon enough that's where the project is going.

The story mode is currently working fine. The algorithm implemented for the AI in levels 2 and 3 are the minimax algorithm
with a depth of 1 and 2 respectively.

For more details on the project you can read the writeup I created for it here:
https://drive.google.com/open?id=0BycM7UgpPalmU2NGVmo1MnJuSHNKbk1GSExlOV91REpvaDlV

PS: The code has not been refactored yet so it will be annoying to get through, I tried my best to make naming
as good as possible but there's a lot of feature envy in some of the methods and a lot of the methods and classes are bloated.
However, as time goes on, I'm looking to refactor the code and create a set of reusable classes that can be used to implement
more Mancala type games in the future.

### How do I get set up?

so there are two folders:

1.) The setup folder which contains the setup file and should be able to install on any Windows PC

2.) The scripts folder which contains the scripts from the Unity project. The scripts can be viewed in any text editor

### Contribution guidelines

At the moment I can't make the main Unity project open source even though I'd like to because some art assets in the project (the hand and UI sprites, specifically)
were purchased from Unity's asset store and I don't have the rights to distribute those assets.

### Who do I talk to?

Talk to me, Rukky Kofi, just send an email to rukykf@gmail.com
