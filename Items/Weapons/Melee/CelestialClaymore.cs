using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class CelestialClaymore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3, BuffID.Frostburn2, BuffID.Ichor];
        }
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 82;
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<CosmicSpiritBomb>();
            Item.shootSpeed = 0.1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
                mouseYDist = Main.screenPosition.Y + Main.screenHeight + Main.mouseY + realPlayerPos.Y;

            Vector2 realPositionOrig = realPlayerPos;
            mouseXDist *= 0.8f;
            mouseYDist *= 0.8f;

            for (int i = 0; i < 2; i++)
            {
                realPlayerPos = realPositionOrig;
                realPlayerPos.X += Main.rand.Next(-75, 76);
                realPlayerPos.Y += Main.rand.Next(-75, 76);
                realPlayerPos += new Vector2(mouseXDist, mouseYDist);
                Projectile.NewProjectile(source, realPlayerPos, Vector2.Zero, type, damage, knockback, player.whoAmI, Main.rand.Next(3));
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dustType = -1;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        dustType = DustID.MagicMirror;
                        break;
                    case 1:
                        dustType = DustID.PinkFairy;
                        break;
                    case 2:
                        dustType = DustID.CopperCoin;
                        break;
                }
                int swingDust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType, player.direction * 2, 0f, 150, default, 1.3f);
                Main.dust[swingDust].velocity *= 0.2f;
            }
        }
    }
}
