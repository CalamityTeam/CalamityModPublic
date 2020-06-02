using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
	public class Ectoheart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ectoheart");
            Tooltip.SetDefault("Permanently makes Adrenaline Mode take 5 less seconds to charge\n" +
                "Revengeance drop");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 3));
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
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.adrenalineBoostThree)
            {
                return false;
            }
            return true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            CalamityUtils.DrawItemGlowmask(item, spriteBatch, 3, rotation, ModContent.GetTexture("CalamityMod/Items/PermanentBoosters/EctoheartGlow"));
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
