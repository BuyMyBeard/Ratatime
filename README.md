# Ratatime
2023 Gamedev.js jam submission and 1st place finish for the Open Source category

[Play here!](https://buymybeard.itch.io/ratatime)

![Thumbnail](https://user-images.githubusercontent.com/95039323/236713469-a8f10a2c-86d4-4a52-8e91-7e74a32df7db.png)


This Game jam was my first experience managing a team, since I managed to get 4 other people on board, those being:
- **Rebecca Harrie**, Artist
- **Seth Barrere**, Programmer
- **Connor Hoss**, Level Designer
- **Andre Roz**, Audio Engineer

I give my utmost gratitude to this wonderful team! Without them this project wouldn't have been possible.

Since We had an artist and an audio engineer that wanted to keep their intellectual property over what they produced, we figured the best way to do it was
to use a Submodule containing all the private files, which was a challenge to manage, but ultimately ended up working as intended.

## Implementation Challenge
For this project, a big implementation challenge for me was the player physics. I decided to build them from the ground up, and when I realized how monumental 
the task was, I gavitated towards Unity's Rigidbody. I decided to handle jump mechanics myself, and made my own system for handling slopes, which
came with it's own set of challenges. Ultimately, I am pretty happy about the physics system, giving my character ragdoll physics when getting stunned, while
making jumping as forgiving as possible.

This project also made me touch a lot of features of Unity, like Cinemachine, Unity Tilemaps, Animator, Raycasts, Rigidbody physics and importing/setting up 2D
pixel art assets for the game.

## Source Version Control
The use of Unity YAML Merge Tool made handling merge conflicts easier, but I think YAML files support is lacking for Git. It is incredibly easy to cause
a merge conflict while working on the same scene, and lose a lot of progress. It's not always possible to divide a scene into multiple, since it makes the Level
Designer's job a lot harder. I have tried alternatives to Git in the past, like PlasticSCM, but overall didn't enjoy the experience and didn't feel like it did
much more than Git except for the Unity integration. 

Also, we had a lot of issues with the submodule and Github Desktop. I think this app should have better support for submodules and make linking a submodule and
main repository a lot more intuitive.

Anyhow, enjoy browsing our code implementation for the game!
