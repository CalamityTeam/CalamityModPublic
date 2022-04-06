using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class BloodyVein : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Vein");
            Tooltip.SetDefault("Summons an amalgamated pile of flesh");
        }

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.width = 24;
            Item.height = 48;
            Item.UseSound = SoundID.NPCHit9;
            Item.shoot = ModContent.ProjectileType<PerforaMini>();
            Item.buffType = ModContent.BuffType<BloodBound>();

            Item.value = Item.buyPrice(gold: 4);
            Item.rare = ItemRarityID.Orange;
            Item.Calamity().donorItem = true;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
}
