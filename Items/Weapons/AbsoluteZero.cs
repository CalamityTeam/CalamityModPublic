using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
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
            item.damage = 42;
            item.melee = true;
            item.width = 58;
            item.height = 58;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTurn = false;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DarkIceZero>();
            item.shootSpeed = 3f;
        }
        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 600);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 300);

            int p = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<DarkIceZero>(), damage, knockBack * 3f, player.whoAmI);
            Main.projectile[p].Kill();
        }
    }
}
