using CalamityMod.Cooldowns;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PristineFury : LegendaryItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int frameCounter = 0;
        public int frame = 0;
        public bool Trail = true;
        public int shotCount = 0;

        public static int boomTime = 6;

        public override Color? TooltipExtensionColor => new Color(255, 140, 0);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 46;
            Item.damage = 60;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 3;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item34;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PristineFire>();
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25, -10);

        public override bool AltFunctionUse(Player player) => true;

        // Right click consumes ammo at the same rate but faster at spewing
        public override float UseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 0.5f : 1f;
        public override void HoldItem(Player player)
        {
            int max = CalamityPlayer.FuryFuelMax;
            if (player.Calamity().cooldowns.TryGetValue(FuryFuel.ID, out var cooldown))
            {
                cooldown.timeLeft = max - player.Calamity().furyFuel;
            }
            else
            {
                player.AddCooldown(FuryFuel.ID, max);
            }
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse != 2; // Right click doesn't use ammo, it's crystal powder not gel

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.Calamity().furyRefuelTimer = -50;
                if (player.Calamity().furyFuel > 0)
                {
                    Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(5f));
                    Projectile.NewProjectile(source, position, newVel, ModContent.ProjectileType<PristineSecondary>(), (int)(damage * 0.2f), knockback, player.whoAmI); //.2x base damage

                    Dust dust = Dust.NewDustPerfect(position + velocity * 3f + new Vector2(0, -3), ModContent.DustType<LightDust>(), velocity.RotatedBy(0.25f * player.direction).RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f, 2.5f), 0, default, Main.rand.NextFloat(0.4f, 0.8f));
                    dust.noGravity = true;
                    dust.color = Color.Orchid;

                    CritSpark spark = new CritSpark(position + velocity * 3f + new Vector2(0, -3), velocity.RotatedBy(0.25f * player.direction).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.2f, 1.8f), Color.White, Color.Orchid, 0.9f, 18, 2f, 2.2f);
                    GeneralParticleHandler.SpawnParticle(spark);
                    player.Calamity().furyFuel -= 15;
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity * 0.8f, type, damage, knockback, player.whoAmI, Trail ? 1 : 0, 0, shotCount);
                Trail = !Trail;
                for (int i = 0; i <= 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position + velocity * 3f + new Vector2(0, -3), DustID.OrangeTorch, velocity.RotatedBy(0.25f * player.direction).RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f, 2.5f), 0, default, Main.rand.NextFloat(1.6f, 2f));
                    dust.noGravity = true;
                }
                CritSpark spark = new CritSpark(position + velocity * 3f + new Vector2(0, -3), velocity.RotatedBy(0.25f * player.direction).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.2f, 1.8f), Main.rand.NextBool() ? Color.DarkOrange : Color.OrangeRed, Color.OrangeRed, 0.9f, 18, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(spark);
                shotCount += 1;
            }
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Animated").Value;
            spriteBatch.Draw(texture, position, Item.GetCurrentFrame(ref frame, ref frameCounter, 5, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Animated").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 5, 4), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "Glow").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 5, 4, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
