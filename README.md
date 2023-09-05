# Zodiac - A High Fantasy Roguelike
This is my game, temporarily titled 'Zodiac'. Other potential names include:

- Flaming Metal System
- You, But Starspawn
- Summer in Otherworld
- The Stars Came Down Screaming

## About the Project

### Data-driven
You can easily introduce new enemies without having to recompile or even restart the game. By adding the necessary entry to the Blueprints folder, the new enemy will be fully functional.

For instance, here's an entry for the "enthralled alchemist":

```json
{
    "Id": "EnthralledAlchemist",
    "Inherits": "Humanoid",
    "Components": [
      {
        "Type": "Visual",
        "Properties": {
          "DisplayName": "enthralled alchemist",
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
![Halls of Gray](https://github.com/harrisonheld/Zodiac/assets/24709296/d70f2684-7f7f-4fb5-9342-07affe986b7c)
![Voidbirds](https://github.com/harrisonheld/Zodiac/assets/24709296/6d7dafd5-b4ed-49b0-966c-1061c708ff14)
![Pisces' Study](https://github.com/harrisonheld/Zodiac/assets/24709296/2b16bb1f-bc94-4337-91cf-17c2f497102b)
![Talking to Pisces](https://github.com/harrisonheld/Zodiac/assets/24709296/8b7da60b-36f0-4e8f-8d77-a8e8f1e11a42)
![Quest Log](https://github.com/harrisonheld/Zodiac/assets/24709296/82cf3917-b168-4432-944d-046515478413)
