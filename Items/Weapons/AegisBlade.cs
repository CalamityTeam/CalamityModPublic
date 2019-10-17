using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class AegisBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aegis Blade");
            Tooltip.SetDefault("Legendary Drop\n" +
                "Striking an enemy with the blade causes an earthen eruption\n" +
                "Right click to fire an aegis bolt\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.damage = 68;
            item.melee = true;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 4.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 58;
            item.rare = 7;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.shootSpeed = 14f;
            item.Calamity().postMoonLordRarity = 17;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noMelee = true;
                item.useTime = 20;
                item.useAnimation = 20;
                item.UseSound = SoundID.Item73;
                item.shoot = ModContent.ProjectileType<AegisBeam>();
            }
            else
            {
                item.noMelee = false;
                item.useTime = 15;
                item.useAnimation = 15;
                item.UseSound = SoundID.Item1;
                item.shoot = 0;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<AegisBeam>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246, 0f, 0f, 0, new Color(255, Main.DiscoG, 53));
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<AegisBlast>(), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
        }
    }
}
