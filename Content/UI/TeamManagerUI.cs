using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monstermon.Content.Items;
using Monstermon.Content.Types;
using ReLogic.Content;
using ReLogic.Graphics;
using SteelSeries.GameSense;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Monstermon.Content.UI.TeamManager
{
    // ExampleUIs visibility is toggled by typing "/coins" in chat (See CoinCommand.cs)
    // ExampleCoinsUI is a simple UI example showing how to use UIPanel, UIImageButton, and even a custom UIElement
    // For more info about UI you can check https://github.com/tModLoader/tModLoader/wiki/Basic-UI-Element and https://github.com/tModLoader/tModLoader/wiki/Advanced-guide-to-custom-UI 
    internal class TeamManagerUIState : UIState
    {
        public UIPanel teamManagerPanel;

        private UITeamSlots teamSlots;

        // In OnInitialize, we place various UIElements onto our UIState (this class).
        // UIState classes have width and height equal to the full screen, because of this, usually we first define a UIElement that will act as the container for our UI.
        // We then place various other UIElement onto that container UIElement positioned relative to the container UIElement.
        public override void OnInitialize()
        {
            // Here we define our container UIElement. In DraggableUIPanels, you can see that DraggableUIPanel is a UIPanel with a couple added features.
            teamManagerPanel = new TeamManagerUIPanel();
            teamManagerPanel.SetPadding(0);
            // We need to place this UIElement in relation to its Parent. Later we will be calling `base.Append(teamManagerPanel);`. 
            // This means that this class, ExampleCoinsUI, will be our Parent. Since ExampleCoinsUI is a UIState, the Left and Top are relative to the top left of the screen.
            // SetRectangle method help us to set the position and size of UIElement
            SetRectangle(teamManagerPanel, left: 600f, top: 22f, width: 190f, height: 130f);
            teamManagerPanel.BackgroundColor = new Color(73, 94, 171);

            UIText displayText = new UIText(Language.GetText("Mods.Monstermon.UI.TeamManager.TeamDisplayText"));
            displayText.HAlign = 0.5f;
            displayText.Top.Set(10f, 0);
            teamManagerPanel.Append(displayText);

            teamSlots = new UITeamSlots(Main.LocalPlayer.GetModPlayer<Trainer>().team);
            teamSlots.Top.Set(30f, 0f);
            //teamSlots.Left.Set(20f, 0f);
            teamSlots.HAlign = 0.5f;
            teamManagerPanel.Append(teamSlots);

            Append(teamManagerPanel);
        }

        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }
    }

    public class UITeamSlots : UIElement
    {
        private MonsterTeam team;
        private float _slotMarginSize;
        private float _slotInnerSize;

        public UITeamSlots(MonsterTeam team)
        {
            _slotInnerSize = TextureAssets.InventoryBack9.Size().X; //should be 48f according to terraria source code
            _slotMarginSize = 56f - _slotInnerSize; // from Terraria source code (Terraria.Main.DrawInventory)
            Width = new StyleDimension((MonsterTeam.TEAMSIZE - 1) * _slotMarginSize + MonsterTeam.TEAMSIZE * _slotInnerSize, 0f);
            Height = new StyleDimension(48f, 0f);
            this.team = team;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float _previousScale = Main.inventoryScale;
            Main.inventoryScale = 0.85f; // from Terraria code (Terraria.Main.DrawInventory)

            HandleSlotLogic();

            Vector2 position = GetDimensions().Position();
            position.X += (3 - team.EffectiveSize()) * (_slotMarginSize / 2f);

            Vector2 scale = Vector2.One * Main.inventoryScale;
            for (int i = 0; i < MonsterTeam.TEAMSIZE; i++)
            {
                switch (team.teamType)
                {
                    case TeamType.OneTwo:
                        scale.X = (i == 0 ? 1f : 2f) * Main.inventoryScale;
                        break;
                    case TeamType.TwoOne:
                        scale.X = (i == 0 ? 2f : 1f) * Main.inventoryScale;
                        break;
                    case TeamType.Three:
                        scale.X = 3f * Main.inventoryScale;
                        break;

                }
                spriteBatch.Draw(TextureAssets.InventoryBack9.Value, position, null, Main.inventoryBack, 0f, default, scale, SpriteEffects.None, 0f);
                if (!team.IsValidSlot(i))
                {
                    spriteBatch.Draw(TextureAssets.MapDeath.Value, position, null, Color.White, 0f, default, scale, SpriteEffects.None, 0f);
                }
                else if (team.slots[i].type != ItemID.None)
                {
                    ItemSlot.DrawItemIcon(team.slots[i], 0, spriteBatch, position + scale * (_slotInnerSize / 2f), Main.inventoryScale, 32f, Color.White);
                }

                position.X += scale.X * (_slotInnerSize) + _slotMarginSize * Main.inventoryScale;
                if ((i == 1 && team.teamType is TeamType.OneTwo) || (i == 0 && team.teamType is TeamType.TwoOne))
                    i++;
                if (team.teamType is TeamType.Three)
                    i += 2;
            }
            Main.inventoryScale = _previousScale;
        }
        private void HandleSlotLogic()
        {
            if (base.IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                LeftClick();
            }
        }

        private void LeftClick()
        {
            if (!(Main.mouseLeftRelease && Main.mouseLeft))
                return;
            if (HoveredSlot() is not int slot)
                return;

            bool canDropItem = Main.mouseItem.ModItem is CapturedMonster && team.IsValidSlot(slot);
            bool canTakeItem = !team.slots[slot].IsAir;

            if (!canDropItem && !canTakeItem)
                return;

            Utils.Swap(ref team.slots[slot], ref Main.mouseItem);
        }

        private int? HoveredSlot()
        {
            if (!base.IsMouseHovering)
                return null;

            Vector2 position = GetDimensions().Position();
            position.X += (3 - team.EffectiveSize()) * (_slotMarginSize / 2f);
            Vector2 m = Main.MouseScreen;

            if (m.Y < position.Y || m.Y > position.Y + GetDimensions().Height * Main.inventoryScale || m.X < position.X || m.X > position.X + GetDimensions().Width * Main.inventoryScale)
                return null;


            for (int i = 0; i < MonsterTeam.TEAMSIZE; i++)
            {
                float slotWidth;
                switch (team.teamType)
                {
                    case TeamType.OneTwo:
                        slotWidth = (i == 0 ? 1f : 2f);
                        break;
                    case TeamType.TwoOne:
                        slotWidth = (i == 0 ? 2f : 1f);
                        break;
                    case TeamType.Three:
                        slotWidth = 3f;
                        break;
                    default:
                        slotWidth = 1f;
                        break;

                }
                slotWidth *= Main.inventoryScale * _slotInnerSize;

                if (m.X - position.X < slotWidth)
                {
                    return i;
                }

                position.X += slotWidth + _slotMarginSize * Main.inventoryScale;
                if ((i == 1 && team.teamType is TeamType.OneTwo) || (i == 0 && team.teamType is TeamType.TwoOne))
                    i++;
                if (team.teamType is TeamType.Three)
                    i += 2;
            }

            return null;
        }
    }
}