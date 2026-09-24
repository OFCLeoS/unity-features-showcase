# Penetration System
---------
## Overview
- The penetration system will work by using the tag of the object that was hit.
- When a bullet hits a collider, it will first get its tag string and use it on a dictionary get an enum that matches the tag. This enum will represent a material.
- The enum is then used to get the material's penetration resistance by, once again, using a dictionary.
- The bullets penetration potential is then subtracted by the material's penetration resistance. If the bullet's penetration potential is less or equals to 0, the bullet will stop at that material, else it will continue and do the same thing for the next collider it impacts.
- All Bullet ScriptableObject instances will have their own custom dictionary that assigns VFX prefabs and sounds to a material enum, so that each material has different impact sounds and VFXs.