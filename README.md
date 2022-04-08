# OmniBackport

OmniBackport is a complete rewrite of the Act3Cards Inscryption mod that interacts with various new features for the game, as well as with other popular mods.

## Features

* Most Act 2/3 exclusive cards into Act 1 / Kaycee's Mod
* Several new starter decks for the different cost types
* Six new side decks
* Support for the Pack Manager mod
* Lots of configurability
* Fixes that allow non-act 1 sigils to work in act 1
* New nodes relating to the different cost types

## Planned features

* Mox-related consumable
* Support for modded Act 2/3 cards

## Nodes

### Gemify
Choose from any card of yours (that does not already have the Gemified sigil), and it will gain the Gemified sigil.
When a card with the Gemified sigil is played, if its owner controls a blue gem its owner will draw from their side deck.
If its owner controls a green gem, all cards with the Gemified sigil gain +1 health.
If its owner controls an orange gem, all cards with the Gemified sigil gain +1 power.

### Overclock
Choose from any card of yours (that does not already have the Overclock sigil), and it will gain +2 attack and the Overclock sigil.
Cards with the Overclock sigil will be removed from your deck upon perishing.

## Bug reporting/feature requests
To report a bug/request a feature, visit https://github.com/TeamDoodz/OmniBackport and go to the Issues tab.

## Credits

Icon for the Overclock node provided by [Freepik - Flaticon](https://www.flaticon.com/free-icons/energy). <br/>
Thanks to Aaron from the Inscryption Modding Discord for helping with some Harmony issues.

## Changelog

## 0.2.2
- Fixed an error that would occur if a gemified card tried to draw from the side deck when the side deck was empty.
- Sort-of fixed the Looter sigil; it animates weirdly now but it will actually update the card piles. It will also now draw from the side deck if the main deck is empty.
- The Gift Bearer sigil can now give you any card added by OmniBackport. This is configurable.

## 0.2.1
- Dialogue for the Overclock and Gemify nodes will no longer repeat after entering the node more than once.
- Slightly improved the visuals of the Overclock and Gemify nodes.
- Fixed an issue where the fire animation during the Campfire node would not appear.
- Fixed an issue with the Mox side decks.
- Fixed the Bloody Gems challenge not changing the background texture of gems cards to the normal texture.
- View is no longer locked during Overclock and Gemify nodes.

## 0.2.0
- Added the Overclock node.
- Added the Gemification node.
- Added a new secret mechanic. (Hint: It has something to do with the Bone Lord...)
- Added 4 new Mox-related side decks.
- Added the Bloody Gems challenge.
- Cards found in choice nodes now have a small chance of being overclocked/gemified.
- Mox cards can now be found in choice nodes/trader nodes if the wizard pack is enabled.
- Changed the default point cost for the Skeleton from 0 to -5.
- Changed the Blue Mage in the Blue Mox deck to a Hover Mage.
- Fixed the Mental Gemnastics sigil bringing you to the wrong view and not updating the card pile.
- Sigils that target gems will now target any card with a gem sigil.

### 0.1.0
- Initial release