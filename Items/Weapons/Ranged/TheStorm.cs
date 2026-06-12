using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheStorm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public int shots = 0;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 9));
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Electrified];
        }
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 90;
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 2;
            Item.useAnimation = 20;
            Item.useLimitPerAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TheStormLightningShot>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (shots % 2 == 0)
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.6f, PitchVariance = 0.2f }, position);

            for (int i = 0; i < 2; i++)
            {
                float arrowVelAdjust = Main.rand.NextFloat(-40, 40);
                Vector2 arrowSpawnPos = new Vector2(MathHelper.Lerp(player.Calamity().mouseWorld.X, player.Center.X, 0.5f), player.Center.Y) + new Vector2(arrowVelAdjust, Main.rand.NextFloat(-560, -660));
                Vector2 velAdjust = (player.Calamity().mouseWorld - arrowSpawnPos).SafeNormalize(velocity);
                Vector2 arrowVel = (velAdjust).RotatedBy(arrowVelAdjust * -0.004f) * Item.shootSpeed;

                if (CalamityUtils.CheckWoodenAmmo(type, player))
                {
                    Projectile.NewProjectile(source, arrowSpawnPos, arrowVel, ModContent.ProjectileType<TheStormLightningShot>(), (int)(damage * (i == 0 ? 1.5f : 1f)), knockback, -1, i == 0 ? 5 : 0);
                }
                else
                {
                    Projectile arrow1 = Projectile.NewProjectileDirect(source, arrowSpawnPos, arrowVel, i == 0 ? ModContent.ProjectileType<TheStormLightningShot>() : type, damage, knockback);
                    arrow1.tileCollide = false;
                }
            }
            
            shots++;
            return false;
        }
    }
}
