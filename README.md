# ⚔️ An Exercise in Unity 3D | Breath of the Wild / Dark Souls Combat

## Overview

This project is a collective of what I learned during this Fall semester in Unity 3D, specifically in an attempt to implement the character and weapon interactions that I love from games like The Legend of Zelda: Breath of the Wild and Dark Souls.

Worth noting, the implementation of such features here is NOT production quality -- this was a learning experience, and I don't want to imply otherwise. I had a lot of learning to do during this semester, and a lot of what I've created here needs to be redone and refactored to actually be truly scalable. This includes actually creating class hierarchies for my code to make implementations more modular, as most of my code is monolithic at this point. This also includes making use of the Unity Events feature, which should also give way for more modular code, as my components would be able to interact with each other on only a need to know basis for example, without being hard coded within each other.

However, overall I did get plenty of practice in having to make architectural decisions, even if they weren't necessarily following the industry standard. I feel more ready now to explore in-depth all that Unity has to offer, and I plan on continuing to chase my magnum opus that is my dream combat game.

## 🧑‍💻 Process Video

[![Process video](https://drive.google.com/uc?id=1sNwqjpVzpn5H8xGkT0tbxJlOo5r6-vh6)](https://youtu.be/6YSYwRnvl6I)

  - Click image to go to video, or click here https://youtu.be/6YSYwRnvl6I

## ⚙️ Unity Version

- 2022.3.8.1f
- https://unity.com/releases/editor/archive

## ✨ Features

- Character State Management System
  - Character Animations
  - Character Weapon States
 
- NPC Awareness System
  - Modular Patrolling Points
  - Target Tracking / Aggressiveness

- Animation Based Combat w/ Real-time Weapon Collision
 
- Weapon System
  - Material-based Sounds / VFX
  - Enemy Interactions
  - Object Interactions
 
- Character Ragdoll System

- Character Health / Damage System w/ HUD
