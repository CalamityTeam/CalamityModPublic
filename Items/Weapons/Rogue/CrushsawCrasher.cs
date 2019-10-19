using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class CrushsawCrasher : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crushsaw Crasher");
            Tooltip.SetDefault("Throws bouncing axes");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = 65;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 22;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<Crushax>();
            item.shootSpeed = 11f;
            item.Calamity().rogue = true;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }
    }
}
