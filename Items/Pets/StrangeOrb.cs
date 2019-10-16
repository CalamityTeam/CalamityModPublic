using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class StrangeOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Strange Orb");
            Tooltip.SetDefault("Summons a young Siren light pet\n" +
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
