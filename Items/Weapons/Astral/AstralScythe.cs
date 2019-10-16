using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class AstralScythe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Scythe");
            Tooltip.SetDefault("Shoots a scythe ring that accelerates over time");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 60;
            item.damage = 95;
            item.melee = true;
            item.useTurn = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item71;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<AstralScytheProjectile>();
            item.shootSpeed = 5f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }
    }
}
