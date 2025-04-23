using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Monstermon.Content.UI.TeamManager
{
    [Autoload(Side = ModSide.Client)] // This attribute makes this class only load on a particular side. Naturally this makes sense here since UI should only be a thing clientside. Be wary though that accessing this class serverside will error
    public class TeamManagerUISystem : ModSystem
    {
        private UserInterface playerTeamInterface;
        internal TeamManagerUIState playerTeamUI;

        // These two methods will set the state of our custom UI, causing it to show or hide
        public void InitializeUI()
        {
            // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
            playerTeamUI.Activate();
            playerTeamInterface?.SetState(playerTeamUI);
        }

        public override void Load()
        {
            // Create custom interface which can swap between different UIStates
            playerTeamInterface = new UserInterface();
            // Creating custom UIState
            playerTeamUI = new TeamManagerUIState();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // Here we call .Update on our custom UI and propagate it to its state and underlying elements
            if (playerTeamInterface?.CurrentState != null)
            {
                playerTeamInterface?.Update(gameTime);
            }
        }

        // Adding a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
        // Setting the InterfaceScaleType to UI for appropriate UI scaling
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "MonsterAlchemy: Team",
                    delegate
                    {
                        if (playerTeamInterface?.CurrentState != null && Main.playerInventory)
                        {
                            playerTeamInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}