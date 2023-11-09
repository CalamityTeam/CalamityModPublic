using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PristineFury : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int frameCounter = 0;
        public int frame = 0;
        public static int BaseDamage = 70;
        public bool Trail = true;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 100;
            Item.height = 46;
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

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-25, -10);

        public override bool AltFunctionUse(Player player) => true;

        // Right click consumes ammo at the same rate but faster at spewing
        public override float UseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 0.5f : 1f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(5f));
                    Projectile.NewProjectile(source, position, newVel, ModContent.ProjectileType<PristineSecondary>(), damage, knockback, player.whoAmI);
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Trail ? 1 : 0);
                Trail = !Trail;
                for (int i = 0; i <= 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position + velocity * 3f + new Vector2(0, -3), 158, velocity.RotatedBy(0.25f * player.direction).RotatedByRandom(0.35f) * Main.rand.NextFloat(0.5f, 2.5f), 0, default, Main.rand.NextFloat(1.6f, 2f));
                    dust.noGravity = true;
                }
                CritSpark spark = new CritSpark(position + velocity * 3f + new Vector2(0, -3), velocity.RotatedBy(0.25f * player.direction).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.2f, 1.8f), Main.rand.NextBool() ? Color.DarkOrange : Color.OrangeRed, Color.OrangeRed, 0.9f, 18, 2f, 1.9f);
                GeneralParticleHandler.SpawnParticle(spark);
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
