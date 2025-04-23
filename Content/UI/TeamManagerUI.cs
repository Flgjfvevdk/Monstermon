using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Monstermon.Content.UI.TeamManager
{
    // ExampleUIs visibility is toggled by typing "/coins" in chat (See CoinCommand.cs)
    // ExampleCoinsUI is a simple UI example showing how to use UIPanel, UIImageButton, and even a custom UIElement
    // For more info about UI you can check https://github.com/tModLoader/tModLoader/wiki/Basic-UI-Element and https://github.com/tModLoader/tModLoader/wiki/Advanced-guide-to-custom-UI 
    internal class TeamManagerUIState : UIState
    {
        public UIPanel teamManagerPanel;
        public UIItemSlot slot_ui;

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

            // By properly nesting UIElements, we can position things relatively to each other easily.
            for (int i = 0; i < 3; i++)
            {
                slot_ui = new(Main.LocalPlayer.GetModPlayer<Trainer>().team.slots, i, ItemSlot.Context.InventoryItem);
                SetRectangle(slot_ui, left: 10f + i * 60f, top: 50f, width: 50f, height: 50f);
                teamManagerPanel.Append(slot_ui);
            }

            Append(teamManagerPanel);
            // As a recap, ExampleCoinsUI is a UIState, meaning it covers the whole screen. We attach teamManagerPanel to ExampleCoinsUI some distance from the top left corner.
            // We then place playButton, closeButton, and MoneyDisplay onto teamManagerPanel so we can easily place these UIElements relative to teamManagerPanel.
            // Since teamManagerPanel will move, this proper organization will move playButton, closeButton, and MoneyDisplay properly when teamManagerPanel moves.
        }

        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        public void UpdateValue(int pickedUp)
        {

        }
    }

    // public class Team : UIElement
    // {
    //     public UITeamSlot()
    //     {

    //     }

    //     /*protected override void DrawSelf(SpriteBatch spriteBatch)
    //     {
    //         CalculatedStyle innerDimensions = GetInnerDimensions();
    //         // Getting top left position of this UIElement
    //         float shopx = innerDimensions.X;
    //         float shopy = innerDimensions.Y;

    //         // Drawing first line of coins (current collected coins)
    //         // CoinsSplit converts the number of copper coins into an array of all types of coins
    //         DrawCoins(spriteBatch, shopx, shopy, Utils.CoinsSplit(collectedCoins));

    //         // Drawing second line of coins (coins per minute) and text "CPM"
    //         DrawCoins(spriteBatch, shopx, shopy, Utils.CoinsSplit(GetCoinsPerMinute()), 0, 25);
    //         Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, "CPM", shopx + (float)(24 * 4), shopy + 25f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
    //     }*/
    // }
}