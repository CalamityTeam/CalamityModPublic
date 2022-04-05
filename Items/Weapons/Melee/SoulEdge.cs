using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SoulEdge : ModItem
    {
        static internal LegacySoundStyle ProjectileDeathSound = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Edge");
            Tooltip.SetDefault("Fires the ghastly souls of long-deceased abyss dwellers");

            ProjectileDeathSound = SoundID.NPCDeath39.WithVolume(0.5f);
        }

        public override void SetDefaults()
        {
            Item.width = 88;
            Item.height = 88;
            Item.damage = 174;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useTurn = true;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GhastlySoulLarge>();
            Item.shootSpeed = 12f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numShots = 2;
            for (int i = 0; i < numShots; ++i)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-40, 41) * 0.05f;
                float ai1 = Main.rand.NextFloat() + 0.5f;
                // TODO -- unchecked type addition math assumes we can guarantee load order
                // this is extremely unsafe and load order may become non deterministic in 1.4
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, Main.rand.Next(type, type + 3), damage, knockback, player.whoAmI, 0.0f, ai1);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
        }
    }
}
