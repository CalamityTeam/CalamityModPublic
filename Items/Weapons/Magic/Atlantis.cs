using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using CalamityMod.Particles;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Summon;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Atlantis : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 81;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/VividClarityBeamAppear") with { Volume = 0.5f, Pitch = 0.2f };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AtlantisSpear>();
            Item.shootSpeed = 40f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int points = 6;
            float radians = MathHelper.TwoPi / points;
            Vector2 spinningPoint = Vector2.Normalize(velocity);
            for (int k = 0; k < points; k++)
            {
                Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                Particle spark = new GlowSparkParticle((position + velocity2 * 7.5f) + velocity * 2.5f, velocity2 * 0.5f, false, 11, 0.01f, Color.LightBlue * 0.9f, new Vector2(3.5f, 1), true, false);
                GeneralParticleHandler.SpawnParticle(spark);
                for (int b = 0; b < 3; b++)
                {
                    Dust dust = Dust.NewDustPerfect(position + velocity * 2.5f, DustID.FireworksRGB, (velocity2 * 10).RotatedByRandom(0.5) * Main.rand.NextFloat(0.5f, 0.9f));
                    dust.scale = Main.rand.NextFloat(0.3f, 0.5f);
                    dust.color = Main.rand.NextBool() ? Color.Cyan : Color.LightBlue;
                    dust.noGravity = true;
                }
            }
            Particle blastRing = new CustomPulse(position + velocity * 2.5f, Vector2.Zero, Color.CornflowerBlue, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1.25f, 0f, 11, true);
            GeneralParticleHandler.SpawnParticle(blastRing);
            Particle blastRing2 = new CustomPulse(position + velocity * 2.5f, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.6f, 0.1f, 14, true);
            GeneralParticleHandler.SpawnParticle(blastRing2);

            Projectile.NewProjectile(source, position + velocity * 2, velocity, type, damage, knockback, player.whoAmI, 0f, 0f, 5f);
            return false;
        }
    }
}
