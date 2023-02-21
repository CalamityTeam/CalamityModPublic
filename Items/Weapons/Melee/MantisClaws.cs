using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MantisClaws : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mantis Claws");
            Tooltip.SetDefault("Explodes on enemy hits");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.damage = 88;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 8;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);

            //does no damage. Explosion is visual
            Projectile.NewProjectile(source, target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), 0, 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            target.AddBuff(BuffID.OnFire3, 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetSource_ItemUse(Item);

            //does no damage. Explosion is visual
            Projectile.NewProjectile(source, target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<FuckYou>(), 0, 0f, player.whoAmI, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            target.AddBuff(BuffID.OnFire3, 300);
        }
    }
}
