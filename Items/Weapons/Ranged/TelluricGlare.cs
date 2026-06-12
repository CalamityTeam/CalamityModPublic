using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TelluricGlare : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int shots = 0;
        public int frame = 0;
        public int frameCounter = 0;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(3, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HolyFlames>()];
        }
        public override void SetDefaults()
        {
            Item.width = 74;
            Item.height = 126;
            Item.damage = 83;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 3;
            Item.useAnimation = 24;
            Item.useLimitPerAnimation = 8;
            Item.knockBack = 7.5f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();

            Item.UseSound = null;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TelluricGlareArrow>();
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Arrow;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-14f, 0f);
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture;

            //0 = 6 frames, 8 = 3 frames]
            texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, position, Item.GetCurrentFrame(ref frame, ref frameCounter, 3, 5), Color.White, 0f, origin, scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture;

            texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 3, 5), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/TelluricGlareGlow").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 3, 5, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // The arrow appears from a random location "on the bow".
            // They are also moved backwards so that they have some time to build up past positions. This helps make them not appear out of thin air.

            Vector2 offset = Vector2.Normalize(velocity.RotatedBy(MathHelper.PiOver2));
            position += offset * Main.rand.NextFloat(-19f, 19f);
            position -= 3f * velocity;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool isHolyArrow = CalamityUtils.CheckWoodenAmmo(type, player);
            if (isHolyArrow)
                type = Item.shoot;
            if (shots % 2 == 0)
            {
                SoundStyle sound = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianDash");
                SoundEngine.PlaySound(sound with { Volume = 0.25f, Pitch = Main.rand.NextFloat(-0.3f, -0.8f), MaxInstances = -1 }, player.Center);
                type = Item.shoot;
            }
            Projectile.NewProjectile(source, (isHolyArrow || shots % 2 == 0) ? position : player.Center + velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(90)) * Main.rand.NextFloat(-15, 15), velocity, type, (isHolyArrow ? (int)(damage * 1.2f) : damage), knockback, player.whoAmI);
            shots++;
            return false;
        }
    }
}
