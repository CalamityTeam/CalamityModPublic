using CalamityMod.Buffs.Pets;
using CalamityMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Pets
{
    public class StrangeOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Strange Orb");
            Tooltip.SetDefault("Summons a miniature Ocean Spirit light pet\n" +
                "Provides a large amount of light while underwater");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.shoot = ModContent.ProjectileType<SirenYoung>();
            item.buffType = ModContent.BuffType<SirenLightPetBuff>();
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
