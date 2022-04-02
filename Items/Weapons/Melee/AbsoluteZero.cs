using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AbsoluteZero : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Absolute Zero");
            Tooltip.SetDefault("Ancient blade imbued with the Archmage of Ice's magic\nShoots dark ice crystals\nThe blade creates frost explosions on direct hits");
        }
        public override void SetDefaults()
        {
            item.damage = 120;
            item.melee = true;
            item.width = 58;
            item.height = 58;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = false;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DarkIceZero>();
            item.shootSpeed = 3f;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);

            if (crit)
                damage /= 2;

            int p = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<DarkIceZero>(), damage, knockBack * 3f, player.whoAmI);
            Main.projectile[p].Kill();
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 60);

            if (crit)
                damage /= 2;

            int p = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<DarkIceZero>(), damage, 12f, player.whoAmI);
            Main.projectile[p].Kill();
        }
    }
}
