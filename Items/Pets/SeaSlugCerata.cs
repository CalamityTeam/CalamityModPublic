using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class SeaSlugCerata : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Colorful Cerata");
            Tooltip.SetDefault("Summons a Bioluminescent Nudibranch to follow you"
            + "\nThe Nudibranch will change colors and patterns in different environments");
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.width = 28;
            Item.height = 22;

            Item.value = Item.sellPrice(gold: 20);
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<SeaSlug>();
            Item.buffType = ModContent.BuffType<SeaSlugBuff>();
            Item.UseSound = SoundID.Item2;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true);
            }
        }
    }
}
