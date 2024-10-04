using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems.Invasion
{
    [LegacyName("MartianDistressBeacon")]
    public class MartianDistressRemote : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 17; // Solar Tablet (1 above Lihzahrd Power Cell)
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 52;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.UseSound = SoundID.Zombie67;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.EventItem;
        }

        public override bool CanUseItem(Player player)
        {
            // Do not start new invasion when it's already on going
            if (Main.invasionType != InvasionID.None)
                return false;

            // This is the requirement for StartInvasion for some reason
            // If this part is missing MartianRemote will show DD2 invasion GUI without any enemy spawn on MP
            if (!Main.player.Any(p => p.active && p.ConsumedLifeCrystals >= 5))
                return false;

            return true;
        }

        public override bool? UseItem(Player player)
        {
            // Single Player case
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.invasionDelay = 0;
                Main.StartInvasion(InvasionID.MartianMadness);
                return true;
            }

            // MP case: only owner should send Spawn Message to Master
            if (player.whoAmI == Main.myPlayer)
            {
                //-7.0 is hardcoded id for MartianMadness Event
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: -7.0f);
                return true;
            }

            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/SummonItems/Invasion/MartianDistressRemote_Animated").Value;
            spriteBatch.Draw(texture, position, Item.GetCurrentFrame(ref frame, ref frameCounter, 5, 12), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/SummonItems/Invasion/MartianDistressRemote_Animated").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 5, 12), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }
    }
}
