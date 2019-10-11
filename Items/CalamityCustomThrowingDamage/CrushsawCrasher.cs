using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.CalamityCustomThrowingDamage
{
    public class CrushsawCrasher : CalamityDamageItem
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
            item.shoot = mod.ProjectileType("Crushax");
            item.shootSpeed = 11f;
            item.Calamity().rogue = true;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("BrimstoneFlames"), 300);
        }
    }
}
