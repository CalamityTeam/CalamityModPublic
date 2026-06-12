using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class InfernalRift : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3];
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 25;
            Item.mana = 25;
            Item.useTime = 2;
            Item.useAnimation = 30;
            Item.reuseDelay = Item.useAnimation + 6;
            Item.useLimitPerAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<InfernalBlade>();
            Item.shootSpeed = 28f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float projSpeed = Item.shootSpeed;
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            float f = Main.rand.NextFloat() * MathHelper.TwoPi;
            float lowerLerpValue = 20f;
            float upperLerpValue = 60f;
            Vector2 projSpawnOffset = realPlayerPos + f.ToRotationVector2() * MathHelper.Lerp(lowerLerpValue, upperLerpValue, Main.rand.NextFloat());
            for (int i = 0; i < 50; i++)
            {
                projSpawnOffset = realPlayerPos + f.ToRotationVector2() * MathHelper.Lerp(lowerLerpValue, upperLerpValue, Main.rand.NextFloat());
                if (Collision.CanHit(realPlayerPos, 0, 0, projSpawnOffset + (projSpawnOffset - realPlayerPos).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                {
                    break;
                }
                f = Main.rand.NextFloat() * MathHelper.TwoPi;
            }
            Vector2 projSpawnPos = player.ClampedMouseWorld() - projSpawnOffset;
            Vector2 projVelocity = new Vector2(mouseXDist, mouseYDist).SafeNormalize(Vector2.UnitY) * projSpeed;
            projSpawnPos = projSpawnPos.SafeNormalize(projVelocity) * projSpeed;
            projSpawnPos = Vector2.Lerp(projSpawnPos, projVelocity, 0.25f);
            Projectile.NewProjectile(source, projSpawnOffset, projSpawnPos, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SkyFracture).
                AddIngredient<ScoriaBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
