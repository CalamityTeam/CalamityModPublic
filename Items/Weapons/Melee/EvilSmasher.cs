using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EvilSmasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Smasher");
            Tooltip.SetDefault("EViL! sMaSH eVIl! SmAsh...ER!");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.damage = 55;
            item.melee = true;
            item.useAnimation = 30;
            item.useStyle = 1;
            item.useTime = 30;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FossilSpike>(), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
            if (Main.rand.NextBool(3))
            {
                item.damage = 82;
                item.useAnimation = 15;
                item.useTime = 15;
                item.knockBack = 14f;
            }
            else
            {
                item.damage = 55;
                item.useAnimation = 30;
                item.useTime = 30;
                item.knockBack = 8f;
            }
        }
    }
}
