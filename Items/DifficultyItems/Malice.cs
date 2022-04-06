using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DifficultyItems
{
    public class Malice : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malice");
            Tooltip.SetDefault("Enables/disables Malice Mode, can only be used in Death Mode.\n" +

            // Overall description and warning lines
            "[c/c01818:This mode is subjective, play how you want, don't expect to live.]\n" +

            // Rev Mode line
            "All effects from Death Mode are enabled, including the following:\n" +

            // Misc lines
            "Nerfs the effectiveness of life steal a bit more.\n" +

            // Boss lines
            "Enrages all bosses and gives them far more aggressive AI.\n" +
            "Bosses and their projectiles deal 25% more damage.\n" +
            "Increases the velocity of most boss projectiles by 25%.");
        }

        public override void SetDefaults()
        {
            Item.expert = true;
            Item.rare = ItemRarityID.Purple;
            Item.width = 82;
            Item.height = 66;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item119;
            Item.consumable = false;
        }

        public override void UseStyle(Player player, Rectangle rectangle)
        {
            player.itemLocation += new Vector2(-32f * player.direction, player.gravDir).RotatedBy(player.itemRotation);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/DifficultyItems/Malice_Animated").Value;
            spriteBatch.Draw(texture, position, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 8), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/DifficultyItems/Malice_Animated").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 8), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        // Can only be used in Death Mode.
        public override bool CanUseItem(Player player) => CalamityWorld.death || CalamityWorld.malice;

        public override bool? UseItem(Player player)
        {
            // This world syncing code should only be run by one entity- the server, to prevent a race condition
            // with the packets.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            if (CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0 || BossRushEvent.BossRushActive)
            {
                string key = "Mods.CalamityMod.ChangingTheRules";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
                return true;
            }
            if (!CalamityWorld.malice)
            {
                CalamityWorld.malice = true;
                string key = "Mods.CalamityMod.MaliceText";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            else
            {
                CalamityWorld.malice = false;
                string key = "Mods.CalamityMod.MaliceText2";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            CalamityWorld.DoGSecondStageCountdown = 0;
            CalamityNetcode.SyncWorld();

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
