using CalamityMod.Buffs.StatBuffs;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class Affliction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Affliction");
            Tooltip.SetDefault("Gives you and all other players on your team +1 life regen,\n" +
                               "+10% max life, 7% damage reduction, 13 defense, and 10% increased damage");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 44;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.rare = ItemRarityID.Purple;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Accessories/Affliction"));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.affliction = true;
            if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
            {
                if (Main.LocalPlayer.team == player.team && player.team != 0)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<Afflicted>(), 20, true);
                }
            }
        }
    }
}
