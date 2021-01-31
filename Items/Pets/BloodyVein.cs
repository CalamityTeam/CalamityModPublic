using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
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
            item.damage = 0;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.width = 24;
            item.height = 48;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.UseSound = SoundID.NPCHit9;
            item.shoot = ModContent.ProjectileType<PerforaMini>();
            item.buffType = ModContent.BuffType<BloodBound>();
            item.rare = 3;
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}
