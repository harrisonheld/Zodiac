# Zodiac - A High Fantasy Roguelike
This is my game, temporarily titled 'Zodiac'. Other potential names include:

Welcome to 'Zodiac,' a thrilling high fantasy roguelike game. Below are some potential names for the game that we're considering:

- Flaming Metal System
- You, But Starspawn
- Summer in Otherworld

# Image Gallery
## About the Project

### Data-driven
You can easily introduce new enemies without having to recompile or even restart the game. By adding the necessary entry to the Blueprints folder, the new enemy will be fully functional.

For instance, here's an entry for the "Enthralled Alchemist":

```json
{
    "Id": "EnthralledAlchemist",
    "Inherits": "Humanoid",
    "Components": [
      {
        "Type": "Visual",
        "Properties": {
          "DisplayName": "Enthralled Alchemist",
          "Description": "The fledgling alchemist toils with eyes that gleam like gold and hands that are never still.",
          "Sprite": "human3"
        }
      },
      {
        "Type": "EnergyHaver",
        "Properties": {
          "Quickness": 700
        }
      },
      {
        "Type": "Brain",
        "Properties": {
          "Ai": "Seeker"
        }
      },
      {
        "Type": "ItemSet",
        "Properties": {
          "ItemSetName": "HumanoidEquipment1"
        }
      }
    ]
}
```

### Weird and Wild

Any combination of components can be added to a creature. By adding the `Item` component to an enemy, it can be picked up and held in the inventory. Upon being dropped, the enemy will resume its usual activity. No additional code is required to support these bizzarre combinations.

## Image Gallery

All artwork currently in the game is either made or photographed by me. I am not an artist or a photographer - Enjoy.

![Sunbaked Canyons](https://github.com/harrisonheld/Zodiac/assets/24709296/12cb51a4-6f61-4625-b687-60fc9a980640)
![Voidbirds](https://github.com/harrisonheld/Zodiac/assets/24709296/6d7dafd5-b4ed-49b0-966c-1061c708ff14)
![Pisces' Study](https://github.com/harrisonheld/Zodiac/assets/24709296/68c72890-da63-4c60-8099-5bb223bf67f4)
![Talking to Pisces](https://github.com/harrisonheld/Zodiac/assets/24709296/ef3b67eb-1f0c-4532-b987-40ca32e7f1d6)
