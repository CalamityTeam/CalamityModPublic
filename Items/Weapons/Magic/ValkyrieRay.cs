using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ValkyrieRay : ModItem
    {
        public static readonly Color LightColor = new Color(235, 40, 121);
        private const float GemDistance = 18f;
        private const int ReuseCount = 14;
        private int useCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valkyrie Ray");
            Tooltip.SetDefault("Casts a devastating ray of holy power");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.height = 52;
            item.damage = 73;
            item.knockBack = 8.5f;
            item.magic = true;
            item.mana = 26;
            item.useTime = 2;
            item.useAnimation = item.useTime * ReuseCount;
            item.useStyle = 5;
            item.UseSound = SoundID.NPCDeath7;
            item.useTurn = false;
            item.noMelee = true;
            item.value = Item.buyPrice(gold: 36);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<ValkyrieRayBeam>();
            item.shootSpeed = 25f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(10, 10);

        // The weapon functions as a fake Clockwork Assault Rifle where only one the many shots actually fires something.
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            useCounter++;

            // Compute the gem position, which is needed for both charging and firing
            float angle = new Vector2(speedX, speedY).ToRotation();
            Vector2 gemOffset = Vector2.One * GemDistance * 1.4142f; // distance to gem on staff
            Vector2 gemPos = position + gemOffset.RotatedBy(angle - MathHelper.PiOver4);

            // Actually fire the weapon if it's the final reuse
            if (useCounter == ReuseCount)
            {
                // Produce firing dust and play sound
                if (Main.netMode != NetmodeID.Server)
                {
                    SpawnFiringDust(gemPos);
                    Main.PlaySound(SoundID.Item28, gemPos);
                    Main.PlaySound(SoundID.Item60, gemPos);
                }
                useCounter = 0;

                // Change the projectile's spawning position to the gem position
                position = gemPos;

                // Manually set the player's item times to longer when the weapon is fired so the staff sticks around a little longer
                player.itemTime = 19;
                player.itemAnimation = 19;
                player.itemAnimationMax = 19;
                return true;
            }

            // Otherwise, produce small amounts of charging dust and light which scales with the charge.
            else if (Main.netMode != NetmodeID.Server)
            {
                SpawnChargeDust(gemPos);
                float brightness = (float)useCounter / ReuseCount;
                Lighting.AddLight(gemPos, LightColor.ToVector3() * brightness);
            }
            return false;
        }

        private void SpawnFiringDust(Vector2 center)
        {
            int numDust = 36;
            int dustID = 73;
            for(int i = 0; i < numDust; ++i)
            {
                Dust d = Dust.NewDustDirect(center, 0, 0, dustID, 0f, 0f);
                d.velocity = (i * MathHelper.TwoPi / numDust).ToRotationVector2() * 2.2f;
                d.scale = 1.4f;
                d.noGravity = true;
            } 
        }

        private void SpawnChargeDust(Vector2 center)
        {
            int numDust = 2;
            int dustID = 73;
            float incomingRadius = 9f;
            for(int i = 0; i < numDust; ++i)
            {
                Vector2 offsetUnit = Main.rand.NextVector2Unit();
                Vector2 dustPos = center + offsetUnit * incomingRadius;
                Dust d = Dust.NewDustDirect(dustPos, 0, 0, dustID, 0f, 0f);
                d.velocity = offsetUnit * -Main.rand.NextFloat(2f, 3.5f);
                d.scale = Main.rand.NextFloat(0.4f, 1f);
                d.noGravity = true;
            } 
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedBar, 12);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 6);
            recipe.AddIngredient(ItemID.Ruby);
            recipe.AddIngredient(ItemID.SoulofSight);
            recipe.AddIngredient(ItemID.SoulofMight);
            recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
