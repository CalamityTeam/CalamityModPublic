using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class Ectoheart : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ectoheart");
            Tooltip.SetDefault("Permanently increases Adrenaline Mode damage by 15% and damage reduction by 5%\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 44;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item122;
            item.consumable = true;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player) => !player.Calamity().adrenalineBoostThree;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/PermanentBoosters/Ectoheart_Animated");
            spriteBatch.Draw(texture, position, item.GetCurrentFrame(ref frame, ref frameCounter, 8, 3), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/PermanentBoosters/Ectoheart_Animated");
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 8, 3), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/PermanentBoosters/EctoheartGlow");
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 8, 3, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = item.useTime;
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.adrenalineBoostThree = true;
            }
            return true;
        }
    }
}
